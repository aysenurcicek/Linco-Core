using System;
using System.Collections.Generic;

namespace linco_core.Models;

public partial class Yoneticiler
{
    public int Id { get; set; }

    public string KullaniciAdi { get; set; } = null!;

    public string Sifre { get; set; } = null!;

    public byte YetkiSeviyesi { get; set; }
}
