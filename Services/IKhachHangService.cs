using QuanLyBanHangDaChiNhanh.DTOs;

namespace QuanLyBanHangDaChiNhanh.Services
{
    public interface IKhachHangService
    {
        Task<IEnumerable<KhachHangDto>> GetAllAsync();
        Task<KhachHangDto?> GetByIdAsync(int maKH);
        Task<IEnumerable<KhachHangDto>> TimKiemAsync(string keyword);
        Task<KhachHangDto> CreateAsync(CreateKhachHangDto dto);
        Task<KhachHangDto> UpdateAsync(int maKH, UpdateKhachHangDto dto);
        Task DeleteAsync(int maKH);
        Task<IEnumerable<LichSuMuaHangDto>> GetLichSuMuaHangAsync(int maKH);
    }
}
