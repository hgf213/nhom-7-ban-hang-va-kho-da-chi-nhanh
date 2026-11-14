using System;
using System.Collections.Generic;

namespace QuanLyBanHangDaChiNhanh.Models.Entities;

public partial class CtHoaDon
{
    public int MaHd { get; set; }

    public int MaSp { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public decimal? TienGiam { get; set; }

    public decimal ThanhTien { get; set; }

    public virtual HoaDon MaHdNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
