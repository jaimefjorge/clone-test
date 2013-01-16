using System.Web.Mvc;
using Webpage.Models;
using Webpage.Models.Repositories;

namespace Webpage.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly IClientRepository _clientRepository;

        // If you are using Dependency Injection, you can delete the following constructor
        public ClientsController()
            : this(new ClientRepository())
        {
        }

        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        //
        // GET: /Clients/

        public ViewResult Index()
        {
            return View(_clientRepository.AllIncluding(client => client.Sales));
        }


        public ActionResult Details(int id)
        {
            return View(_clientRepository.Find(id));
        }

        //
        // GET: /Clients/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Clients/Create

        [HttpPost]
        public ActionResult Create(Client client)
        {
            if (ModelState.IsValid)
            {
                _clientRepository.InsertOrUpdate(client);
                _clientRepository.Save();
                return RedirectToAction("Index");
            }
            return View();
        }

        //
        // GET: /Clients/Edit/5

        public ActionResult Edit(int id)
        {
            return View(_clientRepository.Find(id));
        }

        //
        // POST: /Clients/Edit/5

        [HttpPost]
        public ActionResult Edit(Client client)
        {
            if (ModelState.IsValid)
            {
                _clientRepository.InsertOrUpdate(client);
                _clientRepository.Save();
                return RedirectToAction("Index");
            }
            return View();
        }

        //
        // GET: /Clients/Delete/5

        public ActionResult Delete(int id)
        {
            return View(_clientRepository.Find(id));
        }

        //
        // POST: /Clients/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            _clientRepository.Delete(id);
            _clientRepository.Save();

            return RedirectToAction("Index");
        }
    }
}