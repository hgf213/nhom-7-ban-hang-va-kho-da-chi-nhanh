using System;
using System.Collections.Generic;

namespace QuanLyBanHangDaChiNhanh.Models.Entities;

public partial class PhieuKiemKe
{
    public int MaKk { get; set; }

    public int MaCn { get; set; }

    public int MaNv { get; set; }

    public DateTime? NgayKiemKe { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<CtKiemKe> CtKiemKes { get; set; } = new List<CtKiemKe>();

    public virtual ChiNhanh MaCnNavigation { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
