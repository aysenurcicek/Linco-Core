using System;
using System.Collections.Generic;

namespace linco_core.Models;

public partial class GenelSozluk
{
    public int Id { get; set; }

    public string Ingilizce { get; set; } = null!;

    public string Turkce { get; set; } = null!;

    public string Seviye { get; set; } = null!;

    public string Harf { get; set; } = null!;

    public string? OrnekCumle { get; set; }

    public string? TelaffuzUrl { get; set; }

    public string? Okunus { get; set; }

    public virtual ICollection<KullaniciKelimeleri> KullaniciKelimeleris { get; set; } = new List<KullaniciKelimeleri>();

    public virtual ICollection<KullaniciSeviyeIlerleme> KullaniciSeviyeIlerlemes { get; set; } = new List<KullaniciSeviyeIlerleme>();
}
