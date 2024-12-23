using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // L?y m? sinh vi�n t? session
            var maSv = HttpContext.Session.GetString("MaSv");

            // Ki?m tra n?u m? sinh vi�n kh�ng c� trong session (ch�a ��ng nh?p)
            if (string.IsNullOrEmpty(maSv))
            {
                // N?u ch�a ��ng nh?p, chuy?n h�?ng �?n trang ��ng nh?p
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
