using Microsoft.EntityFrameworkCore;
using QuanLyBanHangDaChiNhanh.Data;
using QuanLyBanHangDaChiNhanh.Models.Entities;

namespace QuanLyBanHangDaChiNhanh.Repositories
{
    public class NhaCungCapRepository : INhaCungCapRepository
    {
        private readonly ApplicationDbContext _context;

        public NhaCungCapRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NhaCungCap>> GetAllAsync()
        {
            return await _context.NhaCungCaps
                .OrderBy(n => n.TenNcc)
                .ToListAsync();
        }

        public async Task<NhaCungCap?> GetByIdAsync(int maNCC)
        {
            return await _context.NhaCungCaps
                .FirstOrDefaultAsync(n => n.MaNcc == maNCC);
        }

        public async Task<NhaCungCap?> GetByTenAsync(string tenNCC)
        {
            return await _context.NhaCungCaps
                .FirstOrDefaultAsync(n => n.TenNcc.ToLower() == tenNCC.ToLower());
        }

        public async Task<NhaCungCap> AddAsync(NhaCungCap nhaCungCap)
        {
            _context.NhaCungCaps.Add(nhaCungCap);
            await _context.SaveChangesAsync();
            return nhaCungCap;
        }

        public async Task<NhaCungCap> UpdateAsync(NhaCungCap nhaCungCap)
        {
            _context.NhaCungCaps.Update(nhaCungCap);
            await _context.SaveChangesAsync();
            return nhaCungCap;
        }

        public async Task DeleteAsync(int maNCC)
        {
            var entity = await _context.NhaCungCaps.FindAsync(maNCC);
            if (entity != null)
            {
                _context.NhaCungCaps.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int maNCC)
        {
            return await _context.NhaCungCaps.AnyAsync(n => n.MaNcc == maNCC);
        }

        public async Task<IEnumerable<PhieuNhap>> GetLichSuNhapHangAsync(int maNCC)
        {
            return await _context.PhieuNhaps
                .Where(p => p.MaNcc == maNCC)
                .OrderByDescending(p => p.NgayNhap)
                .ToListAsync();
        }
    }
}
