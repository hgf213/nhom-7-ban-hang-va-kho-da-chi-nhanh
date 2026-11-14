using QuanLyBanHangDaChiNhanh.Models.Entities;

namespace QuanLyBanHangDaChiNhanh.Repositories;

public interface IDanhMucRepository
{
    Task<List<DanhMuc>> GetAllAsync();
    Task<DanhMuc?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> IsNameExistsAsync(string tenDm, int? exceptId = null);
    Task AddAsync(DanhMuc danhMuc);
    Task UpdateAsync(DanhMuc danhMuc);
    Task<bool> DeleteAsync(DanhMuc danhMuc);
}
