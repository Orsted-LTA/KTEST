using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLySinhVien.Models;

public partial class SinhVien
{
    [StringLength(10)]
    public string? MaSv { get; set; }
    [StringLength(10)]

    public string HoTen { get; set; } = null!;

    public string? GioiTinh { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string? Hinh { get; set; }

    public string? MaNganh { get; set; }

    public virtual ICollection<DangKy> DangKies { get; set; } = new List<DangKy>();

    public virtual NganhHoc? MaNganhNavigation { get; set; }
}
