using System;
using System.IO;


namespace DAL;

public static class CauHinhKetNoi
{
    public static bool DaCauHinh => Doc() != null;
    // Có thể sửa connection.txt cạnh file .exe hoặc đặt biến môi trường QLKS_CONNECTION.
    public static string Chuoi => Doc() ?? throw new InvalidOperationException("Cần cấu hình QLKS_CONNECTION hoặc connection.txt để dùng dữ liệu SQL Server.");

    private static string Doc()
    {
        var chuoi = Environment.GetEnvironmentVariable("QLKS_CONNECTION");
        if (!string.IsNullOrWhiteSpace(chuoi)) return chuoi.Trim();

        var file = Path.Combine(AppContext.BaseDirectory, "connection.txt");
        chuoi = File.Exists(file) ? File.ReadAllText(file).Trim() : null;
        return string.IsNullOrWhiteSpace(chuoi) ? null : chuoi;
    }

}
