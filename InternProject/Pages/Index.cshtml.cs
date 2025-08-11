using InternProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    public IndexModel(AppDbContext context) => _context = context;

    [BindProperty(SupportsGet = true)] public int? Nendo { get; set; }
    [BindProperty(SupportsGet = true)] public string? DenpyonoFrom { get; set; }
    [BindProperty(SupportsGet = true)] public string? DenpyonoTo { get; set; }
    [BindProperty(SupportsGet = true)] public string? DenpyodtFrom { get; set; }
    [BindProperty(SupportsGet = true)] public string? DenpyodtTo { get; set; }
    [BindProperty(SupportsGet = true)] public string? ShinseibFrom { get; set; }
    [BindProperty(SupportsGet = true)] public string? ShinseibTo { get; set; }
    [BindProperty(SupportsGet = true)] public string? ShutsunoHoho { get; set; } // select

    public List<EsYdenpyo> Results { get; set; } = new();
    public List<int> AvailableYears { get; set; } = new();

    public void OnGet()
    {
        var currentYear = DateTime.Now.Year;
        AvailableYears = Enumerable.Range(2020, currentYear - 2020 + 3).ToList();

        var query = _context.EsYdenpyos
            .Include(x => x.BumoncdYkanrNavigation)
            .AsQueryable();

        if (Nendo.HasValue) query = query.Where(x => x.Kaikeind == Nendo.Value);

        if (!string.IsNullOrEmpty(DenpyonoFrom) && decimal.TryParse(DenpyonoFrom, out var dpf))
            query = query.Where(x => x.Denpyono >= dpf);

        if (!string.IsNullOrEmpty(DenpyonoTo) && decimal.TryParse(DenpyonoTo, out var dpt))
            query = query.Where(x => x.Denpyono <= dpt);

        // helper convert yyyy/MM/dd or yyyy-MM-dd -> yyyy-MM-dd (string column in DB)
        string ToDbDate(string s)
        {
            s = s.Trim();
            if (DateTime.TryParseExact(s, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out var d)
                || DateTime.TryParseExact(s, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out d))
            {
                return d.ToString("yyyy-MM-dd");
            }
            return s.Replace("/", "-");
        }

        if (!string.IsNullOrEmpty(DenpyodtFrom))
        {
            var fromDate = ToDbDate(DenpyodtFrom);
            query = query.Where(x => string.Compare(x.Denpyodt, fromDate, StringComparison.Ordinal) >= 0);
        }

        if (!string.IsNullOrEmpty(DenpyodtTo))
        {
            var toDate = ToDbDate(DenpyodtTo);
            query = query.Where(x => string.Compare(x.Denpyodt, toDate, StringComparison.Ordinal) <= 0);
        }

        if (!string.IsNullOrEmpty(ShinseibFrom))
        {
            var fromDate = ToDbDate(ShinseibFrom);
            query = query.Where(x => string.Compare(x.Uketukedt, fromDate, StringComparison.Ordinal) >= 0);
        }

        if (!string.IsNullOrEmpty(ShinseibTo))
        {
            var toDate = ToDbDate(ShinseibTo);
            query = query.Where(x => string.Compare(x.Uketukedt, toDate, StringComparison.Ordinal) <= 0);
        }

        if (!string.IsNullOrEmpty(ShutsunoHoho))
            query = query.Where(x => x.Suitokb == ShutsunoHoho);

        Results = query.OrderBy(x => x.Denpyono).ToList();

        // giữ lại giá trị input khi dùng <input type="date">
        DenpyodtFrom = string.IsNullOrEmpty(DenpyodtFrom) ? null : ToDbDate(DenpyodtFrom);
        DenpyodtTo = string.IsNullOrEmpty(DenpyodtTo) ? null : ToDbDate(DenpyodtTo);
        ShinseibFrom = string.IsNullOrEmpty(ShinseibFrom) ? null : ToDbDate(ShinseibFrom);
        ShinseibTo = string.IsNullOrEmpty(ShinseibTo) ? null : ToDbDate(ShinseibTo);
    }
}
