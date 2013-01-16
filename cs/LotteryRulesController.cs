using System.Web.Mvc;
using Webpage.Models;
using Webpage.Models.Repositories;

namespace Webpage.Controllers
{
    [Authorize]
    public class LotteryRulesController : Controller
    {
        private readonly ILotteryPriceRepository _lotterypriceRepository;
        private readonly ILotteryRuleRepository _lotteryruleRepository;

        // If you are using Dependency Injection, you can delete the following constructor
        public LotteryRulesController()
            : this(new LotteryPriceRepository(), new LotteryRuleRepository())
        {
        }

        public LotteryRulesController(ILotteryPriceRepository lotterypriceRepository,
                                      ILotteryRuleRepository lotteryruleRepository)
        {
            _lotterypriceRepository = lotterypriceRepository;
            _lotteryruleRepository = lotteryruleRepository;
        }

        //
        // GET: /LotteryRules/

        public ViewResult Index()
        {
            return View(_lotteryruleRepository.AllIncluding(lotteryrule => lotteryrule.LotteryPrice));
        }

        //
        // GET: /LotteryRules/Create

        public ActionResult Create()
        {
            ViewBag.PossibleLotteryPrices = _lotterypriceRepository.All;
            return View();
        }

        //
        // POST: /LotteryRules/Create

        [HttpPost]
        public ActionResult Create(LotteryRule lotteryrule)
        {
            if (ModelState.IsValid)
            {
                _lotteryruleRepository.InsertOrUpdate(lotteryrule);
                _lotteryruleRepository.Save();
                return RedirectToAction("Index");
            }
            ViewBag.PossibleLotteryPrices = _lotterypriceRepository.All;
            return View();
        }

        //
        // GET: /LotteryRules/Edit/5

        public ActionResult Edit(int id)
        {
            ViewBag.PossibleLotteryPrices = _lotterypriceRepository.All;
            return View(_lotteryruleRepository.Find(id));
        }

        //
        // POST: /LotteryRules/Edit/5

        [HttpPost]
        public ActionResult Edit(LotteryRule lotteryrule)
        {
            if (ModelState.IsValid)
            {
                _lotteryruleRepository.InsertOrUpdate(lotteryrule);
                _lotteryruleRepository.Save();
                return RedirectToAction("Index");
            }
            ViewBag.PossibleLotteryPrices = _lotterypriceRepository.All;
            return View();
        }

        //
        // GET: /LotteryRules/Delete/5

        public ActionResult Delete(int id)
        {
            return View(_lotteryruleRepository.Find(id));
        }

        //
        // POST: /LotteryRules/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            _lotteryruleRepository.Delete(id);
            _lotteryruleRepository.Save();

            return RedirectToAction("Index");
        }
    }
}