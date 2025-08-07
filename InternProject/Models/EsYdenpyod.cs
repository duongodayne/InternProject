using System;
using System.Collections.Generic;

namespace InternProject.Models;

public partial class EsYdenpyod
{
    public decimal? Gyono { get; set; }

    public decimal Denpyono { get; set; }

    public string? Idodt { get; set; }

    public string? ShuppatsuPlc { get; set; }

    public string? MokutekiPlc { get; set; }

    public string? Keiro { get; set; }

    public decimal? Kingaku { get; set; }

    public string? InsertOpeId { get; set; }

    public string? InsertPgmId { get; set; }

    public string? InsertPgmPrm { get; set; }

    public string? InsertDate { get; set; }

    public string? UpdateOpeId { get; set; }

    public string? UpdatePgmId { get; set; }

    public string? UpdatePgmPrm { get; set; }

    public string? UpdateDate { get; set; }

    public virtual EsYdenpyo DenpyonoNavigation { get; set; } = null!;
}
