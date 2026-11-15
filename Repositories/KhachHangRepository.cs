using Microsoft.EntityFrameworkCore;
using QuanLyBanHangDaChiNhanh.Data;
using QuanLyBanHangDaChiNhanh.Models.Entities;

namespace QuanLyBanHangDaChiNhanh.Repositories
{
    public class KhachHangRepository : IKhachHangRepository
    {
        private readonly ApplicationDbContext _context;

        public KhachHangRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KhachHang>> GetAllAsync()
        {
            return await _context.KhachHangs
                .OrderBy(k => k.HoTen)
                .ToListAsync();
        }

        public async Task<KhachHang?> GetByIdAsync(int maKH)
        {
            return await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.MaKh == maKH);
        }

        public async Task<KhachHang?> GetBySdtAsync(string sdt)
        {
            return await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.SoDienThoai == sdt);
        }

        public async Task<KhachHang> AddAsync(KhachHang khachHang)
        {
            _context.KhachHangs.Add(khachHang);
            await _context.SaveChangesAsync();
            return khachHang;
        }

        public async Task<KhachHang> UpdateAsync(KhachHang khachHang)
        {
            _context.KhachHangs.Update(khachHang);
            await _context.SaveChangesAsync();
            return khachHang;
        }

        public async Task DeleteAsync(int maKH)
        {
            var entity = await _context.KhachHangs.FindAsync(maKH);
            if (entity != null)
            {
                _context.KhachHangs.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int maKH)
        {
            return await _context.KhachHangs.AnyAsync(k => k.MaKh == maKH);
        }
    }
}
