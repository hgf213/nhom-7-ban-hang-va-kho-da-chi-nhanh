using System;
using System.Collections.Generic;

namespace QuanLyBanHangDaChiNhanh.Models.Entities;

public partial class CtPhieuTraHang
{
    public int MaPth { get; set; }

    public int MaHd { get; set; }

    public int MaSp { get; set; }

    public int SoLuongTra { get; set; }

    public decimal DonGiaHoan { get; set; }

    public decimal ThanhTienHoan { get; set; }

    public string? TinhTrang { get; set; }

    public bool? NhapKho { get; set; }

    public virtual HoaDon MaHdNavigation { get; set; } = null!;

    public virtual PhieuTraHang MaPthNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
