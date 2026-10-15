/*
    Nâng cấp trực tiếp DB hiện có, không tạo lại database và không xóa dữ liệu.
    Mở trong SSMS rồi Execute toàn bộ file. Đổi dòng USE nếu DB có tên khác.
    Có thể chạy lại. Mọi thay đổi cấu trúc được thực hiện trong một transaction.

    MaLich cũ được giữ nguyên; mã mới dùng DEFAULT + SEQUENCE để tránh dựng lại bảng.
    Cột LichLamViec.MaChamCong cũ chỉ giữ tương thích dữ liệu/ET, không dùng làm liên kết.
    Quan hệ thực tế: ChamCong.MaLich -> LichLamViec.MaLich.
    Khi thêm lịch: gọi usp_ThemLichLamViec hoặc INSERT bỏ qua MaLich để DEFAULT sinh mã.
    Không dùng SCOPE_IDENTITY() để lấy MaLich; dùng OUTPUT INSERTED.MaLich.

    KhenThuong lưu chi tiết. BangLuong.TienThuong vẫn là tổng thưởng đã chốt của kỳ lương;
    script không tự cộng tiền thưởng hoặc sửa các bảng lương cũ.
    Script chuẩn bị DB; các tab đang dùng dữ liệu mẫu vẫn cần nối DAL/BUS/giao diện.
*/
USE [QuanLyKhachSan5Sao2];

SET NOCOUNT ON;
SET XACT_ABORT ON;

IF @@TRANCOUNT <> 0
    THROW 51000, N'Hãy chạy script trong cửa sổ truy vấn không có transaction đang mở.', 1;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @Khoa int;
    EXEC @Khoa = sys.sp_getapplock
        @Resource = N'QLKS_CapNhat_NhanSu', @LockMode = N'Exclusive',
        @LockOwner = N'Transaction', @LockTimeout = 15000;
    IF @Khoa < 0
        THROW 51001, N'Có phiên khác đang nâng cấp Nhân sự. Hãy chạy lại sau.', 1;

    IF OBJECT_ID(N'dbo.PhongBan', N'U') IS NULL
        OR OBJECT_ID(N'dbo.ChucVu', N'U') IS NULL
        OR OBJECT_ID(N'dbo.NhanVien', N'U') IS NULL
        OR OBJECT_ID(N'dbo.CaLamViec', N'U') IS NULL
        OR OBJECT_ID(N'dbo.LichLamViec', N'U') IS NULL
        OR OBJECT_ID(N'dbo.ChamCong', N'U') IS NULL
        OR OBJECT_ID(N'dbo.BangLuong', N'U') IS NULL
        THROW 51002, N'DB thiếu các bảng Nhân sự gốc. Kiểm tra lại database ở dòng USE.', 1;

    DECLARE @MaLichLonNhat bigint, @SoChamCong bigint;
    SELECT @MaLichLonNhat = ISNULL(MAX(MaLich), 0)
    FROM dbo.LichLamViec WITH (TABLOCKX, HOLDLOCK);
    SELECT @SoChamCong = COUNT_BIG(*) FROM dbo.ChamCong WITH (TABLOCKX, HOLDLOCK);

    IF EXISTS (
        SELECT 1 FROM dbo.ChamCong c
        LEFT JOIN dbo.LichLamViec l ON l.MaLich = c.MaLich
        WHERE l.MaLich IS NULL
            OR c.MaNhanVien <> l.MaNhanVien OR c.NgayChamCong <> l.NgayLam
    )
        THROW 51003, N'Có chấm công thiếu lịch hoặc khác nhân viên/ngày của lịch. Chưa thay đổi DB; cần đối chiếu các dòng này trước.', 1;

    IF EXISTS (SELECT 1 FROM dbo.ChamCong
        WHERE GioRa IS NOT NULL AND (GioVao IS NULL OR GioRa < GioVao))
        THROW 51004, N'Có giờ ra trước giờ vào hoặc thiếu giờ vào. Chưa thay đổi DB; cần kiểm tra dữ liệu chấm công.', 1;

    -- Bỏ liên kết ngược, kể cả khi tên khóa ngoại đã được SQL Server tự sinh.
    DECLARE @Lenh nvarchar(max) = N'';
    SELECT @Lenh = @Lenh + N'ALTER TABLE dbo.LichLamViec DROP CONSTRAINT ' + QUOTENAME(f.name) + N';'
    FROM sys.foreign_keys f
    JOIN sys.foreign_key_columns k ON k.constraint_object_id = f.object_id
    JOIN sys.columns c ON c.object_id = k.parent_object_id AND c.column_id = k.parent_column_id
    WHERE f.parent_object_id = OBJECT_ID(N'dbo.LichLamViec')
        AND f.referenced_object_id = OBJECT_ID(N'dbo.ChamCong') AND c.name = N'MaChamCong';
    IF @Lenh <> N'' EXEC sys.sp_executesql @Lenh;

    IF COLUMNPROPERTY(OBJECT_ID(N'dbo.LichLamViec'), N'MaLich', 'IsIdentity') = 0
    BEGIN
        IF @MaLichLonNhat = 9223372036854775807
            THROW 51005, N'Mã lịch đã đạt giới hạn bigint.', 1;

        DECLARE @MaKeTiep bigint = @MaLichLonNhat + 1;
        IF OBJECT_ID(N'dbo.Seq_LichLamViec', N'SO') IS NULL
        BEGIN
            SET @Lenh = N'CREATE SEQUENCE dbo.Seq_LichLamViec AS bigint START WITH '
                + CONVERT(nvarchar(20), @MaKeTiep) + N' INCREMENT BY 1 NO CYCLE;';
            EXEC sys.sp_executesql @Lenh;
        END
        ELSE
        BEGIN
            IF EXISTS (SELECT 1 FROM sys.sequences
                WHERE object_id = OBJECT_ID(N'dbo.Seq_LichLamViec')
                    AND (CONVERT(bigint, increment) <> 1 OR is_cycling = 1))
                THROW 51006, N'Seq_LichLamViec phải tăng 1 và không quay vòng.', 1;

            IF EXISTS (SELECT 1 FROM sys.sequences
                WHERE object_id = OBJECT_ID(N'dbo.Seq_LichLamViec')
                    AND CONVERT(bigint, current_value) <= @MaLichLonNhat)
            BEGIN
                SET @Lenh = N'ALTER SEQUENCE dbo.Seq_LichLamViec RESTART WITH '
                    + CONVERT(nvarchar(20), @MaKeTiep) + N';';
                EXEC sys.sp_executesql @Lenh;
            END
        END

        IF NOT EXISTS (SELECT 1 FROM sys.default_constraints d
            JOIN sys.columns c ON c.object_id = d.parent_object_id AND c.column_id = d.parent_column_id
            WHERE d.parent_object_id = OBJECT_ID(N'dbo.LichLamViec') AND c.name = N'MaLich')
            EXEC(N'ALTER TABLE dbo.LichLamViec ADD CONSTRAINT DF_LichLamViec_MaLich
                DEFAULT (NEXT VALUE FOR dbo.Seq_LichLamViec) FOR MaLich;');
        ELSE IF EXISTS (SELECT 1 FROM sys.default_constraints d
            JOIN sys.columns c ON c.object_id = d.parent_object_id AND c.column_id = d.parent_column_id
            WHERE d.parent_object_id = OBJECT_ID(N'dbo.LichLamViec') AND c.name = N'MaLich'
                AND d.definition NOT LIKE N'%Seq_LichLamViec%')
            THROW 51007, N'MaLich đã có DEFAULT khác. Script không tự ghi đè cấu hình này.', 1;
    END

    -- Khóa ghép ngăn chấm công cho sai nhân viên hoặc sai ngày của lịch.
    IF NOT EXISTS (SELECT 1 FROM sys.indexes
        WHERE object_id = OBJECT_ID(N'dbo.LichLamViec') AND name = N'UX_LichLamViec_LichNhanVienNgay')
        CREATE UNIQUE INDEX UX_LichLamViec_LichNhanVienNgay
            ON dbo.LichLamViec(MaLich, MaNhanVien, NgayLam);

    IF OBJECT_ID(N'dbo.FK_ChamCong_LichLamViec', N'F') IS NULL
        ALTER TABLE dbo.ChamCong WITH CHECK ADD CONSTRAINT FK_ChamCong_LichLamViec
            FOREIGN KEY (MaLich, MaNhanVien, NgayChamCong)
            REFERENCES dbo.LichLamViec(MaLich, MaNhanVien, NgayLam);
    ALTER TABLE dbo.ChamCong WITH CHECK CHECK CONSTRAINT FK_ChamCong_LichLamViec;

    IF OBJECT_ID(N'dbo.CK_ChamCong_ThuTuGio', N'C') IS NULL
        ALTER TABLE dbo.ChamCong WITH CHECK ADD CONSTRAINT CK_ChamCong_ThuTuGio
            CHECK (GioRa IS NULL OR (GioVao IS NOT NULL AND GioRa >= GioVao));
    ALTER TABLE dbo.ChamCong WITH CHECK CHECK CONSTRAINT CK_ChamCong_ThuTuGio;

    IF OBJECT_ID(N'dbo.KhenThuong', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.KhenThuong (
            MaKhenThuong bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_KhenThuong PRIMARY KEY,
            MaNhanVien int NOT NULL,
            NgayKhenThuong date NOT NULL CONSTRAINT DF_KhenThuong_Ngay DEFAULT (CONVERT(date, SYSDATETIME())),
            LyDo nvarchar(500) NOT NULL,
            HinhThuc varchar(20) NOT NULL CONSTRAINT DF_KhenThuong_HinhThuc DEFAULT ('TienThuong'),
            SoTien decimal(18,2) NOT NULL CONSTRAINT DF_KhenThuong_SoTien DEFAULT (0),
            TrangThai varchar(20) NOT NULL CONSTRAINT DF_KhenThuong_TrangThai DEFAULT ('ChoDuyet'),
            GhiChu nvarchar(500) NULL,
            CONSTRAINT FK_KhenThuong_NhanVien FOREIGN KEY (MaNhanVien) REFERENCES dbo.NhanVien(MaNhanVien),
            CONSTRAINT CK_KhenThuong_LyDo CHECK (LEN(LTRIM(RTRIM(LyDo))) > 0),
            CONSTRAINT CK_KhenThuong_SoTien CHECK (SoTien >= 0),
            CONSTRAINT CK_KhenThuong_HinhThuc CHECK (HinhThuc IN ('TienThuong', 'BieuDuong', 'GiayKhen')),
            CONSTRAINT CK_KhenThuong_TrangThai CHECK (TrangThai IN ('ChoDuyet', 'DaDuyet', 'TuChoi'))
        );
        CREATE INDEX IX_KhenThuong_NhanVienNgay ON dbo.KhenThuong(MaNhanVien, NgayKhenThuong);
    END

    -- MaLich dùng SEQUENCE nên trả khóa bằng OUTPUT, không dùng SCOPE_IDENTITY.
    EXEC(N'CREATE OR ALTER PROCEDURE dbo.usp_ThemLichLamViec
        @MaNhanVien int, @MaCa int, @NgayLam date,
        @TrangThai varchar(20) = ''DaXep'', @GhiChu nvarchar(max) = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        IF @MaNhanVien IS NULL OR @MaNhanVien <= 0 OR @MaCa IS NULL OR @MaCa <= 0 OR @NgayLam IS NULL
            THROW 51100, N''Vui lòng chọn nhân viên, ca và ngày làm hợp lệ.'', 1;
        IF DATALENGTH(@GhiChu) > 600
            THROW 51101, N''Ghi chú chỉ được tối đa 300 ký tự.'', 1;
        INSERT INTO dbo.LichLamViec(MaNhanVien, MaCa, NgayLam, TrangThai, GhiChu)
        OUTPUT INSERTED.MaLich
        VALUES (@MaNhanVien, @MaCa, @NgayLam, @TrangThai, @GhiChu);
    END;');

    -- DAL_BangLuong.HienThiBangLuongAll hiện đã gọi đúng tên thủ tục này.
    EXEC(N'CREATE OR ALTER PROCEDURE dbo.usp_HienThiBangLuong
        @MaNhanVien int = NULL, @Thang int = NULL, @Nam int = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT b.*, n.HoTen AS NhanVien
        FROM dbo.BangLuong b JOIN dbo.NhanVien n ON n.MaNhanVien = b.MaNhanVien
        WHERE (@MaNhanVien IS NULL OR b.MaNhanVien = @MaNhanVien)
            AND (@Thang IS NULL OR b.Thang = @Thang) AND (@Nam IS NULL OR b.Nam = @Nam)
        ORDER BY b.Nam DESC, b.Thang DESC, b.MaNhanVien;
    END;');

    COMMIT TRANSACTION;
    PRINT N'Nâng cấp Nhân sự thành công. Đã giữ nguyên dữ liệu cũ; có thể chạy lại script.';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
