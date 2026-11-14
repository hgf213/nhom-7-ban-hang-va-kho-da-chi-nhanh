using QuanLyBanHangDaChiNhanh.Models.Entities;

namespace QuanLyBanHangDaChiNhanh.Repositories
{
    public interface INhaCungCapRepository
    {
        Task<IEnumerable<NhaCungCap>> GetAllAsync();
        Task<NhaCungCap?> GetByIdAsync(int maNCC);
        Task<NhaCungCap?> GetByTenAsync(string tenNCC);
        Task<NhaCungCap> AddAsync(NhaCungCap nhaCungCap);
        Task<NhaCungCap> UpdateAsync(NhaCungCap nhaCungCap);
        Task DeleteAsync(int maNCC);
        Task<bool> ExistsAsync(int maNCC);
        Task<IEnumerable<PhieuNhap>> GetLichSuNhapHangAsync(int maNCC);
    }
}
