using Microsoft.EntityFrameworkCore;
using QuanLyBanHangDaChiNhanh.Data;
using QuanLyBanHangDaChiNhanh.Repositories;
using QuanLyBanHangDaChiNhanh.Services;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký Controller
builder.Services.AddControllers();

// Kết nối SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISanPhamRepository, SanPhamRepository>();
builder.Services.AddScoped<IDanhMucRepository, DanhMucRepository>();
builder.Services.AddScoped<ISanPhamService, SanPhamService>();
builder.Services.AddScoped<IDanhMucService, DanhMucService>();
builder.Services.AddScoped<INhaCungCapRepository, NhaCungCapRepository>();
builder.Services.AddScoped<INhaCungCapService, NhaCungCapService>();
builder.Services.AddScoped<IPhieuNhapRepository, PhieuNhapRepository>();
builder.Services.AddScoped<IPhieuNhapService, PhieuNhapService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGet("/api/kiem-tra-ket-noi", async (ApplicationDbContext db) =>
{
    try
    {
        bool ketNoi = await db.Database.CanConnectAsync();

        return Results.Ok(new
        {
            success = ketNoi,
            message = ketNoi
                ? "Ket noi SQL Server thanh cong!"
                : "Khong the ket noi SQL Server."
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500
        );
    }
});
app.Run();