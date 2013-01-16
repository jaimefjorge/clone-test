using System.Web.Mvc;
using Webpage.Models;
using Webpage.Models.Repositories;

namespace Webpage.Controllers
{
    [Authorize]
    public class LotteryNumbersController : Controller
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ILotteryRepository _lotteryRepository;
        private readonly ILotteryNumberRepository _lotterynumberRepository;
        private readonly IPrizeRepository _prizeRepository;

        // If you are using Dependency Injection, you can delete the following constructor
        public LotteryNumbersController()
            : this(new PrizeRepository(), new SaleRepository(), 
            new LotteryRepository(), new LotteryNumberRepository()
                )
        {
        }

        public LotteryNumbersController(IPrizeRepository prizeRepository, ISaleRepository saleRepository,
                                        ILotteryRepository lotteryRepository,
                                        ILotteryNumberRepository lotterynumberRepository)
        {
            _prizeRepository = prizeRepository;
            _saleRepository = saleRepository;
            _lotteryRepository = lotteryRepository;
            _lotterynumberRepository = lotterynumberRepository;
        }

        //
        // GET: /LotteryNumbers/

        public ViewResult Index()
        {
            return
                View(_lotterynumberRepository.AllIncluding(lotterynumber => lotterynumber.Prize,
                                                           lotterynumber => lotterynumber.Sale,
                                                           lotterynumber => lotterynumber.Lottery));
        }

        //
        // GET: /LotteryNumbers/Create

        public ActionResult Create()
        {
            ViewBag.PossiblePrizes = _prizeRepository.All;
            ViewBag.PossibleSales = _saleRepository.AllIncluding(sale => sale.Client);
            ViewBag.PossibleLotteries = _lotteryRepository.All;
            return View();
        }

        //
        // POST: /LotteryNumbers/Create

        [HttpPost]
        public ActionResult Create(LotteryNumber lotterynumber)
        {
            if (ModelState.IsValid)
            {
                _lotterynumberRepository.InsertOrUpdate(lotterynumber);
                _lotterynumberRepository.Save();
                return RedirectToAction("Index");
            }
            ViewBag.PossiblePrizes = _prizeRepository.All;
            ViewBag.PossibleSales = _saleRepository.AllIncluding(sale => sale.Client);
            ViewBag.PossibleLotteries = _lotteryRepository.All;
            return View();
        }

        //
        // GET: /LotteryNumbers/Edit/5

        public ActionResult Edit(int id)
        {
            ViewBag.PossiblePrizes = _prizeRepository.All;
            ViewBag.PossibleSales = _saleRepository.AllIncluding(sale => sale.Client);
            ViewBag.PossibleLotteries = _lotteryRepository.All;
            return View(_lotterynumberRepository.Find(id));
        }

        //
        // POST: /LotteryNumbers/Edit/5

        [HttpPost]
        public ActionResult Edit(LotteryNumber lotterynumber)
        {
            if (ModelState.IsValid)
            {
                _lotterynumberRepository.InsertOrUpdate(lotterynumber);
                _lotterynumberRepository.Save();
                return RedirectToAction("Index");
            }
            ViewBag.PossiblePrizes = _prizeRepository.All;
            ViewBag.PossibleSales = _saleRepository.AllIncluding(sale => sale.Client);
            ViewBag.PossibleLotteries = _lotteryRepository.All;
            return View();
        }

        //
        // GET: /LotteryNumbers/Delete/5

        public ActionResult Delete(int id)
        {
            return View(_lotterynumberRepository.Find(id));
        }

        //
        // POST: /LotteryNumbers/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            _lotterynumberRepository.Delete(id);
            _lotterynumberRepository.Save();

            return RedirectToAction("Index");
        }
    }
}