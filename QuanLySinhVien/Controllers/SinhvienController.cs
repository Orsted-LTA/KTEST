using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Controllers
{
    public class SinhVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SinhVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SinhVien
        public async Task<IActionResult> Index()
        {
            var sinhViens = await _context.SinhViens.Include(s => s.MaNganhNavigation).ToListAsync();
            return View(sinhViens);
        }

        // GET: SinhVien/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhViens
                .Include(s => s.MaNganhNavigation)
                .FirstOrDefaultAsync(m => m.MaSv == id);

            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }

        // GET: SinhVien/Create
        public IActionResult Create()
        {
            ViewData["MaNganh"] = new SelectList(_context.NganhHocs, "MaNganh", "TenNganh");
            return View();
        }

        // POST: SinhVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSv,HoTen,GioiTinh,NgaySinh,Hinh,MaNganh")] SinhVien sinhVien, IFormFile hinh)
        {
            if (ModelState.IsValid)
            {
                // Xử lý lưu hình ảnh
                if (hinh != null && hinh.Length > 0)
                {
                    // Sử dụng phương thức SaveImage để lưu hình ảnh
                    var imagePath = await SaveImage(hinh);
                    sinhVien.Hinh = imagePath; // Lưu đường dẫn hình ảnh vào cơ sở dữ liệu
                }

                _context.Add(sinhVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNganh"] = new SelectList(_context.NganhHocs, "MaNganh", "TenNganh", sinhVien.MaNganh);
            return View(sinhVien);
        }
        private async Task<string> SaveImage(IFormFile image)
        {
            // Thay đổi đường dẫn theo cấu hình của bạn
            var savePath = Path.Combine("wwwroot/images", image.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName; // Trả về đường dẫn tương đối
        }



        // GET: SinhVien/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien == null)
            {
                return NotFound();
            }

            // Truyền danh sách ngành học vào ViewBag
            ViewBag.MaNganh = new SelectList(_context.NganhHocs, "MaNganh", "TenNganh", sinhVien.MaNganh);

            return View(sinhVien);
        }


        // POST: SinhVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaSv,HoTen,GioiTinh,NgaySinh,Hinh,MaNganh")] SinhVien sinhVien, IFormFile hinh)
        {
            if (id != sinhVien.MaSv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra và lưu hình ảnh nếu có
                    if (hinh != null && hinh.Length > 0)
                    {
                        var imagePath = await SaveImage(hinh);
                        sinhVien.Hinh = imagePath; // Lưu đường dẫn hình ảnh vào cơ sở dữ liệu
                    }
                    else if (string.IsNullOrEmpty(sinhVien.Hinh))
                    {
                        // Kiểm tra nếu không chọn hình và sinh viên chưa có hình ảnh
                        ModelState.AddModelError("Hinh", "Vui lòng chọn hình ảnh hoặc giữ hình ảnh hiện tại.");
                    }

                    _context.Update(sinhVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SinhVienExists(sinhVien.MaSv))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));  // Quay lại trang danh sách sau khi cập nhật
            }

            // Nếu model không hợp lệ, truyền lại danh sách ngành học vào ViewBag
            ViewData["MaNganh"] = new SelectList(_context.NganhHocs, "MaNganh", "TenNganh", sinhVien.MaNganh);
            return View(sinhVien);
        }




        // GET: SinhVien/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhViens
                .Include(s => s.MaNganhNavigation)
                .FirstOrDefaultAsync(m => m.MaSv == id);
            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }

        // POST: SinhVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien != null)
            {
                _context.SinhViens.Remove(sinhVien);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index)); // Quay lại trang Index
        }


        private bool SinhVienExists(string id)
        {
            return _context.SinhViens.Any(e => e.MaSv == id);
        }
    }
}
