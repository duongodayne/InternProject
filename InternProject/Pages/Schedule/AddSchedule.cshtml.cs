using InternProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace InternProject.Pages.Schedule
{
    public class AddScheduleModel : PageModel
    {
        private readonly AppDbContext _context;

        public AddScheduleModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty] public bool IsNew { get; set; }
        [BindProperty] public decimal? Denpyono { get; set; }
        [BindProperty] public DateTime? Denpyodt { get; set; }
        public string DenpyodtDisplay => (Denpyodt ?? DateTime.Today).ToString("yyyy/MM/dd");

        [BindProperty, Required(ErrorMessage = "年度は必須です。")]
        public string? Nendo { get; set; }

        [BindProperty, Required(ErrorMessage = "申請日は必須です。"),
         RegularExpression(@"^\d{4}/\d{2}/\d{2}$", ErrorMessage = "申請日は yyyy/MM/dd 形式です。")]
        public string? Shinseibi { get; set; }

        [BindProperty, RegularExpression(@"^\d{4}/\d{2}/\d{2}$", ErrorMessage = "支払予定日は yyyy/MM/dd 形式です。")]
        public string? ShiharaiYoteibi { get; set; }

        [BindProperty, Required(ErrorMessage = "出納方法は必須です。")]
        public string? Suitokb { get; set; }

        [BindProperty, Required(ErrorMessage = "出張目的は必須です。")]
        public string? Biko { get; set; }

        [BindProperty, Required(ErrorMessage = "起票部門は必須です。"),
         RegularExpression(@"^\d+$", ErrorMessage = "起票部門は数字です。")]
        public string? BumoncdYkanr { get; set; }

        public string? BumonName { get; set; }
        [BindProperty] public decimal? Kingaku { get; set; }
        [BindProperty] public string? ConfirmDelete { get; set; }

        public List<SelectListItem> NendoItems { get; set; } = new();
        public string? Message { get; set; }

        public async Task OnGetAsync(decimal? denpyono)
        {
            BuildNendoItems();

            if (denpyono is null)
            {
                IsNew = true;
                Denpyodt = DateTime.Today;
                Shinseibi = DateTime.Today.ToString("yyyy/MM/dd");
                Nendo = DateTime.Today.Year.ToString();
                return;
            }

            IsNew = false;
            var h = await _context.EsYdenpyos
                .Include(x => x.BumoncdYkanrNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Denpyono == denpyono.Value);

            if (h == null)
            {
                Message = "データが見つかりません。";
                IsNew = true;
                Denpyodt = DateTime.Today;
                Shinseibi = DateTime.Today.ToString("yyyy/MM/dd");
                return;
            }

            Denpyono = h.Denpyono;
            Denpyodt = ConvertToDateTime(h.Denpyodt);
            Nendo = h.Kaikeind?.ToString();
            Shinseibi = ConvertDateFormat(h.Uketukedt);
            ShiharaiYoteibi = ConvertDateFormat(h.Shiharaidt);
            Suitokb = h.Suitokb;
            Biko = h.Biko;
            BumoncdYkanr = h.BumoncdYkanr;
            BumonName = h.BumoncdYkanrNavigation?.Bumonnm;
            Kingaku = h.Kingaku;
        }

        public async Task<IActionResult> OnGetBumonNameAsync(string bumoncd)
        {
            if (string.IsNullOrEmpty(bumoncd))
                return new JsonResult(new { name = "" });

            var bumon = await _context.Bumons.FirstOrDefaultAsync(x => x.Bumoncd == bumoncd);
            return new JsonResult(new { name = bumon?.Bumonnm ?? "" });
        }

        public async Task<IActionResult> OnPostRegisterAsync()
        {
            BuildNendoItems();

            ModelState.Remove("Denpyono");
            ModelState.Remove("Denpyodt");
            ModelState.Remove("ConfirmDelete");
            ModelState.Remove("Message");
            ModelState.Remove("BumonName");
            ModelState.Remove("Kingaku");

            if (!ModelState.IsValid)
            {
                Message = "入力内容を確認してください。";
                return Page();
            }

            try
            {
                DateTime shinsei = DateTime.ParseExact(Shinseibi!, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime? shiharai = string.IsNullOrWhiteSpace(ShiharaiYoteibi)
                    ? (DateTime?)null
                    : DateTime.ParseExact(ShiharaiYoteibi!, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                string shinseiDb = shinsei.ToString("yyyy-MM-dd");
                string? shiharaiDb = shiharai?.ToString("yyyy-MM-dd");
                string denpyodtForDb = DateTime.Today.ToString("yyyy-MM-dd");

                if (IsNew)
                {
                    // ==== FIX ORACLE: KHÔNG DÙNG Any() ====
                    // Lấy MAX nullable, nếu null thì dùng 1000
                    using var tx = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

                    var maxNoNullable = await _context.EsYdenpyos
                        .Select(x => (decimal?)x.Denpyono)
                        .MaxAsync();                          // -> SELECT MAX(DENPYONO) FROM ES_YDENPYO

                    var maxNo = maxNoNullable ?? 1000m;      // Oracle dịch thành NVL/COALESCE, không có FALSE/TRUE
                    Denpyono = maxNo + 1;

                    var e = new EsYdenpyo
                    {
                        Denpyono = Denpyono.Value,
                        Kaikeind = decimal.Parse(Nendo!),
                        Uketukedt = shinseiDb,
                        Denpyodt = denpyodtForDb,
                        Shiharaidt = shiharaiDb,
                        Suitokb = Suitokb,
                        Biko = Biko,
                        BumoncdYkanr = BumoncdYkanr,
                        Kingaku = Kingaku ?? 0
                    };

                    _context.EsYdenpyos.Add(e);
                    await _context.SaveChangesAsync();
                    await tx.CommitAsync();

                    // Ở NEW: ở lại trang, thông báo & clear form
                    Message = $"登録に成功しました。伝票番号: {Denpyono}";

                    Denpyono = null;
                    Denpyodt = DateTime.Today;
                    Shinseibi = DateTime.Today.ToString("yyyy/MM/dd");
                    ShiharaiYoteibi = null;
                    Suitokb = null;
                    Biko = null;
                    BumoncdYkanr = null;
                    BumonName = null;
                    Kingaku = null;
                    IsNew = true;

                    return Page();
                }
                else
                {
                    var e = await _context.EsYdenpyos.FirstOrDefaultAsync(x => x.Denpyono == Denpyono);
                    if (e == null)
                    {
                        Message = "データが見つかりません。";
                        return Page();
                    }

                    e.Kaikeind = decimal.Parse(Nendo!);
                    e.Uketukedt = shinseiDb;
                    e.Shiharaidt = shiharaiDb;
                    e.Suitokb = Suitokb;
                    e.Biko = Biko;
                    e.BumoncdYkanr = BumoncdYkanr;
                    e.Kingaku = Kingaku ?? e.Kingaku;
                    e.Denpyodt = denpyodtForDb;

                    await _context.SaveChangesAsync();

                    TempData["Msg"] = $"Cập nhật dữ liệu thành công. 伝票番号: {Denpyono}";
                    return RedirectToPage("/Index");
                }
            }
            catch (Exception ex)
            {
                Message = $"エラーが発生しました: {ex.Message}";
                return Page();
            }
        }


        public async Task<IActionResult> OnPostDeleteAsync()
        {

            if (string.IsNullOrEmpty(ConfirmDelete))
            {
                return RedirectToPage("/Schedule/AddSchedule", new { denpyono = Denpyono });
            }

            if (Denpyono == null || IsNew)
            {
                TempData["Msg"] = "Xóa thất bại: không có 伝票番号 hoặc đang ở chế độ tạo mới.";
                return RedirectToPage("/Index");
            }

            using var tx = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);
            try
            {
                // Xóa header trước - cascade delete sẽ tự động xóa details
                var header = await _context.EsYdenpyos
                    .Include(x => x.EsYdenpyods) // Include để load related data
                    .FirstOrDefaultAsync(x => x.Denpyono == Denpyono.Value);

                if (header == null)
                {
                    await tx.RollbackAsync();
                    TempData["Msg"] = "Không tìm thấy dữ liệu để xóa.";
                    return RedirectToPage("/Index");
                }

                // Log số lượng detail records trước khi xóa
                var detailsCount = header.EsYdenpyods.Count;

                // Xóa header - cascade delete sẽ tự động xóa tất cả details
                _context.EsYdenpyos.Remove(header);
                var affectedRows = await _context.SaveChangesAsync();



                await tx.CommitAsync();

                TempData["Msg"] = "削除に成功しました。";
                return RedirectToPage("/Index");
            }
            catch (DbUpdateException ex)
            {
                await tx.RollbackAsync();
                // log inner exception từ Oracle (lý do FK, v.v.)
                TempData["Msg"] = $"Xóa thất bại (DB). {ex.InnerException?.Message ?? ex.Message}";
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                TempData["Msg"] = $"Xóa thất bại. {ex.Message}";
                return RedirectToPage("/Index");
            }
        }



        private void BuildNendoItems()
        {
            var currentYear = DateTime.Now.Year;
            NendoItems = Enumerable.Range(2020, currentYear - 2020 + 3)
                .Select(y => new SelectListItem($"{y}年度", y.ToString()))
                .ToList();
        }

        private string? ConvertDateFormat(string? dateStr)
        {
            if (string.IsNullOrEmpty(dateStr)) return null;
            return dateStr.Replace("-", "/");
        }

        private DateTime? ConvertToDateTime(string? dateStr)
        {
            if (string.IsNullOrEmpty(dateStr)) return null;
            if (DateTime.TryParse(dateStr, out var result)) return result;
            return null;
        }
    }
}
