using Microsoft.EntityFrameworkCore;
using QuanLyBanHangDaChiNhanh.Data;
using QuanLyBanHangDaChiNhanh.Models.Entities;

namespace QuanLyBanHangDaChiNhanh.Repositories;

public class DanhMucRepository : IDanhMucRepository
{
    private readonly ApplicationDbContext _context;

    public DanhMucRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<DanhMuc>> GetAllAsync()
    {
        return _context.DanhMucs.AsNoTracking()
            .OrderBy(dm => dm.MaDm).ToListAsync();
    }

    public Task<DanhMuc?> GetByIdAsync(int id)
    {
        return _context.DanhMucs
            .FirstOrDefaultAsync(dm => dm.MaDm == id);
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _context.DanhMucs.AnyAsync(dm => dm.MaDm == id);
    }

    public Task<bool> IsNameExistsAsync(string tenDm, int? exceptId = null)
    {
        var query = _context.DanhMucs
            .Where(dm => dm.TenDm.ToLower() == tenDm.ToLower());

        if (exceptId.HasValue)
            query = query.Where(dm => dm.MaDm != exceptId.Value);

        return query.AnyAsync();
    }

    public async Task AddAsync(DanhMuc danhMuc)
    {
        _context.DanhMucs.Add(danhMuc);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DanhMuc danhMuc)
    {
        _context.DanhMucs.Update(danhMuc);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(DanhMuc danhMuc)
    {
        _context.DanhMucs.Remove(danhMuc);

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            _context.Entry(danhMuc).State = EntityState.Unchanged;
            return false;
        }
    }
}
