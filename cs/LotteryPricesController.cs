using System.Web.Mvc;
using Webpage.Models;
using Webpage.Models.Repositories;

namespace Webpage.Controllers
{
    [Authorize]
    public class LotteryPricesController : Controller
    {
        private readonly ILotteryPriceRepository _lotterypriceRepository;

        // If you are using Dependency Injection, you can delete the following constructor
        public LotteryPricesController()
            : this(new LotteryPriceRepository())
        {
        }

        public LotteryPricesController(ILotteryPriceRepository lotterypriceRepository)
        {
            _lotterypriceRepository = lotterypriceRepository;
        }

        //
        // GET: /LotteryPrices/

        public ViewResult Index()
        {
            return
                View(_lotterypriceRepository.AllIncluding(lotteryprice => lotteryprice.Lotteries,
                                                         lotteryprice => lotteryprice.LotteryRules));
        }

        //
        // GET: /LotteryPrices/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /LotteryPrices/Create

        [HttpPost]
        public ActionResult Create(LotteryPrice lotteryprice)
        {
            if (ModelState.IsValid)
            {
                _lotterypriceRepository.InsertOrUpdate(lotteryprice);
                _lotterypriceRepository.Save();
                return RedirectToAction("Index");
            }
            return View();
        }

        //
        // GET: /LotteryPrices/Edit/5

        public ActionResult Edit(int id)
        {
            return View(_lotterypriceRepository.Find(id));
        }

        //
        // POST: /LotteryPrices/Edit/5

        [HttpPost]
        public ActionResult Edit(LotteryPrice lotteryprice)
        {
            if (ModelState.IsValid)
            {
                _lotterypriceRepository.InsertOrUpdate(lotteryprice);
                _lotterypriceRepository.Save();
                return RedirectToAction("Index");
            }
            return View();
        }

        //
        // GET: /LotteryPrices/Delete/5

        public ActionResult Delete(int id)
        {
            return View(_lotterypriceRepository.Find(id));
        }

        //
        // POST: /LotteryPrices/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var lotteryPrice = _lotterypriceRepository.Find(id);

            if (lotteryPrice.Lotteries== null || lotteryPrice.Lotteries.Count == 0)
            {
                _lotterypriceRepository.Delete(id);
                _lotterypriceRepository.Save();
            }
            return RedirectToAction("Index");
        }
    }
}