using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHangDaChiNhanh.DTOs
{
    public class KhachHangDto
    {
        public int MaKH { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
        public string? DiaChi { get; set; }
    }

    public class CreateKhachHangDto
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [MaxLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [MaxLength(15)]
        [RegularExpression(@"^[0-9\+\-\s]{7,15}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [MaxLength(255)]
        public string? DiaChi { get; set; }
    }

    public class UpdateKhachHangDto
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [MaxLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [MaxLength(15)]
        [RegularExpression(@"^[0-9\+\-\s]{7,15}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [MaxLength(255)]
        public string? DiaChi { get; set; }
    }

    public class LichSuMuaHangDto
    {
        public int MaHD { get; set; }
        public string TenCN { get; set; } = string.Empty;
        public string TenNV { get; set; } = string.Empty;
        public DateTime NgayLap { get; set; }
        public decimal TongTien { get; set; }
        public decimal TongTienGiam { get; set; }
        public decimal ThanhToanThucTe { get; set; }
        public string? PhuongThucThanhToan { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public int SoMatHang { get; set; }
    }
}
