using QuanLyBanHangDaChiNhanh.DTOs;
using QuanLyBanHangDaChiNhanh.Models.Entities;
using QuanLyBanHangDaChiNhanh.Repositories;

namespace QuanLyBanHangDaChiNhanh.Services;

public class SanPhamService : ISanPhamService
{
    private readonly ISanPhamRepository _sanPhamRepository;
    private readonly IDanhMucRepository _danhMucRepository;

    public SanPhamService(
        ISanPhamRepository sanPhamRepository,
        IDanhMucRepository danhMucRepository)
    {
        _sanPhamRepository = sanPhamRepository;
        _danhMucRepository = danhMucRepository;
    }

    public async Task<List<SanPhamDTO>> GetAllAsync(string? tuKhoa, int? maDm)
    {
        var list = await _sanPhamRepository.GetAllAsync(tuKhoa, maDm);
        return list.Select(ToDTO).ToList();
    }

    public async Task<SanPhamDTO?> GetByIdAsync(int id)
    {
        var sp = await _sanPhamRepository.GetByIdAsync(id);
        return sp == null ? null : ToDTO(sp);
    }

    public async Task<(SanPhamDTO? Data, string? Error)> CreateAsync(TaoSanPhamDTO dto)
    {
        string? error = await ValidateAsync(dto.MaDm, dto.TenSp, dto.GiaBan);
        if (error != null) return (null, error);

        var sp = new SanPham
        {
            MaDm = dto.MaDm,
            TenSp = dto.TenSp.Trim(),
            DonViTinh = Clean(dto.DonViTinh),
            GiaBan = dto.GiaBan,
            MoTa = Clean(dto.MoTa)
        };

        await _sanPhamRepository.AddAsync(sp);
        var saved = await _sanPhamRepository.GetByIdAsync(sp.MaSp);
        return (saved == null ? null : ToDTO(saved), null);
    }

    public async Task<(SanPhamDTO? Data, string? Error)> UpdateAsync(int id, CapNhatSanPhamDTO dto)
    {
        var sp = await _sanPhamRepository.GetByIdAsync(id);
        if (sp == null) return (null, "Không tìm thấy sản phẩm.");

        string? error = await ValidateAsync(dto.MaDm, dto.TenSp, dto.GiaBan);
        if (error != null) return (null, error);

        sp.MaDm = dto.MaDm;
        sp.TenSp = dto.TenSp.Trim();
        sp.DonViTinh = Clean(dto.DonViTinh);
        sp.GiaBan = dto.GiaBan;
        sp.MoTa = Clean(dto.MoTa);

        await _sanPhamRepository.UpdateAsync(sp);
        var updated = await _sanPhamRepository.GetByIdAsync(id);
        return (updated == null ? null : ToDTO(updated), null);
    }

    public async Task<string?> DeleteAsync(int id)
    {
        var sp = await _sanPhamRepository.GetByIdAsync(id);
        if (sp == null) return "Không tìm thấy sản phẩm.";

        bool deleted = await _sanPhamRepository.DeleteAsync(sp);
        return deleted ? null :
            "Không thể xóa sản phẩm vì sản phẩm đang được sử dụng trong dữ liệu nhập, xuất, hóa đơn hoặc tồn kho.";
    }

    private async Task<string?> ValidateAsync(int maDm, string tenSp, decimal giaBan)
    {
        if (string.IsNullOrWhiteSpace(tenSp))
            return "Tên sản phẩm không được để trống.";

        if (tenSp.Trim().Length > 150)
            return "Tên sản phẩm không được vượt quá 150 ký tự.";

        if (giaBan < 0)
            return "Giá bán không được âm.";

        if (!await _danhMucRepository.ExistsAsync(maDm))
            return "Danh mục không tồn tại.";

        return null;
    }

    private static string? Clean(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static SanPhamDTO ToDTO(SanPham sp) => new()
    {
        MaSp = sp.MaSp,
        MaDm = sp.MaDm,
        TenSp = sp.TenSp,
        DonViTinh = sp.DonViTinh,
        GiaBan = sp.GiaBan,
        MoTa = sp.MoTa,
        TenDanhMuc = sp.MaDmNavigation?.TenDm
    };
}
