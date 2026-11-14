using System;
using System.Collections.Generic;

namespace QuanLyBanHangDaChiNhanh.Models.Entities;

public partial class SanPham
{
    public int MaSp { get; set; }

    public int MaDm { get; set; }

    public string TenSp { get; set; } = null!;

    public string? DonViTinh { get; set; }

    public decimal GiaBan { get; set; }

    public string? MoTa { get; set; }

    public virtual ICollection<CtDieuChuyen> CtDieuChuyens { get; set; } = new List<CtDieuChuyen>();

    public virtual ICollection<CtHoaDon> CtHoaDons { get; set; } = new List<CtHoaDon>();

    public virtual ICollection<CtKiemKe> CtKiemKes { get; set; } = new List<CtKiemKe>();

    public virtual ICollection<CtPhieuNhap> CtPhieuNhaps { get; set; } = new List<CtPhieuNhap>();

    public virtual ICollection<CtPhieuTraHang> CtPhieuTraHangs { get; set; } = new List<CtPhieuTraHang>();

    public virtual ICollection<CtPhieuXuat> CtPhieuXuats { get; set; } = new List<CtPhieuXuat>();

    public virtual DanhMuc MaDmNavigation { get; set; } = null!;

    public virtual ICollection<TonKho> TonKhos { get; set; } = new List<TonKho>();

    public virtual ICollection<KhuyenMai> MaKms { get; set; } = new List<KhuyenMai>();
}
