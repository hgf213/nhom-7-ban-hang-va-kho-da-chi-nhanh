using QuanLyBanHangDaChiNhanh.DTOs;
using QuanLyBanHangDaChiNhanh.Models.Entities;
using QuanLyBanHangDaChiNhanh.Repositories;

namespace QuanLyBanHangDaChiNhanh.Services;

public class DanhMucService : IDanhMucService
{
    private readonly IDanhMucRepository _repository;

    public DanhMucService(IDanhMucRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<DanhMucDTO>> GetAllAsync()
        => (await _repository.GetAllAsync()).Select(ToDTO).ToList();

    public async Task<DanhMucDTO?> GetByIdAsync(int id)
    {
        var dm = await _repository.GetByIdAsync(id);
        return dm == null ? null : ToDTO(dm);
    }

    public async Task<(DanhMucDTO? Data, string? Error)> CreateAsync(TaoDanhMucDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.TenDm))
            return (null, "Tên danh mục không được để trống.");

        string ten = dto.TenDm.Trim();
        if (ten.Length > 100)
            return (null, "Tên danh mục không được vượt quá 100 ký tự.");

        if (await _repository.IsNameExistsAsync(ten))
            return (null, "Tên danh mục đã tồn tại.");

        var dm = new DanhMuc { TenDm = ten };
        await _repository.AddAsync(dm);
        return (ToDTO(dm), null);
    }

    public async Task<(DanhMucDTO? Data, string? Error)> UpdateAsync(int id, CapNhatDanhMucDTO dto)
    {
        var dm = await _repository.GetByIdAsync(id);
        if (dm == null) return (null, "Không tìm thấy danh mục.");

        if (string.IsNullOrWhiteSpace(dto.TenDm))
            return (null, "Tên danh mục không được để trống.");

        string ten = dto.TenDm.Trim();
        if (ten.Length > 100)
            return (null, "Tên danh mục không được vượt quá 100 ký tự.");

        if (await _repository.IsNameExistsAsync(ten, id))
            return (null, "Tên danh mục đã tồn tại.");

        dm.TenDm = ten;
        await _repository.UpdateAsync(dm);
        return (ToDTO(dm), null);
    }

    public async Task<string?> DeleteAsync(int id)
    {
        var dm = await _repository.GetByIdAsync(id);
        if (dm == null) return "Không tìm thấy danh mục.";

        bool deleted = await _repository.DeleteAsync(dm);
        return deleted ? null :
            "Không thể xóa danh mục vì vẫn còn sản phẩm thuộc danh mục này.";
    }

    private static DanhMucDTO ToDTO(DanhMuc dm) => new()
    {
        MaDm = dm.MaDm,
        TenDm = dm.TenDm
    };
}
