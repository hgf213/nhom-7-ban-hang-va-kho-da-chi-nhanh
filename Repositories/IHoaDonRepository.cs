using QuanLyBanHangDaChiNhanh.Models.Entities;

namespace QuanLyBanHangDaChiNhanh.Repositories
{
    public interface IHoaDonRepository
    {
        Task<HoaDon?> GetByIdAsync(int maHD);
        Task<IEnumerable<HoaDon>> GetAllAsync();
        Task<IEnumerable<HoaDon>> GetByChiNhanhAsync(int maCN);
        Task<IEnumerable<HoaDon>> GetByKhachHangAsync(int maKH);
        Task<HoaDon> AddAsync(HoaDon hoaDon);
        Task<HoaDon> UpdateAsync(HoaDon hoaDon);
        Task<bool> ExistsAsync(int maHD);

        Task<CtHoaDon?> GetChiTietAsync(int maHD, int maSP);
        Task AddChiTietAsync(CtHoaDon chiTiet);
        Task UpdateChiTietAsync(CtHoaDon chiTiet);
        Task RemoveChiTietAsync(int maHD, int maSP);
    }
}
