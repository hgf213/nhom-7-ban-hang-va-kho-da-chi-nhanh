using System;
using System.Collections.Generic;

namespace QuanLyBanHangDaChiNhanh.Models.Entities;

public partial class PhieuTraHang
{
    public int MaPth { get; set; }

    public int MaHd { get; set; }

    public int MaNv { get; set; }

    public DateTime? NgayTra { get; set; }

    public string? LyDoTra { get; set; }

    public decimal? ThanhTienHoan { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<CtPhieuTraHang> CtPhieuTraHangs { get; set; } = new List<CtPhieuTraHang>();

    public virtual HoaDon MaHdNavigation { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
