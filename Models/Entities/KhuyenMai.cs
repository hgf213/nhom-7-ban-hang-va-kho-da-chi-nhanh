using System;
using System.Collections.Generic;

namespace QuanLyBanHangDaChiNhanh.Models.Entities;

public partial class KhuyenMai
{
    public int MaKm { get; set; }

    public string TenKm { get; set; } = null!;

    public string? LoaiGiamGia { get; set; }

    public decimal GiaTriGiam { get; set; }

    public decimal? DonHangToiThieu { get; set; }

    public decimal? GiamToiDa { get; set; }

    public DateTime? NgayBatDau { get; set; }

    public DateTime? NgayKetThuc { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<SanPham> MaSps { get; set; } = new List<SanPham>();
}
