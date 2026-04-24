using System;
using System.Collections.Generic;

namespace linco_core.Models;

public partial class SinavSonuclari
{
    public int Id { get; set; }

    public int KullaniciId { get; set; }

    public byte DogruSayisi { get; set; }

    public byte YanlisSayisi { get; set; }

    public byte BosSayisi { get; set; }

    public byte BasariPuani { get; set; }

    public string? SinavKonusu { get; set; }

    public DateTime? TestTarihi { get; set; }

    public virtual Kullanicilar Kullanici { get; set; } = null!;
}
