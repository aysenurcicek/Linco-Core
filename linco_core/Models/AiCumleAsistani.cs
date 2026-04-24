using System;
using System.Collections.Generic;

namespace linco_core.Models;

public partial class AiCumleAsistani
{
    public int Id { get; set; }

    public string KelimeGirilen { get; set; } = null!;

    public string SecilenSeviye { get; set; } = null!;

    public string KurulanCumle { get; set; } = null!;

    public string CumleTurkce { get; set; } = null!;

    public DateTime? OlusturmaTarihi { get; set; }
}
