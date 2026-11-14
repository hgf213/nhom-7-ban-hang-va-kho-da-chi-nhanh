using System;
using System.Collections.Generic;

namespace QuanLyBanHangDaChiNhanh.Models.Entities;

public partial class TaiKhoan
{
    public int MaTk { get; set; }

    public int MaNv { get; set; }

    public int MaVaiTro { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhauHash { get; set; } = null!;

    public string? TrangThai { get; set; }

    public virtual NhanVien MaNvNavigation { get; set; } = null!;

    public virtual VaiTro MaVaiTroNavigation { get; set; } = null!;
}
