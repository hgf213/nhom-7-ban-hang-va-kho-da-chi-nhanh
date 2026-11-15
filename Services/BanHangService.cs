using Microsoft.EntityFrameworkCore;
using QuanLyBanHangDaChiNhanh.Data;
using QuanLyBanHangDaChiNhanh.DTOs;
using QuanLyBanHangDaChiNhanh.Models.Entities;
using QuanLyBanHangDaChiNhanh.Repositories;

namespace QuanLyBanHangDaChiNhanh.Services
{
    public class BanHangService : IBanHangService
    {
        private readonly IHoaDonRepository _repo;
        private readonly ApplicationDbContext _context;

        public BanHangService(IHoaDonRepository repo, ApplicationDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        // ---------------------------------------------------------------
        // 1. Tìm kiếm sản phẩm
        // ---------------------------------------------------------------
        public async Task<IEnumerable<SanPhamBanHangDto>> TimKiemSanPhamAsync(string keyword, int maCN)
        {
            var now = DateTime.Now;

            var sanPhams = await _context.SanPhams
                .Include(s => s.MaDmNavigation)
                .Where(s => s.TenSp.Contains(keyword))
                .ToListAsync();

            var result = new List<SanPhamBanHangDto>();

            foreach (var sp in sanPhams)
            {
                var tonKho = await _context.TonKhos
                    .FirstOrDefaultAsync(t => t.MaCn == maCN && t.MaSp == sp.MaSp);

                int soLuongTon = tonKho?.SoLuongTon ?? 0;

                var km = await _context.KhuyenMais
                    .Include(k => k.MaSps)
                    .FirstOrDefaultAsync(k =>
                        k.MaSps.Any(s => s.MaSp == sp.MaSp)
                        && k.TrangThai == "Đang chạy"
                        && k.NgayBatDau <= now
                        && k.NgayKetThuc >= now);

                decimal giaSauGiam = sp.GiaBan;
                bool coKhuyenMai = false;

                if (km != null)
                {
                    coKhuyenMai = true;
                    if (km.LoaiGiamGia == "Phần trăm")
                    {
                        decimal tienGiam = sp.GiaBan * km.GiaTriGiam / 100;
                        if (km.GiamToiDa.HasValue && tienGiam > km.GiamToiDa.Value)
                            tienGiam = km.GiamToiDa.Value;
                        giaSauGiam = sp.GiaBan - tienGiam;
                    }
                    else
                    {
                        giaSauGiam = sp.GiaBan - km.GiaTriGiam;
                        if (giaSauGiam < 0) giaSauGiam = 0;
                    }
                }

                result.Add(new SanPhamBanHangDto
                {
                    MaSP = sp.MaSp,
                    TenSP = sp.TenSp,
                    TenDM = sp.MaDmNavigation?.TenDm ?? "",
                    DonViTinh = sp.DonViTinh ?? "",
                    GiaBan = sp.GiaBan,
                    TonKho = soLuongTon,
                    CoKhuyenMai = coKhuyenMai,
                    GiaSauGiam = giaSauGiam
                });
            }

            return result.OrderBy(s => s.TenSP);
        }

        // ---------------------------------------------------------------
        // 2. Tạo hóa đơn mới
        // ---------------------------------------------------------------
        public async Task<HoaDonDto> TaoHoaDonAsync(CreateHoaDonDto dto)
        {
            if (!await _context.ChiNhanhs.AnyAsync(c => c.MaCn == dto.MaCN))
                throw new KeyNotFoundException($"Chi nhánh {dto.MaCN} không tồn tại.");
            if (!await _context.NhanViens.AnyAsync(n => n.MaNv == dto.MaNV))
                throw new KeyNotFoundException($"Nhân viên {dto.MaNV} không tồn tại.");
            if (!await _context.KhachHangs.AnyAsync(k => k.MaKh == dto.MaKH))
                throw new KeyNotFoundException($"Khách hàng {dto.MaKH} không tồn tại.");

            var hoaDon = new HoaDon
            {
                MaCn = dto.MaCN,
                MaNv = dto.MaNV,
                MaKh = dto.MaKH,
                NgayLap = DateTime.Now,
                TongTien = 0,
                TongTienGiam = 0,
                TrangThai = "Chưa thanh toán"
            };

            var created = await _repo.AddAsync(hoaDon);
            var result = await _repo.GetByIdAsync(created.MaHd);
            return MapToDto(result!);
        }

        // ---------------------------------------------------------------
        // 3. Thêm sản phẩm vào hóa đơn
        // ---------------------------------------------------------------
        public async Task<HoaDonDto> ThemSanPhamAsync(int maHD, ThemSanPhamHoaDonDto dto)
        {
            var hoaDon = await _repo.GetByIdAsync(maHD)
                ?? throw new KeyNotFoundException($"Hóa đơn #{maHD} không tồn tại.");

            if (hoaDon.TrangThai == "Đã thanh toán")
                throw new InvalidOperationException("Hóa đơn đã thanh toán, không thể chỉnh sửa.");

            var sanPham = await _context.SanPhams.FindAsync(dto.MaSP)
                ?? throw new KeyNotFoundException($"Sản phẩm {dto.MaSP} không tồn tại.");

            var tonKho = await _context.TonKhos
                .FirstOrDefaultAsync(t => t.MaCn == hoaDon.MaCn && t.MaSp == dto.MaSP);

            int soLuongHienCo = tonKho?.SoLuongTon ?? 0;

            var existing = await _repo.GetChiTietAsync(maHD, dto.MaSP);
            int soLuongDaCoTrongHD = existing?.SoLuong ?? 0;
            int soLuongCanThem = dto.SoLuong - soLuongDaCoTrongHD;

            if (soLuongHienCo < soLuongCanThem)
                throw new InvalidOperationException(
                    $"Tồn kho không đủ. Hiện còn {soLuongHienCo}, đã đặt {soLuongDaCoTrongHD}, cần thêm {soLuongCanThem}.");

            decimal tienGiam = 0;
            if (dto.MaKM.HasValue)
            {
                var km = await _context.KhuyenMais
                    .Include(k => k.MaSps)
                    .FirstOrDefaultAsync(k => k.MaKm == dto.MaKM.Value);
                if (km != null)
                {
                    bool spCoTrongKM = km.MaSps.Any(s => s.MaSp == dto.MaSP);

                    if (spCoTrongKM)
                    {
                        if (km.LoaiGiamGia == "Phần trăm")
                        {
                            tienGiam = sanPham.GiaBan * dto.SoLuong * km.GiaTriGiam / 100;
                            if (km.GiamToiDa.HasValue && tienGiam > km.GiamToiDa.Value)
                                tienGiam = km.GiamToiDa.Value;
                        }
                        else
                        {
                            tienGiam = km.GiaTriGiam * dto.SoLuong;
                        }
                    }
                }
            }

            decimal thanhTien = sanPham.GiaBan * dto.SoLuong - tienGiam;
            if (thanhTien < 0) thanhTien = 0;

            if (existing != null)
            {
                existing.SoLuong = dto.SoLuong;
                existing.DonGia = sanPham.GiaBan;
                existing.TienGiam = tienGiam;
                existing.ThanhTien = thanhTien;
                await _repo.UpdateChiTietAsync(existing);
            }
            else
            {
                await _repo.AddChiTietAsync(new CtHoaDon
                {
                    MaHd = maHD,
                    MaSp = dto.MaSP,
                    SoLuong = dto.SoLuong,
                    DonGia = sanPham.GiaBan,
                    TienGiam = tienGiam,
                    ThanhTien = thanhTien
                });
            }

            var allCt = await _context.CtHoaDons.Where(ct => ct.MaHd == maHD).ToListAsync();
            hoaDon.TongTien = allCt.Sum(ct => ct.SoLuong * ct.DonGia);
            hoaDon.TongTienGiam = allCt.Sum(ct => ct.TienGiam ?? 0);
            await _repo.UpdateAsync(hoaDon);

            var result = await _repo.GetByIdAsync(maHD);
            return MapToDto(result!);
        }

        // ---------------------------------------------------------------
        // 4. Thanh toán hóa đơn → trừ tồn kho
        // ---------------------------------------------------------------
        public async Task<HoaDonDto> ThanhToanAsync(int maHD, ThanhToanDto dto)
        {
            var hoaDon = await _repo.GetByIdAsync(maHD)
                ?? throw new KeyNotFoundException($"Hóa đơn #{maHD} không tồn tại.");

            if (hoaDon.TrangThai == "Đã thanh toán")
                throw new InvalidOperationException("Hóa đơn này đã được thanh toán trước đó.");

            if (!hoaDon.CtHoaDons.Any())
                throw new InvalidOperationException("Hóa đơn chưa có sản phẩm nào.");

            foreach (var ct in hoaDon.CtHoaDons)
            {
                var tonKho = await _context.TonKhos
                    .FirstOrDefaultAsync(t => t.MaCn == hoaDon.MaCn && t.MaSp == ct.MaSp)
                    ?? throw new InvalidOperationException($"Không tìm thấy tồn kho sản phẩm {ct.MaSp} tại chi nhánh {hoaDon.MaCn}.");

                if (tonKho.SoLuongTon < ct.SoLuong)
                    throw new InvalidOperationException(
                        $"Tồn kho sản phẩm '{ct.MaSpNavigation?.TenSp}' không đủ (còn {tonKho.SoLuongTon}, cần {ct.SoLuong}).");

                tonKho.SoLuongTon -= ct.SoLuong;
                _context.TonKhos.Update(tonKho);
            }

            hoaDon.PhuongThucThanhToan = dto.PhuongThucThanhToan;
            hoaDon.TrangThai = "Đã thanh toán";
            await _repo.UpdateAsync(hoaDon);
            await _context.SaveChangesAsync();

            var result = await _repo.GetByIdAsync(maHD);
            return MapToDto(result!);
        }

        // ---------------------------------------------------------------
        // 5. Tra cứu hóa đơn theo mã
        // ---------------------------------------------------------------
        public async Task<HoaDonDto?> GetByIdAsync(int maHD)
        {
            var hoaDon = await _repo.GetByIdAsync(maHD);
            return hoaDon == null ? null : MapToDto(hoaDon);
        }

        // ---------------------------------------------------------------
        // 6. Tra cứu danh sách hóa đơn
        // ---------------------------------------------------------------
        public async Task<IEnumerable<HoaDonSummaryDto>> TraCuuAsync(
            int? maCN, int? maKH, DateTime? tuNgay, DateTime? denNgay)
        {
            var query = _context.HoaDons
                .Include(h => h.MaCnNavigation)
                .Include(h => h.MaNvNavigation)
                .Include(h => h.MaKhNavigation)
                .Include(h => h.CtHoaDons)
                .AsQueryable();

            if (maCN.HasValue) query = query.Where(h => h.MaCn == maCN.Value);
            if (maKH.HasValue) query = query.Where(h => h.MaKh == maKH.Value);
            if (tuNgay.HasValue) query = query.Where(h => h.NgayLap >= tuNgay.Value);
            if (denNgay.HasValue) query = query.Where(h => h.NgayLap <= denNgay.Value.AddDays(1));

            var list = await query.OrderByDescending(h => h.NgayLap).ToListAsync();

            return list.Select(h => new HoaDonSummaryDto
            {
                MaHD = h.MaHd,
                TenCN = h.MaCnNavigation?.TenCn ?? "",
                TenNV = h.MaNvNavigation?.HoTen ?? "",
                TenKH = h.MaKhNavigation?.HoTen ?? "",
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
        private static HoaDonDto MapToDto(HoaDon h) => new()
        {
            MaHD = h.MaHd,
            MaCN = h.MaCn,
            TenCN = h.MaCnNavigation?.TenCn ?? "",
            MaNV = h.MaNv,
            TenNV = h.MaNvNavigation?.HoTen ?? "",
            MaKH = h.MaKh,
            TenKH = h.MaKhNavigation?.HoTen ?? "",
            SdtKH = h.MaKhNavigation?.SoDienThoai,
            NgayLap = h.NgayLap ?? DateTime.MinValue,
            TongTien = h.TongTien ?? 0,
            TongTienGiam = h.TongTienGiam ?? 0,
            ThanhToanThucTe = (h.TongTien ?? 0) - (h.TongTienGiam ?? 0),
            PhuongThucThanhToan = h.PhuongThucThanhToan ?? "",
            TrangThai = h.TrangThai ?? "",
            ChiTiets = h.CtHoaDons.Select(ct => new CtHoaDonDto
            {
                MaSP = ct.MaSp,
                TenSP = ct.MaSpNavigation?.TenSp ?? "",
                DonViTinh = ct.MaSpNavigation?.DonViTinh ?? "",
                SoLuong = ct.SoLuong,
                DonGia = ct.DonGia,
                TienGiam = ct.TienGiam ?? 0,
                ThanhTien = ct.ThanhTien
            }).ToList()
        };
    }
}
