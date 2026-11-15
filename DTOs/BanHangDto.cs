using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHangDaChiNhanh.DTOs
{
    // ---- Kết quả tìm kiếm sản phẩm ----
    public class SanPhamBanHangDto
    {
        public int     MaSP      { get; set; }
        public string  TenSP     { get; set; } = string.Empty;
        public string  TenDM     { get; set; } = string.Empty;
        public string  DonViTinh { get; set; } = string.Empty;
        public decimal GiaBan    { get; set; }
        public int     TonKho    { get; set; }  // tồn kho tại chi nhánh
        public bool    CoKhuyenMai { get; set; }
        public decimal GiaSauGiam  { get; set; }
    }

    // ---- Chi tiết 1 dòng trong hóa đơn ----
    public class CtHoaDonDto
    {
        public int     MaSP      { get; set; }
        public string  TenSP     { get; set; } = string.Empty;
        public string  DonViTinh { get; set; } = string.Empty;
        public int     SoLuong   { get; set; }
        public decimal DonGia    { get; set; }
        public decimal TienGiam  { get; set; }
        public decimal ThanhTien { get; set; }
    }

    // ---- Trả về hóa đơn đầy đủ ----
    public class HoaDonDto
    {
        public int      MaHD                 { get; set; }
        public int      MaCN                 { get; set; }
        public string   TenCN                { get; set; } = string.Empty;
        public int      MaNV                 { get; set; }
        public string   TenNV                { get; set; } = string.Empty;
        public int      MaKH                 { get; set; }
        public string   TenKH                { get; set; } = string.Empty;
        public string?  SdtKH                { get; set; }
        public DateTime NgayLap              { get; set; }
        public decimal  TongTien             { get; set; }
        public decimal  TongTienGiam         { get; set; }
        public decimal  ThanhToanThucTe      { get; set; }  // TongTien - TongTienGiam
        public string?  PhuongThucThanhToan  { get; set; }
        public string   TrangThai            { get; set; } = string.Empty;
        public List<CtHoaDonDto> ChiTiets    { get; set; } = new();
    }

    // ---- Tạo hóa đơn mới ----
    public class CreateHoaDonDto
    {
        [Required(ErrorMessage = "Mã chi nhánh không được để trống")]
        public int MaCN { get; set; }

        [Required(ErrorMessage = "Mã nhân viên không được để trống")]
        public int MaNV { get; set; }

        // MaKH = 1 nếu khách lẻ
        [Required(ErrorMessage = "Mã khách hàng không được để trống")]
        public int MaKH { get; set; }
    }

    // ---- Thêm sản phẩm vào hóa đơn ----
    public class ThemSanPhamHoaDonDto
    {
        [Required(ErrorMessage = "Mã sản phẩm không được để trống")]
        public int MaSP { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; }

        // Mã khuyến mãi áp dụng (tùy chọn)
        public int? MaKM { get; set; }
    }

    // ---- Thanh toán hóa đơn ----
    public class ThanhToanDto
    {
        [Required(ErrorMessage = "Phương thức thanh toán không được để trống")]
        [MaxLength(50)]
        public string PhuongThucThanhToan { get; set; } = string.Empty;
    }

    // ---- Tra cứu hóa đơn (danh sách gọn) ----
    public class HoaDonSummaryDto
    {
        public int      MaHD                { get; set; }
        public string   TenCN               { get; set; } = string.Empty;
        public string   TenNV               { get; set; } = string.Empty;
        public string   TenKH               { get; set; } = string.Empty;
        public DateTime NgayLap             { get; set; }
        public decimal  TongTien            { get; set; }
        public decimal  TongTienGiam        { get; set; }
        public decimal  ThanhToanThucTe     { get; set; }
        public string?  PhuongThucThanhToan { get; set; }
        public string   TrangThai           { get; set; } = string.Empty;
        public int      SoMatHang           { get; set; }
    }
}
