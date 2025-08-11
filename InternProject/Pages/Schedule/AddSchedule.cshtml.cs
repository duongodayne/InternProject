using InternProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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

        [BindProperty, Required(ErrorMessage = "???????")]
        public string? Nendo { get; set; }

        [BindProperty, Required(ErrorMessage = "????????")]
        public string? Shinseibi { get; set; }

        [BindProperty]
        public string? ShiharaiYoteibi { get; set; }

        [BindProperty, Required(ErrorMessage = "?????????")]
        public string? Suitokb { get; set; }

        [BindProperty, Required(ErrorMessage = "?????????")]
        public string? Biko { get; set; }

        [BindProperty, Required(ErrorMessage = "?????????")]
        public string? BumoncdYkanr { get; set; }

        // Thêm property ?? hi?n th? tên b? ph?n
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
                // NEW MODE
                IsNew = true;
                Denpyodt = DateTime.Today;
                Shinseibi = DateTime.Today.ToString("yyyy/MM/dd");
                Nendo = DateTime.Today.Year.ToString();
                return;
            }

            // EDIT MODE
            IsNew = false;
            var h = await _context.EsYdenpyos
                .Include(x => x.BumoncdYkanrNavigation) // Include ?? l?y tên b? ph?n
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Denpyono == denpyono.Value);

            if (h == null)
            {
                Message = "????????????";
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
            BumonName = h.BumoncdYkanrNavigation?.Bumonnm; // L?y tên b? ph?n
            Kingaku = h.Kingaku;
        }

        // Thêm method ?? get tên b? ph?n qua AJAX
        public async Task<IActionResult> OnGetBumonNameAsync(string bumoncd)
        {
            if (string.IsNullOrEmpty(bumoncd))
                return new JsonResult(new { name = "" });

            var bumon = await _context.Bumons
                .FirstOrDefaultAsync(x => x.Bumoncd == bumoncd);

            return new JsonResult(new { name = bumon?.Bumonnm ?? "" });
        }

        public async Task<IActionResult> OnPostRegisterAsync()
        {
            BuildNendoItems();

            // Remove validation cho các field không b?t bu?c
            ModelState.Remove("ShiharaiYoteibi");
            ModelState.Remove("Kingaku");
            ModelState.Remove("Denpyono");
            ModelState.Remove("Denpyodt");
            ModelState.Remove("ConfirmDelete");
            ModelState.Remove("Message");
            ModelState.Remove("BumonName");

            if (!ModelState.IsValid)
            {
                Message = "??????????????";
                return Page();
            }

            try
            {
                string shinsebiForDb = ConvertToDbFormat(Shinseibi);
                string? shiharaiForDb = ConvertToDbFormat(ShiharaiYoteibi);
                string denpyodtForDb = (Denpyodt ?? DateTime.Today).ToString("yyyy-MM-dd");

                if (IsNew)
                {
                    // ????
                    var maxNo = await _context.EsYdenpyos
                        .Select(x => x.Denpyono)
                        .DefaultIfEmpty(1000)
                        .MaxAsync();

                    Denpyono = maxNo + 1;

                    var e = new EsYdenpyo
                    {
                        Denpyono = Denpyono.Value,
                        Kaikeind = decimal.Parse(Nendo!),
                        Uketukedt = shinsebiForDb,
                        Denpyodt = denpyodtForDb,
                        Shiharaidt = shiharaiForDb,
                        Suitokb = Suitokb,
                        Biko = Biko,
                        BumoncdYkanr = BumoncdYkanr,
                        Kingaku = Kingaku ?? 0
                    };

                    _context.EsYdenpyos.Add(e);
                    await _context.SaveChangesAsync();

                    TempData["Msg"] = $"??????????{Denpyono}";
                    return RedirectToPage("/Index");
                }
                else
                {
                    var e = await _context.EsYdenpyos.FirstOrDefaultAsync(x => x.Denpyono == Denpyono);
                    if (e == null)
                    {
                        Message = "????????????";
                        return Page();
                    }

                    e.Kaikeind = decimal.Parse(Nendo!);
                    e.Uketukedt = shinsebiForDb;
                    e.Shiharaidt = shiharaiForDb;
                    e.Suitokb = Suitokb;
                    e.Biko = Biko;
                    e.BumoncdYkanr = BumoncdYkanr;
                    e.Kingaku = Kingaku ?? e.Kingaku;

                    await _context.SaveChangesAsync();
                    TempData["Msg"] = $"C?p nh?t d? li?u thành công. ?????{Denpyono}";
                    return RedirectToPage("/Index");
                }
            }
            catch (Exception ex)
            {
                Message = $"??????????: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (string.IsNullOrEmpty(ConfirmDelete))
                return RedirectToPage("/Schedule/AddSchedule", new { denpyono = Denpyono });

            var e = await _context.EsYdenpyos.FirstOrDefaultAsync(x => x.Denpyono == Denpyono);
            if (e != null)
            {
                _context.Remove(e);
                await _context.SaveChangesAsync();
                TempData["Msg"] = "???????";
            }
            return RedirectToPage("/Index");
        }

        private void BuildNendoItems()
        {
            var currentYear = DateTime.Now.Year;
            NendoItems = Enumerable.Range(2020, currentYear - 2020 + 3)
                .Select(y => new SelectListItem($"{y}??", y.ToString()))
                .ToList();
        }

        private string ConvertToDbFormat(string? dateStr)
        {
            if (string.IsNullOrEmpty(dateStr))
                return DateTime.Today.ToString("yyyy-MM-dd");

            return dateStr.Replace("/", "-");
        }

        private string? ConvertDateFormat(string? dateStr)
        {
            if (string.IsNullOrEmpty(dateStr))
                return null;

            return dateStr.Replace("-", "/");
        }

        private DateTime? ConvertToDateTime(string? dateStr)
        {
            if (string.IsNullOrEmpty(dateStr))
                return null;

            if (DateTime.TryParse(dateStr, out var result))
                return result;

            return null;
        }
    }
}
