-- Chọn đúng database hiện tại trong SSMS. Chỉ cập nhật các proc, không xóa dữ liệu.
GO
CREATE OR ALTER PROCEDURE dbo.sp_HienThiDatPhong
AS
BEGIN
    SET NOCOUNT ON;

    SELECT dp.MaDatPhong,
           kh.HoTen AS HoTen,
           MIN(ct.NgayNhanDuKien) AS NgayNhanDuKien,
           MAX(ct.NgayTraDuKien) AS NgayTraDuKien,
           STRING_AGG(p.SoPhong, N', ') AS SoPhong,
           CONCAT(
               ISNULL(SUM(ct.NguoiLon), 0), N' lớn + ',
               ISNULL(SUM(ct.TreEm), 0), N' trẻ'
           ) AS SoKhach,
           dp.TrangThai
    FROM dbo.DatPhong dp
    LEFT JOIN dbo.KhachHang kh ON dp.MaKhachHangDat = kh.MaKhachHang
    LEFT JOIN dbo.ChiTietDatPhong ct ON dp.MaDatPhong = ct.MaDatPhong
    LEFT JOIN
    (
        SELECT xp.MaChiTietDatPhong,
               STRING_AGG(CAST(ph.SoPhong AS nvarchar(max)), N', ') AS SoPhong
        FROM dbo.XepPhong xp
        JOIN dbo.Phong ph ON xp.MaPhong = ph.MaPhong
        WHERE xp.TrangThai <> 'Huy'
        GROUP BY xp.MaChiTietDatPhong
    ) p ON ct.MaChiTietDatPhong = p.MaChiTietDatPhong
    GROUP BY dp.MaDatPhong, kh.HoTen, dp.TrangThai
    ORDER BY dp.MaDatPhong DESC;
END;
GO

-- 2. Phân bổ phòng. Số khách thuộc chi tiết đặt phòng, có thể lặp giữa các phòng.
CREATE OR ALTER PROCEDURE dbo.sp_HienThiPhanBoPhong
AS
BEGIN
    SET NOCOUNT ON;

    SELECT xp.MaXepPhong, xp.MaChiTietDatPhong, xp.MaPhong, ct.MaDatPhong,
           p.SoPhong,
           lp.TenLoaiPhong,
           ct.NguoiLon,
           ct.TreEm,
           ct.DonGiaDuKien,
           xp.TrangThai
    FROM dbo.XepPhong xp
    JOIN dbo.ChiTietDatPhong ct ON xp.MaChiTietDatPhong = ct.MaChiTietDatPhong
    JOIN dbo.DatPhong dp ON ct.MaDatPhong = dp.MaDatPhong
    JOIN dbo.Phong p ON xp.MaPhong = p.MaPhong
    JOIN dbo.LoaiPhong lp ON p.MaLoaiPhong = lp.MaLoaiPhong
    WHERE dp.TrangThai <> 'DaHuy' AND xp.TrangThai <> 'Huy'
    ORDER BY xp.MaXepPhong DESC;
END;
GO

-- 3. Khách theo phòng: chỉ lượt đang ở. Họ tên/giấy tờ là của người đặt phòng.
CREATE OR ALTER PROCEDURE dbo.sp_HienThiKhachLuuTru
AS
BEGIN
    SET NOCOUNT ON;

    SELECT xp.MaXepPhong, xp.MaChiTietDatPhong, xp.MaPhong,
           ct.MaDatPhong,
           p.SoPhong,
           kh.HoTen,
           kh.LoaiGiayTo,
           kh.SoGiayTo,
           xp.NgayNhanThucTe,
           xp.NgayTraThucTe
    FROM dbo.XepPhong xp
    JOIN dbo.ChiTietDatPhong ct ON xp.MaChiTietDatPhong = ct.MaChiTietDatPhong
    JOIN dbo.DatPhong dp ON ct.MaDatPhong = dp.MaDatPhong
    JOIN dbo.KhachHang kh ON dp.MaKhachHangDat = kh.MaKhachHang
    JOIN dbo.Phong p ON xp.MaPhong = p.MaPhong
    WHERE xp.TrangThai = 'DaCheckIn'
    ORDER BY xp.NgayNhanThucTe DESC, xp.MaXepPhong DESC;
END;
GO

-- 4. Lịch sử xếp phòng: mọi trạng thái, ngày thực tế có thể NULL.
CREATE OR ALTER PROCEDURE dbo.sp_HienThiLichSuLuuTru
AS
BEGIN
    SET NOCOUNT ON;

    SELECT xp.MaXepPhong, xp.MaChiTietDatPhong, xp.MaPhong, xp.TrangThai, kh.HoTen,
           ct.MaDatPhong,
           p.SoPhong,
           xp.NgayNhanThucTe,
           xp.NgayTraThucTe
    FROM dbo.XepPhong xp
    JOIN dbo.ChiTietDatPhong ct ON xp.MaChiTietDatPhong = ct.MaChiTietDatPhong
    JOIN dbo.DatPhong dp ON ct.MaDatPhong = dp.MaDatPhong
    JOIN dbo.KhachHang kh ON dp.MaKhachHangDat = kh.MaKhachHang
    JOIN dbo.Phong p ON xp.MaPhong = p.MaPhong
    ORDER BY xp.MaXepPhong DESC;
END;
GO


-- 5. Hóa đơn: chỉ cộng phiếu thu ThanhCong. Hóa đơn hủy hoặc đã thu đủ có ConThieu = 0.
CREATE OR ALTER PROCEDURE dbo.sp_HienThiHoaDon
AS
BEGIN
    SET NOCOUNT ON;

    SELECT hd.MaHoaDon,
           hd.MaDatPhong,
           kh.HoTen,
           hd.TongTien,
           hd.TienGiam,
           hd.ThanhTien,
           ISNULL(tt.DaThu, 0) AS DaThu,
           CASE
               WHEN hd.TrangThai = 'Huy' THEN 0
               WHEN hd.ThanhTien > ISNULL(tt.DaThu, 0)
                   THEN hd.ThanhTien - ISNULL(tt.DaThu, 0)
               ELSE 0
           END AS ConThieu,
           hd.TrangThai
    FROM dbo.HoaDon hd
    JOIN dbo.KhachHang kh ON hd.MaKhachHang = kh.MaKhachHang
    LEFT JOIN
    (
        SELECT MaHoaDon, SUM(SoTien) AS DaThu
        FROM dbo.ThanhToan
        WHERE TrangThai = 'ThanhCong'
        GROUP BY MaHoaDon
    ) tt ON hd.MaHoaDon = tt.MaHoaDon
    ORDER BY hd.NgayLap DESC, hd.MaHoaDon DESC;
END;
GO

-- 6. Lịch sử thu tiền: mọi trạng thái; HoTen là nhân viên thu tiền.
CREATE OR ALTER PROCEDURE dbo.sp_HienThiLichSuThuTien
AS
BEGIN
    SET NOCOUNT ON;

    SELECT tt.MaThanhToan,
           tt.MaHoaDon, tt.MaNhanVien, tt.TrangThai,
           tt.ThoiGianThanhToan,
           tt.SoTien,
           nv.HoTen,
           tt.GhiChu
    FROM dbo.ThanhToan tt
    LEFT JOIN dbo.NhanVien nv ON tt.MaNhanVien = nv.MaNhanVien
    ORDER BY tt.ThoiGianThanhToan DESC, tt.MaThanhToan DESC;
END;
GO

-- Đơn chưa hủy và chưa từng lập hóa đơn: đối chiếu DatPhong với HoaDon.
CREATE OR ALTER PROCEDURE dbo.sp_DatPhongChuaLapHoaDon
AS
BEGIN
    SET NOCOUNT ON;
    SELECT dp.*
    FROM dbo.DatPhong dp
    WHERE dp.TrangThai <> 'DaHuy'
      AND NOT EXISTS (SELECT 1 FROM dbo.HoaDon hd WHERE hd.MaDatPhong = dp.MaDatPhong)
    ORDER BY dp.MaDatPhong DESC;
END;
GO

-- Khoản thu: tiền phòng và các dịch vụ đã sử dụng của đơn đặt phòng.
CREATE OR ALTER PROCEDURE dbo.sp_KhoanThuDatPhong @MaDatPhong bigint
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CAST('TienPhong' AS varchar(20)) AS LoaiChiPhi,
           ct.MaChiTietDatPhong, CAST(NULL AS bigint) AS MaSuDungDichVu,
           CAST(NULL AS int) AS MaGoiDichVu,
           CAST(CONCAT(lp.TenLoaiPhong, N' — ', ct.SoLuongPhong, N' phòng / ',
                DATEDIFF(day, ct.NgayNhanDuKien, ct.NgayTraDuKien), N' đêm') AS nvarchar(300)) AS NoiDung,
           CAST(ct.SoLuongPhong * CASE WHEN DATEDIFF(day, ct.NgayNhanDuKien, ct.NgayTraDuKien) < 1
                     THEN 1 ELSE DATEDIFF(day, ct.NgayNhanDuKien, ct.NgayTraDuKien) END AS decimal(12,2)) AS SoLuong,
           ct.DonGiaDuKien AS DonGia
    FROM dbo.ChiTietDatPhong ct JOIN dbo.LoaiPhong lp ON ct.MaLoaiPhong = lp.MaLoaiPhong
    WHERE ct.MaDatPhong = @MaDatPhong
    UNION ALL
    SELECT CASE WHEN sd.MaGoiDichVu IS NULL THEN 'DichVu' ELSE 'GoiDichVu' END,
           NULL, sd.MaSuDungDichVu, sd.MaGoiDichVu,
           CAST(COALESCE(dv.TenDichVu, gd.TenGoiDichVu) AS nvarchar(300)), sd.SoLuong, sd.DonGia
    FROM dbo.SuDungDichVu sd
    LEFT JOIN dbo.DichVu dv ON sd.MaDichVu = dv.MaDichVu
    LEFT JOIN dbo.GoiDichVu gd ON sd.MaGoiDichVu = gd.MaGoiDichVu
    WHERE sd.MaDatPhong = @MaDatPhong AND sd.TrangThai = 'DaSuDung';
END;
GO
-- Các lệnh xem thử (bỏ dấu -- nếu cần):
-- EXEC dbo.sp_HienThiDatPhong;
-- EXEC dbo.sp_HienThiPhanBoPhong;
-- EXEC dbo.sp_HienThiKhachLuuTru;
-- EXEC dbo.sp_HienThiLichSuLuuTru;
-- EXEC dbo.sp_HienThiHoaDon;
-- EXEC dbo.sp_HienThiLichSuThuTien;

CREATE OR ALTER PROCEDURE dbo.sp_ChiTietDatPhong @MaDatPhong bigint
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ct.*, lp.TenLoaiPhong
    FROM dbo.ChiTietDatPhong ct
    JOIN dbo.LoaiPhong lp ON ct.MaLoaiPhong = lp.MaLoaiPhong
    WHERE ct.MaDatPhong = @MaDatPhong
    ORDER BY ct.MaChiTietDatPhong;
END;
GO
