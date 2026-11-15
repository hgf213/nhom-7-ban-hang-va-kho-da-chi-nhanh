using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Data;
using QuanLyBanHang.DTOs;
using QuanLyBanHang.Models;
using QuanLyBanHang.Repositories;

namespace QuanLyBanHang.Services
{
    public class BanHangService : IBanHangService
    {
        private readonly IHoaDonRepository _repo;
        private readonly AppDbContext _context;

        public BanHangService(IHoaDonRepository repo, AppDbContext context)
        {
            _repo    = repo;
            _context = context;
        }

        // ---------------------------------------------------------------
        // 1. Tìm kiếm sản phẩm
        // ---------------------------------------------------------------
        public async Task<IEnumerable<SanPhamBanHangDto>> TimKiemSanPhamAsync(string keyword, int maCN)
        {
            var now = DateTime.Now;

            var sanPhams = await _context.SanPhams
                .Include(s => s.DanhMuc)
                .Where(s => s.TenSP.Contains(keyword))
                .ToListAsync();

            var result = new List<SanPhamBanHangDto>();

            foreach (var sp in sanPhams)
            {
                // Lấy tồn kho tại chi nhánh
                var tonKho = await _context.TonKhos
                    .FirstOrDefaultAsync(t => t.MaCN == maCN && t.MaSP == sp.MaSP);

                int soLuongTon = tonKho?.SoLuongTon ?? 0;

                // Kiểm tra khuyến mãi đang chạy cho sản phẩm này
                var km = await _context.CtKhuyenMais
                    .Include(c => c.KhuyenMai)
                    .Where(c => c.MaSP == sp.MaSP
                             && c.KhuyenMai.TrangThai == "Đang chạy"
                             && c.KhuyenMai.NgayBatDau <= now
                             && c.KhuyenMai.NgayKetThuc >= now)
                    .Select(c => c.KhuyenMai)
                    .FirstOrDefaultAsync();

                decimal giaSauGiam = sp.GiaBan;
                bool coKhuyenMai   = false;

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
                    else // Số tiền cố định
                    {
                        giaSauGiam = sp.GiaBan - km.GiaTriGiam;
                        if (giaSauGiam < 0) giaSauGiam = 0;
                    }
                }

                result.Add(new SanPhamBanHangDto
                {
                    MaSP        = sp.MaSP,
                    TenSP       = sp.TenSP,
                    TenDM       = sp.DanhMuc?.TenDM ?? "",
                    DonViTinh   = sp.DonViTinh ?? "",
                    GiaBan      = sp.GiaBan,
                    TonKho      = soLuongTon,
                    CoKhuyenMai = coKhuyenMai,
                    GiaSauGiam  = giaSauGiam
                });
            }

            return result.OrderBy(s => s.TenSP);
        }

        // ---------------------------------------------------------------
        // 2. Tạo hóa đơn mới
        // ---------------------------------------------------------------
        public async Task<HoaDonDto> TaoHoaDonAsync(CreateHoaDonDto dto)
        {
            if (!await _context.ChiNhanhs.AnyAsync(c => c.MaCN == dto.MaCN))
                throw new KeyNotFoundException($"Chi nhánh {dto.MaCN} không tồn tại.");
            if (!await _context.NhanViens.AnyAsync(n => n.MaNV == dto.MaNV))
                throw new KeyNotFoundException($"Nhân viên {dto.MaNV} không tồn tại.");
            if (!await _context.KhachHangs.AnyAsync(k => k.MaKH == dto.MaKH))
                throw new KeyNotFoundException($"Khách hàng {dto.MaKH} không tồn tại.");

            var hoaDon = new HoaDon
            {
                MaCN     = dto.MaCN,
                MaNV     = dto.MaNV,
                MaKH     = dto.MaKH,
                NgayLap  = DateTime.Now,
                TongTien = 0,
                TongTienGiam = 0,
                TrangThai = "Chưa thanh toán"
            };

            var created = await _repo.AddAsync(hoaDon);
            var result  = await _repo.GetByIdAsync(created.MaHD);
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

            // Kiểm tra tồn kho tại chi nhánh
            var tonKho = await _context.TonKhos
                .FirstOrDefaultAsync(t => t.MaCN == hoaDon.MaCN && t.MaSP == dto.MaSP);

            int soLuongHienCo = tonKho?.SoLuongTon ?? 0;

            // Lấy số lượng SP này đã có trong hóa đơn (nếu thêm lại)
            var existing = await _repo.GetChiTietAsync(maHD, dto.MaSP);
            int soLuongDaCoTrongHD = existing?.SoLuong ?? 0;
            int soLuongCanThem     = dto.SoLuong - soLuongDaCoTrongHD;

            if (soLuongHienCo < soLuongCanThem)
                throw new InvalidOperationException(
                    $"Tồn kho không đủ. Hiện còn {soLuongHienCo}, đã đặt {soLuongDaCoTrongHD}, cần thêm {soLuongCanThem}.");

            // Tính giảm giá nếu có khuyến mãi
            decimal tienGiam = 0;
            if (dto.MaKM.HasValue)
            {
                var km = await _context.KhuyenMais.FindAsync(dto.MaKM.Value);
                if (km != null)
                {
                    bool spCoTrongKM = await _context.CtKhuyenMais
                        .AnyAsync(c => c.MaKM == dto.MaKM.Value && c.MaSP == dto.MaSP);

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
                existing.SoLuong  = dto.SoLuong;
                existing.DonGia   = sanPham.GiaBan;
                existing.TienGiam = tienGiam;
                existing.ThanhTien = thanhTien;
                await _repo.UpdateChiTietAsync(existing);
            }
            else
            {
                await _repo.AddChiTietAsync(new CtHoaDon
                {
                    MaHD      = maHD,
                    MaSP      = dto.MaSP,
                    SoLuong   = dto.SoLuong,
                    DonGia    = sanPham.GiaBan,
                    TienGiam  = tienGiam,
                    ThanhTien = thanhTien
                });
            }

            // Cập nhật tổng tiền hóa đơn
            var allCt = await _context.CtHoaDons.Where(ct => ct.MaHD == maHD).ToListAsync();
            hoaDon.TongTien     = allCt.Sum(ct => ct.SoLuong * ct.DonGia);
            hoaDon.TongTienGiam = allCt.Sum(ct => ct.TienGiam);
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

            if (!hoaDon.ChiTiets.Any())
                throw new InvalidOperationException("Hóa đơn chưa có sản phẩm nào.");

            // Trừ tồn kho
            foreach (var ct in hoaDon.ChiTiets)
            {
                var tonKho = await _context.TonKhos
                    .FirstOrDefaultAsync(t => t.MaCN == hoaDon.MaCN && t.MaSP == ct.MaSP)
                    ?? throw new InvalidOperationException($"Không tìm thấy tồn kho sản phẩm {ct.MaSP} tại chi nhánh {hoaDon.MaCN}.");

                if (tonKho.SoLuongTon < ct.SoLuong)
                    throw new InvalidOperationException(
                        $"Tồn kho sản phẩm '{ct.SanPham?.TenSP}' không đủ (còn {tonKho.SoLuongTon}, cần {ct.SoLuong}).");

                tonKho.SoLuongTon -= ct.SoLuong;
                _context.TonKhos.Update(tonKho);
            }

            hoaDon.PhuongThucThanhToan = dto.PhuongThucThanhToan;
            hoaDon.TrangThai           = "Đã thanh toán";
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
                .Include(h => h.ChiNhanh)
                .Include(h => h.NhanVien)
                .Include(h => h.KhachHang)
                .Include(h => h.ChiTiets)
                .AsQueryable();

            if (maCN.HasValue)   query = query.Where(h => h.MaCN == maCN.Value);
            if (maKH.HasValue)   query = query.Where(h => h.MaKH == maKH.Value);
            if (tuNgay.HasValue) query = query.Where(h => h.NgayLap >= tuNgay.Value);
            if (denNgay.HasValue) query = query.Where(h => h.NgayLap <= denNgay.Value.AddDays(1));

            var list = await query.OrderByDescending(h => h.NgayLap).ToListAsync();

            return list.Select(h => new HoaDonSummaryDto
            {
                MaHD                = h.MaHD,
                TenCN               = h.ChiNhanh?.TenCN   ?? "",
                TenNV               = h.NhanVien?.HoTen    ?? "",
                TenKH               = h.KhachHang?.HoTen   ?? "",
                NgayLap             = h.NgayLap,
                TongTien            = h.TongTien,
                TongTienGiam        = h.TongTienGiam,
                ThanhToanThucTe     = h.TongTien - h.TongTienGiam,
                PhuongThucThanhToan = h.PhuongThucThanhToan,
                TrangThai           = h.TrangThai,
                SoMatHang           = h.ChiTiets.Count
            });
        }

        // ---------------------------------------------------------------
        // Mapper
        // ---------------------------------------------------------------
        private static HoaDonDto MapToDto(HoaDon h) => new()
        {
            MaHD                = h.MaHD,
            MaCN                = h.MaCN,
            TenCN               = h.ChiNhanh?.TenCN    ?? "",
            MaNV                = h.MaNV,
            TenNV               = h.NhanVien?.HoTen     ?? "",
            MaKH                = h.MaKH,
            TenKH               = h.KhachHang?.HoTen    ?? "",
            SdtKH               = h.KhachHang?.SoDienThoai,
            NgayLap             = h.NgayLap,
            TongTien            = h.TongTien,
            TongTienGiam        = h.TongTienGiam,
            ThanhToanThucTe     = h.TongTien - h.TongTienGiam,
            PhuongThucThanhToan = h.PhuongThucThanhToan,
            TrangThai           = h.TrangThai,
            ChiTiets = h.ChiTiets.Select(ct => new CtHoaDonDto
            {
                MaSP      = ct.MaSP,
                TenSP     = ct.SanPham?.TenSP     ?? "",
                DonViTinh = ct.SanPham?.DonViTinh ?? "",
                SoLuong   = ct.SoLuong,
                DonGia    = ct.DonGia,
                TienGiam  = ct.TienGiam,
                ThanhTien = ct.ThanhTien
            }).ToList()
        };
    }
}
