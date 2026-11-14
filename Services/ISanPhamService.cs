using QuanLyBanHangDaChiNhanh.DTOs;

namespace QuanLyBanHangDaChiNhanh.Services;

public interface ISanPhamService
{
    Task<List<SanPhamDTO>> GetAllAsync(string? tuKhoa, int? maDm);
    Task<SanPhamDTO?> GetByIdAsync(int id);
    Task<(SanPhamDTO? Data, string? Error)> CreateAsync(TaoSanPhamDTO dto);
    Task<(SanPhamDTO? Data, string? Error)> UpdateAsync(int id, CapNhatSanPhamDTO dto);
    Task<string?> DeleteAsync(int id);
}
