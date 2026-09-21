using LifeSure.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LifeSure.Areas.Admin.Controllers;

[Area("Admin")]
public class AccountController(
    SignInManager<IdentityUser> signInManager,
    UserManager<IdentityUser> userManager) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true
            && User.IsInRole("Admin"))
        {
            return RedirectToAction(
                "Index",
                "Dashboard",
                new { area = "Admin" });
        }

        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userManager.FindByEmailAsync(
            model.Email.Trim());

        if (user is null
            || !await userManager.IsInRoleAsync(user, "Admin"))
        {
            ModelState.AddModelError(
                string.Empty,
                "E-posta veya parola hatalı.");

            return View(model);
        }

        var result = await signInManager.PasswordSignInAsync(
            user,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl)
                && Url.IsLocalUrl(model.ReturnUrl))
            {
                return LocalRedirect(model.ReturnUrl);
            }

            return RedirectToAction(
                "Index",
                "Dashboard",
                new { area = "Admin" });
        }

        ModelState.AddModelError(
            string.Empty,
            result.IsLockedOut
                ? "Çok fazla başarısız deneme yapıldı. Lütfen daha sonra tekrar deneyiniz."
                : "Giriş yapılamadı. Bilgilerinizi kontrol ediniz.");

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();

        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return StatusCode(
            StatusCodes.Status403Forbidden,
            "Bu alana erişim yetkiniz bulunmuyor.");
    }
}