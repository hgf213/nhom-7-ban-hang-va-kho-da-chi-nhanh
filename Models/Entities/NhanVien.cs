using System;
using System.Collections.Generic;

namespace QuanLyBanHangDaChiNhanh.Models.Entities;

public partial class NhanVien
{
    public int MaNv { get; set; }

    public int MaCn { get; set; }

    public string HoTen { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public string? Email { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual ChiNhanh MaCnNavigation { get; set; } = null!;

    public virtual ICollection<PhieuDieuChuyen> PhieuDieuChuyens { get; set; } = new List<PhieuDieuChuyen>();

    public virtual ICollection<PhieuKiemKe> PhieuKiemKes { get; set; } = new List<PhieuKiemKe>();

    public virtual ICollection<PhieuNhap> PhieuNhaps { get; set; } = new List<PhieuNhap>();

    public virtual ICollection<PhieuTraHang> PhieuTraHangs { get; set; } = new List<PhieuTraHang>();

    public virtual ICollection<PhieuXuatKho> PhieuXuatKhos { get; set; } = new List<PhieuXuatKho>();

    public virtual TaiKhoan? TaiKhoan { get; set; }
}
