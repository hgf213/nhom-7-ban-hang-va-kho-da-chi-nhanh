using QuanLyBanHangDaChiNhanh.Models.Entities;

namespace QuanLyBanHangDaChiNhanh.Repositories
{
    public interface IPhieuNhapRepository
    {
        Task<PhieuNhap?> GetByIdAsync(int maPN);
        Task<IEnumerable<PhieuNhap>> GetAllAsync();
        Task<IEnumerable<PhieuNhap>> GetByChiNhanhAsync(int maCN);
        Task<IEnumerable<PhieuNhap>> GetByNhaCungCapAsync(int maNCC);
        Task<PhieuNhap> AddAsync(PhieuNhap phieuNhap);
        Task<PhieuNhap> UpdateAsync(PhieuNhap phieuNhap);
        Task<bool> ExistsAsync(int maPN);

        Task<CtPhieuNhap?> GetChiTietAsync(int maPN, int maSP);
        Task AddChiTietAsync(CtPhieuNhap chiTiet);
        Task UpdateChiTietAsync(CtPhieuNhap chiTiet);
        Task RemoveChiTietAsync(int maPN, int maSP);
    }
}
