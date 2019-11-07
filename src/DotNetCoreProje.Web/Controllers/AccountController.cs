using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreProje.Domain.Identity;
using DotNetCoreProje.Web.ViewModels;
using DotNetCoreProje.Web.ViewModels.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreProje.Web.Controllers
{
    public class AccountController : Controller
    {
        //Kullanıcı kaydetmek için veya kullanıcı bilgilerinde değişiklik yapmmak için kullanılan servis
        private readonly UserManager<ApplicationUser> _userManager;

        //Kullanıcının uygulamaya giriş çıkışı işlemlerini yönettiğimiz servis
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login(string returnUrl = null)
        {

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            //gelen modeli doğrula
            if (ModelState.IsValid)
            {
                //model doğruysa kullanıcıyı kontrol et
                var existUser = await _userManager.FindByEmailAsync(model.Username);
                //yoksa hata dön
                if (existUser == null)
                {
                    ModelState.AddModelError(string.Empty, "Bu email ile kayıtlı bir kullanıcı bulunamadı");
                    return View(model);
                }

                //kullanıcı adı ve sifre eşleşiyor mu? kontrol et
                var login = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);

                //eşleşmiyorsa hata dön
                if (!login.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Bu email ve şifre ile uyumlu bir kullanıcı bulunamadı!Şifrenizi kontrol ediniz");
                    return View(model);

                }

                //return RedirectToAction("Index", "Home");
                // ana sayfaya yonlendir (simdilik)

                ////Ana Sayfaya Yönlendirme
                if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            //başarılı değilse hata dön
            return View(model);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            //gelen modeli kaydet
            if (ModelState.IsValid)
            {
                // validse kaydet

                var newUser = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName=model.FirstName,
                    LastName = model.LastName,
                    EmailConfirmed = true,
                    TwoFactorEnabled = false,
                    NationalIdNumber = model.NationalIdNumber
                };

                var registerUser = await _userManager.CreateAsync(newUser, model.Password);
                if (registerUser.Succeeded)
                {
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(registerUser);

            }
            // degilse hatalari don
            return View(model);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var err in result.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }



    }
}