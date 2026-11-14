using QuanLyBanHangDaChiNhanh.DTOs;

namespace QuanLyBanHangDaChiNhanh.Services
{
    public interface INhaCungCapService
    {
        Task<IEnumerable<NhaCungCapDto>> GetAllAsync();
        Task<NhaCungCapDto?> GetByIdAsync(int maNCC);
        Task<IEnumerable<NhaCungCapDto>> SearchAsync(string keyword);
        Task<NhaCungCapDto> CreateAsync(CreateNhaCungCapDto dto);
        Task<NhaCungCapDto> UpdateAsync(int maNCC, UpdateNhaCungCapDto dto);
        Task DeleteAsync(int maNCC);
        Task<IEnumerable<LichSuNhapHangDto>> GetLichSuNhapHangAsync(int maNCC);
    }
}
