using Microsoft.EntityFrameworkCore;
using QuanLyBanHangDaChiNhanh.Data;
using QuanLyBanHangDaChiNhanh.Models.Entities;

namespace QuanLyBanHangDaChiNhanh.Repositories
{
    public class HoaDonRepository : IHoaDonRepository
    {
        private readonly ApplicationDbContext _context;

        public HoaDonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HoaDon?> GetByIdAsync(int maHD)
        {
            return await _context.HoaDons
                .Include(h => h.MaCnNavigation)
                .Include(h => h.MaNvNavigation)
                .Include(h => h.MaKhNavigation)
                .Include(h => h.CtHoaDons)
                    .ThenInclude(ct => ct.MaSpNavigation)
                .FirstOrDefaultAsync(h => h.MaHd == maHD);
        }

        public async Task<IEnumerable<HoaDon>> GetAllAsync()
        {
            return await _context.HoaDons
                .Include(h => h.MaCnNavigation)
                .Include(h => h.MaNvNavigation)
                .Include(h => h.MaKhNavigation)
                .Include(h => h.CtHoaDons)
                .OrderByDescending(h => h.NgayLap)
                .ToListAsync();
        }

        public async Task<IEnumerable<HoaDon>> GetByChiNhanhAsync(int maCN)
        {
            return await _context.HoaDons
                .Include(h => h.MaNvNavigation)
                .Include(h => h.MaKhNavigation)
                .Include(h => h.CtHoaDons)
                .Where(h => h.MaCn == maCN)
                .OrderByDescending(h => h.NgayLap)
                .ToListAsync();
        }

        public async Task<IEnumerable<HoaDon>> GetByKhachHangAsync(int maKH)
        {
            return await _context.HoaDons
                .Include(h => h.MaCnNavigation)
                .Include(h => h.MaNvNavigation)
                .Include(h => h.CtHoaDons)
                .Where(h => h.MaKh == maKH)
                .OrderByDescending(h => h.NgayLap)
                .ToListAsync();
        }

        public async Task<HoaDon> AddAsync(HoaDon hoaDon)
        {
            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync();
            return hoaDon;
        }

        public async Task<HoaDon> UpdateAsync(HoaDon hoaDon)
        {
            _context.HoaDons.Update(hoaDon);
            await _context.SaveChangesAsync();
            return hoaDon;
        }

        public async Task<bool> ExistsAsync(int maHD)
        {
            return await _context.HoaDons.AnyAsync(h => h.MaHd == maHD);
        }

        public async Task<CtHoaDon?> GetChiTietAsync(int maHD, int maSP)
        {
            return await _context.CtHoaDons
                .Include(ct => ct.MaSpNavigation)
                .FirstOrDefaultAsync(ct => ct.MaHd == maHD && ct.MaSp == maSP);
        }

        public async Task AddChiTietAsync(CtHoaDon chiTiet)
        {
            _context.CtHoaDons.Add(chiTiet);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateChiTietAsync(CtHoaDon chiTiet)
        {
            _context.CtHoaDons.Update(chiTiet);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveChiTietAsync(int maHD, int maSP)
        {
            var entity = await _context.CtHoaDons
                .FirstOrDefaultAsync(ct => ct.MaHd == maHD && ct.MaSp == maSP);
            if (entity != null)
            {
                _context.CtHoaDons.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
