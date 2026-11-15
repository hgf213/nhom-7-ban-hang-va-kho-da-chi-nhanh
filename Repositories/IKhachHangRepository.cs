using QuanLyBanHangDaChiNhanh.Models.Entities;

namespace QuanLyBanHangDaChiNhanh.Repositories
{
    public interface IKhachHangRepository
    {
        Task<IEnumerable<KhachHang>> GetAllAsync();
        Task<KhachHang?> GetByIdAsync(int maKH);
        Task<KhachHang?> GetBySdtAsync(string sdt);
        Task<KhachHang> AddAsync(KhachHang khachHang);
        Task<KhachHang> UpdateAsync(KhachHang khachHang);
        Task DeleteAsync(int maKH);
        Task<bool> ExistsAsync(int maKH);
    }
}
