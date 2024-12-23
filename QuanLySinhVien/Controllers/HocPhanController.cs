using Microsoft.AspNetCore.Mvc;
using QuanLySinhVien.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLySinhVien.Controllers
{
    public class HocPhanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HocPhanController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hocPhans = await _context.HocPhans.ToListAsync();
            return View(hocPhans);
        }

        [HttpPost]
        public async Task<IActionResult> DangKy(string maHp)
        {
            var maSV = "2080601424"; // Mã sinh viên, cần thay thế bằng giá trị thực tế từ session hoặc authentication  

            // Kiểm tra học phần có tồn tại không  
            var hocPhan = await _context.HocPhans.FindAsync(maHp);
            if (hocPhan == null)
            {
                TempData["ErrorMessage"] = "Học phần không tồn tại.";
                return RedirectToAction(nameof(Index)); // Quay về trang danh sách học phần  
            }

            // Kiểm tra xem sinh viên đã đăng ký học phần này chưa  
            var existingRegistration = await _context.DangKies
                .Include(dk => dk.MaHps)
                .FirstOrDefaultAsync(dk => dk.MaSv == maSV && dk.MaHps.Any(hp => hp.MaHp == maHp));

            if (existingRegistration != null)
            {
                TempData["ErrorMessage"] = "Sinh viên đã đăng ký học phần này.";
                return RedirectToAction(nameof(Index));
            }

            // Đăng ký học phần cho sinh viên  
            var dangKy = new DangKy
            {
                MaSv = maSV,
                NgayDk = DateOnly.FromDateTime(DateTime.Now),
                MaHps = new List<HocPhan> { hocPhan }
            };

            // Thêm thông tin đăng ký vào cơ sở dữ liệu  
            _context.DangKies.Add(dangKy);
            await _context.SaveChangesAsync();

            // Chuyển hướng đến trang giỏ hàng  
            return RedirectToAction("GioHang", "GioHang");
        }
    }
}