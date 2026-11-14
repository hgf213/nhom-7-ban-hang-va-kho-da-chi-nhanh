using QuanLyBanHangDaChiNhanh.DTOs;

namespace QuanLyBanHangDaChiNhanh.Services
{
    public interface IPhieuNhapService
    {
        // Tạo phiếu nhập mới (header)
        Task<PhieuNhapSummaryDto> TaoPhieuNhapAsync(CreatePhieuNhapDto dto);

        // Thêm sản phẩm vào phiếu
        Task<PhieuNhapDto> ThemSanPhamAsync(int maPN, AddSanPhamVaoPhieuDto dto);

        // Xác nhận phiếu → cập nhật tồn kho
        Task<PhieuNhapSummaryDto> XacNhanPhieuNhapAsync(int maPN);

        // Hủy phiếu (chỉ được hủy khi còn "Chờ xác nhận")
        Task<PhieuNhapSummaryDto> HuyPhieuNhapAsync(int maPN);

        // Lấy chi tiết 1 phiếu
        Task<PhieuNhapDto?> GetByIdAsync(int maPN);

        // Lịch sử nhập hàng (toàn bộ hoặc lọc theo chi nhánh / NCC)
        Task<IEnumerable<LichSuNhapDto>> GetLichSuAsync(int? maCN, int? maNCC);
    }
}
