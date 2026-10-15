using System;
using System.IO;


namespace DAL_QuanlyKhachSan.TienIch;

public static class CauHinhKetNoi
{
    // Có thể sửa connection.txt cạnh file .exe hoặc đặt biến môi trường QLKS_CONNECTION.
    public static string Chuoi => Environment.GetEnvironmentVariable("QLKS_CONNECTION")
        ?? (File.Exists(Path.Combine(AppContext.BaseDirectory, "connection.txt"))
            ? File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "connection.txt")).Trim()
            : @"Data Source=DESKTOP-URMOUHG\PHUC;Initial Catalog=QuanLyKhachSan5Sao2;Integrated Security=True;Trust Server Certificate=True");

}
