/*
DỮ LIỆU MẪU QUẢN LÝ KHÁCH SẠN
Nguồn cấu trúc: quanlykhachsan(1).sql; database: QuanLyKhachSan5Sao2.
Cách dùng: mở file này trong SSMS, chạy toàn bộ bằng F5 sau khi đã tạo các bảng.
Không chạy lại file tạo database gốc nếu cần giữ dữ liệu: file gốc có DROP DATABASE.
File này chỉ thêm dữ liệu, không xóa/sửa dữ liệu cũ hoặc thay đổi cấu trúc bảng.
Mọi thông tin cá nhân trong file đều là giả lập.

- 40 bảng, mỗi bảng ít nhất 5 dòng; mã tự tăng được lấy bằng SCOPE_IDENTITY().
- Nếu xảy ra lỗi, toàn bộ phần thêm dữ liệu được ROLLBACK.
- Nếu đã có tài khoản mau_admin, dừng để tránh nạp trùng bộ mẫu.
- Tài khoản mẫu ở trạng thái Khoa. MatKhauHash là SHA-256 dạng hex của
  Mau@2026_1 ... Mau@2026_5, chỉ để minh họa dữ liệu; không cam kết phù hợp
  thuật toán đăng nhập của ứng dụng. Cần tạo mật khẩu qua ứng dụng để sử dụng.
- LichLamViec.MaLich không tự tăng; MaChamCong mới là cột IDENTITY và là FK.
  Do đó chèn ChamCong trước, sau đó dùng IDENTITY_INSERT riêng cho LichLamViec.
- VatTu dùng khóa MaLoaiVatTu đúng như file gốc.
- Không chèn vào ChiTietHoaDon.ThanhTien vì đó là cột tính toán tự động.
*/
USE [QuanLyKhachSan5Sao2];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET ARITHABORT ON;
SET NUMERIC_ROUNDABORT OFF;

IF EXISTS (SELECT 1 FROM dbo.TaiKhoan WHERE TenDangNhap = 'mau_admin')
BEGIN
    PRINT N'Đã có tài khoản mau_admin. Dừng để tránh thêm trùng bộ dữ liệu mẫu.';
    RETURN;
END;

DECLARE @DangBatIdentityInsert bit = 0;
BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @KhuVuc1 int, @KhuVuc2 int, @KhuVuc3 int, @KhuVuc4 int, @KhuVuc5 int;
    DECLARE @LoaiPhong1 int, @LoaiPhong2 int, @LoaiPhong3 int, @LoaiPhong4 int, @LoaiPhong5 int;
    DECLARE @TienNghi1 int, @TienNghi2 int, @TienNghi3 int, @TienNghi4 int, @TienNghi5 int;
    DECLARE @PhongBan1 int, @PhongBan2 int, @PhongBan3 int, @PhongBan4 int, @PhongBan5 int;
    DECLARE @ChucVu1 int, @ChucVu2 int, @ChucVu3 int, @ChucVu4 int, @ChucVu5 int;
    DECLARE @CaLamViec1 int, @CaLamViec2 int, @CaLamViec3 int, @CaLamViec4 int, @CaLamViec5 int;
    DECLARE @VaiTro1 int, @VaiTro2 int, @VaiTro3 int, @VaiTro4 int, @VaiTro5 int;
    DECLARE @Quyen1 int, @Quyen2 int, @Quyen3 int, @Quyen4 int, @Quyen5 int;
    DECLARE @LoaiDichVu1 int, @LoaiDichVu2 int, @LoaiDichVu3 int, @LoaiDichVu4 int, @LoaiDichVu5 int;
    DECLARE @Kho1 int, @Kho2 int, @Kho3 int, @Kho4 int, @Kho5 int;
    DECLARE @NhomVatTu1 int, @NhomVatTu2 int, @NhomVatTu3 int, @NhomVatTu4 int, @NhomVatTu5 int;
    DECLARE @NhaCungCap1 int, @NhaCungCap2 int, @NhaCungCap3 int, @NhaCungCap4 int, @NhaCungCap5 int;
    DECLARE @UuDai1 int, @UuDai2 int, @UuDai3 int, @UuDai4 int, @UuDai5 int;
    DECLARE @Tang1 int, @Tang2 int, @Tang3 int, @Tang4 int, @Tang5 int;
    DECLARE @Phong1 int, @Phong2 int, @Phong3 int, @Phong4 int, @Phong5 int;
    DECLARE @NhanVien1 int, @NhanVien2 int, @NhanVien3 int, @NhanVien4 int, @NhanVien5 int;
    DECLARE @TaiKhoan1 int, @TaiKhoan2 int, @TaiKhoan3 int, @TaiKhoan4 int, @TaiKhoan5 int;
    DECLARE @KhachHang1 int, @KhachHang2 int, @KhachHang3 int, @KhachHang4 int, @KhachHang5 int;
    DECLARE @DichVu1 int, @DichVu2 int, @DichVu3 int, @DichVu4 int, @DichVu5 int;
    DECLARE @GoiDichVu1 int, @GoiDichVu2 int, @GoiDichVu3 int, @GoiDichVu4 int, @GoiDichVu5 int;
    DECLARE @VatTu1 int, @VatTu2 int, @VatTu3 int, @VatTu4 int, @VatTu5 int;
    DECLARE @ThietBi1 int, @ThietBi2 int, @ThietBi3 int, @ThietBi4 int, @ThietBi5 int;
    DECLARE @DatPhong1 bigint, @DatPhong2 bigint, @DatPhong3 bigint, @DatPhong4 bigint, @DatPhong5 bigint;
    DECLARE @ChiTietDatPhong1 bigint, @ChiTietDatPhong2 bigint, @ChiTietDatPhong3 bigint, @ChiTietDatPhong4 bigint, @ChiTietDatPhong5 bigint;
    DECLARE @XepPhong1 bigint, @XepPhong2 bigint, @XepPhong3 bigint, @XepPhong4 bigint, @XepPhong5 bigint;
    DECLARE @SuDungDichVu1 bigint, @SuDungDichVu2 bigint, @SuDungDichVu3 bigint, @SuDungDichVu4 bigint, @SuDungDichVu5 bigint;
    DECLARE @HoaDon1 bigint, @HoaDon2 bigint, @HoaDon3 bigint, @HoaDon4 bigint, @HoaDon5 bigint;
    DECLARE @ChiTietHoaDon1 bigint, @ChiTietHoaDon2 bigint, @ChiTietHoaDon3 bigint, @ChiTietHoaDon4 bigint, @ChiTietHoaDon5 bigint, @ChiTietHoaDon6 bigint, @ChiTietHoaDon7 bigint, @ChiTietHoaDon8 bigint, @ChiTietHoaDon9 bigint, @ChiTietHoaDon10 bigint;
    DECLARE @ThanhToan1 bigint, @ThanhToan2 bigint, @ThanhToan3 bigint, @ThanhToan4 bigint, @ThanhToan5 bigint;
    DECLARE @CongViecDonPhong1 bigint, @CongViecDonPhong2 bigint, @CongViecDonPhong3 bigint, @CongViecDonPhong4 bigint, @CongViecDonPhong5 bigint;
    DECLARE @BaoTriPhong1 bigint, @BaoTriPhong2 bigint, @BaoTriPhong3 bigint, @BaoTriPhong4 bigint, @BaoTriPhong5 bigint;
    DECLARE @ChamCong1 bigint, @ChamCong2 bigint, @ChamCong3 bigint, @ChamCong4 bigint, @ChamCong5 bigint;
    DECLARE @BangLuong1 bigint, @BangLuong2 bigint, @BangLuong3 bigint, @BangLuong4 bigint, @BangLuong5 bigint;
    DECLARE @NhatKyThaoTac1 bigint, @NhatKyThaoTac2 bigint, @NhatKyThaoTac3 bigint, @NhatKyThaoTac4 bigint, @NhatKyThaoTac5 bigint;
    -- Chọn mã lịch mới, tránh trùng cả lịch đã có và mã lịch trong chấm công.
    DECLARE @MaLichLonNhat bigint;
    SELECT @MaLichLonNhat = ISNULL(MAX(MaLich), 0)
    FROM (SELECT MaLich FROM dbo.LichLamViec
          UNION ALL SELECT MaLich FROM dbo.ChamCong) AS L;
    DECLARE @LichLamViec1 bigint = @MaLichLonNhat + 1,
            @LichLamViec2 bigint = @MaLichLonNhat + 2,
            @LichLamViec3 bigint = @MaLichLonNhat + 3,
            @LichLamViec4 bigint = @MaLichLonNhat + 4,
            @LichLamViec5 bigint = @MaLichLonNhat + 5;


    -- 01. KhuVuc: thêm 5 dòng
    INSERT INTO dbo.[KhuVuc] ([TenKhuVuc], [MoTa], [TrangThai])
    VALUES (N'Khu A (Mẫu)', N'Khu trung tâm', 1);
    SET @KhuVuc1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[KhuVuc] ([TenKhuVuc], [MoTa], [TrangThai])
    VALUES (N'Khu B (Mẫu)', N'Khu hướng biển', 1);
    SET @KhuVuc2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[KhuVuc] ([TenKhuVuc], [MoTa], [TrangThai])
    VALUES (N'Khu C (Mẫu)', N'Khu sân vườn', 1);
    SET @KhuVuc3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[KhuVuc] ([TenKhuVuc], [MoTa], [TrangThai])
    VALUES (N'Khu D (Mẫu)', N'Khu gia đình', 1);
    SET @KhuVuc4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[KhuVuc] ([TenKhuVuc], [MoTa], [TrangThai])
    VALUES (N'Khu E (Mẫu)', N'Khu nghỉ dưỡng cao cấp', 1);
    SET @KhuVuc5 = CONVERT(int, SCOPE_IDENTITY());

    -- 02. LoaiPhong: thêm 5 dòng
    INSERT INTO dbo.[LoaiPhong] ([TenLoaiPhong], [SucChuaNguoiLon], [SucChuaTreEm], [GiaCoBan], [MoTa], [TrangThai])
    VALUES (N'Standard (Mẫu)', 2, 1, 800000, N'Phòng tiêu chuẩn, giường đôi', 1);
    SET @LoaiPhong1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[LoaiPhong] ([TenLoaiPhong], [SucChuaNguoiLon], [SucChuaTreEm], [GiaCoBan], [MoTa], [TrangThai])
    VALUES (N'Superior (Mẫu)', 2, 1, 1200000, N'Phòng hướng vườn', 1);
    SET @LoaiPhong2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[LoaiPhong] ([TenLoaiPhong], [SucChuaNguoiLon], [SucChuaTreEm], [GiaCoBan], [MoTa], [TrangThai])
    VALUES (N'Deluxe (Mẫu)', 2, 2, 1800000, N'Phòng hướng biển', 1);
    SET @LoaiPhong3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[LoaiPhong] ([TenLoaiPhong], [SucChuaNguoiLon], [SucChuaTreEm], [GiaCoBan], [MoTa], [TrangThai])
    VALUES (N'Family (Mẫu)', 4, 2, 2500000, N'Phòng gia đình hai giường lớn', 1);
    SET @LoaiPhong4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[LoaiPhong] ([TenLoaiPhong], [SucChuaNguoiLon], [SucChuaTreEm], [GiaCoBan], [MoTa], [TrangThai])
    VALUES (N'Suite (Mẫu)', 2, 2, 4000000, N'Phòng suite có phòng khách riêng', 1);
    SET @LoaiPhong5 = CONVERT(int, SCOPE_IDENTITY());

    -- 03. TienNghi: thêm 5 dòng
    INSERT INTO dbo.[TienNghi] ([TenTienNghi], [MoTa], [TrangThai])
    VALUES (N'Điều hòa (Mẫu)', N'Điều hòa hai chiều', 1);
    SET @TienNghi1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[TienNghi] ([TenTienNghi], [MoTa], [TrangThai])
    VALUES (N'Tivi (Mẫu)', N'Tivi thông minh', 1);
    SET @TienNghi2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[TienNghi] ([TenTienNghi], [MoTa], [TrangThai])
    VALUES (N'Tủ lạnh (Mẫu)', N'Tủ lạnh mini', 1);
    SET @TienNghi3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[TienNghi] ([TenTienNghi], [MoTa], [TrangThai])
    VALUES (N'Máy sấy tóc (Mẫu)', N'Máy sấy trong phòng tắm', 1);
    SET @TienNghi4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[TienNghi] ([TenTienNghi], [MoTa], [TrangThai])
    VALUES (N'Két sắt (Mẫu)', N'Két sắt điện tử', 1);
    SET @TienNghi5 = CONVERT(int, SCOPE_IDENTITY());

    -- 04. PhongBan: thêm 5 dòng
    INSERT INTO dbo.[PhongBan] ([TenPhongBan], [MoTa], [TrangThai])
    VALUES (N'Ban quản lý (Mẫu)', N'Điều hành khách sạn', 1);
    SET @PhongBan1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[PhongBan] ([TenPhongBan], [MoTa], [TrangThai])
    VALUES (N'Lễ tân (Mẫu)', N'Đặt phòng, nhận và trả phòng', 1);
    SET @PhongBan2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[PhongBan] ([TenPhongBan], [MoTa], [TrangThai])
    VALUES (N'Buồng phòng (Mẫu)', N'Vệ sinh phòng', 1);
    SET @PhongBan3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[PhongBan] ([TenPhongBan], [MoTa], [TrangThai])
    VALUES (N'Kỹ thuật (Mẫu)', N'Bảo trì thiết bị', 1);
    SET @PhongBan4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[PhongBan] ([TenPhongBan], [MoTa], [TrangThai])
    VALUES (N'Kế toán (Mẫu)', N'Hóa đơn và tiền lương', 1);
    SET @PhongBan5 = CONVERT(int, SCOPE_IDENTITY());

    -- 05. ChucVu: thêm 5 dòng
    INSERT INTO dbo.[ChucVu] ([TenChucVu], [Chucvu], [MoTa], [TrangThai])
    VALUES (N'Quản lý (Mẫu)', 2000000, N'Giá trị cột Chucvu được giữ đúng cấu trúc SQL gốc', 1);
    SET @ChucVu1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChucVu] ([TenChucVu], [Chucvu], [MoTa], [TrangThai])
    VALUES (N'Nhân viên lễ tân (Mẫu)', 500000, N'Giá trị cột Chucvu được giữ đúng cấu trúc SQL gốc', 1);
    SET @ChucVu2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChucVu] ([TenChucVu], [Chucvu], [MoTa], [TrangThai])
    VALUES (N'Nhân viên buồng phòng (Mẫu)', 300000, N'Giá trị cột Chucvu được giữ đúng cấu trúc SQL gốc', 1);
    SET @ChucVu3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChucVu] ([TenChucVu], [Chucvu], [MoTa], [TrangThai])
    VALUES (N'Nhân viên kỹ thuật (Mẫu)', 500000, N'Giá trị cột Chucvu được giữ đúng cấu trúc SQL gốc', 1);
    SET @ChucVu4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChucVu] ([TenChucVu], [Chucvu], [MoTa], [TrangThai])
    VALUES (N'Kế toán viên (Mẫu)', 700000, N'Giá trị cột Chucvu được giữ đúng cấu trúc SQL gốc', 1);
    SET @ChucVu5 = CONVERT(int, SCOPE_IDENTITY());

    -- 06. CaLamViec: thêm 5 dòng
    INSERT INTO dbo.[CaLamViec] ([TenCa], [GioBatDau], [GioKetThuc], [SoGioLam], [TrangThai], [GhiChu])
    VALUES (N'Ca sáng (Mẫu)', N'06:00:00', N'14:00:00', 8, 1, NULL);
    SET @CaLamViec1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[CaLamViec] ([TenCa], [GioBatDau], [GioKetThuc], [SoGioLam], [TrangThai], [GhiChu])
    VALUES (N'Ca chiều (Mẫu)', N'14:00:00', N'22:00:00', 8, 1, NULL);
    SET @CaLamViec2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[CaLamViec] ([TenCa], [GioBatDau], [GioKetThuc], [SoGioLam], [TrangThai], [GhiChu])
    VALUES (N'Ca đêm (Mẫu)', N'22:00:00', N'06:00:00', 8, 1, N'Kết thúc vào ngày hôm sau');
    SET @CaLamViec3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[CaLamViec] ([TenCa], [GioBatDau], [GioKetThuc], [SoGioLam], [TrangThai], [GhiChu])
    VALUES (N'Ca hành chính (Mẫu)', N'08:00:00', N'17:00:00', 8, 1, N'Nghỉ trưa 12:00 đến 13:00');
    SET @CaLamViec4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[CaLamViec] ([TenCa], [GioBatDau], [GioKetThuc], [SoGioLam], [TrangThai], [GhiChu])
    VALUES (N'Ca tăng cường (Mẫu)', N'18:00:00', N'22:00:00', 4, 1, NULL);
    SET @CaLamViec5 = CONVERT(int, SCOPE_IDENTITY());

    -- 07. VaiTro: thêm 5 dòng
    INSERT INTO dbo.[VaiTro] ([TenVaiTro], [MoTa], [TrangThai])
    VALUES (N'Quản trị viên (Mẫu)', N'Quản lý toàn hệ thống', 1);
    SET @VaiTro1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[VaiTro] ([TenVaiTro], [MoTa], [TrangThai])
    VALUES (N'Lễ tân (Mẫu)', N'Quản lý đặt phòng', 1);
    SET @VaiTro2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[VaiTro] ([TenVaiTro], [MoTa], [TrangThai])
    VALUES (N'Buồng phòng (Mẫu)', N'Quản lý dọn phòng', 1);
    SET @VaiTro3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[VaiTro] ([TenVaiTro], [MoTa], [TrangThai])
    VALUES (N'Kỹ thuật (Mẫu)', N'Quản lý bảo trì', 1);
    SET @VaiTro4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[VaiTro] ([TenVaiTro], [MoTa], [TrangThai])
    VALUES (N'Kế toán (Mẫu)', N'Quản lý thu chi', 1);
    SET @VaiTro5 = CONVERT(int, SCOPE_IDENTITY());

    -- 08. Quyen: thêm 5 dòng
    INSERT INTO dbo.[Quyen] ([MaChucNang], [TenQuyen], [MoTa], [TrangThai])
    VALUES (N'MAU_HE_THONG', N'Quản lý hệ thống', NULL, 1);
    SET @Quyen1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Quyen] ([MaChucNang], [TenQuyen], [MoTa], [TrangThai])
    VALUES (N'MAU_DAT_PHONG', N'Quản lý đặt phòng', NULL, 1);
    SET @Quyen2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Quyen] ([MaChucNang], [TenQuyen], [MoTa], [TrangThai])
    VALUES (N'MAU_DON_PHONG', N'Quản lý dọn phòng', NULL, 1);
    SET @Quyen3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Quyen] ([MaChucNang], [TenQuyen], [MoTa], [TrangThai])
    VALUES (N'MAU_BAO_TRI', N'Quản lý bảo trì', NULL, 1);
    SET @Quyen4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Quyen] ([MaChucNang], [TenQuyen], [MoTa], [TrangThai])
    VALUES (N'MAU_KE_TOAN', N'Quản lý kế toán', NULL, 1);
    SET @Quyen5 = CONVERT(int, SCOPE_IDENTITY());

    -- 09. LoaiDichVu: thêm 5 dòng
    INSERT INTO dbo.[LoaiDichVu] ([TenLoaiDichVu], [MoTa], [TrangThai])
    VALUES (N'Ẩm thực (Mẫu)', NULL, 1);
    SET @LoaiDichVu1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[LoaiDichVu] ([TenLoaiDichVu], [MoTa], [TrangThai])
    VALUES (N'Giặt là (Mẫu)', NULL, 1);
    SET @LoaiDichVu2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[LoaiDichVu] ([TenLoaiDichVu], [MoTa], [TrangThai])
    VALUES (N'Spa (Mẫu)', NULL, 1);
    SET @LoaiDichVu3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[LoaiDichVu] ([TenLoaiDichVu], [MoTa], [TrangThai])
    VALUES (N'Đồ uống (Mẫu)', NULL, 1);
    SET @LoaiDichVu4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[LoaiDichVu] ([TenLoaiDichVu], [MoTa], [TrangThai])
    VALUES (N'Minibar (Mẫu)', NULL, 1);
    SET @LoaiDichVu5 = CONVERT(int, SCOPE_IDENTITY());

    -- 10. Kho: thêm 5 dòng
    INSERT INTO dbo.[Kho] ([TenKho], [TrangThai])
    VALUES (N'Kho thực phẩm (Mẫu)', 1);
    SET @Kho1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Kho] ([TenKho], [TrangThai])
    VALUES (N'Kho giặt là (Mẫu)', 1);
    SET @Kho2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Kho] ([TenKho], [TrangThai])
    VALUES (N'Kho spa (Mẫu)', 1);
    SET @Kho3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Kho] ([TenKho], [TrangThai])
    VALUES (N'Kho đồ uống (Mẫu)', 1);
    SET @Kho4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Kho] ([TenKho], [TrangThai])
    VALUES (N'Kho minibar (Mẫu)', 1);
    SET @Kho5 = CONVERT(int, SCOPE_IDENTITY());

    -- 11. NhomVatTu: thêm 5 dòng
    INSERT INTO dbo.[NhomVatTu] ([TenNhomVatTu], [MoTa], [TrangThai])
    VALUES (N'Nguyên liệu ăn sáng (Mẫu)', NULL, 1);
    SET @NhomVatTu1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhomVatTu] ([TenNhomVatTu], [MoTa], [TrangThai])
    VALUES (N'Hóa chất giặt (Mẫu)', NULL, 1);
    SET @NhomVatTu2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhomVatTu] ([TenNhomVatTu], [MoTa], [TrangThai])
    VALUES (N'Dầu massage (Mẫu)', NULL, 1);
    SET @NhomVatTu3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhomVatTu] ([TenNhomVatTu], [MoTa], [TrangThai])
    VALUES (N'Nguyên liệu pha chế (Mẫu)', NULL, 1);
    SET @NhomVatTu4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhomVatTu] ([TenNhomVatTu], [MoTa], [TrangThai])
    VALUES (N'Nước đóng chai (Mẫu)', NULL, 1);
    SET @NhomVatTu5 = CONVERT(int, SCOPE_IDENTITY());

    -- 12. NhaCungCap: thêm 5 dòng
    INSERT INTO dbo.[NhaCungCap] ([TenNhaCungCap], [LoaiCungCap], [SoDienThoai], [Email], [DiaChi], [NguoiDaiDien], [TrangThai], [GhiChu])
    VALUES (N'Nhà cung cấp An Phát (Mẫu)', N'Thực phẩm và thiết bị bếp', N'02800000001', N'ncc1@example.com', N'Địa chỉ giả lập', N'Người đại diện 1', 1, N'Dữ liệu giả lập phục vụ học tập');
    SET @NhaCungCap1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhaCungCap] ([TenNhaCungCap], [LoaiCungCap], [SoDienThoai], [Email], [DiaChi], [NguoiDaiDien], [TrangThai], [GhiChu])
    VALUES (N'Nhà cung cấp Sạch Việt (Mẫu)', N'Hóa chất và thiết bị giặt', N'02800000002', N'ncc2@example.com', N'Địa chỉ giả lập', N'Người đại diện 2', 1, N'Dữ liệu giả lập phục vụ học tập');
    SET @NhaCungCap2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhaCungCap] ([TenNhaCungCap], [LoaiCungCap], [SoDienThoai], [Email], [DiaChi], [NguoiDaiDien], [TrangThai], [GhiChu])
    VALUES (N'Nhà cung cấp An Nhiên (Mẫu)', N'Vật tư và thiết bị spa', N'02800000003', N'ncc3@example.com', N'Địa chỉ giả lập', N'Người đại diện 3', 1, N'Dữ liệu giả lập phục vụ học tập');
    SET @NhaCungCap3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhaCungCap] ([TenNhaCungCap], [LoaiCungCap], [SoDienThoai], [Email], [DiaChi], [NguoiDaiDien], [TrangThai], [GhiChu])
    VALUES (N'Nhà cung cấp Hương Việt (Mẫu)', N'Đồ uống và thiết bị pha chế', N'02800000004', N'ncc4@example.com', N'Địa chỉ giả lập', N'Người đại diện 4', 1, N'Dữ liệu giả lập phục vụ học tập');
    SET @NhaCungCap4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhaCungCap] ([TenNhaCungCap], [LoaiCungCap], [SoDienThoai], [Email], [DiaChi], [NguoiDaiDien], [TrangThai], [GhiChu])
    VALUES (N'Nhà cung cấp Minh An (Mẫu)', N'Nước uống và tủ lạnh', N'02800000005', N'ncc5@example.com', N'Địa chỉ giả lập', N'Người đại diện 5', 1, N'Dữ liệu giả lập phục vụ học tập');
    SET @NhaCungCap5 = CONVERT(int, SCOPE_IDENTITY());

    -- 13. UuDai: thêm 5 dòng
    INSERT INTO dbo.[UuDai] ([MaCode], [TenUuDai], [LoaiGiam], [GiaTriGiam], [NgayBatDau], [NgayKetThuc], [DieuKien], [TrangThai])
    VALUES (N'MAU2026_1', N'Ưu đãi mẫu 1', N'PhanTram', 5, N'20260901', N'20261231', N'Áp dụng trên tổng hóa đơn; không cộng dồn ưu đãi', 1);
    SET @UuDai1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[UuDai] ([MaCode], [TenUuDai], [LoaiGiam], [GiaTriGiam], [NgayBatDau], [NgayKetThuc], [DieuKien], [TrangThai])
    VALUES (N'MAU2026_2', N'Ưu đãi mẫu 2', N'PhanTram', 10, N'20260901', N'20261231', N'Áp dụng trên tổng hóa đơn; không cộng dồn ưu đãi', 1);
    SET @UuDai2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[UuDai] ([MaCode], [TenUuDai], [LoaiGiam], [GiaTriGiam], [NgayBatDau], [NgayKetThuc], [DieuKien], [TrangThai])
    VALUES (N'MAU2026_3', N'Ưu đãi mẫu 3', N'PhanTram', 15, N'20260901', N'20261231', N'Áp dụng trên tổng hóa đơn; không cộng dồn ưu đãi', 1);
    SET @UuDai3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[UuDai] ([MaCode], [TenUuDai], [LoaiGiam], [GiaTriGiam], [NgayBatDau], [NgayKetThuc], [DieuKien], [TrangThai])
    VALUES (N'MAU2026_4', N'Ưu đãi mẫu 4', N'PhanTram', 20, N'20260901', N'20261231', N'Áp dụng trên tổng hóa đơn; không cộng dồn ưu đãi', 1);
    SET @UuDai4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[UuDai] ([MaCode], [TenUuDai], [LoaiGiam], [GiaTriGiam], [NgayBatDau], [NgayKetThuc], [DieuKien], [TrangThai])
    VALUES (N'MAU2026_5', N'Ưu đãi mẫu 5', N'PhanTram', 25, N'20260901', N'20261231', N'Áp dụng trên tổng hóa đơn; không cộng dồn ưu đãi', 1);
    SET @UuDai5 = CONVERT(int, SCOPE_IDENTITY());

    -- 14. Tang: thêm 5 dòng
    INSERT INTO dbo.[Tang] ([MaKhuVuc], [SoTang], [TenTang], [MoTa], [TrangThai])
    VALUES (@KhuVuc1, 1, N'Tầng 1', N'Tầng phục vụ lưu trú', 1);
    SET @Tang1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Tang] ([MaKhuVuc], [SoTang], [TenTang], [MoTa], [TrangThai])
    VALUES (@KhuVuc2, 2, N'Tầng 2', N'Tầng phục vụ lưu trú', 1);
    SET @Tang2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Tang] ([MaKhuVuc], [SoTang], [TenTang], [MoTa], [TrangThai])
    VALUES (@KhuVuc3, 3, N'Tầng 3', N'Tầng phục vụ lưu trú', 1);
    SET @Tang3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Tang] ([MaKhuVuc], [SoTang], [TenTang], [MoTa], [TrangThai])
    VALUES (@KhuVuc4, 4, N'Tầng 4', N'Tầng phục vụ lưu trú', 1);
    SET @Tang4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Tang] ([MaKhuVuc], [SoTang], [TenTang], [MoTa], [TrangThai])
    VALUES (@KhuVuc5, 5, N'Tầng 5', N'Tầng phục vụ lưu trú', 1);
    SET @Tang5 = CONVERT(int, SCOPE_IDENTITY());

    -- 15. Phong: thêm 5 dòng
    INSERT INTO dbo.[Phong] ([SoPhong], [MaTang], [MaLoaiPhong], [LoaiView], [KhongHutThuoc], [TrangThaiKhaiThac], [TrangThaiVeSinh], [GhiChu])
    VALUES (N'M101', @Tang1, @LoaiPhong1, N'Thành phố', 1, N'SanSang', N'Sach', N'Phòng mẫu; đã hoàn tất vệ sinh và bảo trì');
    SET @Phong1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Phong] ([SoPhong], [MaTang], [MaLoaiPhong], [LoaiView], [KhongHutThuoc], [TrangThaiKhaiThac], [TrangThaiVeSinh], [GhiChu])
    VALUES (N'M201', @Tang2, @LoaiPhong2, N'Sân vườn', 1, N'SanSang', N'Sach', N'Phòng mẫu; đã hoàn tất vệ sinh và bảo trì');
    SET @Phong2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Phong] ([SoPhong], [MaTang], [MaLoaiPhong], [LoaiView], [KhongHutThuoc], [TrangThaiKhaiThac], [TrangThaiVeSinh], [GhiChu])
    VALUES (N'M301', @Tang3, @LoaiPhong3, N'Biển', 1, N'SanSang', N'Sach', N'Phòng mẫu; đã hoàn tất vệ sinh và bảo trì');
    SET @Phong3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Phong] ([SoPhong], [MaTang], [MaLoaiPhong], [LoaiView], [KhongHutThuoc], [TrangThaiKhaiThac], [TrangThaiVeSinh], [GhiChu])
    VALUES (N'M401', @Tang4, @LoaiPhong4, N'Hồ bơi', 1, N'SanSang', N'Sach', N'Phòng mẫu; đã hoàn tất vệ sinh và bảo trì');
    SET @Phong4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[Phong] ([SoPhong], [MaTang], [MaLoaiPhong], [LoaiView], [KhongHutThuoc], [TrangThaiKhaiThac], [TrangThaiVeSinh], [GhiChu])
    VALUES (N'M501', @Tang5, @LoaiPhong5, N'Biển', 1, N'SanSang', N'Sach', N'Phòng mẫu; đã hoàn tất vệ sinh và bảo trì');
    SET @Phong5 = CONVERT(int, SCOPE_IDENTITY());

    -- 16. LoaiPhong_TienNghi: thêm 25 dòng
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong1, @TienNghi1, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong1, @TienNghi2, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong1, @TienNghi3, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong1, @TienNghi4, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong1, @TienNghi5, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong2, @TienNghi1, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong2, @TienNghi2, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong2, @TienNghi3, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong2, @TienNghi4, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong2, @TienNghi5, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong3, @TienNghi1, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong3, @TienNghi2, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong3, @TienNghi3, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong3, @TienNghi4, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong3, @TienNghi5, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong4, @TienNghi1, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong4, @TienNghi2, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong4, @TienNghi3, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong4, @TienNghi4, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong4, @TienNghi5, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong5, @TienNghi1, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong5, @TienNghi2, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong5, @TienNghi3, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong5, @TienNghi4, 1, NULL);
    INSERT INTO dbo.[LoaiPhong_TienNghi] ([MaLoaiPhong], [MaTienNghi], [SoLuong], [GhiChu])
    VALUES (@LoaiPhong5, @TienNghi5, 1, NULL);

    -- 17. NhanVien: thêm 5 dòng
    INSERT INTO dbo.[NhanVien] ([HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [MaPhongBan], [MaChucVu], [NgayVaoLam], [LuongCoBan], [TrangThai])
    VALUES (N'Nguyễn Minh Quân', N'19910115', N'M', N'0900000001', N'nhanvien1@example.com', N'Địa chỉ giả lập tại TP. Hồ Chí Minh', @PhongBan1, @ChucVu1, N'20250101', 20000000, N'DangLam');
    SET @NhanVien1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhanVien] ([HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [MaPhongBan], [MaChucVu], [NgayVaoLam], [LuongCoBan], [TrangThai])
    VALUES (N'Trần Thu Hà', N'19920215', N'F', N'0900000002', N'nhanvien2@example.com', N'Địa chỉ giả lập tại TP. Hồ Chí Minh', @PhongBan2, @ChucVu2, N'20250101', 9000000, N'DangLam');
    SET @NhanVien2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhanVien] ([HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [MaPhongBan], [MaChucVu], [NgayVaoLam], [LuongCoBan], [TrangThai])
    VALUES (N'Lê Thị Mai', N'19930315', N'F', N'0900000003', N'nhanvien3@example.com', N'Địa chỉ giả lập tại TP. Hồ Chí Minh', @PhongBan3, @ChucVu3, N'20250101', 8000000, N'DangLam');
    SET @NhanVien3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhanVien] ([HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [MaPhongBan], [MaChucVu], [NgayVaoLam], [LuongCoBan], [TrangThai])
    VALUES (N'Phạm Quốc Huy', N'19940415', N'M', N'0900000004', N'nhanvien4@example.com', N'Địa chỉ giả lập tại TP. Hồ Chí Minh', @PhongBan4, @ChucVu4, N'20250101', 11000000, N'DangLam');
    SET @NhanVien4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhanVien] ([HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [MaPhongBan], [MaChucVu], [NgayVaoLam], [LuongCoBan], [TrangThai])
    VALUES (N'Võ Ngọc Anh', N'19950515', N'F', N'0900000005', N'nhanvien5@example.com', N'Địa chỉ giả lập tại TP. Hồ Chí Minh', @PhongBan5, @ChucVu5, N'20250101', 12000000, N'DangLam');
    SET @NhanVien5 = CONVERT(int, SCOPE_IDENTITY());

    -- 18. TaiKhoan: thêm 5 dòng
    INSERT INTO dbo.[TaiKhoan] ([MaNhanVien], [TenDangNhap], [MatKhauHash], [MaVaiTro], [TrangThai], [LanDangNhapCuoi], [NgayTao])
    VALUES (@NhanVien1, N'mau_admin', N'bb211e4f27037eccfd865c7d11d808b9391c04a2a694ecad5f4669b2a146b3d1', @VaiTro1, N'Khoa', NULL, N'2026-09-01T08:00:00');
    SET @TaiKhoan1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[TaiKhoan] ([MaNhanVien], [TenDangNhap], [MatKhauHash], [MaVaiTro], [TrangThai], [LanDangNhapCuoi], [NgayTao])
    VALUES (@NhanVien2, N'mau_letan', N'6ebe4bbf6daaf1b3a6f944b20edd52601fc25aa7801d2aecedd8ceca5b69a35c', @VaiTro2, N'Khoa', NULL, N'2026-09-01T08:00:00');
    SET @TaiKhoan2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[TaiKhoan] ([MaNhanVien], [TenDangNhap], [MatKhauHash], [MaVaiTro], [TrangThai], [LanDangNhapCuoi], [NgayTao])
    VALUES (@NhanVien3, N'mau_buongphong', N'4568d1e1584dca6c0581f107c52fe492d7bc17fdb111ba99740703ca2589327a', @VaiTro3, N'Khoa', NULL, N'2026-09-01T08:00:00');
    SET @TaiKhoan3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[TaiKhoan] ([MaNhanVien], [TenDangNhap], [MatKhauHash], [MaVaiTro], [TrangThai], [LanDangNhapCuoi], [NgayTao])
    VALUES (@NhanVien4, N'mau_kythuat', N'20eeb7033e8f016431a9e6e1592e893901e036c408cca3b4a8bf0d9b662d2f3c', @VaiTro4, N'Khoa', NULL, N'2026-09-01T08:00:00');
    SET @TaiKhoan4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[TaiKhoan] ([MaNhanVien], [TenDangNhap], [MatKhauHash], [MaVaiTro], [TrangThai], [LanDangNhapCuoi], [NgayTao])
    VALUES (@NhanVien5, N'mau_ketoan', N'b234c687393ec9bc357fcc4cb5cce695d0b3195c27049b6d93b5ee618a4eb89b', @VaiTro5, N'Khoa', NULL, N'2026-09-01T08:00:00');
    SET @TaiKhoan5 = CONVERT(int, SCOPE_IDENTITY());

    -- 19. VaiTro_Quyen: thêm 9 dòng
    INSERT INTO dbo.[VaiTro_Quyen] ([MaVaiTro], [MaQuyen])
    VALUES (@VaiTro1, @Quyen1);
    INSERT INTO dbo.[VaiTro_Quyen] ([MaVaiTro], [MaQuyen])
    VALUES (@VaiTro1, @Quyen2);
    INSERT INTO dbo.[VaiTro_Quyen] ([MaVaiTro], [MaQuyen])
    VALUES (@VaiTro1, @Quyen3);
    INSERT INTO dbo.[VaiTro_Quyen] ([MaVaiTro], [MaQuyen])
    VALUES (@VaiTro1, @Quyen4);
    INSERT INTO dbo.[VaiTro_Quyen] ([MaVaiTro], [MaQuyen])
    VALUES (@VaiTro1, @Quyen5);
    INSERT INTO dbo.[VaiTro_Quyen] ([MaVaiTro], [MaQuyen])
    VALUES (@VaiTro2, @Quyen2);
    INSERT INTO dbo.[VaiTro_Quyen] ([MaVaiTro], [MaQuyen])
    VALUES (@VaiTro3, @Quyen3);
    INSERT INTO dbo.[VaiTro_Quyen] ([MaVaiTro], [MaQuyen])
    VALUES (@VaiTro4, @Quyen4);
    INSERT INTO dbo.[VaiTro_Quyen] ([MaVaiTro], [MaQuyen])
    VALUES (@VaiTro5, @Quyen5);

    -- 20. KhachHang: thêm 5 dòng
    INSERT INTO dbo.[KhachHang] ([HoTen], [NgaySinh], [GioiTinh], [LoaiGiayTo], [SoGiayTo], [QuocTich], [SoDienThoai], [Email], [DiaChi], [GhiChu], [NgayTao], [TrangThai])
    VALUES (N'Nguyễn Văn An', N'19910120', N'M', N'CCCD', N'MAU_CCCD_0001', N'Việt Nam', N'0910000001', N'khachhang1@example.com', N'Hà Nội', N'Thông tin giả lập, không phải giấy tờ thật', N'2026-09-01T09:00:00', 1);
    SET @KhachHang1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[KhachHang] ([HoTen], [NgaySinh], [GioiTinh], [LoaiGiayTo], [SoGiayTo], [QuocTich], [SoDienThoai], [Email], [DiaChi], [GhiChu], [NgayTao], [TrangThai])
    VALUES (N'Trần Thị Bình', N'19920220', N'F', N'CCCD', N'MAU_CCCD_0002', N'Việt Nam', N'0910000002', N'khachhang2@example.com', N'Đà Nẵng', N'Thông tin giả lập, không phải giấy tờ thật', N'2026-09-01T09:00:00', 1);
    SET @KhachHang2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[KhachHang] ([HoTen], [NgaySinh], [GioiTinh], [LoaiGiayTo], [SoGiayTo], [QuocTich], [SoDienThoai], [Email], [DiaChi], [GhiChu], [NgayTao], [TrangThai])
    VALUES (N'Lê Hoàng Nam', N'19930320', N'M', N'CCCD', N'MAU_CCCD_0003', N'Việt Nam', N'0910000003', N'khachhang3@example.com', N'TP. Hồ Chí Minh', N'Thông tin giả lập, không phải giấy tờ thật', N'2026-09-01T09:00:00', 1);
    SET @KhachHang3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[KhachHang] ([HoTen], [NgaySinh], [GioiTinh], [LoaiGiayTo], [SoGiayTo], [QuocTich], [SoDienThoai], [Email], [DiaChi], [GhiChu], [NgayTao], [TrangThai])
    VALUES (N'Phạm Minh Châu', N'19940420', N'F', N'CCCD', N'MAU_CCCD_0004', N'Việt Nam', N'0910000004', N'khachhang4@example.com', N'Cần Thơ', N'Thông tin giả lập, không phải giấy tờ thật', N'2026-09-01T09:00:00', 1);
    SET @KhachHang4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[KhachHang] ([HoTen], [NgaySinh], [GioiTinh], [LoaiGiayTo], [SoGiayTo], [QuocTich], [SoDienThoai], [Email], [DiaChi], [GhiChu], [NgayTao], [TrangThai])
    VALUES (N'Võ Thanh Tùng', N'19950520', N'M', N'CCCD', N'MAU_CCCD_0005', N'Việt Nam', N'0910000005', N'khachhang5@example.com', N'Huế', N'Thông tin giả lập, không phải giấy tờ thật', N'2026-09-01T09:00:00', 1);
    SET @KhachHang5 = CONVERT(int, SCOPE_IDENTITY());

    -- 21. DichVu: thêm 5 dòng
    INSERT INTO dbo.[DichVu] ([MaLoaiDichVu], [TenDichVu], [DonViTinh], [DonGia], [MoTa], [TrangThai])
    VALUES (@LoaiDichVu1, N'Buffet sáng (Mẫu)', N'Suất', 200000, N'Dịch vụ dành cho khách lưu trú', 1);
    SET @DichVu1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[DichVu] ([MaLoaiDichVu], [TenDichVu], [DonViTinh], [DonGia], [MoTa], [TrangThai])
    VALUES (@LoaiDichVu2, N'Giặt quần áo (Mẫu)', N'Kg', 80000, N'Dịch vụ dành cho khách lưu trú', 1);
    SET @DichVu2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[DichVu] ([MaLoaiDichVu], [TenDichVu], [DonViTinh], [DonGia], [MoTa], [TrangThai])
    VALUES (@LoaiDichVu3, N'Massage 60 phút (Mẫu)', N'Lượt', 500000, N'Dịch vụ dành cho khách lưu trú', 1);
    SET @DichVu3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[DichVu] ([MaLoaiDichVu], [TenDichVu], [DonViTinh], [DonGia], [MoTa], [TrangThai])
    VALUES (@LoaiDichVu4, N'Cà phê tại phòng (Mẫu)', N'Ly', 60000, N'Dịch vụ dành cho khách lưu trú', 1);
    SET @DichVu4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[DichVu] ([MaLoaiDichVu], [TenDichVu], [DonViTinh], [DonGia], [MoTa], [TrangThai])
    VALUES (@LoaiDichVu5, N'Nước suối minibar (Mẫu)', N'Chai', 30000, N'Dịch vụ dành cho khách lưu trú', 1);
    SET @DichVu5 = CONVERT(int, SCOPE_IDENTITY());

    -- 22. GoiDichVu: thêm 5 dòng
    INSERT INTO dbo.[GoiDichVu] ([TenGoiDichVu], [DonGia], [MoTa], [TrangThai])
    VALUES (N'Gói 2 suất ăn sáng (Mẫu)', 360000, N'Giá ưu đãi khi mua theo gói', 1);
    SET @GoiDichVu1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[GoiDichVu] ([TenGoiDichVu], [DonGia], [MoTa], [TrangThai])
    VALUES (N'Gói giặt 5 kg (Mẫu)', 350000, N'Giá ưu đãi khi mua theo gói', 1);
    SET @GoiDichVu2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[GoiDichVu] ([TenGoiDichVu], [DonGia], [MoTa], [TrangThai])
    VALUES (N'Gói 2 lượt massage (Mẫu)', 900000, N'Giá ưu đãi khi mua theo gói', 1);
    SET @GoiDichVu3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[GoiDichVu] ([TenGoiDichVu], [DonGia], [MoTa], [TrangThai])
    VALUES (N'Gói 2 ly cà phê (Mẫu)', 100000, N'Giá ưu đãi khi mua theo gói', 1);
    SET @GoiDichVu4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[GoiDichVu] ([TenGoiDichVu], [DonGia], [MoTa], [TrangThai])
    VALUES (N'Gói 6 chai nước (Mẫu)', 150000, N'Giá ưu đãi khi mua theo gói', 1);
    SET @GoiDichVu5 = CONVERT(int, SCOPE_IDENTITY());

    -- 23. ChiTietGoiDichVu: thêm 5 dòng
    INSERT INTO dbo.[ChiTietGoiDichVu] ([MaGoiDichVu], [MaDichVu], [SoLuong], [GhiChu])
    VALUES (@GoiDichVu1, @DichVu1, 2, NULL);
    INSERT INTO dbo.[ChiTietGoiDichVu] ([MaGoiDichVu], [MaDichVu], [SoLuong], [GhiChu])
    VALUES (@GoiDichVu2, @DichVu2, 5, NULL);
    INSERT INTO dbo.[ChiTietGoiDichVu] ([MaGoiDichVu], [MaDichVu], [SoLuong], [GhiChu])
    VALUES (@GoiDichVu3, @DichVu3, 2, NULL);
    INSERT INTO dbo.[ChiTietGoiDichVu] ([MaGoiDichVu], [MaDichVu], [SoLuong], [GhiChu])
    VALUES (@GoiDichVu4, @DichVu4, 2, NULL);
    INSERT INTO dbo.[ChiTietGoiDichVu] ([MaGoiDichVu], [MaDichVu], [SoLuong], [GhiChu])
    VALUES (@GoiDichVu5, @DichVu5, 6, NULL);

    -- 24. VatTu: thêm 5 dòng
    INSERT INTO dbo.[VatTu] ([MaNhomVatTu], [MaKhoMacDinh], [TenVatTu], [DonViTinh], [MoTa], [SoLuongToiThieu], [TrangThai])
    VALUES (@NhomVatTu1, @Kho1, N'Trứng gà (Mẫu)', N'Quả', N'Vật tư dùng cho dịch vụ tương ứng', 100, 1);
    SET @VatTu1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[VatTu] ([MaNhomVatTu], [MaKhoMacDinh], [TenVatTu], [DonViTinh], [MoTa], [SoLuongToiThieu], [TrangThai])
    VALUES (@NhomVatTu2, @Kho2, N'Nước giặt (Mẫu)', N'Lít', N'Vật tư dùng cho dịch vụ tương ứng', 20, 1);
    SET @VatTu2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[VatTu] ([MaNhomVatTu], [MaKhoMacDinh], [TenVatTu], [DonViTinh], [MoTa], [SoLuongToiThieu], [TrangThai])
    VALUES (@NhomVatTu3, @Kho3, N'Dầu massage (Mẫu)', N'Lít', N'Vật tư dùng cho dịch vụ tương ứng', 5, 1);
    SET @VatTu3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[VatTu] ([MaNhomVatTu], [MaKhoMacDinh], [TenVatTu], [DonViTinh], [MoTa], [SoLuongToiThieu], [TrangThai])
    VALUES (@NhomVatTu4, @Kho4, N'Cà phê hạt (Mẫu)', N'Kg', N'Vật tư dùng cho dịch vụ tương ứng', 10, 1);
    SET @VatTu4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[VatTu] ([MaNhomVatTu], [MaKhoMacDinh], [TenVatTu], [DonViTinh], [MoTa], [SoLuongToiThieu], [TrangThai])
    VALUES (@NhomVatTu5, @Kho5, N'Nước suối 500 ml (Mẫu)', N'Chai', N'Vật tư dùng cho dịch vụ tương ứng', 100, 1);
    SET @VatTu5 = CONVERT(int, SCOPE_IDENTITY());

    -- 25. VatTu_NhaCungCap: thêm 5 dòng
    INSERT INTO dbo.[VatTu_NhaCungCap] ([MaVatTu], [MaNhaCungCap], [MaSanPhamNCC], [DonGiaNhap], [ThoiGianCungCap], [TrangThai], [GhiChu])
    VALUES (@VatTu1, @NhaCungCap1, N'MAU_VT_001', 3000, N'20260901', 1, NULL);
    INSERT INTO dbo.[VatTu_NhaCungCap] ([MaVatTu], [MaNhaCungCap], [MaSanPhamNCC], [DonGiaNhap], [ThoiGianCungCap], [TrangThai], [GhiChu])
    VALUES (@VatTu2, @NhaCungCap2, N'MAU_VT_002', 40000, N'20260901', 1, NULL);
    INSERT INTO dbo.[VatTu_NhaCungCap] ([MaVatTu], [MaNhaCungCap], [MaSanPhamNCC], [DonGiaNhap], [ThoiGianCungCap], [TrangThai], [GhiChu])
    VALUES (@VatTu3, @NhaCungCap3, N'MAU_VT_003', 300000, N'20260901', 1, NULL);
    INSERT INTO dbo.[VatTu_NhaCungCap] ([MaVatTu], [MaNhaCungCap], [MaSanPhamNCC], [DonGiaNhap], [ThoiGianCungCap], [TrangThai], [GhiChu])
    VALUES (@VatTu4, @NhaCungCap4, N'MAU_VT_004', 200000, N'20260901', 1, NULL);
    INSERT INTO dbo.[VatTu_NhaCungCap] ([MaVatTu], [MaNhaCungCap], [MaSanPhamNCC], [DonGiaNhap], [ThoiGianCungCap], [TrangThai], [GhiChu])
    VALUES (@VatTu5, @NhaCungCap5, N'MAU_VT_005', 6000, N'20260901', 1, NULL);

    -- 26. TieuHaoVatTu: thêm 5 dòng
    INSERT INTO dbo.[TieuHaoVatTu] ([MaDichVu], [MaVatTu], [SoLuongTieuHao], [GhiChu])
    VALUES (@DichVu1, @VatTu1, 2, N'Định mức trên một đơn vị dịch vụ');
    INSERT INTO dbo.[TieuHaoVatTu] ([MaDichVu], [MaVatTu], [SoLuongTieuHao], [GhiChu])
    VALUES (@DichVu2, @VatTu2, 0.05, N'Định mức trên một đơn vị dịch vụ');
    INSERT INTO dbo.[TieuHaoVatTu] ([MaDichVu], [MaVatTu], [SoLuongTieuHao], [GhiChu])
    VALUES (@DichVu3, @VatTu3, 0.03, N'Định mức trên một đơn vị dịch vụ');
    INSERT INTO dbo.[TieuHaoVatTu] ([MaDichVu], [MaVatTu], [SoLuongTieuHao], [GhiChu])
    VALUES (@DichVu4, @VatTu4, 0.02, N'Định mức trên một đơn vị dịch vụ');
    INSERT INTO dbo.[TieuHaoVatTu] ([MaDichVu], [MaVatTu], [SoLuongTieuHao], [GhiChu])
    VALUES (@DichVu5, @VatTu5, 1, N'Định mức trên một đơn vị dịch vụ');

    -- 27. ThietBi: thêm 5 dòng
    INSERT INTO dbo.[ThietBi] ([MaPhong], [MaNhaCungCap], [TenThietBi], [MaThietBiNCC], [NgayMua], [HanBaoHanh], [TrangThai], [GhiChu])
    VALUES (@Phong1, @NhaCungCap5, N'Tủ lạnh mini phòng M101', N'MAU_TL_001', N'20250110', N'20270110', N'DangSuDung', N'Đã kiểm tra sau bảo trì');
    SET @ThietBi1 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[ThietBi] ([MaPhong], [MaNhaCungCap], [TenThietBi], [MaThietBiNCC], [NgayMua], [HanBaoHanh], [TrangThai], [GhiChu])
    VALUES (@Phong2, @NhaCungCap5, N'Tủ lạnh mini phòng M201', N'MAU_TL_002', N'20250110', N'20270110', N'DangSuDung', N'Đã kiểm tra sau bảo trì');
    SET @ThietBi2 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[ThietBi] ([MaPhong], [MaNhaCungCap], [TenThietBi], [MaThietBiNCC], [NgayMua], [HanBaoHanh], [TrangThai], [GhiChu])
    VALUES (@Phong3, @NhaCungCap5, N'Tủ lạnh mini phòng M301', N'MAU_TL_003', N'20250110', N'20270110', N'DangSuDung', N'Đã kiểm tra sau bảo trì');
    SET @ThietBi3 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[ThietBi] ([MaPhong], [MaNhaCungCap], [TenThietBi], [MaThietBiNCC], [NgayMua], [HanBaoHanh], [TrangThai], [GhiChu])
    VALUES (@Phong4, @NhaCungCap5, N'Tủ lạnh mini phòng M401', N'MAU_TL_004', N'20250110', N'20270110', N'DangSuDung', N'Đã kiểm tra sau bảo trì');
    SET @ThietBi4 = CONVERT(int, SCOPE_IDENTITY());
    INSERT INTO dbo.[ThietBi] ([MaPhong], [MaNhaCungCap], [TenThietBi], [MaThietBiNCC], [NgayMua], [HanBaoHanh], [TrangThai], [GhiChu])
    VALUES (@Phong5, @NhaCungCap5, N'Tủ lạnh mini phòng M501', N'MAU_TL_005', N'20250110', N'20270110', N'DangSuDung', N'Đã kiểm tra sau bảo trì');
    SET @ThietBi5 = CONVERT(int, SCOPE_IDENTITY());

    -- 28. DatPhong: thêm 5 dòng
    INSERT INTO dbo.[DatPhong] ([MaKhachHangDat], [MaNhanVienTao], [NgayDat], [TrangThai], [GhiChu])
    VALUES (@KhachHang1, @NhanVien2, N'2026-09-01T10:00:00', N'DaCheckOut', N'Đặt một phòng trong hai đêm');
    SET @DatPhong1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[DatPhong] ([MaKhachHangDat], [MaNhanVienTao], [NgayDat], [TrangThai], [GhiChu])
    VALUES (@KhachHang2, @NhanVien2, N'2026-09-02T10:00:00', N'DaCheckOut', N'Đặt một phòng trong hai đêm');
    SET @DatPhong2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[DatPhong] ([MaKhachHangDat], [MaNhanVienTao], [NgayDat], [TrangThai], [GhiChu])
    VALUES (@KhachHang3, @NhanVien2, N'2026-09-03T10:00:00', N'DaCheckOut', N'Đặt một phòng trong hai đêm');
    SET @DatPhong3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[DatPhong] ([MaKhachHangDat], [MaNhanVienTao], [NgayDat], [TrangThai], [GhiChu])
    VALUES (@KhachHang4, @NhanVien2, N'2026-09-04T10:00:00', N'DaCheckOut', N'Đặt một phòng trong hai đêm');
    SET @DatPhong4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[DatPhong] ([MaKhachHangDat], [MaNhanVienTao], [NgayDat], [TrangThai], [GhiChu])
    VALUES (@KhachHang5, @NhanVien2, N'2026-09-05T10:00:00', N'DaCheckOut', N'Đặt một phòng trong hai đêm');
    SET @DatPhong5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 29. ChiTietDatPhong: thêm 5 dòng
    INSERT INTO dbo.[ChiTietDatPhong] ([MaDatPhong], [MaLoaiPhong], [NgayNhanDuKien], [NgayTraDuKien], [SoLuongPhong], [NguoiLon], [TreEm], [DonGiaDuKien], [GhiChu])
    VALUES (@DatPhong1, @LoaiPhong1, N'2026-09-10T14:00:00', N'2026-09-12T12:00:00', 1, 2, 0, 800000, N'Giá phòng tính theo đêm');
    SET @ChiTietDatPhong1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietDatPhong] ([MaDatPhong], [MaLoaiPhong], [NgayNhanDuKien], [NgayTraDuKien], [SoLuongPhong], [NguoiLon], [TreEm], [DonGiaDuKien], [GhiChu])
    VALUES (@DatPhong2, @LoaiPhong2, N'2026-09-10T14:00:00', N'2026-09-12T12:00:00', 1, 2, 0, 1200000, N'Giá phòng tính theo đêm');
    SET @ChiTietDatPhong2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietDatPhong] ([MaDatPhong], [MaLoaiPhong], [NgayNhanDuKien], [NgayTraDuKien], [SoLuongPhong], [NguoiLon], [TreEm], [DonGiaDuKien], [GhiChu])
    VALUES (@DatPhong3, @LoaiPhong3, N'2026-09-10T14:00:00', N'2026-09-12T12:00:00', 1, 2, 0, 1800000, N'Giá phòng tính theo đêm');
    SET @ChiTietDatPhong3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietDatPhong] ([MaDatPhong], [MaLoaiPhong], [NgayNhanDuKien], [NgayTraDuKien], [SoLuongPhong], [NguoiLon], [TreEm], [DonGiaDuKien], [GhiChu])
    VALUES (@DatPhong4, @LoaiPhong4, N'2026-09-10T14:00:00', N'2026-09-12T12:00:00', 1, 4, 0, 2500000, N'Giá phòng tính theo đêm');
    SET @ChiTietDatPhong4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietDatPhong] ([MaDatPhong], [MaLoaiPhong], [NgayNhanDuKien], [NgayTraDuKien], [SoLuongPhong], [NguoiLon], [TreEm], [DonGiaDuKien], [GhiChu])
    VALUES (@DatPhong5, @LoaiPhong5, N'2026-09-10T14:00:00', N'2026-09-12T12:00:00', 1, 2, 0, 4000000, N'Giá phòng tính theo đêm');
    SET @ChiTietDatPhong5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 30. XepPhong: thêm 5 dòng
    INSERT INTO dbo.[XepPhong] ([MaChiTietDatPhong], [MaPhong], [NgayNhanThucTe], [NgayTraThucTe], [TrangThai], [GhiChu])
    VALUES (@ChiTietDatPhong1, @Phong1, N'2026-09-10T14:00:00', N'2026-09-12T12:00:00', N'DaCheckOut', NULL);
    SET @XepPhong1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[XepPhong] ([MaChiTietDatPhong], [MaPhong], [NgayNhanThucTe], [NgayTraThucTe], [TrangThai], [GhiChu])
    VALUES (@ChiTietDatPhong2, @Phong2, N'2026-09-10T14:00:00', N'2026-09-12T12:00:00', N'DaCheckOut', NULL);
    SET @XepPhong2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[XepPhong] ([MaChiTietDatPhong], [MaPhong], [NgayNhanThucTe], [NgayTraThucTe], [TrangThai], [GhiChu])
    VALUES (@ChiTietDatPhong3, @Phong3, N'2026-09-10T14:00:00', N'2026-09-12T12:00:00', N'DaCheckOut', NULL);
    SET @XepPhong3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[XepPhong] ([MaChiTietDatPhong], [MaPhong], [NgayNhanThucTe], [NgayTraThucTe], [TrangThai], [GhiChu])
    VALUES (@ChiTietDatPhong4, @Phong4, N'2026-09-10T14:00:00', N'2026-09-12T12:00:00', N'DaCheckOut', NULL);
    SET @XepPhong4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[XepPhong] ([MaChiTietDatPhong], [MaPhong], [NgayNhanThucTe], [NgayTraThucTe], [TrangThai], [GhiChu])
    VALUES (@ChiTietDatPhong5, @Phong5, N'2026-09-10T14:00:00', N'2026-09-12T12:00:00', N'DaCheckOut', NULL);
    SET @XepPhong5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 31. SuDungDichVu: thêm 5 dòng
    INSERT INTO dbo.[SuDungDichVu] ([MaDatPhong], [MaKhachHang], [MaDichVu], [MaGoiDichVu], [MaNhanVien], [SoLuong], [DonGia], [ThoiGianSuDung], [TrangThai], [GhiChu])
    VALUES (@DatPhong1, @KhachHang1, @DichVu1, NULL, @NhanVien2, 2, 200000, N'2026-09-11T10:00:00', N'DaSuDung', NULL);
    SET @SuDungDichVu1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[SuDungDichVu] ([MaDatPhong], [MaKhachHang], [MaDichVu], [MaGoiDichVu], [MaNhanVien], [SoLuong], [DonGia], [ThoiGianSuDung], [TrangThai], [GhiChu])
    VALUES (@DatPhong2, @KhachHang2, @DichVu2, NULL, @NhanVien2, 2, 80000, N'2026-09-11T10:00:00', N'DaSuDung', NULL);
    SET @SuDungDichVu2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[SuDungDichVu] ([MaDatPhong], [MaKhachHang], [MaDichVu], [MaGoiDichVu], [MaNhanVien], [SoLuong], [DonGia], [ThoiGianSuDung], [TrangThai], [GhiChu])
    VALUES (@DatPhong3, @KhachHang3, @DichVu3, NULL, @NhanVien2, 2, 500000, N'2026-09-11T10:00:00', N'DaSuDung', NULL);
    SET @SuDungDichVu3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[SuDungDichVu] ([MaDatPhong], [MaKhachHang], [MaDichVu], [MaGoiDichVu], [MaNhanVien], [SoLuong], [DonGia], [ThoiGianSuDung], [TrangThai], [GhiChu])
    VALUES (@DatPhong4, @KhachHang4, @DichVu4, NULL, @NhanVien2, 2, 60000, N'2026-09-11T10:00:00', N'DaSuDung', NULL);
    SET @SuDungDichVu4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[SuDungDichVu] ([MaDatPhong], [MaKhachHang], [MaDichVu], [MaGoiDichVu], [MaNhanVien], [SoLuong], [DonGia], [ThoiGianSuDung], [TrangThai], [GhiChu])
    VALUES (@DatPhong5, @KhachHang5, @DichVu5, NULL, @NhanVien2, 2, 30000, N'2026-09-11T10:00:00', N'DaSuDung', NULL);
    SET @SuDungDichVu5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 32. HoaDon: thêm 5 dòng
    INSERT INTO dbo.[HoaDon] ([MaDatPhong], [MaKhachHang], [MaUuDai], [MaNhanVienLap], [NgayLap], [TongTien], [TienGiam], [ThanhTien], [TrangThai], [GhiChu])
    VALUES (@DatPhong1, @KhachHang1, @UuDai1, @NhanVien5, N'2026-09-12T12:00:00', 2000000, 100000, 1900000, N'DaThanhToan', N'Hai đêm phòng và dịch vụ; giảm theo ưu đãi');
    SET @HoaDon1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[HoaDon] ([MaDatPhong], [MaKhachHang], [MaUuDai], [MaNhanVienLap], [NgayLap], [TongTien], [TienGiam], [ThanhTien], [TrangThai], [GhiChu])
    VALUES (@DatPhong2, @KhachHang2, @UuDai2, @NhanVien5, N'2026-09-12T12:00:00', 2560000, 256000, 2304000, N'DaThanhToan', N'Hai đêm phòng và dịch vụ; giảm theo ưu đãi');
    SET @HoaDon2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[HoaDon] ([MaDatPhong], [MaKhachHang], [MaUuDai], [MaNhanVienLap], [NgayLap], [TongTien], [TienGiam], [ThanhTien], [TrangThai], [GhiChu])
    VALUES (@DatPhong3, @KhachHang3, @UuDai3, @NhanVien5, N'2026-09-12T12:00:00', 4600000, 690000, 3910000, N'DaThanhToan', N'Hai đêm phòng và dịch vụ; giảm theo ưu đãi');
    SET @HoaDon3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[HoaDon] ([MaDatPhong], [MaKhachHang], [MaUuDai], [MaNhanVienLap], [NgayLap], [TongTien], [TienGiam], [ThanhTien], [TrangThai], [GhiChu])
    VALUES (@DatPhong4, @KhachHang4, @UuDai4, @NhanVien5, N'2026-09-12T12:00:00', 5120000, 1024000, 4096000, N'DaThanhToan', N'Hai đêm phòng và dịch vụ; giảm theo ưu đãi');
    SET @HoaDon4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[HoaDon] ([MaDatPhong], [MaKhachHang], [MaUuDai], [MaNhanVienLap], [NgayLap], [TongTien], [TienGiam], [ThanhTien], [TrangThai], [GhiChu])
    VALUES (@DatPhong5, @KhachHang5, @UuDai5, @NhanVien5, N'2026-09-12T12:00:00', 8060000, 2015000, 6045000, N'DaThanhToan', N'Hai đêm phòng và dịch vụ; giảm theo ưu đãi');
    SET @HoaDon5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 33. ChiTietHoaDon: thêm 10 dòng
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon1, N'TienPhong', @ChiTietDatPhong1, NULL, NULL, N'Tiền phòng M101: 2 đêm', 2, 800000);
    SET @ChiTietHoaDon1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon1, N'DichVu', NULL, @SuDungDichVu1, NULL, N'Buffet sáng', 2, 200000);
    SET @ChiTietHoaDon2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon2, N'TienPhong', @ChiTietDatPhong2, NULL, NULL, N'Tiền phòng M201: 2 đêm', 2, 1200000);
    SET @ChiTietHoaDon3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon2, N'DichVu', NULL, @SuDungDichVu2, NULL, N'Giặt quần áo', 2, 80000);
    SET @ChiTietHoaDon4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon3, N'TienPhong', @ChiTietDatPhong3, NULL, NULL, N'Tiền phòng M301: 2 đêm', 2, 1800000);
    SET @ChiTietHoaDon5 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon3, N'DichVu', NULL, @SuDungDichVu3, NULL, N'Massage 60 phút', 2, 500000);
    SET @ChiTietHoaDon6 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon4, N'TienPhong', @ChiTietDatPhong4, NULL, NULL, N'Tiền phòng M401: 2 đêm', 2, 2500000);
    SET @ChiTietHoaDon7 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon4, N'DichVu', NULL, @SuDungDichVu4, NULL, N'Cà phê tại phòng', 2, 60000);
    SET @ChiTietHoaDon8 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon5, N'TienPhong', @ChiTietDatPhong5, NULL, NULL, N'Tiền phòng M501: 2 đêm', 2, 4000000);
    SET @ChiTietHoaDon9 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChiTietHoaDon] ([MaHoaDon], [LoaiChiPhi], [MaChiTietDatPhong], [MaSuDungDichVu], [MaGoiDichVu], [NoiDung], [SoLuong], [DonGia])
    VALUES (@HoaDon5, N'DichVu', NULL, @SuDungDichVu5, NULL, N'Nước suối minibar', 2, 30000);
    SET @ChiTietHoaDon10 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 34. ThanhToan: thêm 5 dòng
    INSERT INTO dbo.[ThanhToan] ([MaHoaDon], [MaNhanVien], [SoTien], [ThoiGianThanhToan], [MaGiaoDich], [TrangThai], [GhiChu])
    VALUES (@HoaDon1, @NhanVien5, 1900000, N'2026-09-12T12:05:00', N'MAU_TT_202609_001', N'ThanhCong', N'Thanh toán toàn bộ hóa đơn');
    SET @ThanhToan1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ThanhToan] ([MaHoaDon], [MaNhanVien], [SoTien], [ThoiGianThanhToan], [MaGiaoDich], [TrangThai], [GhiChu])
    VALUES (@HoaDon2, @NhanVien5, 2304000, N'2026-09-12T12:05:00', N'MAU_TT_202609_002', N'ThanhCong', N'Thanh toán toàn bộ hóa đơn');
    SET @ThanhToan2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ThanhToan] ([MaHoaDon], [MaNhanVien], [SoTien], [ThoiGianThanhToan], [MaGiaoDich], [TrangThai], [GhiChu])
    VALUES (@HoaDon3, @NhanVien5, 3910000, N'2026-09-12T12:05:00', N'MAU_TT_202609_003', N'ThanhCong', N'Thanh toán toàn bộ hóa đơn');
    SET @ThanhToan3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ThanhToan] ([MaHoaDon], [MaNhanVien], [SoTien], [ThoiGianThanhToan], [MaGiaoDich], [TrangThai], [GhiChu])
    VALUES (@HoaDon4, @NhanVien5, 4096000, N'2026-09-12T12:05:00', N'MAU_TT_202609_004', N'ThanhCong', N'Thanh toán toàn bộ hóa đơn');
    SET @ThanhToan4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ThanhToan] ([MaHoaDon], [MaNhanVien], [SoTien], [ThoiGianThanhToan], [MaGiaoDich], [TrangThai], [GhiChu])
    VALUES (@HoaDon5, @NhanVien5, 6045000, N'2026-09-12T12:05:00', N'MAU_TT_202609_005', N'ThanhCong', N'Thanh toán toàn bộ hóa đơn');
    SET @ThanhToan5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 35. CongViecDonPhong: thêm 5 dòng
    INSERT INTO dbo.[CongViecDonPhong] ([MaPhong], [MaNhanVien], [NgayGiao], [LoaiCongViec], [TrangThai], [ThoiGianBatDau], [ThoiGianKetThuc], [GhiChu])
    VALUES (@Phong1, @NhanVien3, N'20260912', N'DonPhong', N'HoanThanh', N'2026-09-12T13:00:00', N'2026-09-12T13:45:00', N'Dọn phòng sau khi khách trả phòng');
    SET @CongViecDonPhong1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[CongViecDonPhong] ([MaPhong], [MaNhanVien], [NgayGiao], [LoaiCongViec], [TrangThai], [ThoiGianBatDau], [ThoiGianKetThuc], [GhiChu])
    VALUES (@Phong2, @NhanVien3, N'20260912', N'DonPhong', N'HoanThanh', N'2026-09-12T14:00:00', N'2026-09-12T14:45:00', N'Dọn phòng sau khi khách trả phòng');
    SET @CongViecDonPhong2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[CongViecDonPhong] ([MaPhong], [MaNhanVien], [NgayGiao], [LoaiCongViec], [TrangThai], [ThoiGianBatDau], [ThoiGianKetThuc], [GhiChu])
    VALUES (@Phong3, @NhanVien3, N'20260912', N'DonPhong', N'HoanThanh', N'2026-09-12T15:00:00', N'2026-09-12T15:45:00', N'Dọn phòng sau khi khách trả phòng');
    SET @CongViecDonPhong3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[CongViecDonPhong] ([MaPhong], [MaNhanVien], [NgayGiao], [LoaiCongViec], [TrangThai], [ThoiGianBatDau], [ThoiGianKetThuc], [GhiChu])
    VALUES (@Phong4, @NhanVien3, N'20260912', N'DonPhong', N'HoanThanh', N'2026-09-12T16:00:00', N'2026-09-12T16:45:00', N'Dọn phòng sau khi khách trả phòng');
    SET @CongViecDonPhong4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[CongViecDonPhong] ([MaPhong], [MaNhanVien], [NgayGiao], [LoaiCongViec], [TrangThai], [ThoiGianBatDau], [ThoiGianKetThuc], [GhiChu])
    VALUES (@Phong5, @NhanVien3, N'20260912', N'DonPhong', N'HoanThanh', N'2026-09-12T17:00:00', N'2026-09-12T17:45:00', N'Dọn phòng sau khi khách trả phòng');
    SET @CongViecDonPhong5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 36. BaoTriPhong: thêm 5 dòng
    INSERT INTO dbo.[BaoTriPhong] ([MaPhong], [MaThietBi], [MaNhanVien], [MaNhaCungCap], [MaNguoiLienHe], [NgayYeuCau], [NoiDung], [MucDo], [TrangThai], [NgayHoanThanh], [ChiPhi], [GhiChu])
    VALUES (@Phong1, @ThietBi1, @NhanVien4, @NhaCungCap5, NULL, N'2026-09-14T08:00:00', N'Kiểm tra và vệ sinh tủ lạnh phòng M101', N'Thap', N'HoanThanh', N'2026-09-14T09:00:00', 100000, N'Thiết bị hoạt động bình thường');
    SET @BaoTriPhong1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[BaoTriPhong] ([MaPhong], [MaThietBi], [MaNhanVien], [MaNhaCungCap], [MaNguoiLienHe], [NgayYeuCau], [NoiDung], [MucDo], [TrangThai], [NgayHoanThanh], [ChiPhi], [GhiChu])
    VALUES (@Phong2, @ThietBi2, @NhanVien4, @NhaCungCap5, NULL, N'2026-09-14T08:00:00', N'Kiểm tra và vệ sinh tủ lạnh phòng M201', N'Thap', N'HoanThanh', N'2026-09-14T10:00:00', 100000, N'Thiết bị hoạt động bình thường');
    SET @BaoTriPhong2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[BaoTriPhong] ([MaPhong], [MaThietBi], [MaNhanVien], [MaNhaCungCap], [MaNguoiLienHe], [NgayYeuCau], [NoiDung], [MucDo], [TrangThai], [NgayHoanThanh], [ChiPhi], [GhiChu])
    VALUES (@Phong3, @ThietBi3, @NhanVien4, @NhaCungCap5, NULL, N'2026-09-14T08:00:00', N'Kiểm tra và vệ sinh tủ lạnh phòng M301', N'Thap', N'HoanThanh', N'2026-09-14T11:00:00', 100000, N'Thiết bị hoạt động bình thường');
    SET @BaoTriPhong3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[BaoTriPhong] ([MaPhong], [MaThietBi], [MaNhanVien], [MaNhaCungCap], [MaNguoiLienHe], [NgayYeuCau], [NoiDung], [MucDo], [TrangThai], [NgayHoanThanh], [ChiPhi], [GhiChu])
    VALUES (@Phong4, @ThietBi4, @NhanVien4, @NhaCungCap5, NULL, N'2026-09-14T08:00:00', N'Kiểm tra và vệ sinh tủ lạnh phòng M401', N'Thap', N'HoanThanh', N'2026-09-14T12:00:00', 100000, N'Thiết bị hoạt động bình thường');
    SET @BaoTriPhong4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[BaoTriPhong] ([MaPhong], [MaThietBi], [MaNhanVien], [MaNhaCungCap], [MaNguoiLienHe], [NgayYeuCau], [NoiDung], [MucDo], [TrangThai], [NgayHoanThanh], [ChiPhi], [GhiChu])
    VALUES (@Phong5, @ThietBi5, @NhanVien4, @NhaCungCap5, NULL, N'2026-09-14T08:00:00', N'Kiểm tra và vệ sinh tủ lạnh phòng M501', N'Thap', N'HoanThanh', N'2026-09-14T13:00:00', 100000, N'Thiết bị hoạt động bình thường');
    SET @BaoTriPhong5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 37. ChamCong: thêm 5 dòng
    INSERT INTO dbo.[ChamCong] ([MaLich], [MaNhanVien], [NgayChamCong], [GioVao], [GioRa], [SoGioLam], [SoGioTangCa], [TrangThai], [GhiChu])
    VALUES (@LichLamViec1, @NhanVien1, N'20260920', N'2026-09-20T08:00:00', N'2026-09-20T17:00:00', 8, 0, N'DuCong', N'Ca hành chính; nghỉ trưa một giờ');
    SET @ChamCong1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChamCong] ([MaLich], [MaNhanVien], [NgayChamCong], [GioVao], [GioRa], [SoGioLam], [SoGioTangCa], [TrangThai], [GhiChu])
    VALUES (@LichLamViec2, @NhanVien2, N'20260920', N'2026-09-20T08:00:00', N'2026-09-20T17:00:00', 8, 0, N'DuCong', N'Ca hành chính; nghỉ trưa một giờ');
    SET @ChamCong2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChamCong] ([MaLich], [MaNhanVien], [NgayChamCong], [GioVao], [GioRa], [SoGioLam], [SoGioTangCa], [TrangThai], [GhiChu])
    VALUES (@LichLamViec3, @NhanVien3, N'20260920', N'2026-09-20T08:00:00', N'2026-09-20T17:00:00', 8, 0, N'DuCong', N'Ca hành chính; nghỉ trưa một giờ');
    SET @ChamCong3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChamCong] ([MaLich], [MaNhanVien], [NgayChamCong], [GioVao], [GioRa], [SoGioLam], [SoGioTangCa], [TrangThai], [GhiChu])
    VALUES (@LichLamViec4, @NhanVien4, N'20260920', N'2026-09-20T08:00:00', N'2026-09-20T17:00:00', 8, 0, N'DuCong', N'Ca hành chính; nghỉ trưa một giờ');
    SET @ChamCong4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[ChamCong] ([MaLich], [MaNhanVien], [NgayChamCong], [GioVao], [GioRa], [SoGioLam], [SoGioTangCa], [TrangThai], [GhiChu])
    VALUES (@LichLamViec5, @NhanVien5, N'20260920', N'2026-09-20T08:00:00', N'2026-09-20T17:00:00', 8, 0, N'DuCong', N'Ca hành chính; nghỉ trưa một giờ');
    SET @ChamCong5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 38. LichLamViec: thêm 5 dòng
    SET IDENTITY_INSERT dbo.LichLamViec ON;
    SET @DangBatIdentityInsert = 1;
    INSERT INTO dbo.[LichLamViec] ([MaLich], [MaNhanVien], [MaCa], [NgayLam], [TrangThai], [MaChamCong], [GhiChu])
    VALUES (@LichLamViec1, @NhanVien1, @CaLamViec4, N'20260920', N'DaXep', @ChamCong1, N'Lịch mẫu khớp với bản ghi chấm công');
    INSERT INTO dbo.[LichLamViec] ([MaLich], [MaNhanVien], [MaCa], [NgayLam], [TrangThai], [MaChamCong], [GhiChu])
    VALUES (@LichLamViec2, @NhanVien2, @CaLamViec4, N'20260920', N'DaXep', @ChamCong2, N'Lịch mẫu khớp với bản ghi chấm công');
    INSERT INTO dbo.[LichLamViec] ([MaLich], [MaNhanVien], [MaCa], [NgayLam], [TrangThai], [MaChamCong], [GhiChu])
    VALUES (@LichLamViec3, @NhanVien3, @CaLamViec4, N'20260920', N'DaXep', @ChamCong3, N'Lịch mẫu khớp với bản ghi chấm công');
    INSERT INTO dbo.[LichLamViec] ([MaLich], [MaNhanVien], [MaCa], [NgayLam], [TrangThai], [MaChamCong], [GhiChu])
    VALUES (@LichLamViec4, @NhanVien4, @CaLamViec4, N'20260920', N'DaXep', @ChamCong4, N'Lịch mẫu khớp với bản ghi chấm công');
    INSERT INTO dbo.[LichLamViec] ([MaLich], [MaNhanVien], [MaCa], [NgayLam], [TrangThai], [MaChamCong], [GhiChu])
    VALUES (@LichLamViec5, @NhanVien5, @CaLamViec4, N'20260920', N'DaXep', @ChamCong5, N'Lịch mẫu khớp với bản ghi chấm công');
    SET IDENTITY_INSERT dbo.LichLamViec OFF;
    SET @DangBatIdentityInsert = 0;

    -- 39. BangLuong: thêm 5 dòng
    INSERT INTO dbo.[BangLuong] ([MaNhanVien], [Thang], [Nam], [LuongCoBan], [TienCong], [TienTangCa], [TienThuong], [TienPhuCap], [TienKhauTru], [Thue], [TongLuong], [ThucLinh], [NgayLap], [TrangThai], [GhiChu])
    VALUES (@NhanVien1, 9, 2026, 20000000, 20000000, 0, 500000, 700000, 0, 0, 21200000, 21200000, N'2026-09-30T16:00:00', N'TamTinh', N'Mẫu tạm tính: TienCong là lương thực tế; TongLuong = TienCong + TienTangCa + TienThuong + TienPhuCap. Thue chưa tính.');
    SET @BangLuong1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[BangLuong] ([MaNhanVien], [Thang], [Nam], [LuongCoBan], [TienCong], [TienTangCa], [TienThuong], [TienPhuCap], [TienKhauTru], [Thue], [TongLuong], [ThucLinh], [NgayLap], [TrangThai], [GhiChu])
    VALUES (@NhanVien2, 9, 2026, 9000000, 9000000, 0, 500000, 700000, 0, 0, 10200000, 10200000, N'2026-09-30T16:00:00', N'TamTinh', N'Mẫu tạm tính: TienCong là lương thực tế; TongLuong = TienCong + TienTangCa + TienThuong + TienPhuCap. Thue chưa tính.');
    SET @BangLuong2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[BangLuong] ([MaNhanVien], [Thang], [Nam], [LuongCoBan], [TienCong], [TienTangCa], [TienThuong], [TienPhuCap], [TienKhauTru], [Thue], [TongLuong], [ThucLinh], [NgayLap], [TrangThai], [GhiChu])
    VALUES (@NhanVien3, 9, 2026, 8000000, 8000000, 0, 500000, 700000, 0, 0, 9200000, 9200000, N'2026-09-30T16:00:00', N'TamTinh', N'Mẫu tạm tính: TienCong là lương thực tế; TongLuong = TienCong + TienTangCa + TienThuong + TienPhuCap. Thue chưa tính.');
    SET @BangLuong3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[BangLuong] ([MaNhanVien], [Thang], [Nam], [LuongCoBan], [TienCong], [TienTangCa], [TienThuong], [TienPhuCap], [TienKhauTru], [Thue], [TongLuong], [ThucLinh], [NgayLap], [TrangThai], [GhiChu])
    VALUES (@NhanVien4, 9, 2026, 11000000, 11000000, 0, 500000, 700000, 0, 0, 12200000, 12200000, N'2026-09-30T16:00:00', N'TamTinh', N'Mẫu tạm tính: TienCong là lương thực tế; TongLuong = TienCong + TienTangCa + TienThuong + TienPhuCap. Thue chưa tính.');
    SET @BangLuong4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[BangLuong] ([MaNhanVien], [Thang], [Nam], [LuongCoBan], [TienCong], [TienTangCa], [TienThuong], [TienPhuCap], [TienKhauTru], [Thue], [TongLuong], [ThucLinh], [NgayLap], [TrangThai], [GhiChu])
    VALUES (@NhanVien5, 9, 2026, 12000000, 12000000, 0, 500000, 700000, 0, 0, 13200000, 13200000, N'2026-09-30T16:00:00', N'TamTinh', N'Mẫu tạm tính: TienCong là lương thực tế; TongLuong = TienCong + TienTangCa + TienThuong + TienPhuCap. Thue chưa tính.');
    SET @BangLuong5 = CONVERT(bigint, SCOPE_IDENTITY());

    -- 40. NhatKyThaoTac: thêm 5 dòng
    INSERT INTO dbo.[NhatKyThaoTac] ([MaTaiKhoan], [HanhDong], [DoiTuong], [MaBanGhi], [ThoiGian], [MoTa])
    VALUES (@TaiKhoan1, N'TaoDuLieuMau', N'NhanVien', @NhanVien1, N'2026-09-01T08:30:00', N'Nhật ký giả lập phục vụ kiểm thử giao diện');
    SET @NhatKyThaoTac1 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhatKyThaoTac] ([MaTaiKhoan], [HanhDong], [DoiTuong], [MaBanGhi], [ThoiGian], [MoTa])
    VALUES (@TaiKhoan2, N'TaoDuLieuMau', N'NhanVien', @NhanVien2, N'2026-09-01T08:30:00', N'Nhật ký giả lập phục vụ kiểm thử giao diện');
    SET @NhatKyThaoTac2 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhatKyThaoTac] ([MaTaiKhoan], [HanhDong], [DoiTuong], [MaBanGhi], [ThoiGian], [MoTa])
    VALUES (@TaiKhoan3, N'TaoDuLieuMau', N'NhanVien', @NhanVien3, N'2026-09-01T08:30:00', N'Nhật ký giả lập phục vụ kiểm thử giao diện');
    SET @NhatKyThaoTac3 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhatKyThaoTac] ([MaTaiKhoan], [HanhDong], [DoiTuong], [MaBanGhi], [ThoiGian], [MoTa])
    VALUES (@TaiKhoan4, N'TaoDuLieuMau', N'NhanVien', @NhanVien4, N'2026-09-01T08:30:00', N'Nhật ký giả lập phục vụ kiểm thử giao diện');
    SET @NhatKyThaoTac4 = CONVERT(bigint, SCOPE_IDENTITY());
    INSERT INTO dbo.[NhatKyThaoTac] ([MaTaiKhoan], [HanhDong], [DoiTuong], [MaBanGhi], [ThoiGian], [MoTa])
    VALUES (@TaiKhoan5, N'TaoDuLieuMau', N'NhanVien', @NhanVien5, N'2026-09-01T08:30:00', N'Nhật ký giả lập phục vụ kiểm thử giao diện');
    SET @NhatKyThaoTac5 = CONVERT(bigint, SCOPE_IDENTITY());

    COMMIT TRANSACTION;
    PRINT N'Đã thêm thành công bộ dữ liệu mẫu vào 40 bảng.';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    IF @DangBatIdentityInsert = 1 SET IDENTITY_INSERT dbo.LichLamViec OFF;
    THROW;
END CATCH;

-- Kết quả: số dòng đã thêm của bộ mẫu và tổng số dòng hiện có ở mỗi bảng.

SELECT N'BangLuong' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[BangLuong]
UNION ALL
SELECT N'BaoTriPhong' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[BaoTriPhong]
UNION ALL
SELECT N'CaLamViec' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[CaLamViec]
UNION ALL
SELECT N'ChamCong' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[ChamCong]
UNION ALL
SELECT N'ChiTietDatPhong' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[ChiTietDatPhong]
UNION ALL
SELECT N'ChiTietGoiDichVu' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[ChiTietGoiDichVu]
UNION ALL
SELECT N'ChiTietHoaDon' AS TenBang, 10 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[ChiTietHoaDon]
UNION ALL
SELECT N'ChucVu' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[ChucVu]
UNION ALL
SELECT N'CongViecDonPhong' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[CongViecDonPhong]
UNION ALL
SELECT N'DatPhong' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[DatPhong]
UNION ALL
SELECT N'DichVu' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[DichVu]
UNION ALL
SELECT N'GoiDichVu' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[GoiDichVu]
UNION ALL
SELECT N'HoaDon' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[HoaDon]
UNION ALL
SELECT N'KhachHang' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[KhachHang]
UNION ALL
SELECT N'Kho' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[Kho]
UNION ALL
SELECT N'KhuVuc' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[KhuVuc]
UNION ALL
SELECT N'LichLamViec' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[LichLamViec]
UNION ALL
SELECT N'LoaiDichVu' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[LoaiDichVu]
UNION ALL
SELECT N'LoaiPhong' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[LoaiPhong]
UNION ALL
SELECT N'LoaiPhong_TienNghi' AS TenBang, 25 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[LoaiPhong_TienNghi]
UNION ALL
SELECT N'NhaCungCap' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[NhaCungCap]
UNION ALL
SELECT N'NhanVien' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[NhanVien]
UNION ALL
SELECT N'NhatKyThaoTac' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[NhatKyThaoTac]
UNION ALL
SELECT N'NhomVatTu' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[NhomVatTu]
UNION ALL
SELECT N'Phong' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[Phong]
UNION ALL
SELECT N'PhongBan' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[PhongBan]
UNION ALL
SELECT N'Quyen' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[Quyen]
UNION ALL
SELECT N'SuDungDichVu' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[SuDungDichVu]
UNION ALL
SELECT N'TaiKhoan' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[TaiKhoan]
UNION ALL
SELECT N'Tang' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[Tang]
UNION ALL
SELECT N'ThanhToan' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[ThanhToan]
UNION ALL
SELECT N'ThietBi' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[ThietBi]
UNION ALL
SELECT N'TienNghi' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[TienNghi]
UNION ALL
SELECT N'TieuHaoVatTu' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[TieuHaoVatTu]
UNION ALL
SELECT N'UuDai' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[UuDai]
UNION ALL
SELECT N'VaiTro' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[VaiTro]
UNION ALL
SELECT N'VaiTro_Quyen' AS TenBang, 9 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[VaiTro_Quyen]
UNION ALL
SELECT N'VatTu' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[VatTu]
UNION ALL
SELECT N'VatTu_NhaCungCap' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[VatTu_NhaCungCap]
UNION ALL
SELECT N'XepPhong' AS TenBang, 5 AS SoDongThem, COUNT_BIG(*) AS TongSoDong FROM dbo.[XepPhong]
ORDER BY TenBang;
GO
