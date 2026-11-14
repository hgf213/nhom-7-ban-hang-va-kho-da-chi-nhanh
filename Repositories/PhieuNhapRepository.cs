using Microsoft.EntityFrameworkCore;
using QuanLyBanHangDaChiNhanh.Data;
using QuanLyBanHangDaChiNhanh.Models.Entities;

namespace QuanLyBanHangDaChiNhanh.Repositories
{
    public class PhieuNhapRepository : IPhieuNhapRepository
    {
        private readonly ApplicationDbContext _context;

        public PhieuNhapRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PhieuNhap?> GetByIdAsync(int maPN)
        {
            return await _context.PhieuNhaps
                .Include(p => p.MaNccNavigation)
                .Include(p => p.MaCnNavigation)
                .Include(p => p.MaNvNavigation)
                .Include(p => p.CtPhieuNhaps)
                    .ThenInclude(ct => ct.MaSpNavigation)
                .FirstOrDefaultAsync(p => p.MaPn == maPN);
        }

        public async Task<IEnumerable<PhieuNhap>> GetAllAsync()
        {
            return await _context.PhieuNhaps
                .Include(p => p.MaNccNavigation)
                .Include(p => p.MaCnNavigation)
                .Include(p => p.MaNvNavigation)
                .Include(p => p.CtPhieuNhaps)
                .OrderByDescending(p => p.NgayNhap)
                .ToListAsync();
        }

        public async Task<IEnumerable<PhieuNhap>> GetByChiNhanhAsync(int maCN)
        {
            return await _context.PhieuNhaps
                .Include(p => p.MaNccNavigation)
                .Include(p => p.MaNvNavigation)
                .Include(p => p.CtPhieuNhaps)
                .Where(p => p.MaCn == maCN)
                .OrderByDescending(p => p.NgayNhap)
                .ToListAsync();
        }

        public async Task<IEnumerable<PhieuNhap>> GetByNhaCungCapAsync(int maNCC)
        {
            return await _context.PhieuNhaps
                .Include(p => p.MaCnNavigation)
                .Include(p => p.MaNvNavigation)
                .Include(p => p.CtPhieuNhaps)
                .Where(p => p.MaNcc == maNCC)
                .OrderByDescending(p => p.NgayNhap)
                .ToListAsync();
        }

        public async Task<PhieuNhap> AddAsync(PhieuNhap phieuNhap)
        {
            _context.PhieuNhaps.Add(phieuNhap);
            await _context.SaveChangesAsync();
            return phieuNhap;
        }

        public async Task<PhieuNhap> UpdateAsync(PhieuNhap phieuNhap)
        {
            _context.PhieuNhaps.Update(phieuNhap);
            await _context.SaveChangesAsync();
            return phieuNhap;
        }

        public async Task<bool> ExistsAsync(int maPN)
        {
            return await _context.PhieuNhaps.AnyAsync(p => p.MaPn == maPN);
        }

        // ---- Chi tiết ----
        public async Task<CtPhieuNhap?> GetChiTietAsync(int maPN, int maSP)
        {
            return await _context.CtPhieuNhaps
                .Include(ct => ct.MaSpNavigation)
                .FirstOrDefaultAsync(ct => ct.MaPn == maPN && ct.MaSp == maSP);
        }

        public async Task AddChiTietAsync(CtPhieuNhap chiTiet)
        {
            _context.CtPhieuNhaps.Add(chiTiet);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateChiTietAsync(CtPhieuNhap chiTiet)
        {
            _context.CtPhieuNhaps.Update(chiTiet);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveChiTietAsync(int maPN, int maSP)
        {
            var entity = await _context.CtPhieuNhaps
                .FirstOrDefaultAsync(ct => ct.MaPn == maPN && ct.MaSp == maSP);
            if (entity != null)
            {
                _context.CtPhieuNhaps.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
