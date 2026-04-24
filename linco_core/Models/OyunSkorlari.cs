using System;
using System.Collections.Generic;

namespace linco_core.Models;

public partial class OyunSkorlari
{
    public int Id { get; set; }

    public int KullaniciId { get; set; }

    public int OyunId { get; set; }

    public int? AlinanSkor { get; set; }

    public int? SureSaniye { get; set; }

    public DateTime? OynamaTarihi { get; set; }

    public virtual Kullanicilar Kullanici { get; set; } = null!;

    public virtual Oyunlar Oyun { get; set; } = null!;
}
