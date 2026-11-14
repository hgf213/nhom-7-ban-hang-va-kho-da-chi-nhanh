using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHangDaChiNhanh.DTOs
{
    // DTO trả về danh sách / chi tiết
    public class NhaCungCapDto
    {
        public int MaNCC { get; set; }
        public string TenNCC { get; set; } = string.Empty;
        public string? DiaChi { get; set; }
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
    }

    // DTO tạo mới
    public class CreateNhaCungCapDto
    {
        [Required(ErrorMessage = "Tên nhà cung cấp không được để trống")]
        [MaxLength(150, ErrorMessage = "Tên nhà cung cấp tối đa 150 ký tự")]
        public string TenNCC { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? DiaChi { get; set; }

        [MaxLength(15)]
        [RegularExpression(@"^[0-9\+\-\s]{7,15}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }
    }

    // DTO cập nhật
    public class UpdateNhaCungCapDto
    {
        [Required(ErrorMessage = "Tên nhà cung cấp không được để trống")]
        [MaxLength(150)]
        public string TenNCC { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? DiaChi { get; set; }

        [MaxLength(15)]
        [RegularExpression(@"^[0-9\+\-\s]{7,15}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }
    }

    // DTO lịch sử nhập hàng của NCC
    public class LichSuNhapHangDto
    {
        public int MaPN { get; set; }
        public int MaCN { get; set; }
        public string TenCN { get; set; } = string.Empty;
        public int MaNV { get; set; }
        public string TenNV { get; set; } = string.Empty;
        public DateTime NgayNhap { get; set; }
        public decimal TongTien { get; set; }
    }
}
