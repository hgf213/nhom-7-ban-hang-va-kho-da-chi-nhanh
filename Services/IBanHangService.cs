using QuanLyBanHang.DTOs;

namespace QuanLyBanHang.Services
{
    public interface IBanHangService
    {
        // Tìm kiếm sản phẩm (theo tên, lọc tồn kho tại chi nhánh)
        Task<IEnumerable<SanPhamBanHangDto>> TimKiemSanPhamAsync(string keyword, int maCN);

        // Tạo hóa đơn mới
        Task<HoaDonDto> TaoHoaDonAsync(CreateHoaDonDto dto);

        // Thêm sản phẩm vào hóa đơn
        Task<HoaDonDto> ThemSanPhamAsync(int maHD, ThemSanPhamHoaDonDto dto);

        // Thanh toán hóa đơn → trừ tồn kho
        Task<HoaDonDto> ThanhToanAsync(int maHD, ThanhToanDto dto);

        // Tra cứu hóa đơn theo mã
        Task<HoaDonDto?> GetByIdAsync(int maHD);

        // Tra cứu danh sách hóa đơn (lọc theo chi nhánh / khách hàng)
        Task<IEnumerable<HoaDonSummaryDto>> TraCuuAsync(int? maCN, int? maKH, DateTime? tuNgay, DateTime? denNgay);
    }
}
