using System;
using System.Collections.Generic;

namespace QuanLyBanHangDaChiNhanh.Models.Entities;

public partial class CtPhieuNhap
{
    public int MaPn { get; set; }

    public int MaSp { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGiaNhap { get; set; }

    public decimal ThanhTien { get; set; }

    public virtual PhieuNhap MaPnNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
