using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHangDaChiNhanh.Models.Entities;

namespace QuanLyBanHangDaChiNhanh.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiNhanh> ChiNhanhs { get; set; }

    public virtual DbSet<CtDieuChuyen> CtDieuChuyens { get; set; }

    public virtual DbSet<CtHoaDon> CtHoaDons { get; set; }

    public virtual DbSet<CtKiemKe> CtKiemKes { get; set; }

    public virtual DbSet<CtPhieuNhap> CtPhieuNhaps { get; set; }

    public virtual DbSet<CtPhieuTraHang> CtPhieuTraHangs { get; set; }

    public virtual DbSet<CtPhieuXuat> CtPhieuXuats { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<PhieuDieuChuyen> PhieuDieuChuyens { get; set; }

    public virtual DbSet<PhieuKiemKe> PhieuKiemKes { get; set; }

    public virtual DbSet<PhieuNhap> PhieuNhaps { get; set; }

    public virtual DbSet<PhieuTraHang> PhieuTraHangs { get; set; }

    public virtual DbSet<PhieuXuatKho> PhieuXuatKhos { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<TonKho> TonKhos { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiNhanh>(entity =>
        {
            entity.HasKey(e => e.MaCn).HasName("PK__CHI_NHAN__27258E0E637B95A7");

            entity.ToTable("CHI_NHANH");

            entity.Property(e => e.MaCn).HasColumnName("MaCN");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TenCn)
                .HasMaxLength(100)
                .HasColumnName("TenCN");
            entity.Property(e => e.TrangThai).HasMaxLength(50);
        });

        modelBuilder.Entity<CtDieuChuyen>(entity =>
        {
            entity.HasKey(e => new { e.MaPdc, e.MaSp }).HasName("PK__CT_DIEU___E892187B8FBC85C2");

            entity.ToTable("CT_DIEU_CHUYEN");

            entity.Property(e => e.MaPdc).HasColumnName("MaPDC");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");

            entity.HasOne(d => d.MaPdcNavigation).WithMany(p => p.CtDieuChuyens)
                .HasForeignKey(d => d.MaPdc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CT_DIEU_C__MaPDC__25518C17");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.CtDieuChuyens)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CT_DIEU_CH__MaSP__2645B050");
        });

        modelBuilder.Entity<CtHoaDon>(entity =>
        {
            entity.HasKey(e => new { e.MaHd, e.MaSp }).HasName("PK__CT_HOA_D__F557F6610E428444");

            entity.ToTable("CT_HOA_DON");

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TienGiam)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.CtHoaDons)
                .HasForeignKey(d => d.MaHd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CT_HOA_DON__MaHD__74AE54BC");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.CtHoaDons)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CT_HOA_DON__MaSP__75A278F5");
        });

        modelBuilder.Entity<CtKiemKe>(entity =>
        {
            entity.HasKey(e => new { e.MaKk, e.MaSp }).HasName("PK__CT_KIEM___F5579F9A938BC64B");

            entity.ToTable("CT_KIEM_KE");

            entity.Property(e => e.MaKk).HasColumnName("MaKK");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");

            entity.HasOne(d => d.MaKkNavigation).WithMany(p => p.CtKiemKes)
                .HasForeignKey(d => d.MaKk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CT_KIEM_KE__MaKK__1BC821DD");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.CtKiemKes)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CT_KIEM_KE__MaSP__1CBC4616");
        });

        modelBuilder.Entity<CtPhieuNhap>(entity =>
        {
            entity.HasKey(e => new { e.MaPn, e.MaSp }).HasName("PK__CT_PHIEU__F557B771C5578BB6");

            entity.ToTable("CT_PHIEU_NHAP");

            entity.Property(e => e.MaPn).HasColumnName("MaPN");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.DonGiaNhap).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaPnNavigation).WithMany(p => p.CtPhieuNhaps)
                .HasForeignKey(d => d.MaPn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CT_PHIEU_N__MaPN__0A9D95DB");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.CtPhieuNhaps)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CT_PHIEU_N__MaSP__0B91BA14");
        });

        modelBuilder.Entity<CtPhieuTraHang>(entity =>
        {
            entity.HasKey(e => new { e.MaPth, e.MaHd, e.MaSp }).HasName("PK__CT_PHIEU__35B6A9B5CBCBE0D3");

            entity.ToTable("CT_PHIEU_TRA_HANG");

            entity.Property(e => e.MaPth).HasColumnName("MaPTH");
            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.DonGiaHoan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NhapKho).HasDefaultValue(true);
            entity.Property(e => e.ThanhTienHoan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TinhTrang).HasMaxLength(100);

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.CtPhieuTraHangs)
                .HasForeignKey(d => d.MaHd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CT_PHIEU_T__MaHD__00200768");

            entity.HasOne(d => d.MaPthNavigation).WithMany(p => p.CtPhieuTraHangs)
                .HasForeignKey(d => d.MaPth)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CT_PHIEU___MaPTH__7F2BE32F");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.CtPhieuTraHangs)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CT_PHIEU_T__MaSP__01142BA1");
        });

        modelBuilder.Entity<CtPhieuXuat>(entity =>
        {
            entity.HasKey(e => new { e.MaPx, e.MaSp }).HasName("PK__CT_PHIEU__F557B74BB6285972");

            entity.ToTable("CT_PHIEU_XUAT");

            entity.Property(e => e.MaPx).HasColumnName("MaPX");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");

            entity.HasOne(d => d.MaPxNavigation).WithMany(p => p.CtPhieuXuats)
                .HasForeignKey(d => d.MaPx)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CT_PHIEU_X__MaPX__1332DBDC");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.CtPhieuXuats)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CT_PHIEU_X__MaSP__14270015");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.MaDm).HasName("PK__DANH_MUC__2725866E0779C5DE");

            entity.ToTable("DANH_MUC");

            entity.Property(e => e.MaDm).HasColumnName("MaDM");
            entity.Property(e => e.TenDm)
                .HasMaxLength(100)
                .HasColumnName("TenDM");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK__HOA_DON__2725A6E0E710DB60");

            entity.ToTable("HOA_DON");

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.MaCn).HasColumnName("MaCN");
            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.NgayLap)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhuongThucThanhToan).HasMaxLength(50);
            entity.Property(e => e.TongTien)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TongTienGiam)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaCnNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaCn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HOA_DON__MaCN__6EF57B66");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HOA_DON__MaKH__70DDC3D8");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HOA_DON__MaNV__6FE99F9F");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__KHACH_HA__2725CF1E1318DB54");

            entity.ToTable("KHACH_HANG");

            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<KhuyenMai>(entity =>
        {
            entity.HasKey(e => e.MaKm).HasName("PK__KHUYEN_M__2725CF1586D84A9B");

            entity.ToTable("KHUYEN_MAI");

            entity.Property(e => e.MaKm).HasColumnName("MaKM");
            entity.Property(e => e.DonHangToiThieu)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiaTriGiam).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiamToiDa).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LoaiGiamGia).HasMaxLength(50);
            entity.Property(e => e.NgayBatDau).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.TenKm)
                .HasMaxLength(150)
                .HasColumnName("TenKM");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasMany(d => d.MaSps).WithMany(p => p.MaKms)
                .UsingEntity<Dictionary<string, object>>(
                    "CtKhuyenMai",
                    r => r.HasOne<SanPham>().WithMany()
                        .HasForeignKey("MaSp")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CT_KHUYEN___MaSP__6383C8BA"),
                    l => l.HasOne<KhuyenMai>().WithMany()
                        .HasForeignKey("MaKm")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CT_KHUYEN___MaKM__628FA481"),
                    j =>
                    {
                        j.HasKey("MaKm", "MaSp").HasName("PK__CT_KHUYE__F5579F9453BE8FB3");
                        j.ToTable("CT_KHUYEN_MAI");
                        j.IndexerProperty<int>("MaKm").HasColumnName("MaKM");
                        j.IndexerProperty<int>("MaSp").HasColumnName("MaSP");
                    });
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.MaNcc).HasName("PK__NHA_CUNG__3A185DEBB90C2EAF");

            entity.ToTable("NHA_CUNG_CAP");

            entity.Property(e => e.MaNcc).HasColumnName("MaNCC");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TenNcc)
                .HasMaxLength(150)
                .HasColumnName("TenNCC");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NHAN_VIE__2725D70A4F2A32E3");

            entity.ToTable("NHAN_VIEN");

            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MaCn).HasColumnName("MaCN");
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaCnNavigation).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.MaCn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NHAN_VIEN__MaCN__5441852A");
        });

        modelBuilder.Entity<PhieuDieuChuyen>(entity =>
        {
            entity.HasKey(e => e.MaPdc).HasName("PK__PHIEU_DI__3AE048FA60F06D28");

            entity.ToTable("PHIEU_DIEU_CHUYEN");

            entity.Property(e => e.MaPdc).HasColumnName("MaPDC");
            entity.Property(e => e.MaCngiao).HasColumnName("MaCNGiao");
            entity.Property(e => e.MaCnnhan).HasColumnName("MaCNNhan");
            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.NgayChuyen)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaCngiaoNavigation).WithMany(p => p.PhieuDieuChuyenMaCngiaoNavigations)
                .HasForeignKey(d => d.MaCngiao)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PHIEU_DIE__MaCNG__208CD6FA");

            entity.HasOne(d => d.MaCnnhanNavigation).WithMany(p => p.PhieuDieuChuyenMaCnnhanNavigations)
                .HasForeignKey(d => d.MaCnnhan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PHIEU_DIE__MaCNN__2180FB33");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.PhieuDieuChuyens)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PHIEU_DIEU__MaNV__22751F6C");
        });

        modelBuilder.Entity<PhieuKiemKe>(entity =>
        {
            entity.HasKey(e => e.MaKk).HasName("PK__PHIEU_KI__2725CF1B346112D8");

            entity.ToTable("PHIEU_KIEM_KE");

            entity.Property(e => e.MaKk).HasColumnName("MaKK");
            entity.Property(e => e.MaCn).HasColumnName("MaCN");
            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.NgayKiemKe)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaCnNavigation).WithMany(p => p.PhieuKiemKes)
                .HasForeignKey(d => d.MaCn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PHIEU_KIEM__MaCN__17F790F9");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.PhieuKiemKes)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PHIEU_KIEM__MaNV__18EBB532");
        });

        modelBuilder.Entity<PhieuNhap>(entity =>
        {
            entity.HasKey(e => e.MaPn).HasName("PK__PHIEU_NH__2725E7F0672ACE46");

            entity.ToTable("PHIEU_NHAP");

            entity.Property(e => e.MaPn).HasColumnName("MaPN");
            entity.Property(e => e.MaCn).HasColumnName("MaCN");
            entity.Property(e => e.MaNcc).HasColumnName("MaNCC");
            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.NgayNhap)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TongTien)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaCnNavigation).WithMany(p => p.PhieuNhaps)
                .HasForeignKey(d => d.MaCn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PHIEU_NHAP__MaCN__05D8E0BE");

            entity.HasOne(d => d.MaNccNavigation).WithMany(p => p.PhieuNhaps)
                .HasForeignKey(d => d.MaNcc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PHIEU_NHA__MaNCC__06CD04F7");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.PhieuNhaps)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PHIEU_NHAP__MaNV__07C12930");
        });

        modelBuilder.Entity<PhieuTraHang>(entity =>
        {
            entity.HasKey(e => e.MaPth).HasName("PK__PHIEU_TR__3AE3D6D3A7ED44FA");

            entity.ToTable("PHIEU_TRA_HANG");

            entity.Property(e => e.MaPth).HasColumnName("MaPTH");
            entity.Property(e => e.LyDoTra).HasMaxLength(255);
            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.NgayTra)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ThanhTienHoan)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.PhieuTraHangs)
                .HasForeignKey(d => d.MaHd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PHIEU_TRA___MaHD__7A672E12");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.PhieuTraHangs)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PHIEU_TRA___MaNV__7B5B524B");
        });

        modelBuilder.Entity<PhieuXuatKho>(entity =>
        {
            entity.HasKey(e => e.MaPx).HasName("PK__PHIEU_XU__2725E7CA0F15E566");

            entity.ToTable("PHIEU_XUAT_KHO");

            entity.Property(e => e.MaPx).HasColumnName("MaPX");
            entity.Property(e => e.LyDoXuat).HasMaxLength(255);
            entity.Property(e => e.MaCn).HasColumnName("MaCN");
            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.NgayXuat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaCnNavigation).WithMany(p => p.PhieuXuatKhos)
                .HasForeignKey(d => d.MaCn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PHIEU_XUAT__MaCN__0F624AF8");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.PhieuXuatKhos)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PHIEU_XUAT__MaNV__10566F31");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSp).HasName("PK__SAN_PHAM__2725081C12437A51");

            entity.ToTable("SAN_PHAM");

            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.DonViTinh).HasMaxLength(50);
            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaDm).HasColumnName("MaDM");
            entity.Property(e => e.TenSp)
                .HasMaxLength(150)
                .HasColumnName("TenSP");

            entity.HasOne(d => d.MaDmNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaDm)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SAN_PHAM__MaDM__5CD6CB2B");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTk).HasName("PK__TAI_KHOA__272500707F5BF586");

            entity.ToTable("TAI_KHOAN");

            entity.HasIndex(e => e.MaNv, "UQ__TAI_KHOA__2725D70BE089798F").IsUnique();

            entity.HasIndex(e => e.TenDangNhap, "UQ__TAI_KHOA__55F68FC06A9C7A22").IsUnique();

            entity.Property(e => e.MaTk).HasColumnName("MaTK");
            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.MatKhauHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaNvNavigation).WithOne(p => p.TaiKhoan)
                .HasForeignKey<TaiKhoan>(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TAI_KHOAN__MaNV__59063A47");

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.MaVaiTro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TAI_KHOAN__MaVai__59FA5E80");
        });

        modelBuilder.Entity<TonKho>(entity =>
        {
            entity.HasKey(e => new { e.MaCn, e.MaSp }).HasName("PK__TON_KHO__F557DE8F3D455447");

            entity.ToTable("TON_KHO");

            entity.Property(e => e.MaCn).HasColumnName("MaCN");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.MucTonToiThieu).HasDefaultValue(5);

            entity.HasOne(d => d.MaCnNavigation).WithMany(p => p.TonKhos)
                .HasForeignKey(d => d.MaCn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TON_KHO__MaCN__68487DD7");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.TonKhos)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TON_KHO__MaSP__693CA210");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("PK__VAI_TRO__C24C41CF41A328C1");

            entity.ToTable("VAI_TRO");

            entity.Property(e => e.TenVaiTro).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
