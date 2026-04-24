using System;
using System.Collections.Generic;

namespace linco_core.Models;

public partial class KullaniciKelimeleri
{
    public int Id { get; set; }

    public int KullaniciId { get; set; }

    public int KelimeId { get; set; }

    public byte? Durum { get; set; }

    public DateTime? EklenmeTarihi { get; set; }

    public virtual GenelSozluk Kelime { get; set; } = null!;

    public virtual Kullanicilar Kullanici { get; set; } = null!;
}
