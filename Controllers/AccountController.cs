 using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WorkMate.Models;
using WorkMate.ViewModels;


namespace WorkMate.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db = new AppDbContext();//Other DB setup
        private readonly IdentityDbContext _idb = new IdentityDbContext();//Login and Identity setup
        private AppUserManager _userManager;
        private AppSignInManager _signInManager;

        public AppUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public AppSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<AppSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }



        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View(new LoginViewModel());

        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Find user by username or email
            var user = await UserManager.FindByNameAsync(model.UsernameOrEmail)
                       ?? await UserManager.FindByEmailAsync(model.UsernameOrEmail);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or email.");
                return View(model);
            }

            // Check if account is active
            if (!user.IsActive) // Assuming you have IsActive in ApplicationUser
            {
                ModelState.AddModelError("", "Your account is inactive or locked.");
                return View(model);
            }

            // Attempt sign-in
            var result = await SignInManager.PasswordSignInAsync(
                user.UserName,
                model.Password,
                model.RememberMe,
                shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Home");

                case SignInStatus.LockedOut:
                    ModelState.AddModelError("", "User account is locked.");
                    return View(model);

                // Uncomment if using two-factor auth
                // case SignInStatus.RequiresVerification:
                //     return RedirectToAction("SendCode", new { ReturnUrl = "/", RememberMe = false });

                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }



        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel registration)
        {
            if (!ModelState.IsValid)
                return View(registration);

            var user = new AppUsers
            {
                UserName = registration.Username,
                Email = registration.Email,
            };

            var result = await UserManager.CreateAsync(user, registration.Password);
            if (result.Succeeded)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

            }
            return View(registration);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }
    }
}