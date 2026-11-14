using Microsoft.EntityFrameworkCore;
using QuanLyBanHangDaChiNhanh.Data;
using QuanLyBanHangDaChiNhanh.Models.Entities;

namespace QuanLyBanHangDaChiNhanh.Repositories;

public class SanPhamRepository : ISanPhamRepository
{
    private readonly ApplicationDbContext _context;

    public SanPhamRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SanPham>> GetAllAsync(string? tuKhoa, int? maDm)
    {
        IQueryable<SanPham> query = _context.SanPhams
            .AsNoTracking()
            .Include(sp => sp.MaDmNavigation);

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            string keyword = tuKhoa.Trim();
            query = query.Where(sp =>
                sp.TenSp.Contains(keyword) ||
                sp.MaSp.ToString().Contains(keyword));
        }

        if (maDm.HasValue)
            query = query.Where(sp => sp.MaDm == maDm.Value);

        return await query.OrderBy(sp => sp.MaSp).ToListAsync();
    }

    public Task<SanPham?> GetByIdAsync(int id)
    {
        return _context.SanPhams
            .Include(sp => sp.MaDmNavigation)
            .FirstOrDefaultAsync(sp => sp.MaSp == id);
    }

    public async Task AddAsync(SanPham sanPham)
    {
        _context.SanPhams.Add(sanPham);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(SanPham sanPham)
    {
        _context.SanPhams.Update(sanPham);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(SanPham sanPham)
    {
        _context.SanPhams.Remove(sanPham);

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            // Sản phẩm đang được tham chiếu ở chi tiết hóa đơn,
            // phiếu nhập, tồn kho... nên không thể xóa.
            _context.Entry(sanPham).State = EntityState.Unchanged;
            return false;
        }
    }
}
