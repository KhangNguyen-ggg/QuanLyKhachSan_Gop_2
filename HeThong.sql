CREATE OR ALTER PROCEDURE dbo.sp_HienThitaiKhoang
AS
BEGIN
    SET NOCOUNT ON;
    Select TK.MaTaiKhoan, NV.HoTen, TK.TenDangNhap ,Tk.LanDangNhapCuoi,TK.TrangThai,VT.TenVaiTro
    from TaiKhoan TK 
    join NhanVien NV on TK.MaNhanVien=NV.MaNhanVien
    join VaiTro VT on TK.MaVaiTro=VT.MaVaiTro
END;
GO
