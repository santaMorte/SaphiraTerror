using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SaphiraTerror.Infrastructure.Entities;   // ✅ ApplicationUser do mesmo namespace do DbContext
using SaphiraTerror.Web.Models;

namespace SaphiraTerror.Web.Controllers;

public class AuthController(
    SignInManager<ApplicationUser> signIn,
    UserManager<ApplicationUser> users) : Controller
{
    private readonly SignInManager<ApplicationUser> _signIn = signIn;
    private readonly UserManager<ApplicationUser> _users = users;

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
        => View(new LoginVm { ReturnUrl = returnUrl });

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _users.FindByEmailAsync(model.Email);
        if (user is null || (user is { } && (user as dynamic).IsActive == false))
        {
            ModelState.AddModelError("", "Usuário inválido ou inativo.");
            return View(model);
        }

        var res = await _signIn.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
        if (!res.Succeeded)
        {
            ModelState.AddModelError("", "Credenciais inválidas.");
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Index", "Home", new { section = "genres" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Denied() => Content("Acesso negado.");
}
