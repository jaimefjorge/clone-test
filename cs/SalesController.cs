using System;
using System.Web.Mvc;
using Webpage.Models;
using Webpage.Models.Repositories;

namespace Webpage.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
		private readonly IClientRepository _clientRepository;
		private readonly ISaleRepository _saleRepository;
        private readonly ILotteryRepository _lotteryRepository;

		// If you are using Dependency Injection, you can delete the following constructor
        public SalesController()
            : this(new ClientRepository(), new SaleRepository(), new LotteryRepository())
        {
        }

        public SalesController(IClientRepository clientRepository, ISaleRepository saleRepository,
            ILotteryRepository lotteryRepository)
        {
			_clientRepository = clientRepository;
			_saleRepository = saleRepository;
            _lotteryRepository = lotteryRepository;
        }

        //
        // GET: /Sales/

        public ViewResult Index()
        {
            return View(_saleRepository.AllIncluding(
                sale => sale.Client, 
                sale => sale.LotteryNumbers,
                sale => sale.Lottery));
        }

        //
        // GET: /Sales/Create

        public ActionResult Create()
        {
            ViewBag.PossibleClients = _clientRepository.All;
            ViewBag.PossibleLotteries = _lotteryRepository.All;
            var sale = new Sale {Date = DateTime.Now};
            return View(sale);
        } 

        //
        // POST: /Sales/Create

        [HttpPost]
        public ActionResult Create(Sale sale)
        {
            if (ModelState.IsValid) {
                _saleRepository.InsertOrUpdate(sale);
                _saleRepository.Save();
                return RedirectToAction("Index");
            }
            ViewBag.PossibleClients = _clientRepository.All;
            ViewBag.PossibleLotteries = _lotteryRepository.All;
            return View(sale);
        }
        
        //
        // GET: /Sales/Edit/5
 
        public ActionResult Edit(int id)
        {
			ViewBag.PossibleClients = _clientRepository.All;
            
			ViewBag.PossibleLotteries = _lotteryRepository.All;
            
             return View(_saleRepository.Find(id));
        }

        //
        // POST: /Sales/Edit/5

        [HttpPost]
        public ActionResult Edit(Sale sale)
        {
            if (ModelState.IsValid) {
                _saleRepository.InsertOrUpdate(sale);
                _saleRepository.Save();
                return RedirectToAction("Index");
            }
            ViewBag.PossibleClients = _clientRepository.All;
            ViewBag.PossibleLotteries = _lotteryRepository.All;
            return View();
        }

        //
        // GET: /Sales/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View(_saleRepository.Find(id));
        }

        //
        // POST: /Sales/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            _saleRepository.Delete(id);
            _saleRepository.Save();

            return RedirectToAction("Index");
        }
    }
}

