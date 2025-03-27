using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using MauiApp1.Model;

namespace MiniProjectRegister.ViewModels;

public partial class HistoryPageViewModel : ObservableObject
{
	[ObservableProperty]
    private string email;
	[ObservableProperty]
    private User currentEmail;
	public HistoryPageViewModel(string email = "")
	{
		Email = email;
		LoadDataAsync();
	}
	async Task LoadDataAsync()
    {
        Debug.WriteLine("===== เริ่มโหลดข้อมูล =====");
		
        var users = await ReadUserJsonAsync();
        CurrentEmail = users.FirstOrDefault(u => u.Email == Email);
        Debug.WriteLine(CurrentEmail != null 
            ? $"พบผู้ใช้ที่มีอีเมล {Email}" 
            : $"ไม่พบผู้ใช้ที่มีอีเมล {Email}");

        Debug.WriteLine("===== โหลดข้อมูลเสร็จสิ้น =====");
    }
	async Task<List<User>> ReadUserJsonAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("user.json");
            using var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();
            var users = User.FromJson(contents);
            Debug.WriteLine($"[ReadUserJsonAsync] อ่านข้อมูลสำเร็จ: {users.Count} ผู้ใช้");
            return users;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ReadUserJsonAsync] Error: {ex.Message}");
            return new List<User>();
        }
    }
}