using Microsoft.AspNetCore.Mvc;
using QuanLySinhVien.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLySinhVien.Controllers
{
    public class GioHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GioHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GioHang()
        {
            var maSV = HttpContext.Session.GetString("MaSv");
            if (string.IsNullOrEmpty(maSV))
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }

            var dangKyList = await _context.DangKies
                .Include(dk => dk.MaHps)
                .Where(dk => dk.MaSv == maSV)
                .ToListAsync();

            return View(dangKyList);
        }


        [HttpPost]
        public async Task<IActionResult> XoaDangKy(string maSv, string maHp)
        {
            // Tìm kiếm đăng ký của sinh viên trong cơ sở dữ liệu  
            var dangKy = await _context.DangKies
                .Include(dk => dk.MaHps)
                .FirstOrDefaultAsync(dk => dk.MaSv == maSv && dk.MaHps.Any(hp => hp.MaHp == maHp));

            if (dangKy != null)
            {
                // Tìm và xóa học phần khỏi danh sách học phần đã đăng ký  
                var hocPhanToRemove = dangKy.MaHps.FirstOrDefault(hp => hp.MaHp == maHp);
                if (hocPhanToRemove != null)
                {
                    dangKy.MaHps.Remove(hocPhanToRemove);

                    // Nếu không còn học phần nào, xóa cả bản ghi đăng ký  
                    if (!dangKy.MaHps.Any())
                    {
                        _context.DangKies.Remove(dangKy);
                    }

                    await _context.SaveChangesAsync();
                }
            }

            // Chuyển hướng về trang giỏ hàng  
            return RedirectToAction("GioHang");
        }
        [HttpPost]
        public async Task<IActionResult> LuuDangKy(string maSv, List<string> maHps)
        {
            if (string.IsNullOrEmpty(maSv) || maHps == null || !maHps.Any())
            {
                ModelState.AddModelError("", "Vui lòng chọn sinh viên và học phần.");
                return RedirectToAction("DatHang");
            }

            // Kiểm tra xem đã tồn tại đăng ký chưa
            var dangKy = await _context.DangKies
                .Include(dk => dk.MaHps)
                .FirstOrDefaultAsync(dk => dk.MaSv == maSv);

            if (dangKy == null)
            {
                // Nếu chưa tồn tại, tạo mới
                dangKy = new DangKy
                {
                    MaSv = maSv,
                    NgayDk = DateOnly.FromDateTime(DateTime.Now),
                    MaHps = new List<HocPhan>()
                };

                _context.DangKies.Add(dangKy);
            }

            // Thêm học phần vào đăng ký
            foreach (var maHp in maHps)
            {
                var hocPhan = await _context.HocPhans.FindAsync(maHp);
                if (hocPhan != null && !dangKy.MaHps.Any(hp => hp.MaHp == maHp))
                {
                    dangKy.MaHps.Add(hocPhan);
                }
            }

            await _context.SaveChangesAsync();

            // Chuyển hướng về trang GioHang
            return RedirectToAction("GioHang");
        }





        public IActionResult DatHang(string maSv)
        {
            // Truy xuất thông tin sinh viên và đăng ký học phần
            var dangKys = _context.DangKies
                .Where(d => d.MaSv == maSv)
                .Include(d => d.MaHps) // Đảm bảo load danh sách học phần
                .ToList();

            // Truyền dữ liệu vào view
            return View(dangKys);
        }

    }
}