using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHangDaChiNhanh.DTOs
{
    // ---- Chi tiết 1 dòng sản phẩm trong phiếu ----
    public class CtPhieuNhapDto
    {
        public int    MaSP       { get; set; }
        public string TenSP      { get; set; } = string.Empty;
        public string DonViTinh  { get; set; } = string.Empty;
        public int    SoLuong    { get; set; }
        public decimal DonGiaNhap { get; set; }
        public decimal ThanhTien  { get; set; }
    }

    // ---- Trả về phiếu nhập đầy đủ ----
    public class PhieuNhapDto
    {
        public int      MaPN       { get; set; }
        public int      MaCN       { get; set; }
        public string   TenCN      { get; set; } = string.Empty;
        public int      MaNCC      { get; set; }
        public string   TenNCC     { get; set; } = string.Empty;
        public int      MaNV       { get; set; }
        public string   TenNV      { get; set; } = string.Empty;
        public DateTime NgayNhap   { get; set; }
        public decimal  TongTien   { get; set; }
        public List<CtPhieuNhapDto> ChiTiets { get; set; } = new();
    }

    // ---- Tạo phiếu nhập mới (header) ----
    public class CreatePhieuNhapDto
    {
        [Required(ErrorMessage = "Mã chi nhánh không được để trống")]
        public int MaCN { get; set; }

        [Required(ErrorMessage = "Mã nhà cung cấp không được để trống")]
        public int MaNCC { get; set; }

        [Required(ErrorMessage = "Mã nhân viên không được để trống")]
        public int MaNV { get; set; }
    }

    // ---- Thêm 1 sản phẩm vào phiếu nhập ----
    public class AddSanPhamVaoPhieuDto
    {
        [Required(ErrorMessage = "Mã sản phẩm không được để trống")]
        public int MaSP { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Đơn giá nhập phải lớn hơn 0")]
        public decimal DonGiaNhap { get; set; }
    }

    // ---- Trả về tóm tắt sau khi tạo / xác nhận ----
    public class PhieuNhapSummaryDto
    {
        public int     MaPN      { get; set; }
        public decimal TongTien  { get; set; }
        public string  Message   { get; set; } = string.Empty;
    }

    // ---- Lịch sử nhập hàng (dạng danh sách gọn) ----
    public class LichSuNhapDto
    {
        public int      MaPN      { get; set; }
        public string   TenCN     { get; set; } = string.Empty;
        public string   TenNCC    { get; set; } = string.Empty;
        public string   TenNV     { get; set; } = string.Empty;
        public DateTime NgayNhap  { get; set; }
        public decimal  TongTien  { get; set; }
        public int      SoMatHang { get; set; }
    }
}
