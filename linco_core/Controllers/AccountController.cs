using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using linco_core.Models;

namespace linco_core.Controllers;

public class AccountController : Controller
{
    private readonly LincoDbContext _context;

    public AccountController(LincoDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string kullaniciAdi, string sifre)
    {
        if (string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(sifre))
        {
            ViewBag.Error = "Kullanıcı adı ve şifre boş bırakılamaz.";
            return View();
        }

        var user = await _context.Kullanicilars.FirstOrDefaultAsync(u => u.KullaniciAdi == kullaniciAdi && u.Sifre == sifre);

        if (user == null)
        {
            ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
            return View();
        }

        user.SonGirisTarihi = DateTime.Now;
        await _context.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.KullaniciAdi),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string kullaniciAdi, string eposta, string sifre)
    {
        if (string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(eposta) || string.IsNullOrEmpty(sifre))
        {
            ViewBag.Error = "Tüm alanları doldurmanız gerekmektedir.";
            return View();
        }

        var existingUser = await _context.Kullanicilars.FirstOrDefaultAsync(u => u.KullaniciAdi == kullaniciAdi || u.Eposta == eposta);
        if (existingUser != null)
        {
            ViewBag.Error = "Bu kullanıcı adı veya e-posta zaten kullanımda.";
            return View();
        }

        var newUser = new Kullanicilar
        {
            KullaniciAdi = kullaniciAdi,
            Eposta = eposta,
            Sifre = sifre,
            KayitTarihi = DateTime.Now
        };

        _context.Kullanicilars.Add(newUser);
        await _context.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, newUser.KullaniciAdi),
            new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
