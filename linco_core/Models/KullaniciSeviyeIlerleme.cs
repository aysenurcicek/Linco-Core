using System;
using System.Collections.Generic;

namespace linco_core.Models;

public partial class KullaniciSeviyeIlerleme
{
    public int Id { get; set; }

    public int KullaniciId { get; set; }

    public string Harf { get; set; } = null!;

    public string Seviye { get; set; } = null!;

    public int SonKelimeId { get; set; }

    public virtual Kullanicilar Kullanici { get; set; } = null!;

    public virtual GenelSozluk SonKelime { get; set; } = null!;
}
