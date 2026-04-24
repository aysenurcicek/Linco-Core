using System;
using System.Collections.Generic;

namespace linco_core.Models;

public partial class Kullanicilar
{
    public int Id { get; set; }

    public string KullaniciAdi { get; set; } = null!;

    public string Eposta { get; set; } = null!;

    public string Sifre { get; set; } = null!;

    public string? MevcutSeviye { get; set; }

    public int? ToplamXp { get; set; }

    public DateTime? KayitTarihi { get; set; }

    public DateTime? SonGirisTarihi { get; set; }

    public virtual ICollection<KullaniciKelimeleri> KullaniciKelimeleris { get; set; } = new List<KullaniciKelimeleri>();

    public virtual ICollection<KullaniciSeviyeIlerleme> KullaniciSeviyeIlerlemes { get; set; } = new List<KullaniciSeviyeIlerleme>();

    public virtual ICollection<OyunSkorlari> OyunSkorlaris { get; set; } = new List<OyunSkorlari>();

    public virtual ICollection<SinavSonuclari> SinavSonuclaris { get; set; } = new List<SinavSonuclari>();
}
