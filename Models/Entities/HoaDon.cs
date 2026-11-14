using System;
using System.Collections.Generic;

namespace QuanLyBanHangDaChiNhanh.Models.Entities;

public partial class HoaDon
{
    public int MaHd { get; set; }

    public int MaCn { get; set; }

    public int MaNv { get; set; }

    public int MaKh { get; set; }

    public DateTime? NgayLap { get; set; }

    public decimal? TongTien { get; set; }

    public decimal? TongTienGiam { get; set; }

    public string? PhuongThucThanhToan { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<CtHoaDon> CtHoaDons { get; set; } = new List<CtHoaDon>();

    public virtual ICollection<CtPhieuTraHang> CtPhieuTraHangs { get; set; } = new List<CtPhieuTraHang>();

    public virtual ChiNhanh MaCnNavigation { get; set; } = null!;

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;

    public virtual ICollection<PhieuTraHang> PhieuTraHangs { get; set; } = new List<PhieuTraHang>();
}
