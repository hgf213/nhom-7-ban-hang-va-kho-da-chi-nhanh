using Microsoft.EntityFrameworkCore;
using QuanLyBanHangDaChiNhanh.Data;
using QuanLyBanHangDaChiNhanh.DTOs;
using QuanLyBanHangDaChiNhanh.Models.Entities;
using QuanLyBanHangDaChiNhanh.Repositories;

namespace QuanLyBanHangDaChiNhanh.Services
{
    public class NhaCungCapService : INhaCungCapService
    {
        private readonly INhaCungCapRepository _repository;
        private readonly ApplicationDbContext _context;

        public NhaCungCapService(INhaCungCapRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<NhaCungCapDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToDto);
        }

        public async Task<NhaCungCapDto?> GetByIdAsync(int maNCC)
        {
            var entity = await _repository.GetByIdAsync(maNCC);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<IEnumerable<NhaCungCapDto>> SearchAsync(string keyword)
        {
            var normalizedKeyword = keyword.Trim();
            var list = await _context.Set<NhaCungCap>()
                .Where(n =>
                    n.TenNcc.Contains(normalizedKeyword) ||
                    (n.SoDienThoai != null && n.SoDienThoai.Contains(normalizedKeyword)) ||
                    (n.Email != null && n.Email.Contains(normalizedKeyword)))
                .OrderBy(n => n.TenNcc)
                .ToListAsync();
            return list.Select(MapToDto);
        }

        public async Task<NhaCungCapDto> CreateAsync(CreateNhaCungCapDto dto)
        {
            // Kiểm tra trùng tên
            var existing = await _repository.GetByTenAsync(dto.TenNCC);
            if (existing != null)
                throw new InvalidOperationException($"Nhà cung cấp '{dto.TenNCC}' đã tồn tại.");

            var entity = new NhaCungCap
            {
                TenNcc = dto.TenNCC.Trim(),
                DiaChi = dto.DiaChi?.Trim(),
                SoDienThoai = dto.SoDienThoai?.Trim(),
                Email = dto.Email?.Trim()
            };

            var created = await _repository.AddAsync(entity);
            return MapToDto(created);
        }

        public async Task<NhaCungCapDto> UpdateAsync(int maNCC, UpdateNhaCungCapDto dto)
        {
            var entity = await _repository.GetByIdAsync(maNCC)
                ?? throw new KeyNotFoundException($"Không tìm thấy nhà cung cấp với mã {maNCC}.");

            // Kiểm tra trùng tên với NCC khác
            var duplicate = await _context.Set<NhaCungCap>()
                .FirstOrDefaultAsync(n => n.TenNcc.ToLower() == dto.TenNCC.ToLower() && n.MaNcc != maNCC);
            if (duplicate != null)
                throw new InvalidOperationException($"Tên nhà cung cấp '{dto.TenNCC}' đã được sử dụng.");

            entity.TenNcc = dto.TenNCC.Trim();
            entity.DiaChi = dto.DiaChi?.Trim();
            entity.SoDienThoai = dto.SoDienThoai?.Trim();
            entity.Email = dto.Email?.Trim();

            var updated = await _repository.UpdateAsync(entity);
            return MapToDto(updated);
        }

        public async Task DeleteAsync(int maNCC)
        {
            if (!await _repository.ExistsAsync(maNCC))
                throw new KeyNotFoundException($"Không tìm thấy nhà cung cấp với mã {maNCC}.");

            // Kiểm tra ràng buộc: NCC đang có phiếu nhập
            var hasPhieuNhap = await _context.Set<PhieuNhap>().AnyAsync(p => p.MaNcc == maNCC);
            if (hasPhieuNhap)
                throw new InvalidOperationException("Không thể xóa nhà cung cấp đang có lịch sử nhập hàng.");

            await _repository.DeleteAsync(maNCC);
        }

        public async Task<IEnumerable<LichSuNhapHangDto>> GetLichSuNhapHangAsync(int maNCC)
        {
            if (!await _repository.ExistsAsync(maNCC))
                throw new KeyNotFoundException($"Không tìm thấy nhà cung cấp với mã {maNCC}.");

            var result = await _context.Set<PhieuNhap>()
                .Where(p => p.MaNcc == maNCC)
                .Join(_context.Set<ChiNhanh>(),
                      p => p.MaCn,
                      cn => cn.MaCn,
                      (p, cn) => new { p, TenCN = cn.TenCn })
                .Join(_context.Set<NhanVien>(),
                      x => x.p.MaNv,
                      nv => nv.MaNv,
                      (x, nv) => new LichSuNhapHangDto
                      {
                          MaPN = x.p.MaPn,
                          MaCN = x.p.MaCn,
                          TenCN = x.TenCN,
                          MaNV = x.p.MaNv,
                          TenNV = nv.HoTen,
                          NgayNhap = x.p.NgayNhap ?? DateTime.MinValue,
                          TongTien = x.p.TongTien ?? 0m
                      })
                .OrderByDescending(x => x.NgayNhap)
                .ToListAsync();

            return result;
        }

        // ---- Mapper ----
        private static NhaCungCapDto MapToDto(NhaCungCap e) => new()
        {
            MaNCC = e.MaNcc,
            TenNCC = e.TenNcc,
            DiaChi = e.DiaChi,
            SoDienThoai = e.SoDienThoai,
            Email = e.Email
        };
    }
}
