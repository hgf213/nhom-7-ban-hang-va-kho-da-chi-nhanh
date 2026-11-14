using System;
using System.Collections.Generic;

namespace QuanLyBanHangDaChiNhanh.Models.Entities;

public partial class CtDieuChuyen
{
    public int MaPdc { get; set; }

    public int MaSp { get; set; }

    public int SoLuong { get; set; }

    public virtual PhieuDieuChuyen MaPdcNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
