using System.Web.Mvc;
using Webpage.Models;
using Webpage.Models.Repositories;

namespace Webpage.Controllers
{
    [Authorize]
    public class PrizesController : Controller
    {
        private readonly ILotteryRepository _lotteryRepository;
        private readonly IPrizeRepository _prizeRepository;

        // If you are using Dependency Injection, you can delete the following constructor
        public PrizesController()
            : this(new LotteryRepository(), new PrizeRepository())
        {
        }

        public PrizesController(ILotteryRepository lotteryRepository, IPrizeRepository prizeRepository)
        {
            _lotteryRepository = lotteryRepository;
            _prizeRepository = prizeRepository;
        }

        //
        // GET: /Prizes/

        public ViewResult Index()
        {
            return View(_prizeRepository.AllIncluding(prize => prize.Lottery, prize => prize.LotteryNumbers));
        }

        //
        // GET: /Prizes/Create

        public ActionResult Create()
        {
            ViewBag.PossibleLotteries = _lotteryRepository.All;
            return View();
        }

        //
        // POST: /Prizes/Create

        [HttpPost]
        public ActionResult Create(Prize prize)
        {
            if (ModelState.IsValid)
            {
                _prizeRepository.InsertOrUpdate(prize);
                _prizeRepository.Save();
                return RedirectToAction("Index");
            }
            ViewBag.PossibleLotteries = _lotteryRepository.All;
            return View();
        }

        //
        // GET: /Prizes/Edit/5

        public ActionResult Edit(int id)
        {
            ViewBag.PossibleLotteries = _lotteryRepository.All;
            return View(_prizeRepository.Find(id));
        }

        //
        // POST: /Prizes/Edit/5

        [HttpPost]
        public ActionResult Edit(Prize prize)
        {
            if (ModelState.IsValid)
            {
                _prizeRepository.InsertOrUpdate(prize);
                _prizeRepository.Save();
                return RedirectToAction("Index");
            }
            ViewBag.PossibleLotteries = _lotteryRepository.All;
            return View();
        }

        //
        // GET: /Prizes/Delete/5

        public ActionResult Delete(int id)
        {
            var item = _prizeRepository.Find(id);
            if (item.LotteryNumbers == null || item.LotteryNumbers.Count == 0)
                return View(item);
            return RedirectToAction("Index");
        }

        //
        // POST: /Prizes/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var item = _prizeRepository.Find(id);
            
            if (item.LotteryNumbers == null || item.LotteryNumbers.Count == 0)
            {
                _prizeRepository.Delete(id);
                _prizeRepository.Save();
            }
            return RedirectToAction("Index");
        }
    }
}