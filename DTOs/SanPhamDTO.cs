namespace QuanLyBanHangDaChiNhanh.DTOs;

public class SanPhamDTO
{
    public int MaSp { get; set; }
    public int MaDm { get; set; }
    public string TenSp { get; set; } = string.Empty;
    public string? DonViTinh { get; set; }
    public decimal GiaBan { get; set; }
    public string? MoTa { get; set; }
    public string? TenDanhMuc { get; set; }
}

public class TaoSanPhamDTO
{
    public int MaDm { get; set; }
    public string TenSp { get; set; } = string.Empty;
    public string? DonViTinh { get; set; }
    public decimal GiaBan { get; set; }
    public string? MoTa { get; set; }
}

public class CapNhatSanPhamDTO
{
    public int MaDm { get; set; }
    public string TenSp { get; set; } = string.Empty;
    public string? DonViTinh { get; set; }
    public decimal GiaBan { get; set; }
    public string? MoTa { get; set; }
}
