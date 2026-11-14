using Microsoft.AspNetCore.Mvc;
using QuanLyBanHangDaChiNhanh.DTOs;
using QuanLyBanHangDaChiNhanh.Services;

namespace QuanLyBanHangDaChiNhanh.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SanPhamController : ControllerBase
{
    private readonly ISanPhamService _service;

    public SanPhamController(ISanPhamService service)
    {
        _service = service;
    }

    // GET: api/SanPham?tuKhoa= sua&maDm=1
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? tuKhoa, [FromQuery] int? maDm)
        => Ok(await _service.GetAllAsync(tuKhoa, maDm));

    // GET: api/SanPham/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound(new { message = "Không tìm thấy sản phẩm." }) : Ok(result);
    }

    // POST: api/SanPham
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaoSanPhamDTO dto)
    {
        var (data, error) = await _service.CreateAsync(dto);
        if (error != null) return BadRequest(new { message = error });

        return CreatedAtAction(nameof(GetById), new { id = data!.MaSp }, data);
    }

    // PUT: api/SanPham/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CapNhatSanPhamDTO dto)
    {
        var (data, error) = await _service.UpdateAsync(id, dto);
        if (error == "Không tìm thấy sản phẩm.") return NotFound(new { message = error });
        if (error != null) return BadRequest(new { message = error });
        return Ok(data);
    }

    // DELETE: api/SanPham/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        string? error = await _service.DeleteAsync(id);
        if (error == "Không tìm thấy sản phẩm.") return NotFound(new { message = error });
        if (error != null) return Conflict(new { message = error });
        return Ok(new { message = "Xóa sản phẩm thành công." });
    }
}
