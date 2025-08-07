using InternProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    public IndexModel(AppDbContext context) => _context = context;

    [BindProperty(SupportsGet = true)]
    public string? Nendo { get; set; } // 年度

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

    public void OnGet()
    {
        var query = _context.EsYdenpyos
            .Include(x => x.BumoncdYkanrNavigation) // Đúng tên navigation property
            .Include(x => x.EsYdenpyods) // Bây giờ sẽ work
            .AsQueryable();

        // Lọc theo năm độ
        if (!string.IsNullOrEmpty(Nendo) && decimal.TryParse(Nendo, out decimal nendoValue))
            query = query.Where(x => x.Kaikeind == nendoValue);

        // Lọc theo số phiếu từ - đến
        if (!string.IsNullOrEmpty(DenpyonoFrom) && decimal.TryParse(DenpyonoFrom, out decimal denpyonoFromValue))
            query = query.Where(x => x.Denpyono >= denpyonoFromValue);

        if (!string.IsNullOrEmpty(DenpyonoTo) && decimal.TryParse(DenpyonoTo, out decimal denpyonoToValue))
            query = query.Where(x => x.Denpyono <= denpyonoToValue);

        // Lọc theo ngày phiếu từ - đến
        if (!string.IsNullOrEmpty(DenpyodtFrom))
            query = query.Where(x => string.Compare(x.Denpyodt, DenpyodtFrom) >= 0);

        if (!string.IsNullOrEmpty(DenpyodtTo))
            query = query.Where(x => string.Compare(x.Denpyodt, DenpyodtTo) <= 0);

        // Lọc theo ngày đăng ký từ - đến  
        if (!string.IsNullOrEmpty(ShinseibFrom))
            query = query.Where(x => string.Compare(x.Uketukedt, ShinseibFrom) >= 0);

        if (!string.IsNullOrEmpty(ShinseibTo))
            query = query.Where(x => string.Compare(x.Uketukedt, ShinseibTo) <= 0);

        // Lọc theo phương thức thanh toán (checkbox)
        if (ShutsunoHoho != null && ShutsunoHoho.Any())
            query = query.Where(x => ShutsunoHoho.Contains(x.Suitokb));

        Results = query.OrderBy(x => x.Denpyono).ToList();
    }
}
