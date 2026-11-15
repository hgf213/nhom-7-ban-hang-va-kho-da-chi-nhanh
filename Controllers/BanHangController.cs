using Microsoft.AspNetCore.Mvc;
using QuanLyBanHangDaChiNhanh.DTOs;
using QuanLyBanHangDaChiNhanh.Services;

namespace QuanLyBanHangDaChiNhanh.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BanHangController : ControllerBase
    {
        private readonly IBanHangService _service;
        private readonly ILogger<BanHangController> _logger;

        public BanHangController(IBanHangService service, ILogger<BanHangController> logger)
        {
            _service = service;
            _logger  = logger;
        }

        // ---------------------------------------------------------------
        // GET api/banhang/sanpham/timkiem?keyword=cerave&maCN=1
        // Tìm kiếm sản phẩm (kèm tồn kho và khuyến mãi tại chi nhánh)
        // ---------------------------------------------------------------
        [HttpGet("sanpham/timkiem")]
        public async Task<IActionResult> TimKiemSanPham(
            [FromQuery] string keyword,
            [FromQuery] int    maCN)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Từ khóa tìm kiếm không được để trống." });

            try
            {
                var list = await _service.TimKiemSanPhamAsync(keyword.Trim(), maCN);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tìm kiếm sản phẩm.");
                return StatusCode(500, new { message = "Đã xảy ra lỗi." });
            }
        }

        // ---------------------------------------------------------------
        // POST api/banhang/hoadon
        // Tạo hóa đơn mới
        // Body: { maCN, maNV, maKH }
        // ---------------------------------------------------------------
        [HttpPost("hoadon")]
        public async Task<IActionResult> TaoHoaDon([FromBody] CreateHoaDonDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.TaoHoaDonAsync(dto);
                return CreatedAtAction(nameof(GetHoaDonById), new { id = result.MaHD }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo hóa đơn.");
                return StatusCode(500, new { message = "Đã xảy ra lỗi." });
            }
        }

        // ---------------------------------------------------------------
        // POST api/banhang/hoadon/{id}/sanpham
        // Thêm sản phẩm vào hóa đơn
        // Body: { maSP, soLuong, maKM? }
        // ---------------------------------------------------------------
        [HttpPost("hoadon/{id:int}/sanpham")]
        public async Task<IActionResult> ThemSanPham(int id, [FromBody] ThemSanPhamHoaDonDto dto)
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
                _logger.LogError(ex, "Lỗi khi thêm sản phẩm vào hóa đơn {Id}.", id);
                return StatusCode(500, new { message = "Đã xảy ra lỗi." });
            }
        }

        // ---------------------------------------------------------------
        // PUT api/banhang/hoadon/{id}/thanhtoan
        // Thanh toán hóa đơn → trừ tồn kho
        // Body: { phuongThucThanhToan }
        // ---------------------------------------------------------------
        [HttpPut("hoadon/{id:int}/thanhtoan")]
        public async Task<IActionResult> ThanhToan(int id, [FromBody] ThanhToanDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.ThanhToanAsync(id, dto);
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
                _logger.LogError(ex, "Lỗi khi thanh toán hóa đơn {Id}.", id);
                return StatusCode(500, new { message = "Đã xảy ra lỗi." });
            }
        }

        // ---------------------------------------------------------------
        // GET api/banhang/hoadon/{id}
        // Tra cứu hóa đơn theo mã
        // ---------------------------------------------------------------
        [HttpGet("hoadon/{id:int}")]
        public async Task<IActionResult> GetHoaDonById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null)
                return NotFound(new { message = $"Không tìm thấy hóa đơn #{id}." });
            return Ok(dto);
        }

        // ---------------------------------------------------------------
        // GET api/banhang/hoadon/tracuu?maCN=1&maKH=2&tuNgay=2026-11-01&denNgay=2026-11-14
        // Tra cứu danh sách hóa đơn với bộ lọc
        // ---------------------------------------------------------------
        [HttpGet("hoadon/tracuu")]
        public async Task<IActionResult> TraCuu(
            [FromQuery] int?      maCN    = null,
            [FromQuery] int?      maKH    = null,
            [FromQuery] DateTime? tuNgay  = null,
            [FromQuery] DateTime? denNgay = null)
        {
            try
            {
                var list = await _service.TraCuuAsync(maCN, maKH, tuNgay, denNgay);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tra cứu hóa đơn.");
                return StatusCode(500, new { message = "Đã xảy ra lỗi." });
            }
        }
    }
}
