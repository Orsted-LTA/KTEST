using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Models;

var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ session
builder.Services.AddDistributedMemoryCache(); // Cấu hình bộ nhớ lưu trữ cho session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Cài đặt thời gian hết hạn của session
    options.Cookie.HttpOnly = true; // Chỉ có thể truy cập cookie từ phía server
    options.Cookie.IsEssential = true; // Đảm bảo session có thể hoạt động ngay cả khi không bật cookie
});

// Add services to the container.  
builder.Services.AddControllersWithViews();

// Cấu hình ApplicationDbContext với chuỗi kết nối  
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Cấu hình middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Thêm middleware session
app.UseSession();

app.UseAuthorization();

// Cấu hình routing cho GioHang  
app.MapControllerRoute(
    name: "giohang",
    pattern: "GioHang/GioHang",
    defaults: new { controller = "GioHang", action = "GioHang" });

// Cấu hình routing mặc định  
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=SinhVien}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "home",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
