using System;
using System.Collections.Generic;

namespace QuanLyBanHangDaChiNhanh.Models.Entities;

public partial class CtKiemKe
{
    public int MaKk { get; set; }

    public int MaSp { get; set; }

    public int SoLuongHeThong { get; set; }

    public int SoLuongThucTe { get; set; }

    public int ChanhLech { get; set; }

    public virtual PhieuKiemKe MaKkNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
