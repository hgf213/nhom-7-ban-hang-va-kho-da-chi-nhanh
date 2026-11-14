using QuanLyBanHangDaChiNhanh.Models.Entities;

namespace QuanLyBanHangDaChiNhanh.Repositories;

public interface ISanPhamRepository
{
    Task<List<SanPham>> GetAllAsync(string? tuKhoa, int? maDm);
    Task<SanPham?> GetByIdAsync(int id);
    Task AddAsync(SanPham sanPham);
    Task UpdateAsync(SanPham sanPham);
    Task<bool> DeleteAsync(SanPham sanPham);
}
