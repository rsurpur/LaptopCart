using LaptopCart.Data.Data;
using LaptopCart.Models;
using LaptopCart.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LaptopCart.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpClientFactory _clientFactory;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IHttpClientFactory clientFactory)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _clientFactory = clientFactory;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register() => View();

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (!_userManager.Users.Any())
                {
                    await _userManager.AddToRoleAsync(user, "Admin"); // first registered user
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "User"); // all others
                }
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
           // if(ModelState.IsValid)
           // {
                var client = _clientFactory.CreateClient();
                var response = await client.PostAsJsonAsync("https://localhost:7142/api/Auth/login", model);
                if (response.IsSuccessStatusCode)
                {
                    var tokenResult = await response.Content.ReadFromJsonAsync<TokenResponseModel>();
                    if (tokenResult != null && !string.IsNullOrEmpty(tokenResult.AccessToken))
                    {
                        // Store the token in session or cookie
                        HttpContext.Session.SetString("JWToken", tokenResult.AccessToken);
                        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                        return RedirectToAction("Index", "Product");
                    }
                }

          //  }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }

}
