using System;
using System.Collections.Generic;

namespace QuanLyBanHangDaChiNhanh.Models.Entities;

public partial class PhieuNhap
{
    public int MaPn { get; set; }

    public int MaCn { get; set; }

    public int MaNcc { get; set; }

    public int MaNv { get; set; }

    public DateTime? NgayNhap { get; set; }

    public decimal? TongTien { get; set; }

    public virtual ICollection<CtPhieuNhap> CtPhieuNhaps { get; set; } = new List<CtPhieuNhap>();

    public virtual ChiNhanh MaCnNavigation { get; set; } = null!;

    public virtual NhaCungCap MaNccNavigation { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
