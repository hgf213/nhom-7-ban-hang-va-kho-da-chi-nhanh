using QuanLyBanHangDaChiNhanh.DTOs;

namespace QuanLyBanHangDaChiNhanh.Services;

public interface IDanhMucService
{
    Task<List<DanhMucDTO>> GetAllAsync();
    Task<DanhMucDTO?> GetByIdAsync(int id);
    Task<(DanhMucDTO? Data, string? Error)> CreateAsync(TaoDanhMucDTO dto);
    Task<(DanhMucDTO? Data, string? Error)> UpdateAsync(int id, CapNhatDanhMucDTO dto);
    Task<string?> DeleteAsync(int id);
}
