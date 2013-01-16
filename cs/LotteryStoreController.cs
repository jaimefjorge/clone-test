using System;
using System.Web.Mvc;
using Resources;
using Webpage.Helpers;
using Webpage.Models;
using Webpage.Models.Repositories;
using Webpage.ViewModels;
using System.Collections.Generic;

namespace Webpage.Controllers
{
    
    public class LotteryStoreController : Controller
    {
       
        private readonly ILotteryRepository _lotteryRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ILotteryNumberRepository _lotteryNumberRepository;
        private readonly IPrizeRepository _prizeRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly IRaffleRepository _raffleRepository;

        // If you are using Dependency Injection, you can delete the following constructor
        public LotteryStoreController()
            : this(
            new LotteryRepository(),
            new ClientRepository(),
            new LotteryNumberRepository(),
            new PrizeRepository(),
            new SaleRepository(),
            new RaffleRepository())
        {
        }

        public LotteryStoreController(
            ILotteryRepository lotteryRepository, 
            IClientRepository clientRepository, 
            ILotteryNumberRepository lotteryNumberRepository,
            IPrizeRepository prizeRepository,
            ISaleRepository saleRepository,
            IRaffleRepository raffleRepository)
        {
            _lotteryRepository = lotteryRepository;
            _clientRepository = clientRepository;
            _lotteryNumberRepository = lotteryNumberRepository;
            _prizeRepository = prizeRepository;
            _saleRepository = saleRepository;
            _raffleRepository = raffleRepository;
        }

        //
        // GET: /LotteryStore/

        public ActionResult Index()
        {
            var lotteries = _lotteryRepository.AllWithSales();

            return View(lotteries); 
        }

        public ActionResult SellNumbers(int lotteryId)
        {
            var clients = _clientRepository.All;
            
            var newList = new List<object>();
            foreach (var client in clients)
            {
                newList.Add(new
                {
                    client.ClientId,
                    Name = client.Name + " (" + client.Email + ")"
                });
               // this.ViewData["Members"] = new SelectList(newList, "Id", "Name");
            }
            ViewBag.PossibleClients = newList;
            //ViewBag.PossibleClients = _clientRepository.All;
            var numbers = _lotteryNumberRepository.AllAvailableFrom(lotteryId);
            return View(new SellNumberModelView { LotteryId = lotteryId, LotteryNumbers = numbers });
        }

        [HttpPost, Transaction]
        public ActionResult SellNumbers(SellNumberModelView model)
        {
            var lottery = _lotteryRepository.Find(model.LotteryId);
            if (lottery.StartDate < DateTime.Now && 
                lottery.EndDate > DateTime.Now &&
                TryValidateModel(model))
            {
                if (model.ClientId != null)
                {
                    var sale = new Sale {
                                             ClientId = (int) model.ClientId, 
                                             Date = DateTime.Now, 
                                             Delivered = false, 
                                             Canceled = false,
                                             Paid = false, 
                                             LotteryId = lottery.LotteryId
                                         };
                    _saleRepository.InsertOrUpdate(sale);
                    _saleRepository.Save();
                    bool savedAllNumbers = true;
                    foreach (var number in model.LotteryNumbers)
                    {
                        var lotteryNumber = _lotteryNumberRepository.FindByNumber(lottery.LotteryId, number);
                        if(lotteryNumber.WasSold)
                        {
                            savedAllNumbers = false;
                            ModelState.AddModelError("", EditorLocalization.LotteryStoreController_AlreadySoldSomeNumbers);
                            break;
                        }
                        lotteryNumber.SoldDate = DateTime.Now;
                        lotteryNumber.WasSold = true;
                        lotteryNumber.SaleId = sale.SaleId;
                        var prize = _prizeRepository.GetOnePrizeForLottery(lottery.LotteryId);
                        if(prize != null)
                            lotteryNumber.PrizeId = prize.PrizeId;
                        _lotteryNumberRepository.InsertOrUpdate(lotteryNumber);
                    }
                    if (savedAllNumbers)
                    {
                        _lotteryNumberRepository.Save();
                        return RedirectToAction("SaleDetails", new { saleId = sale.SaleId });
                    }
                    _saleRepository.Delete(sale.SaleId);
                    _saleRepository.Save();
                }
            }

            ViewBag.PossibleClients = _clientRepository.All;
            model.LotteryNumbers = _lotteryNumberRepository.AllAvailableFrom(model.LotteryId);
            return View(model);
        }

        public ActionResult SaleDetails(int saleId)
        {
            var sale = _saleRepository.FindIncludingAll(saleId);
            if (HttpContext.Request.RequestType == "POST")
            {
                if (sale.Paid)
                {
                    sale.Delivered = true;
                }
                else
                {
                    sale.Paid = true;
                }
                _saleRepository.InsertOrUpdate(sale);
                _saleRepository.Save();
            }
            
            return View(sale);
        }

        public ActionResult RevertSale(int saleId)
        {
            var sale = _saleRepository.FindIncludingAll(saleId);
            if (HttpContext.Request.RequestType == "POST")
            {
                if (!sale.Delivered && !sale.Paid)
                {
                    sale.Paid = false;
                    sale.Delivered = false;
                    sale.Canceled = true;
                    sale.TotalCost = 0;
                    foreach (var lotteryNumber in sale.LotteryNumbers)
                    {
                        lotteryNumber.Prize = null;
                        lotteryNumber.Sale = null;
                        lotteryNumber.WasSold = false;
                        lotteryNumber.SoldDate = null;
                    }
                }
                
                _saleRepository.InsertOrUpdate(sale);
                _saleRepository.Save();
            }

            return View("SaleDetails", sale);
        }

        public ActionResult RaffleNumbers(int lotteryid)
        {
            var prize = _prizeRepository.GetOnePrizeForRaffle(lotteryid);
            var lotteryNumber = _lotteryNumberRepository.GetOneNumberForRaffle(lotteryid);

            int? raffleId = null;
            if (lotteryNumber != null && prize != null)
            {
                var raffle = new Raffle { LotteryNumberId = lotteryNumber.LotteryNumberId, PrizeId = prize.PrizeId };
                _raffleRepository.InsertOrUpdate(raffle);
                _raffleRepository.Save();
                raffleId = raffle.RaffleId;
            }


            return View("Details", raffleId == null ? null : _raffleRepository.Find((int) raffleId));
        }
    }
}