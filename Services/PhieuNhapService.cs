using Microsoft.EntityFrameworkCore;
using QuanLyBanHangDaChiNhanh.Data;
using QuanLyBanHangDaChiNhanh.DTOs;
using QuanLyBanHangDaChiNhanh.Models.Entities;
using QuanLyBanHangDaChiNhanh.Repositories;

namespace QuanLyBanHangDaChiNhanh.Services
{
    public class PhieuNhapService : IPhieuNhapService
    {
        private readonly IPhieuNhapRepository _repo;
        private readonly ApplicationDbContext _context;

        public PhieuNhapService(IPhieuNhapRepository repo, ApplicationDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        // ---------------------------------------------------------------
        // 1. Tạo phiếu nhập mới
        // ---------------------------------------------------------------
        public async Task<PhieuNhapSummaryDto> TaoPhieuNhapAsync(CreatePhieuNhapDto dto)
        {
            if (!await _context.ChiNhanhs.AnyAsync(c => c.MaCn == dto.MaCN))
                throw new KeyNotFoundException($"Chi nhánh {dto.MaCN} không tồn tại.");
            if (!await _context.NhaCungCaps.AnyAsync(n => n.MaNcc == dto.MaNCC))
                throw new KeyNotFoundException($"Nhà cung cấp {dto.MaNCC} không tồn tại.");
            if (!await _context.NhanViens.AnyAsync(n => n.MaNv == dto.MaNV))
                throw new KeyNotFoundException($"Nhân viên {dto.MaNV} không tồn tại.");

            var phieu = new PhieuNhap
            {
                MaCn = dto.MaCN,
                MaNcc = dto.MaNCC,
                MaNv = dto.MaNV,
                NgayNhap = DateTime.Now,
                TongTien = 0
            };

            var created = await _repo.AddAsync(phieu);

            return new PhieuNhapSummaryDto
            {
                MaPN = created.MaPn,
                TongTien = 0,
                Message = $"Tạo phiếu nhập #{created.MaPn} thành công. Hãy thêm sản phẩm vào phiếu."
            };
        }

        // ---------------------------------------------------------------
        // 2. Thêm sản phẩm vào phiếu nhập
        // ---------------------------------------------------------------
        public async Task<PhieuNhapDto> ThemSanPhamAsync(int maPN, AddSanPhamVaoPhieuDto dto)
        {
            var phieu = await _repo.GetByIdAsync(maPN)
                ?? throw new KeyNotFoundException($"Phiếu nhập #{maPN} không tồn tại.");

            if (phieu.MaCnNavigation == null)
                throw new InvalidOperationException($"Phiếu nhập #{maPN} không có chi nhánh hợp lệ.");

            if (phieu.MaNccNavigation == null)
                throw new InvalidOperationException($"Phiếu nhập #{maPN} không có nhà cung cấp hợp lệ.");

            if (phieu.MaNvNavigation == null)
                throw new InvalidOperationException($"Phiếu nhập #{maPN} không có nhân viên hợp lệ.");

            var sanPham = await _context.SanPhams.FindAsync(dto.MaSP)
                ?? throw new KeyNotFoundException($"Sản phẩm {dto.MaSP} không tồn tại.");

            var thanhTien = dto.SoLuong * dto.DonGiaNhap;

            var existing = await _repo.GetChiTietAsync(maPN, dto.MaSP);
            if (existing != null)
            {
                existing.SoLuong += dto.SoLuong;
                existing.DonGiaNhap = dto.DonGiaNhap;
                existing.ThanhTien = existing.SoLuong * dto.DonGiaNhap;
                await _repo.UpdateChiTietAsync(existing);
            }
            else
            {
                await _repo.AddChiTietAsync(new CtPhieuNhap
                {
                    MaPn = maPN,
                    MaSp = dto.MaSP,
                    SoLuong = dto.SoLuong,
                    DonGiaNhap = dto.DonGiaNhap,
                    ThanhTien = thanhTien
                });
            }

            phieu.TongTien = await _context.CtPhieuNhaps
                .Where(ct => ct.MaPn == maPN)
                .SumAsync(ct => (decimal?)ct.ThanhTien) ?? 0m;
            await _repo.UpdateAsync(phieu);

            var updated = await _repo.GetByIdAsync(maPN);
            return MapToDto(updated!);
        }

        // ---------------------------------------------------------------
        // 3. Xác nhận phiếu → nhập tồn kho
        // ---------------------------------------------------------------
        public async Task<PhieuNhapSummaryDto> XacNhanPhieuNhapAsync(int maPN)
        {
            var phieu = await _repo.GetByIdAsync(maPN)
                ?? throw new KeyNotFoundException($"Phiếu nhập #{maPN} không tồn tại.");

            if (!phieu.CtPhieuNhaps.Any())
                throw new InvalidOperationException("Phiếu nhập chưa có sản phẩm nào. Vui lòng thêm sản phẩm trước khi xác nhận.");

            foreach (var ct in phieu.CtPhieuNhaps)
            {
                var tonKho = await _context.TonKhos
                    .FirstOrDefaultAsync(t => t.MaCn == phieu.MaCn && t.MaSp == ct.MaSp);

                if (tonKho == null)
                {
                    _context.TonKhos.Add(new TonKho
                    {
                        MaCn = phieu.MaCn,
                        MaSp = ct.MaSp,
                        SoLuongTon = ct.SoLuong,
                        MucTonToiThieu = 5
                    });
                }
                else
                {
                    tonKho.SoLuongTon += ct.SoLuong;
                    _context.TonKhos.Update(tonKho);
                }
            }

            await _repo.UpdateAsync(phieu);
            await _context.SaveChangesAsync();

            return new PhieuNhapSummaryDto
            {
                MaPN = phieu.MaPn,
                TongTien = phieu.TongTien ?? 0m,
                Message = $"Xác nhận phiếu nhập #{maPN} thành công. Tồn kho đã được cập nhật."
            };
        }

        // ---------------------------------------------------------------
        // 4. Hủy phiếu nhập
        // ---------------------------------------------------------------
        public async Task<PhieuNhapSummaryDto> HuyPhieuNhapAsync(int maPN)
        {
            var phieu = await _repo.GetByIdAsync(maPN)
                ?? throw new KeyNotFoundException($"Phiếu nhập #{maPN} không tồn tại.");

            await _repo.UpdateAsync(phieu);

            return new PhieuNhapSummaryDto
            {
                MaPN = phieu.MaPn,
                TongTien = phieu.TongTien ?? 0m,
                Message = $"Đã hủy phiếu nhập #{maPN}."
            };
        }

        // ---------------------------------------------------------------
        // 5. Lấy chi tiết 1 phiếu
        // ---------------------------------------------------------------
        public async Task<PhieuNhapDto?> GetByIdAsync(int maPN)
        {
            var phieu = await _repo.GetByIdAsync(maPN);
            return phieu == null ? null : MapToDto(phieu);
        }

        // ---------------------------------------------------------------
        // 6. Lịch sử nhập hàng
        // ---------------------------------------------------------------
        public async Task<IEnumerable<LichSuNhapDto>> GetLichSuAsync(int? maCN, int? maNCC)
        {
            var query = _context.PhieuNhaps
                .Include(p => p.MaCnNavigation)
                .Include(p => p.MaNccNavigation)
                .Include(p => p.MaNvNavigation)
                .Include(p => p.CtPhieuNhaps)
                .AsQueryable();

            if (maCN.HasValue)
                query = query.Where(p => p.MaCn == maCN.Value);
            if (maNCC.HasValue)
                query = query.Where(p => p.MaNcc == maNCC.Value);

            var list = await query.OrderByDescending(p => p.NgayNhap).ToListAsync();

            return list.Select(p => new LichSuNhapDto
            {
                MaPN = p.MaPn,
                TenCN = p.MaCnNavigation?.TenCn ?? "",
                TenNCC = p.MaNccNavigation?.TenNcc ?? "",
                TenNV = p.MaNvNavigation?.HoTen ?? "",
                NgayNhap = p.NgayNhap ?? DateTime.MinValue,
                TongTien = p.TongTien ?? 0m,
                SoMatHang = p.CtPhieuNhaps.Count
            });
        }

        // ---- Mapper ----
        private static PhieuNhapDto MapToDto(PhieuNhap p) => new()
        {
            MaPN = p.MaPn,
            MaCN = p.MaCn,
            TenCN = p.MaCnNavigation?.TenCn ?? "",
            MaNCC = p.MaNcc,
            TenNCC = p.MaNccNavigation?.TenNcc ?? "",
            MaNV = p.MaNv,
            TenNV = p.MaNvNavigation?.HoTen ?? "",
            NgayNhap = p.NgayNhap ?? DateTime.MinValue,
            TongTien = p.TongTien ?? 0m,
            ChiTiets = p.CtPhieuNhaps.Select(ct => new CtPhieuNhapDto
            {
                MaSP = ct.MaSp,
                TenSP = ct.MaSpNavigation?.TenSp ?? "",
                DonViTinh = ct.MaSpNavigation?.DonViTinh ?? "",
                SoLuong = ct.SoLuong,
                DonGiaNhap = ct.DonGiaNhap,
                ThanhTien = ct.ThanhTien
            }).ToList()
        };
    }
}
