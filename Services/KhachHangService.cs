using Microsoft.EntityFrameworkCore;
using QuanLyBanHangDaChiNhanh.Data;
using QuanLyBanHangDaChiNhanh.DTOs;
using QuanLyBanHangDaChiNhanh.Models.Entities;
using QuanLyBanHangDaChiNhanh.Repositories;

namespace QuanLyBanHangDaChiNhanh.Services
{
    public class KhachHangService : IKhachHangService
    {
        private readonly IKhachHangRepository _repo;
        private readonly ApplicationDbContext _context;

        public KhachHangService(IKhachHangRepository repo, ApplicationDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        // ---------------------------------------------------------------
        // 1. Lấy tất cả khách hàng
        // ---------------------------------------------------------------
        public async Task<IEnumerable<KhachHangDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(MapToDto);
        }

        // ---------------------------------------------------------------
        // 2. Lấy theo mã
        // ---------------------------------------------------------------
        public async Task<KhachHangDto?> GetByIdAsync(int maKH)
        {
            var entity = await _repo.GetByIdAsync(maKH);
            return entity == null ? null : MapToDto(entity);
        }

        // ---------------------------------------------------------------
        // 3. Tìm kiếm theo tên / SĐT / email
        // ---------------------------------------------------------------
        public async Task<IEnumerable<KhachHangDto>> TimKiemAsync(string keyword)
        {
            var list = await _context.KhachHangs
                .Where(k =>
                    k.HoTen.Contains(keyword) ||
                    (k.SoDienThoai != null && k.SoDienThoai.Contains(keyword)) ||
                    (k.Email != null && k.Email.Contains(keyword)))
                .OrderBy(k => k.HoTen)
                .ToListAsync();

            return list.Select(MapToDto);
        }

        // ---------------------------------------------------------------
        // 4. Thêm khách hàng mới
        // ---------------------------------------------------------------
        public async Task<KhachHangDto> CreateAsync(CreateKhachHangDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.SoDienThoai))
            {
                var existing = await _repo.GetBySdtAsync(dto.SoDienThoai.Trim());
                if (existing != null)
                    throw new InvalidOperationException($"Số điện thoại '{dto.SoDienThoai}' đã được đăng ký.");
            }

            var entity = new KhachHang
            {
                HoTen = dto.HoTen.Trim(),
                SoDienThoai = dto.SoDienThoai?.Trim(),
                Email = dto.Email?.Trim(),
                DiaChi = dto.DiaChi?.Trim()
            };

            var created = await _repo.AddAsync(entity);
            return MapToDto(created);
        }

        // ---------------------------------------------------------------
        // 5. Sửa khách hàng
        // ---------------------------------------------------------------
        public async Task<KhachHangDto> UpdateAsync(int maKH, UpdateKhachHangDto dto)
        {
            var entity = await _repo.GetByIdAsync(maKH)
                ?? throw new KeyNotFoundException($"Không tìm thấy khách hàng với mã {maKH}.");

            if (!string.IsNullOrWhiteSpace(dto.SoDienThoai))
            {
                var duplicate = await _context.KhachHangs
                    .FirstOrDefaultAsync(k => k.SoDienThoai == dto.SoDienThoai.Trim() && k.MaKh != maKH);
                if (duplicate != null)
                    throw new InvalidOperationException($"Số điện thoại '{dto.SoDienThoai}' đã được đăng ký bởi khách hàng khác.");
            }

            entity.HoTen = dto.HoTen.Trim();
            entity.SoDienThoai = dto.SoDienThoai?.Trim();
            entity.Email = dto.Email?.Trim();
            entity.DiaChi = dto.DiaChi?.Trim();

            var updated = await _repo.UpdateAsync(entity);
            return MapToDto(updated);
        }

        // ---------------------------------------------------------------
        // 6. Xóa khách hàng
        // ---------------------------------------------------------------
        public async Task DeleteAsync(int maKH)
        {
            if (!await _repo.ExistsAsync(maKH))
                throw new KeyNotFoundException($"Không tìm thấy khách hàng với mã {maKH}.");

            var coHoaDon = await _context.HoaDons.AnyAsync(h => h.MaKh == maKH);
            if (coHoaDon)
                throw new InvalidOperationException("Không thể xóa khách hàng đã có lịch sử mua hàng.");

            await _repo.DeleteAsync(maKH);
        }

        // ---------------------------------------------------------------
        // 7. Lịch sử mua hàng
        // ---------------------------------------------------------------
        public async Task<IEnumerable<LichSuMuaHangDto>> GetLichSuMuaHangAsync(int maKH)
        {
            if (!await _repo.ExistsAsync(maKH))
                throw new KeyNotFoundException($"Không tìm thấy khách hàng với mã {maKH}.");

            var list = await _context.HoaDons
                .Include(h => h.MaCnNavigation)
                .Include(h => h.MaNvNavigation)
                .Include(h => h.CtHoaDons)
                .Where(h => h.MaKh == maKH)
                .OrderByDescending(h => h.NgayLap)
                .ToListAsync();

            return list.Select(h => new LichSuMuaHangDto
            {
                MaHD = h.MaHd,
                TenCN = h.MaCnNavigation?.TenCn ?? "",
                TenNV = h.MaNvNavigation?.HoTen ?? "",
                NgayLap = h.NgayLap ?? DateTime.MinValue,
                TongTien = h.TongTien ?? 0,
                TongTienGiam = h.TongTienGiam ?? 0,
                ThanhToanThucTe = (h.TongTien ?? 0) - (h.TongTienGiam ?? 0),
                PhuongThucThanhToan = h.PhuongThucThanhToan ?? "",
                TrangThai = h.TrangThai ?? "",
                SoMatHang = h.CtHoaDons.Count
            });
        }

        // ---------------------------------------------------------------
        // Mapper
        // ---------------------------------------------------------------
        private static KhachHangDto MapToDto(KhachHang k) => new()
        {
            MaKH = k.MaKh,
            HoTen = k.HoTen,
            SoDienThoai = k.SoDienThoai,
            Email = k.Email,
            DiaChi = k.DiaChi
        };
    }
}
