using System;
using System.Collections.Generic;

namespace linco_core.Models;

public partial class Oyunlar
{
    public int Id { get; set; }

    public string OyunAdi { get; set; } = null!;

    public string? Aciklama { get; set; }

    public virtual ICollection<OyunSkorlari> OyunSkorlaris { get; set; } = new List<OyunSkorlari>();
}
