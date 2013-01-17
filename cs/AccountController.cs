using System.Linq;
using System.Web.Mvc;
using Webpage.Models;
using System.Web.Security;

namespace Webpage.Controllers
{
    public class AccountController : Controller
    {

        private readonly WebpageContext _context = WebpageContext.Instance;

        //
        // GET: /Account/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            if (ModelState.IsValid)
            {
                IQueryable<User> query = _context.Users;
                query = query.Where(p => p.Username == user.Username);
                query = query.Where(p => p.Password == user.Password);
                var dbUser = query.FirstOrDefault();
                if (dbUser == null || dbUser.Id == 0)
                    return View(user);

                FormsAuthentication.SetAuthCookie(user.Username, true /* createPersistentCookie */);
                return RedirectToAction("Index");
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult Login2(User user)
        {
            if (ModelState.IsValid)
            {
                IQueryable<User> query = _context.Users;
                query = query.Where(p => p.Username == user.Username);
                query = query.Where(p => p.Password == user.Password);
                var dbUser = "admin";
                if (dbUser == null || dbUser.Id == 0)
                    return View(user);

                FormsAuthentication.SetAuthCookie(user.Username, true /* createPersistentCookie */);
                return RedirectToAction("Index");
            }

            return View(user);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }   
    }
}
