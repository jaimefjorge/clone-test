using System.Web.Mvc;
using Webpage.Models;
using Webpage.Models.Repositories;

namespace Webpage.Controllers
{   
    public class RafflesController : Controller
    {
        private readonly IPrizeRepository _prizeRepository;
        private readonly IRaffleRepository _raffleRepository;
        private readonly ILotteryNumberRepository _lotteryNumberRepository;

		// If you are using Dependency Injection, you can delete the following constructor
        public RafflesController()
            : this(new PrizeRepository(), new RaffleRepository(), new LotteryNumberRepository())
        {
        }

        public RafflesController(IPrizeRepository prizeRepository, 
            IRaffleRepository raffleRepository, ILotteryNumberRepository lotteryNumberRepository)
        {
			_prizeRepository = prizeRepository;
			_raffleRepository = raffleRepository;
            _lotteryNumberRepository = lotteryNumberRepository;
        }

        //
        // GET: /Raffles/

        public ViewResult Index()
        {
            return View(_raffleRepository.AllIncluding(raffle => raffle.LotteryNumber, raffle => raffle.Prize));
        }

        //
        // GET: /Raffles/Create

        public ActionResult Create()
        {
            ViewBag.PossiblePrizes = _prizeRepository.ToRaffle;
            ViewBag.PossibleNumbers = _lotteryNumberRepository.SoldAndPaidWithoutRafflePrize;
            return View();
        } 

        //
        // POST: /Raffles/Create

        [HttpPost]
        public ActionResult Create(Raffle raffle)
        {
            if (ModelState.IsValid) {
                _raffleRepository.InsertOrUpdate(raffle);
                _raffleRepository.Save();
                return RedirectToAction("Index");
            }
            ViewBag.PossibleNumbers = _lotteryNumberRepository.SoldAndPaidWithoutRafflePrize;
            ViewBag.PossiblePrizes = _prizeRepository.ToRaffle;
            return View();
        }
        
        //
        // GET: /Raffles/Edit/5
 
        public ActionResult Edit(int id)
        {
            ViewBag.PossibleNumbers = _lotteryNumberRepository.SoldAndPaidWithoutRafflePrize;
            ViewBag.PossiblePrizes = _prizeRepository.ToRaffle;
             return View(_raffleRepository.Find(id));
        }

        //
        // POST: /Raffles/Edit/5

        [HttpPost]
        public ActionResult Edit(Raffle raffle)
        {
            if (ModelState.IsValid) {
                _raffleRepository.InsertOrUpdate(raffle);
                _raffleRepository.Save();
                return RedirectToAction("Index");
            }
            ViewBag.PossibleNumbers = _lotteryNumberRepository.SoldAndPaidWithoutRafflePrize;
            ViewBag.PossiblePrizes = _prizeRepository.ToRaffle;
            return View();
        }

        //
        // GET: /Raffles/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View(_raffleRepository.Find(id));
        }

        //
        // POST: /Raffles/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            _raffleRepository.Delete(id);
            _raffleRepository.Save();

            return RedirectToAction("Index");
        }
    }
}

