using Microsoft.AspNetCore.Mvc;
using QuanLyBanHangDaChiNhanh.DTOs;
using QuanLyBanHangDaChiNhanh.Services;

namespace QuanLyBanHangDaChiNhanh.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KhachHangController : ControllerBase
    {
        private readonly IKhachHangService _service;
        private readonly ILogger<KhachHangController> _logger;

        public KhachHangController(IKhachHangService service, ILogger<KhachHangController> logger)
        {
            _service = service;
            _logger  = logger;
        }

        // ---------------------------------------------------------------
        // GET api/khachhang
        // Lấy danh sách tất cả khách hàng
        // ---------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // ---------------------------------------------------------------
        // GET api/khachhang/{id}
        // Lấy thông tin 1 khách hàng
        // ---------------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null)
                return NotFound(new { message = $"Không tìm thấy khách hàng với mã {id}." });
            return Ok(dto);
        }

        // ---------------------------------------------------------------
        // GET api/khachhang/timkiem?keyword=abc
        // Tìm kiếm theo tên / SĐT / email
        // ---------------------------------------------------------------
        [HttpGet("timkiem")]
        public async Task<IActionResult> TimKiem([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Từ khóa tìm kiếm không được để trống." });

            var list = await _service.TimKiemAsync(keyword.Trim());
            return Ok(list);
        }

        // ---------------------------------------------------------------
        // POST api/khachhang
        // Thêm khách hàng mới
        // ---------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateKhachHangDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.MaKH }, created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm khách hàng.");
                return StatusCode(500, new { message = "Đã xảy ra lỗi." });
            }
        }

        // ---------------------------------------------------------------
        // PUT api/khachhang/{id}
        // Sửa thông tin khách hàng
        // ---------------------------------------------------------------
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateKhachHangDto dto)
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
                _logger.LogError(ex, "Lỗi khi cập nhật khách hàng {Id}.", id);
                return StatusCode(500, new { message = "Đã xảy ra lỗi." });
            }
        }

        // ---------------------------------------------------------------
        // DELETE api/khachhang/{id}
        // Xóa khách hàng (chặn nếu có hóa đơn)
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
                _logger.LogError(ex, "Lỗi khi xóa khách hàng {Id}.", id);
                return StatusCode(500, new { message = "Đã xảy ra lỗi." });
            }
        }

        // ---------------------------------------------------------------
        // GET api/khachhang/{id}/lichsumuahang
        // Xem lịch sử mua hàng của khách hàng
        // ---------------------------------------------------------------
        [HttpGet("{id:int}/lichsumuahang")]
        public async Task<IActionResult> GetLichSuMuaHang(int id)
        {
            try
            {
                var list = await _service.GetLichSuMuaHangAsync(id);
                return Ok(list);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử mua hàng của khách hàng {Id}.", id);
                return StatusCode(500, new { message = "Đã xảy ra lỗi." });
            }
        }
    }
}
