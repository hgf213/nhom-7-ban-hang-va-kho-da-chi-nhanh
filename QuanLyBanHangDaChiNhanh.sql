/* =====================================================================
   QuanLyBanHangDaChiNhanh.sql  -  Script hoàn chỉnh (SQL Server 2016+)
   Gộp: tạo CSDL + cấu trúc cuối cùng + ràng buộc + Temporal Table + dữ liệu mẫu

   !!! CẢNH BÁO: script XÓA và TẠO LẠI database QuanLyBanHangDaChiNhanh.
       Mọi dữ liệu cũ của database này sẽ mất. Muốn giữ dữ liệu, xóa khối DROP bên dưới.

   QUY ƯỚC
   - Computed column (SP KHÔNG insert các cột này):
       CT_HOA_DON.ThanhTien, CT_PHIEU_NHAP.ThanhTien, CT_TRA_HANG.ThanhTien, CT_KIEM_KE.ChanhLech
   - Trạng thái:
       HOA_DON          : Chờ thanh toán / Đã thanh toán / Đã hủy
       PHIEU_NHAP       : Chờ xác nhận / Đã nhập / Đã hủy
       PHIEU_DIEU_CHUYEN: Chờ xuất / Đang vận chuyển / Đã nhận / Đã hủy
       PHIEU_KIEM_KE    : Đang kiểm kê / Đã hoàn thành / Đã hủy
       PHIEU_TRA_HANG   : Đã xử lý / Đã hủy
   - PHIEU_DIEU_CHUYEN.MaNV là người tạo phiếu; MaNVXuat / MaNVNhan là người xuất / nhận.
   - Trả hàng: hàng "Còn tốt" cộng lại kho, hàng "Lỗi" không cộng.
   - Temporal Table (SAN_PHAM, TON_KHO): cột SysStartTime/SysEndTime ẩn, lưu giờ UTC.
   ===================================================================== */
USE master;
GO
IF DB_ID(N'QuanLyBanHangDaChiNhanh') IS NOT NULL
BEGIN
    ALTER DATABASE QuanLyBanHangDaChiNhanh SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyBanHangDaChiNhanh;
END
GO
CREATE DATABASE QuanLyBanHangDaChiNhanh;
GO
USE QuanLyBanHangDaChiNhanh;
GO

/* =====================================================================
   1. CÁC BẢNG DANH MỤC
   ===================================================================== */
CREATE TABLE CHI_NHANH (
    MaCN INT IDENTITY(1,1) PRIMARY KEY,
    TenCN NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(255),
    SoDienThoai VARCHAR(15),
    TrangThai NVARCHAR(50)
);

CREATE TABLE VAI_TRO (
    MaVaiTro INT IDENTITY(1,1) PRIMARY KEY,
    TenVaiTro NVARCHAR(50) NOT NULL
);

CREATE TABLE DANH_MUC (
    MaDM INT IDENTITY(1,1) PRIMARY KEY,
    TenDM NVARCHAR(100) NOT NULL
);

CREATE TABLE NHA_CUNG_CAP (
    MaNCC INT IDENTITY(1,1) PRIMARY KEY,
    TenNCC NVARCHAR(150) NOT NULL,
    DiaChi NVARCHAR(255),
    SoDienThoai VARCHAR(15),
    Email VARCHAR(100)
);

CREATE TABLE KHACH_HANG (
    MaKH INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(15) UNIQUE,
    Email VARCHAR(100),
    DiaChi NVARCHAR(255)
);

/* =====================================================================
   2. NHÂN SỰ & TÀI KHOẢN
   ===================================================================== */
CREATE TABLE NHAN_VIEN (
    MaNV INT IDENTITY(1,1) PRIMARY KEY,
    MaCN INT NOT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(15),
    Email VARCHAR(100),
    TrangThai NVARCHAR(50),
    FOREIGN KEY (MaCN) REFERENCES CHI_NHANH(MaCN)
);

CREATE TABLE TAI_KHOAN (
    MaTK INT IDENTITY(1,1) PRIMARY KEY,
    MaNV INT NOT NULL UNIQUE,
    MaVaiTro INT NOT NULL,
    TenDangNhap VARCHAR(50) NOT NULL UNIQUE,
    MatKhau VARCHAR(255) NOT NULL,
    TrangThai NVARCHAR(50),
    FOREIGN KEY (MaNV) REFERENCES NHAN_VIEN(MaNV),
    FOREIGN KEY (MaVaiTro) REFERENCES VAI_TRO(MaVaiTro)
);

/* =====================================================================
   3. SẢN PHẨM & TỒN KHO  (Temporal Table: lưu lịch sử đổi giá / đổi tồn)
   ===================================================================== */
CREATE TABLE SAN_PHAM (
    MaSP INT IDENTITY(1,1) PRIMARY KEY,
    MaDM INT NOT NULL,
    TenSP NVARCHAR(150) NOT NULL,
    DonViTinh NVARCHAR(50),
    GiaBan DECIMAL(18,2) NOT NULL CONSTRAINT CK_SAN_PHAM_GiaBan CHECK (GiaBan >= 0),
    MoTa NVARCHAR(MAX),
    SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    SysEndTime   DATETIME2 GENERATED ALWAYS AS ROW END   HIDDEN NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    FOREIGN KEY (MaDM) REFERENCES DANH_MUC(MaDM)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.SAN_PHAM_LICHSU));

CREATE TABLE TON_KHO (
    MaCN INT NOT NULL,
    MaSP INT NOT NULL,
    SoLuongTon INT NOT NULL CONSTRAINT DF_TON_KHO_SoLuongTon DEFAULT 0
        CONSTRAINT CK_TON_KHO_SoLuongTon CHECK (SoLuongTon >= 0),
    MucTonToiThieu INT NOT NULL CONSTRAINT DF_TON_KHO_MucTon DEFAULT 5
        CONSTRAINT CK_TON_KHO_MucTon CHECK (MucTonToiThieu >= 0),
    SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    SysEndTime   DATETIME2 GENERATED ALWAYS AS ROW END   HIDDEN NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    PRIMARY KEY (MaCN, MaSP),
    FOREIGN KEY (MaCN) REFERENCES CHI_NHANH(MaCN),
    FOREIGN KEY (MaSP) REFERENCES SAN_PHAM(MaSP)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.TON_KHO_LICHSU));

/* =====================================================================
   4. BÁN HÀNG, THANH TOÁN, TRẢ HÀNG
   ===================================================================== */
CREATE TABLE HOA_DON (
    MaHD INT IDENTITY(1,1) PRIMARY KEY,
    MaCN INT NOT NULL,
    MaNV INT NOT NULL,
    MaKH INT NOT NULL,
    NgayLap DATETIME NOT NULL CONSTRAINT DF_HOA_DON_Ngay DEFAULT GETDATE(),
    TongTien DECIMAL(18,2) NOT NULL CONSTRAINT DF_HOA_DON_TongTien DEFAULT 0
        CONSTRAINT CK_HOA_DON_TongTien CHECK (TongTien >= 0),
    PhuongThucThanhToan NVARCHAR(50),
    TrangThai NVARCHAR(50) NOT NULL CONSTRAINT DF_HOA_DON_TrangThai DEFAULT N'Chờ thanh toán'
        CONSTRAINT CK_HOA_DON_TrangThai CHECK (TrangThai IN (N'Chờ thanh toán', N'Đã thanh toán', N'Đã hủy')),
    FOREIGN KEY (MaCN) REFERENCES CHI_NHANH(MaCN),
    FOREIGN KEY (MaNV) REFERENCES NHAN_VIEN(MaNV),
    FOREIGN KEY (MaKH) REFERENCES KHACH_HANG(MaKH)
);

CREATE TABLE CT_HOA_DON (
    MaHD INT NOT NULL,
    MaSP INT NOT NULL,
    SoLuong INT NOT NULL CONSTRAINT CK_CT_HOA_DON_SoLuong CHECK (SoLuong > 0),
    DonGia DECIMAL(18,2) NOT NULL CONSTRAINT CK_CT_HOA_DON_DonGia CHECK (DonGia >= 0),
    ThanhTien AS CONVERT(DECIMAL(18,2), SoLuong * DonGia) PERSISTED,
    PRIMARY KEY (MaHD, MaSP),
    FOREIGN KEY (MaHD) REFERENCES HOA_DON(MaHD),
    FOREIGN KEY (MaSP) REFERENCES SAN_PHAM(MaSP)
);

CREATE TABLE THANH_TOAN (
    MaTT INT IDENTITY(1,1) PRIMARY KEY,
    MaHD INT NOT NULL,
    MaNV INT NOT NULL,
    SoTien DECIMAL(18,2) NOT NULL CONSTRAINT CK_THANH_TOAN_SoTien CHECK (SoTien > 0),
    PhuongThuc NVARCHAR(50) NOT NULL,
    NgayThanhToan DATETIME NOT NULL CONSTRAINT DF_THANH_TOAN_Ngay DEFAULT GETDATE(),
    FOREIGN KEY (MaHD) REFERENCES HOA_DON(MaHD),
    FOREIGN KEY (MaNV) REFERENCES NHAN_VIEN(MaNV)
);

CREATE TABLE PHIEU_TRA_HANG (
    MaTH INT IDENTITY(1,1) PRIMARY KEY,
    MaHD INT NOT NULL,
    MaCN INT NOT NULL,
    MaNV INT NOT NULL,
    NgayTra DATETIME NOT NULL CONSTRAINT DF_PHIEU_TRA_HANG_Ngay DEFAULT GETDATE(),
    LyDo NVARCHAR(255),
    TongTienHoan DECIMAL(18,2) NOT NULL CONSTRAINT DF_PHIEU_TRA_HANG_Tien DEFAULT 0
        CONSTRAINT CK_PHIEU_TRA_HANG_Tien CHECK (TongTienHoan >= 0),
    TrangThai NVARCHAR(50) NOT NULL CONSTRAINT DF_PHIEU_TRA_HANG_TT DEFAULT N'Đã xử lý'
        CONSTRAINT CK_PHIEU_TRA_HANG_TT CHECK (TrangThai IN (N'Đã xử lý', N'Đã hủy')),
    FOREIGN KEY (MaHD) REFERENCES HOA_DON(MaHD),
    FOREIGN KEY (MaCN) REFERENCES CHI_NHANH(MaCN),
    FOREIGN KEY (MaNV) REFERENCES NHAN_VIEN(MaNV)
);

CREATE TABLE CT_TRA_HANG (
    MaTH INT NOT NULL,
    MaSP INT NOT NULL,
    SoLuong INT NOT NULL CONSTRAINT CK_CT_TRA_HANG_SoLuong CHECK (SoLuong > 0),
    DonGia DECIMAL(18,2) NOT NULL CONSTRAINT CK_CT_TRA_HANG_DonGia CHECK (DonGia >= 0),
    ThanhTien AS CONVERT(DECIMAL(18,2), SoLuong * DonGia) PERSISTED,
    TinhTrangHang NVARCHAR(50) NOT NULL CONSTRAINT DF_CT_TRA_HANG_TinhTrang DEFAULT N'Còn tốt'
        CONSTRAINT CK_CT_TRA_HANG_TinhTrang CHECK (TinhTrangHang IN (N'Còn tốt', N'Lỗi')),
    PRIMARY KEY (MaTH, MaSP),
    FOREIGN KEY (MaTH) REFERENCES PHIEU_TRA_HANG(MaTH),
    FOREIGN KEY (MaSP) REFERENCES SAN_PHAM(MaSP)
);

/* =====================================================================
   5. NHẬP KHO, XUẤT KHO (xuất hủy / xuất khác)
   ===================================================================== */
CREATE TABLE PHIEU_NHAP (
    MaPN INT IDENTITY(1,1) PRIMARY KEY,
    MaCN INT NOT NULL,
    MaNCC INT NOT NULL,
    MaNV INT NOT NULL,
    NgayNhap DATETIME NOT NULL CONSTRAINT DF_PHIEU_NHAP_Ngay DEFAULT GETDATE(),
    TongTien DECIMAL(18,2) NOT NULL CONSTRAINT DF_PHIEU_NHAP_TongTien DEFAULT 0
        CONSTRAINT CK_PHIEU_NHAP_TongTien CHECK (TongTien >= 0),
    TrangThai NVARCHAR(50) NOT NULL CONSTRAINT DF_PHIEU_NHAP_TrangThai DEFAULT N'Chờ xác nhận'
        CONSTRAINT CK_PHIEU_NHAP_TrangThai CHECK (TrangThai IN (N'Chờ xác nhận', N'Đã nhập', N'Đã hủy')),
    FOREIGN KEY (MaCN) REFERENCES CHI_NHANH(MaCN),
    FOREIGN KEY (MaNCC) REFERENCES NHA_CUNG_CAP(MaNCC),
    FOREIGN KEY (MaNV) REFERENCES NHAN_VIEN(MaNV)
);

CREATE TABLE CT_PHIEU_NHAP (
    MaPN INT NOT NULL,
    MaSP INT NOT NULL,
    SoLuong INT NOT NULL CONSTRAINT CK_CT_PHIEU_NHAP_SoLuong CHECK (SoLuong > 0),
    DonGiaNhap DECIMAL(18,2) NOT NULL CONSTRAINT CK_CT_PHIEU_NHAP_DonGia CHECK (DonGiaNhap >= 0),
    ThanhTien AS CONVERT(DECIMAL(18,2), SoLuong * DonGiaNhap) PERSISTED,
    PRIMARY KEY (MaPN, MaSP),
    FOREIGN KEY (MaPN) REFERENCES PHIEU_NHAP(MaPN),
    FOREIGN KEY (MaSP) REFERENCES SAN_PHAM(MaSP)
);

CREATE TABLE PHIEU_XUAT_KHO (
    MaPX INT IDENTITY(1,1) PRIMARY KEY,
    MaCN INT NOT NULL,
    MaNV INT NOT NULL,
    NgayXuat DATETIME NOT NULL CONSTRAINT DF_PHIEU_XUAT_Ngay DEFAULT GETDATE(),
    LyDoXuat NVARCHAR(255),
    FOREIGN KEY (MaCN) REFERENCES CHI_NHANH(MaCN),
    FOREIGN KEY (MaNV) REFERENCES NHAN_VIEN(MaNV)
);

CREATE TABLE CT_PHIEU_XUAT (
    MaPX INT NOT NULL,
    MaSP INT NOT NULL,
    SoLuong INT NOT NULL CONSTRAINT CK_CT_PHIEU_XUAT_SoLuong CHECK (SoLuong > 0),
    PRIMARY KEY (MaPX, MaSP),
    FOREIGN KEY (MaPX) REFERENCES PHIEU_XUAT_KHO(MaPX),
    FOREIGN KEY (MaSP) REFERENCES SAN_PHAM(MaSP)
);

/* =====================================================================
   6. KIỂM KÊ, ĐIỀU CHUYỂN KHO
   ===================================================================== */
CREATE TABLE PHIEU_KIEM_KE (
    MaKK INT IDENTITY(1,1) PRIMARY KEY,
    MaCN INT NOT NULL,
    MaNV INT NOT NULL,
    NgayKiemKe DATETIME NOT NULL CONSTRAINT DF_PHIEU_KIEM_KE_Ngay DEFAULT GETDATE(),
    TrangThai NVARCHAR(50) NOT NULL CONSTRAINT DF_PHIEU_KIEM_KE_TT DEFAULT N'Đang kiểm kê'
        CONSTRAINT CK_PHIEU_KIEM_KE_TT CHECK (TrangThai IN (N'Đang kiểm kê', N'Đã hoàn thành', N'Đã hủy')),
    FOREIGN KEY (MaCN) REFERENCES CHI_NHANH(MaCN),
    FOREIGN KEY (MaNV) REFERENCES NHAN_VIEN(MaNV)
);

CREATE TABLE CT_KIEM_KE (
    MaKK INT NOT NULL,
    MaSP INT NOT NULL,
    SoLuongHeThong INT NOT NULL CONSTRAINT CK_CT_KIEM_KE_HeThong CHECK (SoLuongHeThong >= 0),
    SoLuongThucTe INT NOT NULL CONSTRAINT CK_CT_KIEM_KE_ThucTe CHECK (SoLuongThucTe >= 0),
    ChanhLech AS (SoLuongThucTe - SoLuongHeThong) PERSISTED,
    PRIMARY KEY (MaKK, MaSP),
    FOREIGN KEY (MaKK) REFERENCES PHIEU_KIEM_KE(MaKK),
    FOREIGN KEY (MaSP) REFERENCES SAN_PHAM(MaSP)
);

CREATE TABLE PHIEU_DIEU_CHUYEN (
    MaPDC INT IDENTITY(1,1) PRIMARY KEY,
    MaCNGiao INT NOT NULL,
    MaCNNhan INT NOT NULL,
    MaNV INT NOT NULL,              -- người tạo phiếu
    NgayChuyen DATETIME NOT NULL CONSTRAINT DF_PDC_Ngay DEFAULT GETDATE(),  -- ngày tạo phiếu
    MaNVXuat INT NULL,
    NgayXuat DATETIME NULL,
    MaNVNhan INT NULL,
    NgayNhan DATETIME NULL,
    TrangThai NVARCHAR(50) NOT NULL CONSTRAINT DF_PDC_TrangThai DEFAULT N'Chờ xuất'
        CONSTRAINT CK_PDC_TrangThai CHECK (TrangThai IN (N'Chờ xuất', N'Đang vận chuyển', N'Đã nhận', N'Đã hủy')),
    CONSTRAINT CK_PDC_KhacChiNhanh CHECK (MaCNGiao <> MaCNNhan),
    FOREIGN KEY (MaCNGiao) REFERENCES CHI_NHANH(MaCN),
    FOREIGN KEY (MaCNNhan) REFERENCES CHI_NHANH(MaCN),
    FOREIGN KEY (MaNV) REFERENCES NHAN_VIEN(MaNV),
    FOREIGN KEY (MaNVXuat) REFERENCES NHAN_VIEN(MaNV),
    FOREIGN KEY (MaNVNhan) REFERENCES NHAN_VIEN(MaNV)
);

CREATE TABLE CT_DIEU_CHUYEN (
    MaPDC INT NOT NULL,
    MaSP INT NOT NULL,
    SoLuong INT NOT NULL CONSTRAINT CK_CT_DIEU_CHUYEN_SoLuong CHECK (SoLuong > 0),
    PRIMARY KEY (MaPDC, MaSP),
    FOREIGN KEY (MaPDC) REFERENCES PHIEU_DIEU_CHUYEN(MaPDC),
    FOREIGN KEY (MaSP) REFERENCES SAN_PHAM(MaSP)
);
GO

/* =====================================================================
   7. DỮ LIỆU MẪU  (database mới nên IDENTITY chạy 1,2,3... theo thứ tự insert)
   Tồn kho = nhập - xuất hủy - bán - chuyển đi + chuyển đến
     CN1-SP1: 100 - 1 - 10 = 89    CN1-SP2: 50 - 1 = 49
     CN1-SP3: 200 - 2      = 198   CN2-SP1: 30 + 10 = 40
   ===================================================================== */
INSERT INTO CHI_NHANH (TenCN, DiaChi, SoDienThoai, TrangThai) VALUES
(N'Chi nhánh AEON Mall Hà Đông', N'Dương Nội, Hà Đông, Hà Nội', '0243111222', N'Hoạt động'),
(N'Chi nhánh UTT Triều Khúc', N'54 Triều Khúc, Thanh Xuân, Hà Nội', '0243333444', N'Hoạt động');

INSERT INTO VAI_TRO (TenVaiTro) VALUES (N'Quản trị viên'), (N'Thu ngân'), (N'Thủ kho');

INSERT INTO DANH_MUC (TenDM) VALUES (N'Mỹ phẩm skincare'), (N'Phụ kiện công nghệ');

INSERT INTO NHA_CUNG_CAP (TenNCC, DiaChi, SoDienThoai, Email) VALUES
(N'Công ty TNHH CeraVe Việt Nam', N'Hà Nội', '0901234567', N'contact@cerave.vn');

INSERT INTO KHACH_HANG (HoTen, SoDienThoai, Email, DiaChi) VALUES
(N'Khách lẻ', '0000000000', N'khachle@gmail.com', N'Hà Nội'),
(N'Trần Lan Hương', '0912345678', N'lanhuong@gmail.com', N'Thanh Xuân, Hà Nội');

INSERT INTO NHAN_VIEN (MaCN, HoTen, SoDienThoai, Email, TrangThai) VALUES
(1, N'Nguyễn Tùng', '0988123456', N'tung@gmail.com', N'Đang làm việc'),
(2, N'Lê Minh Anh', '0977654321', N'minhanh@gmail.com', N'Đang làm việc');

INSERT INTO TAI_KHOAN (MaNV, MaVaiTro, TenDangNhap, MatKhau, TrangThai) VALUES
(1, 1, 'tungadmin', '123456', N'Hoạt động'),
(2, 2, 'thungan01', '123456', N'Hoạt động');

INSERT INTO SAN_PHAM (MaDM, TenSP, DonViTinh, GiaBan, MoTa) VALUES
(1, N'Sữa rửa mặt CeraVe 236ml', N'Chai', 350000, N'Dịu nhẹ cho da nhạy cảm'),
(1, N'Toner Klairs 180ml', N'Chai', 280000, N'Cấp ẩm làm dịu da'),
(2, N'Ốp lưng iPhone 15 Pro Max', N'Chiếc', 150000, N'Chống sốc trong suốt');

INSERT INTO TON_KHO (MaCN, MaSP, SoLuongTon, MucTonToiThieu) VALUES
(1, 1, 89, 10), (1, 2, 49, 5), (1, 3, 198, 20), (2, 1, 40, 5);

INSERT INTO HOA_DON (MaCN, MaNV, MaKH, NgayLap, TongTien, PhuongThucThanhToan, TrangThai) VALUES
(1, 1, 2, '2026-11-13 09:15:00', 630000, N'Chuyển khoản', N'Đã thanh toán');
INSERT INTO CT_HOA_DON (MaHD, MaSP, SoLuong, DonGia) VALUES
(1, 1, 1, 350000), (1, 2, 1, 280000);
INSERT INTO THANH_TOAN (MaHD, MaNV, SoTien, PhuongThuc, NgayThanhToan) VALUES
(1, 1, 630000, N'Chuyển khoản', '2026-11-13 09:15:00');

INSERT INTO PHIEU_NHAP (MaCN, MaNCC, MaNV, NgayNhap, TongTien, TrangThai) VALUES
(1, 1, 1, '2026-11-10 08:00:00', 25000000, N'Đã nhập'),
(1, 1, 1, '2026-11-10 08:30:00', 26000000, N'Đã nhập'),
(2, 1, 2, '2026-11-09 08:00:00',  7500000, N'Đã nhập');
INSERT INTO CT_PHIEU_NHAP (MaPN, MaSP, SoLuong, DonGiaNhap) VALUES
(1, 1, 100, 250000), (2, 2, 50, 200000), (2, 3, 200, 80000), (3, 1, 30, 250000);

INSERT INTO PHIEU_XUAT_KHO (MaCN, MaNV, NgayXuat, LyDoXuat) VALUES
(1, 1, '2026-11-11 10:00:00', N'Xuất hủy hàng lỗi móp méo');
INSERT INTO CT_PHIEU_XUAT (MaPX, MaSP, SoLuong) VALUES (1, 3, 2);

INSERT INTO PHIEU_KIEM_KE (MaCN, MaNV, NgayKiemKe, TrangThai) VALUES
(1, 1, '2026-11-12 17:00:00', N'Đã hoàn thành');
INSERT INTO CT_KIEM_KE (MaKK, MaSP, SoLuongHeThong, SoLuongThucTe) VALUES (1, 1, 90, 90);

INSERT INTO PHIEU_DIEU_CHUYEN (MaCNGiao, MaCNNhan, MaNV, NgayChuyen, MaNVXuat, NgayXuat, MaNVNhan, NgayNhan, TrangThai) VALUES
(1, 2, 1, '2026-11-12 14:00:00', 1, '2026-11-12 14:00:00', 2, '2026-11-12 18:00:00', N'Đã nhận');
INSERT INTO CT_DIEU_CHUYEN (MaPDC, MaSP, SoLuong) VALUES (1, 1, 10);
GO

/* =====================================================================
   8. ĐỐI CHIẾU TỒN KHO  (kết quả mong đợi: mọi dòng = 'Khớp')
   Tồn tính toán = nhập(đã nhập) - xuất hủy - bán(đã thanh toán)
                   - chuyển đi(đang vận chuyển / đã nhận) + chuyển đến(đã nhận)
                   + trả hàng(đã xử lý, còn tốt) + chênh lệch kiểm kê(đã hoàn thành)
   ===================================================================== */
;WITH Delta AS (
    SELECT pn.MaCN, ct.MaSP, ct.SoLuong AS SL
    FROM PHIEU_NHAP pn JOIN CT_PHIEU_NHAP ct ON ct.MaPN = pn.MaPN
    WHERE pn.TrangThai = N'Đã nhập'
    UNION ALL
    SELECT px.MaCN, ct.MaSP, -ct.SoLuong
    FROM PHIEU_XUAT_KHO px JOIN CT_PHIEU_XUAT ct ON ct.MaPX = px.MaPX
    UNION ALL
    SELECT hd.MaCN, ct.MaSP, -ct.SoLuong
    FROM HOA_DON hd JOIN CT_HOA_DON ct ON ct.MaHD = hd.MaHD
    WHERE hd.TrangThai = N'Đã thanh toán'
    UNION ALL
    SELECT p.MaCNGiao, ct.MaSP, -ct.SoLuong
    FROM PHIEU_DIEU_CHUYEN p JOIN CT_DIEU_CHUYEN ct ON ct.MaPDC = p.MaPDC
    WHERE p.TrangThai IN (N'Đang vận chuyển', N'Đã nhận')
    UNION ALL
    SELECT p.MaCNNhan, ct.MaSP, ct.SoLuong
    FROM PHIEU_DIEU_CHUYEN p JOIN CT_DIEU_CHUYEN ct ON ct.MaPDC = p.MaPDC
    WHERE p.TrangThai = N'Đã nhận'
    UNION ALL
    SELECT th.MaCN, ct.MaSP, ct.SoLuong
    FROM PHIEU_TRA_HANG th JOIN CT_TRA_HANG ct ON ct.MaTH = th.MaTH
    WHERE th.TrangThai = N'Đã xử lý' AND ct.TinhTrangHang = N'Còn tốt'
    UNION ALL
    SELECT kk.MaCN, ct.MaSP, ct.ChanhLech
    FROM PHIEU_KIEM_KE kk JOIN CT_KIEM_KE ct ON ct.MaKK = kk.MaKK
    WHERE kk.TrangThai = N'Đã hoàn thành'
),
TinhToan AS (
    SELECT MaCN, MaSP, SUM(SL) AS SoLuongTinh
    FROM Delta
    GROUP BY MaCN, MaSP
)
SELECT COALESCE(t.MaCN, k.MaCN) AS MaCN,
       COALESCE(t.MaSP, k.MaSP) AS MaSP,
       t.SoLuongTinh,
       k.SoLuongTon,
       CASE WHEN ISNULL(t.SoLuongTinh, 0) = ISNULL(k.SoLuongTon, 0)
            THEN N'Khớp' ELSE N'LỆCH' END AS KetQua
FROM TinhToan t
FULL JOIN TON_KHO k ON k.MaCN = t.MaCN AND k.MaSP = t.MaSP
ORDER BY MaCN, MaSP;
GO
