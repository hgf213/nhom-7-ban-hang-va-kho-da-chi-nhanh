using Microsoft.AspNetCore.Mvc;
using QuanLyBanHangDaChiNhanh.DTOs;
using QuanLyBanHangDaChiNhanh.Services;

namespace QuanLyBanHangDaChiNhanh.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhieuNhapController : ControllerBase
    {
        private readonly IPhieuNhapService _service;
        private readonly ILogger<PhieuNhapController> _logger;

        public PhieuNhapController(IPhieuNhapService service, ILogger<PhieuNhapController> logger)
        {
            _service = service;
            _logger  = logger;
        }

        // ---------------------------------------------------------------
        // POST api/phieunhap
        // Tạo phiếu nhập mới
        // ---------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> TaoPhieuNhap([FromBody] CreatePhieuNhapDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.TaoPhieuNhapAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.MaPN }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo phiếu nhập.");
                return StatusCode(500, new { message = "Đã xảy ra lỗi." });
            }
        }

        // ---------------------------------------------------------------
        // POST api/phieunhap/{id}/sanpham
        // Thêm sản phẩm vào phiếu nhập
        // ---------------------------------------------------------------
        [HttpPost("{id:int}/sanpham")]
        public async Task<IActionResult> ThemSanPham(int id, [FromBody] AddSanPhamVaoPhieuDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.ThemSanPhamAsync(id, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm sản phẩm vào phiếu nhập {Id}.", id);
                return StatusCode(500, new { message = "Đã xảy ra lỗi." });
            }
        }

        // ---------------------------------------------------------------
        // PUT api/phieunhap/{id}/xacnhan
        // Xác nhận phiếu nhập → cập nhật tồn kho
        // ---------------------------------------------------------------
        [HttpPut("{id:int}/xacnhan")]
        public async Task<IActionResult> XacNhan(int id)
        {
            try
            {
                var result = await _service.XacNhanPhieuNhapAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xác nhận phiếu nhập {Id}.", id);
                return StatusCode(500, new { message = "Đã xảy ra lỗi." });
            }
        }

        // ---------------------------------------------------------------
        // PUT api/phieunhap/{id}/huy
        // Hủy phiếu nhập
        // ---------------------------------------------------------------
        [HttpPut("{id:int}/huy")]
        public async Task<IActionResult> Huy(int id)
        {
            try
            {
                var result = await _service.HuyPhieuNhapAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hủy phiếu nhập {Id}.", id);
                return StatusCode(500, new { message = "Đã xảy ra lỗi." });
            }
        }

        // ---------------------------------------------------------------
        // GET api/phieunhap/{id}
        // Xem chi tiết 1 phiếu nhập
        // ---------------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null)
                return NotFound(new { message = $"Không tìm thấy phiếu nhập #{id}." });
            return Ok(dto);
        }

        // ---------------------------------------------------------------
        // GET api/phieunhap/lichsu?maCN=1&maNCC=2
        // Xem lịch sử nhập hàng (lọc tùy chọn theo chi nhánh / NCC)
        // ---------------------------------------------------------------
        [HttpGet("lichsu")]
        public async Task<IActionResult> GetLichSu(
            [FromQuery] int? maCN  = null,
            [FromQuery] int? maNCC = null)
        {
            try
            {
                var list = await _service.GetLichSuAsync(maCN, maNCC);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử nhập hàng.");
                return StatusCode(500, new { message = "Đã xảy ra lỗi." });
            }
        }
    }
}
