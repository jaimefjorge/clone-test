using System;
using System.Linq;
using System.Web.Mvc;
using Resources;
using Webpage.Models;
using Webpage.Models.Repositories;
using Webpage.ViewModels;
using Lottery = Webpage.Models.Lottery;
using Webpage.Helpers;

namespace Webpage.Controllers
{
    [Authorize]
    public class LotteriesController : Controller
    {
        private readonly ILotteryPriceRepository _lotterypriceRepository;
        private readonly ILotteryRepository _lotteryRepository;
        private readonly ILotteryNumberRepository _lotteryNumber;

		// If you are using Dependency Injection, you can delete the following constructor
        public LotteriesController() :
            this(new LotteryPriceRepository(),
                    new LotteryRepository(),
                    new LotteryNumberRepository())  
        {
        }

        public LotteriesController( ILotteryPriceRepository lotterypriceRepository, 
                                    ILotteryRepository lotteryRepository,
                                    ILotteryNumberRepository lotteryNumber)
        {
			_lotterypriceRepository = lotterypriceRepository;
			_lotteryRepository = lotteryRepository;
            _lotteryNumber = lotteryNumber;
        }

        //
        // GET: /Lotteries/

        public ViewResult Index()
        {
            return View(_lotteryRepository.AllIncluding(lottery => lottery.LotteryPrice, 
                lottery => lottery.Prizes, lottery => lottery.LotteryNumbers));
        }

        //
        // GET: /Lotteries/Create

        public ActionResult Create()
        {
			ViewBag.PossibleLotteryPrices = _lotterypriceRepository.All;
            var lottery = new ViewModelLottery {EndDate = DateTime.Today.AddDays(1), StartDate = DateTime.Today};
            return View(lottery);
        } 

        //
        // POST: /Lotteries/Create

        [HttpPost, Transaction]
        public ActionResult Create(ViewModelLottery viewModelLottery)
        {
            if (ModelState.IsValid) {
                var modelLottery = new Lottery
                {
                    Name = viewModelLottery.Name,
                    StartDate = viewModelLottery.StartDate,
                    EndDate = viewModelLottery.EndDate,
                    LotteryPriceId = viewModelLottery.LotteryPriceId
                };

                _lotteryRepository.InsertOrUpdate(modelLottery);
                _lotteryRepository.Save();

                for (int i = 1; i <= viewModelLottery.NumbersToGenerate; i++ )
                {
                    var number = new LotteryNumber
                                     {
                                         LotteryId = modelLottery.LotteryId,
                                         Number = i,
                                         SoldDate = null,
                                         WasSold = false
                                         
                                     };
                    _lotteryNumber.InsertOrUpdate(number);
                }
                _lotteryNumber.Save();

                return RedirectToAction("Index");
            }
            ViewBag.PossibleLotteryPrices = _lotterypriceRepository.All;
            return View(viewModelLottery);
        }
        
        //
        // GET: /Lotteries/Edit/5
 
        public ActionResult Edit(int id)
        {
			ViewBag.PossibleLotteryPrices = _lotterypriceRepository.All;
            var modelLottery = _lotteryRepository.FindIncluding(id,
                                                                lottery => lottery.LotteryPrice,
                                                                lottery => lottery.Prizes,
                                                                lottery => lottery.LotteryNumbers);

            var lotteryView = new ViewModelLottery
                                  {
                                      LotteryId = modelLottery.LotteryId,
                                      LotteryPrice = modelLottery.LotteryPrice,
                                      Name = modelLottery.Name,
                                      LotteryPriceId = modelLottery.LotteryPriceId,
                                      NumbersToGenerate = modelLottery.LotteryNumbers.Count,
                                      StartDate = modelLottery.StartDate,
                                      EndDate = modelLottery.EndDate,
                                      Prizes = modelLottery.Prizes
                                  };
            return View(lotteryView);
        }

        //
        // POST: /Lotteries/Edit/5

        [HttpPost, Transaction]
        public ActionResult Edit(ViewModelLottery viewModelLottery)
        {
            Lottery modelLottery = _lotteryRepository.FindIncluding(viewModelLottery.LotteryId,
                                                                    lottery => lottery.LotteryPrice,
                                                                    lottery => lottery.Prizes,
                                                                    lottery => lottery.LotteryNumbers);

            if (viewModelLottery.NumbersToGenerate != modelLottery.LotteryNumbers.Count)
            {
                if (modelLottery.LotteryNumbers.Any(lotteryNumber => lotteryNumber.WasSold))
                {
                    ModelState.AddModelError("NumbersToGenerate", EditorLocalization.LotteriesController_Edit_Numeros_ja_foram_vendidos);
                }
            }

            if (ModelState.IsValid) {

                modelLottery.LotteryPriceId = viewModelLottery.LotteryPriceId;
                modelLottery.Name = viewModelLottery.Name;
                modelLottery.StartDate = viewModelLottery.StartDate;
                modelLottery.EndDate = viewModelLottery.EndDate;
                
                _lotteryRepository.InsertOrUpdate(modelLottery);
                _lotteryRepository.Save();

                if (viewModelLottery.NumbersToGenerate != modelLottery.LotteryNumbers.Count)
                {
                    foreach (var lotteryNumber in modelLottery.LotteryNumbers)
                    {
                        _lotteryNumber.Delete(lotteryNumber.LotteryNumberId);
                    }
                    _lotteryNumber.Save();

                    for (int i = 1; i <= viewModelLottery.NumbersToGenerate; i++)
                    {
                        var number = new LotteryNumber
                        {
                            LotteryId = modelLottery.LotteryId,
                            Number = i
                        };
                        _lotteryNumber.InsertOrUpdate(number);
                    }
                    _lotteryNumber.Save();
                }

                return RedirectToAction("Index");
            }

            ViewBag.PossibleLotteryPrices = _lotterypriceRepository.All;
            return View();
        }

        //
        // GET: /Lotteries/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View(_lotteryRepository.Find(id));
        }

        //
        // POST: /Lotteries/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            _lotteryRepository.Delete(id);
            _lotteryRepository.Save();

            return RedirectToAction("Index");
        }
    }
}

