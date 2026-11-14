using Microsoft.AspNetCore.Mvc;
using QuanLyBanHangDaChiNhanh.DTOs;
using QuanLyBanHangDaChiNhanh.Services;

namespace QuanLyBanHangDaChiNhanh.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NhaCungCapController : ControllerBase
    {
        private readonly INhaCungCapService _service;
        private readonly ILogger<NhaCungCapController> _logger;

        public NhaCungCapController(INhaCungCapService service, ILogger<NhaCungCapController> logger)
        {
            _service = service;
            _logger  = logger;
        }

        // ---------------------------------------------------------------
        // GET api/nhacungcap
        // Lấy danh sách tất cả nhà cung cấp
        // ---------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // ---------------------------------------------------------------
        // GET api/nhacungcap/{id}
        // Tra cứu nhà cung cấp theo mã
        // ---------------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null)
                return NotFound(new { message = $"Không tìm thấy nhà cung cấp với mã {id}." });
            return Ok(dto);
        }

        // ---------------------------------------------------------------
        // GET api/nhacungcap/search?keyword=abc
        // Tìm kiếm theo tên / SĐT / email
        // ---------------------------------------------------------------
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Từ khóa tìm kiếm không được để trống." });

            var list = await _service.SearchAsync(keyword.Trim());
            return Ok(list);
        }

        // ---------------------------------------------------------------
        // POST api/nhacungcap
        // Thêm nhà cung cấp mới
        // ---------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNhaCungCapDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.MaNCC }, created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm nhà cung cấp.");
                return StatusCode(500, new { message = "Đã xảy ra lỗi, vui lòng thử lại." });
            }
        }

        // ---------------------------------------------------------------
        // PUT api/nhacungcap/{id}
        // Sửa thông tin nhà cung cấp
        // ---------------------------------------------------------------
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateNhaCungCapDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật nhà cung cấp {Id}.", id);
                return StatusCode(500, new { message = "Đã xảy ra lỗi, vui lòng thử lại." });
            }
        }

        // ---------------------------------------------------------------
        // DELETE api/nhacungcap/{id}
        // Xóa nhà cung cấp (không được xóa nếu còn phiếu nhập)
        // ---------------------------------------------------------------
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa nhà cung cấp {Id}.", id);
                return StatusCode(500, new { message = "Đã xảy ra lỗi, vui lòng thử lại." });
            }
        }

        // ---------------------------------------------------------------
        // GET api/nhacungcap/{id}/lichsunhaphang
        // Xem lịch sử nhập hàng của nhà cung cấp
        // ---------------------------------------------------------------
        [HttpGet("{id:int}/lichsunhaphang")]
        public async Task<IActionResult> GetLichSuNhapHang(int id)
        {
            try
            {
                var list = await _service.GetLichSuNhapHangAsync(id);
                return Ok(list);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử nhập hàng của nhà cung cấp {Id}.", id);
                return StatusCode(500, new { message = "Đã xảy ra lỗi, vui lòng thử lại." });
            }
        }
    }
}
