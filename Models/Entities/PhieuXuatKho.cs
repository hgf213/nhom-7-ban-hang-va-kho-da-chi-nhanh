using System;
using System.Collections.Generic;

namespace QuanLyBanHangDaChiNhanh.Models.Entities;

public partial class PhieuXuatKho
{
    public int MaPx { get; set; }

    public int MaCn { get; set; }

    public int MaNv { get; set; }

    public DateTime? NgayXuat { get; set; }

    public string? LyDoXuat { get; set; }

    public virtual ICollection<CtPhieuXuat> CtPhieuXuats { get; set; } = new List<CtPhieuXuat>();

    public virtual ChiNhanh MaCnNavigation { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
