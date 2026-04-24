using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using linco_core.Models;
using linco_core.Services;

namespace linco_core.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly LincoDbContext _context;
    private readonly IAiService _aiService;

    public HomeController(ILogger<HomeController> logger, LincoDbContext context, IAiService aiService)
    {
        _logger = logger;
        _context = context;
        _aiService = aiService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Translator()
    {
        return View();
    }

    public IActionResult AISentenceWorkshop()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Translate(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return Json(new { success = false, message = "Lütfen çevrilecek metni girin." });
        
        try
        {
            var result = await _aiService.TranslateTextAsync(text);
            return Json(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Translate error");
            return Json(new { success = false, message = "Çeviri sırasında bir hata oluştu." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> GenerateSentences(string word, string level)
    {
        if (string.IsNullOrWhiteSpace(word) || string.IsNullOrWhiteSpace(level))
            return Json(new { success = false, message = "Kelime ve seviye seçilmelidir." });

        try
        {
            var result = await _aiService.GenerateSentencesAsync(word, level);
            return Json(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GenerateSentences error");
            return Json(new { success = false, message = "Cümle oluşturulurken bir hata oluştu." });
        }
    }

    [Authorize]
    public IActionResult MyVocabulary()
    {
        return View();
    }

    [Authorize]
    public IActionResult MyGoalList()
    {
        return View();
    }

    [Authorize]
    public IActionResult BilgiSiniri()
    {
        return View();
    }

    [Authorize]
    public IActionResult BilgiSiniriResult()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> GenerateTest(string topic, string level)
    {
        if (string.IsNullOrWhiteSpace(topic) || string.IsNullOrWhiteSpace(level))
            return Json(new { success = false, message = "Konu ve seviye seçilmelidir." });

        try
        {
            var result = await _aiService.GenerateTestAsync(topic, level);
            return Json(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GenerateTest error");
            return Json(new { success = false, message = "Test oluşturulurken bir hata oluştu." });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SaveTestResult(byte dogru, byte yanlis, byte bos, byte puan, string konu)
    {
        int userId = GetCurrentUserId();
        if (userId == 0) return Json(new { success = false, message = "Oturum bulunamadı." });

        try
        {
            var sinavSonucu = new SinavSonuclari
            {
                KullaniciId = userId,
                DogruSayisi = dogru,
                YanlisSayisi = yanlis,
                BosSayisi = bos,
                BasariPuani = puan,
                SinavKonusu = konu,
                TestTarihi = DateTime.Now
            };

            _context.SinavSonuclaris.Add(sinavSonucu);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SaveTestResult error");
            return Json(new { success = false, message = "Sonuç kaydedilirken hata oluştu." });
        }
    }

    [Authorize]
    public IActionResult LingoArena()
    {
        // Şimdilik boş bir sayfa, ancak yetki korumalı
        return View();
    }

    public IActionResult LostDictionary()
    {
        return View();
    }

    private int GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdString, out int userId))
            return userId;
        return 0;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetNextWord(string harf, string seviye)
    {
        if (string.IsNullOrEmpty(harf) || string.IsNullOrEmpty(seviye))
        {
            return Json(new { success = false, message = "Harf ve seviye seçilmeli." });
        }

        int userId = GetCurrentUserId();
        if (userId == 0) return Json(new { success = false, message = "Oturum bulunamadı." });

        var learnedIds = await _context.KullaniciKelimeleris
            .Where(k => k.KullaniciId == userId)
            .Select(k => k.KelimeId)
            .ToListAsync();

        var nextWord = await _context.GenelSozluks
            .Where(w => w.Harf == harf && w.Seviye == seviye && !learnedIds.Contains(w.Id))
            .OrderBy(w => w.Id)
            .Select(w => new { id = w.Id, ingilizce = w.Ingilizce, turkce = w.Turkce, okunus = w.Okunus })
            .FirstOrDefaultAsync();

        if (nextWord == null)
        {
            return Json(new { success = true, finished = true, message = "Tebrikler! Bu harf ve seviyedeki tüm kelimeleri bitirdiniz!" });
        }

        return Json(new { success = true, data = nextWord });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddWordToVocabulary(int kelimeId, string harf, string seviye)
    {
        int userId = GetCurrentUserId();
        if (userId == 0) return Json(new { success = false, message = "Oturum bulunamadı." });

        var exists = await _context.KullaniciKelimeleris.AnyAsync(k => k.KullaniciId == userId && k.KelimeId == kelimeId);
        if (!exists)
        {
            var userWord = new KullaniciKelimeleri
            {
                KullaniciId = userId,
                KelimeId = kelimeId,
                Durum = 1,
                EklenmeTarihi = DateTime.Now
            };
            _context.KullaniciKelimeleris.Add(userWord);
        }

        var progress = await _context.KullaniciSeviyeIlerlemes.FirstOrDefaultAsync(p => p.KullaniciId == userId && p.Harf == harf && p.Seviye == seviye);
        if (progress == null)
        {
            progress = new KullaniciSeviyeIlerleme
            {
                KullaniciId = userId,
                Harf = harf,
                Seviye = seviye,
                SonKelimeId = kelimeId
            };
            _context.KullaniciSeviyeIlerlemes.Add(progress);
        }
        else
        {
            progress.SonKelimeId = kelimeId;
        }

        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetLearnedWords(string harf, string seviye)
    {
        if (string.IsNullOrEmpty(harf) || string.IsNullOrEmpty(seviye))
        {
            return Json(new { success = false, message = "Harf ve seviye seçilmelidir." });
        }

        int userId = GetCurrentUserId();
        if (userId == 0) return Json(new { success = false, message = "Oturum bulunamadı." });

        var words = await _context.KullaniciKelimeleris
            .Include(k => k.Kelime)
            .Where(k => k.KullaniciId == userId && k.Kelime.Harf == harf && k.Kelime.Seviye == seviye)
            .OrderBy(k => k.Id)
            .Select(k => new {
                ingilizce = k.Kelime.Ingilizce,
                turkce = k.Kelime.Turkce
            })
            .ToListAsync();

        return Json(new { success = true, data = words });
    }

    [HttpGet]
    public async Task<IActionResult> GetGenelSozlukWords(string harf, string seviye)
    {
        if (string.IsNullOrEmpty(harf) || string.IsNullOrEmpty(seviye))
        {
            return Json(new { success = false, message = "Harf ve seviye seçilmelidir." });
        }

        var words = await _context.GenelSozluks
            .Where(w => w.Harf == harf && w.Seviye == seviye)
            .Select(w => new {
                ingilizce = w.Ingilizce,
                turkce = w.Turkce
            })
            .ToListAsync();

        return Json(new { success = true, data = words });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
