using InternProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace InternProject.Pages.YDenpyo;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    public IndexModel(AppDbContext context) => _context = context;

    // ===== Filters =====
    [BindProperty(SupportsGet = true)] public int? Nendo { get; set; }
    [BindProperty(SupportsGet = true)] public long? DenpyonoFrom { get; set; }
    [BindProperty(SupportsGet = true)] public long? DenpyonoTo { get; set; }
    [BindProperty(SupportsGet = true)] public DateTime? DenpyodtFrom { get; set; }
    [BindProperty(SupportsGet = true)] public DateTime? DenpyodtTo { get; set; }
    [BindProperty(SupportsGet = true)] public DateTime? ShinseibFrom { get; set; }
    [BindProperty(SupportsGet = true)] public DateTime? ShinseibTo { get; set; }
    [BindProperty(SupportsGet = true)] public List<string> ShutsunoHoho { get; set; } = new();

    // ===== View data =====
    public List<int> AvailableYears { get; set; } = new();
    public List<EsYdenpyo> Results { get; set; } = new();

    public void OnGet()
    {
        // 年度: 10 năm gần nhất (hiển thị từ hiện tại đổ về trước)
        var thisYear = DateTime.Today.Year;
        AvailableYears = Enumerable.Range(thisYear - 9, 10).Reverse().ToList();

        var q = _context.EsYdenpyos
            .Include(x => x.BumoncdYkanrNavigation)
            .AsQueryable();

        // 年度
        if (Nendo.HasValue)
            q = q.Where(x => x.Kaikeind == Nendo.Value);

        // 伝票番号 From-To
        if (DenpyonoFrom.HasValue) q = q.Where(x => x.Denpyono >= DenpyonoFrom.Value);
        if (DenpyonoTo.HasValue) q = q.Where(x => x.Denpyono <= DenpyonoTo.Value);

        // 伝票日付 From-To  (DB đang lưu chuỗi -> so sánh cùng format)
        if (DenpyodtFrom.HasValue) q = q.Where(x => string.Compare(x.Denpyodt, ToDbDate(DenpyodtFrom.Value)) >= 0);
        if (DenpyodtTo.HasValue) q = q.Where(x => string.Compare(x.Denpyodt, ToDbDate(DenpyodtTo.Value)) <= 0);

        // 申請日 From-To
        if (ShinseibFrom.HasValue) q = q.Where(x => string.Compare(x.Uketukedt, ToDbDate(ShinseibFrom.Value)) >= 0);
        if (ShinseibTo.HasValue) q = q.Where(x => string.Compare(x.Uketukedt, ToDbDate(ShinseibTo.Value)) <= 0);

        // 出納方法 (checkbox multi-select)
        if (ShutsunoHoho?.Any() == true)
            q = q.Where(x => ShutsunoHoho.Contains(x.Suitokb));

        Results = q.OrderByDescending(x => x.Denpyono).ToList();
    }

    // NOTE: Chọn 1 format lưu trong DB và dùng nhất quán. Ở đây giả định DB lưu "yyyy-MM-dd".
    private static string ToDbDate(DateTime dt) => dt.ToString("yyyy-MM-dd");
}
