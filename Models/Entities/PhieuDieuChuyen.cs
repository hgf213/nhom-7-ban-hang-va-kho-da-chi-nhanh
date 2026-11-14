using System;
using System.Collections.Generic;

namespace QuanLyBanHangDaChiNhanh.Models.Entities;

public partial class PhieuDieuChuyen
{
    public int MaPdc { get; set; }

    public int MaCngiao { get; set; }

    public int MaCnnhan { get; set; }

    public int MaNv { get; set; }

    public DateTime? NgayChuyen { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<CtDieuChuyen> CtDieuChuyens { get; set; } = new List<CtDieuChuyen>();

    public virtual ChiNhanh MaCngiaoNavigation { get; set; } = null!;

    public virtual ChiNhanh MaCnnhanNavigation { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
