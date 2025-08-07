using System;
using System.Collections.Generic;

namespace InternProject.Models;

public partial class EsYdenpyo
{
    public decimal Denpyono { get; set; }

    public decimal? Kaikeind { get; set; }

    public string Uketukedt { get; set; } = null!;

    public string? Denpyodt { get; set; }

    public string? BumoncdYkanr { get; set; }

    public string? Biko { get; set; }

    public string? Suitokb { get; set; }

    public string? Shiharaidt { get; set; }

    public decimal? Kingaku { get; set; }

    public string? InsertOpeId { get; set; }

    public string? InsertPgmId { get; set; }

    public string? InsertPgmPrm { get; set; }

    public string? InsertDate { get; set; }

    public string? UpdateOpeId { get; set; }

    public string? UpdatePgmId { get; set; }

    public string? UpdatePgmPrm { get; set; }

    public string? UpdateDate { get; set; }

    public virtual Bumon? BumoncdYkanrNavigation { get; set; }

    public virtual ICollection<EsYdenpyod> EsYdenpyods { get; set; } = new List<EsYdenpyod>();
}
