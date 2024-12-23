using Microsoft.AspNetCore.Mvc;
using QuanLySinhVien.Models;

public class NguoiDungController : Controller
{
    // Hiển thị trang đăng nhập
    [HttpGet]
    public IActionResult DangNhap()
    {
        return View();
    }

    // Xử lý đăng nhập
    [HttpPost]
    public IActionResult DangNhap(DangNhapModel model)
    {
        if (ModelState.IsValid)
        {
            // Xác thực mã sinh viên và mật khẩu (mật khẩu là mã sinh viên)
            if (model.MaSV == model.MatKhau) // So sánh mã sinh viên với mật khẩu
            {
                // Đăng nhập thành công, lưu mã sinh viên vào session
                HttpContext.Session.SetString("MaSv", model.MaSV);

                // Chuyển hướng đến trang chính hoặc trang học phần
                return RedirectToAction("Index", "HocPhan");
            }
            else
            {
                // Mã sinh viên hoặc mật khẩu không đúng
                ModelState.AddModelError("", "Mã sinh viên hoặc mật khẩu không hợp lệ.");
            }
        }
        return View(model);
    }

    // Đăng xuất (nếu cần)
    public IActionResult DangXuat()
    {
        // Xóa session khi người dùng đăng xuất
        HttpContext.Session.Remove("MaSv");
        return RedirectToAction("DangNhap");
    }
}
