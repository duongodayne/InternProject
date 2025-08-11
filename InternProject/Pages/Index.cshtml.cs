using InternProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    public IndexModel(AppDbContext context) => _context = context;

    [BindProperty(SupportsGet = true)]
    public int? Nendo { get; set; } // 年度

    [BindProperty(SupportsGet = true)]
    public string? DenpyonoFrom { get; set; } // 伝票番号 từ

    [BindProperty(SupportsGet = true)]
    public string? DenpyonoTo { get; set; } // 伝票番号 đến

    [BindProperty(SupportsGet = true)]
    public string? DenpyodtFrom { get; set; } // 伝票日付 từ

    [BindProperty(SupportsGet = true)]
    public string? DenpyodtTo { get; set; } // 伝票日付 đến

    [BindProperty(SupportsGet = true)]
    public string? ShinseibFrom { get; set; } // 申請日 từ

    [BindProperty(SupportsGet = true)]
    public string? ShinseibTo { get; set; } // 申請日 đến

    [BindProperty(SupportsGet = true)]
    public List<string> ShutsunoHoho { get; set; } = new(); // 出納方法 (checkbox)

    public List<EsYdenpyo> Results { get; set; } = new();

    // Danh sách năm cho dropdown
    public List<int> AvailableYears { get; set; } = new();

    public void OnGet()
    {
        // Tạo danh sách năm (từ 2020 đến năm hiện tại + 2)
        var currentYear = DateTime.Now.Year;
        AvailableYears = Enumerable.Range(2020, currentYear - 2020 + 3).ToList();

        var query = _context.EsYdenpyos
            .Include(x => x.BumoncdYkanrNavigation)
            .Include(x => x.EsYdenpyods)
            .AsQueryable();

        // 年度
        if (Nendo.HasValue)
            query = query.Where(x => x.Kaikeind == Nendo.Value);

        if (!string.IsNullOrEmpty(DenpyonoFrom) && decimal.TryParse(DenpyonoFrom, out decimal denpyonoFromValue))
            query = query.Where(x => x.Denpyono >= denpyonoFromValue);

        if (!string.IsNullOrEmpty(DenpyonoTo) && decimal.TryParse(DenpyonoTo, out decimal denpyonoToValue))
            query = query.Where(x => x.Denpyono <= denpyonoToValue);

        if (!string.IsNullOrEmpty(DenpyodtFrom))
        {
            // Nếu input là yyyy/MM/dd, chuyển sang yyyy-MM-dd để so sánh với DB
            string fromDate = DenpyodtFrom.Contains("/")
                ? DenpyodtFrom.Replace("/", "-")  // yyyy/MM/dd -> yyyy-MM-dd
                : DenpyodtFrom;                    // Giữ nguyên nếu đã là yyyy-MM-dd

            query = query.Where(x => string.Compare(x.Denpyodt, fromDate) >= 0);
        }

        if (!string.IsNullOrEmpty(DenpyodtTo))
        {
            string toDate = DenpyodtTo.Contains("/")
                ? DenpyodtTo.Replace("/", "-")
                : DenpyodtTo;

            query = query.Where(x => string.Compare(x.Denpyodt, toDate) <= 0);
        }

        if (!string.IsNullOrEmpty(ShinseibFrom))
        {
            string fromDate = ShinseibFrom.Contains("/")
                ? ShinseibFrom.Replace("/", "-")
                : ShinseibFrom;

            query = query.Where(x => string.Compare(x.Uketukedt, fromDate) >= 0);
        }

        if (!string.IsNullOrEmpty(ShinseibTo))
        {
            string toDate = ShinseibTo.Contains("/")
                ? ShinseibTo.Replace("/", "-")
                : ShinseibTo;

            query = query.Where(x => string.Compare(x.Uketukedt, toDate) <= 0);
        }

        if (ShutsunoHoho != null && ShutsunoHoho.Any())
            query = query.Where(x => ShutsunoHoho.Contains(x.Suitokb));

        Results = query.OrderBy(x => x.Denpyono).ToList();
    }


}
