using Microsoft.AspNetCore.Mvc;
using QuanLyBanHangDaChiNhanh.DTOs;
using QuanLyBanHangDaChiNhanh.Services;

namespace QuanLyBanHangDaChiNhanh.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DanhMucController : ControllerBase
{
    private readonly IDanhMucService _service;

    public DanhMucController(IDanhMucService service)
    {
        _service = service;
    }

    // GET: api/DanhMuc
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    // GET: api/DanhMuc/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound(new { message = "Không tìm thấy danh mục." }) : Ok(result);
    }

    // POST: api/DanhMuc
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaoDanhMucDTO dto)
    {
        var (data, error) = await _service.CreateAsync(dto);
        if (error != null) return BadRequest(new { message = error });
        return CreatedAtAction(nameof(GetById), new { id = data!.MaDm }, data);
    }

    // PUT: api/DanhMuc/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CapNhatDanhMucDTO dto)
    {
        var (data, error) = await _service.UpdateAsync(id, dto);
        if (error == "Không tìm thấy danh mục.") return NotFound(new { message = error });
        if (error != null) return BadRequest(new { message = error });
        return Ok(data);
    }

    // DELETE: api/DanhMuc/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        string? error = await _service.DeleteAsync(id);
        if (error == "Không tìm thấy danh mục.") return NotFound(new { message = error });
        if (error != null) return Conflict(new { message = error });
        return Ok(new { message = "Xóa danh mục thành công." });
    }
}
