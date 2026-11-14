using System;
using System.Collections.Generic;

namespace QuanLyBanHangDaChiNhanh.Models.Entities;

public partial class ChiNhanh
{
    public int MaCn { get; set; }

    public string TenCn { get; set; } = null!;

    public string? DiaChi { get; set; }

    public string? SoDienThoai { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();

    public virtual ICollection<PhieuDieuChuyen> PhieuDieuChuyenMaCngiaoNavigations { get; set; } = new List<PhieuDieuChuyen>();

    public virtual ICollection<PhieuDieuChuyen> PhieuDieuChuyenMaCnnhanNavigations { get; set; } = new List<PhieuDieuChuyen>();

    public virtual ICollection<PhieuKiemKe> PhieuKiemKes { get; set; } = new List<PhieuKiemKe>();

    public virtual ICollection<PhieuNhap> PhieuNhaps { get; set; } = new List<PhieuNhap>();

    public virtual ICollection<PhieuXuatKho> PhieuXuatKhos { get; set; } = new List<PhieuXuatKho>();

    public virtual ICollection<TonKho> TonKhos { get; set; } = new List<TonKho>();
}
