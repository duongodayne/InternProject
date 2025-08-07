using System;
using System.Collections.Generic;

namespace InternProject.Models;

public partial class Bumon
{
    public string Bumoncd { get; set; } = null!;

    public string? Bumonnm { get; set; }

    public virtual ICollection<EsYdenpyo> EsYdenpyos { get; set; } = new List<EsYdenpyo>();
}
