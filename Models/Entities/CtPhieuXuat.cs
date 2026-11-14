using System;
using System.Collections.Generic;

namespace QuanLyBanHangDaChiNhanh.Models.Entities;

public partial class CtPhieuXuat
{
    public int MaPx { get; set; }

    public int MaSp { get; set; }

    public int SoLuong { get; set; }

    public virtual PhieuXuatKho MaPxNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
