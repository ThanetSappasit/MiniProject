using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Model;
using MiniProjectRegister.Pages;
using System.Diagnostics;

namespace MiniProjectRegister.ViewModels;

public partial class HomePageViewModel : ObservableObject
{
    [ObservableProperty]
    private string email;
    [ObservableProperty]
    private User currentEmail;

    public HomePageViewModel(string email = "")
    {
        Email = email;
        LoadDataAsync();
    }
    async Task LoadDataAsync()
    {
        var users = await ReadUserJsonAsync();
        CurrentEmail = users.FirstOrDefault(u => u.Email == Email);
        if (CurrentEmail != null)
        {
            Debug.WriteLine($"[LoadDataAsync] CurrentEmail: {CurrentEmail.UserId}");
        }
        else
        {
            Debug.WriteLine("[LoadDataAsync] CurrentEmail is null");
        }
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

    // เพิ่ม Method สำหรับส่ง Email ไปยังหน้าอื่น
    [RelayCommand]
    async Task NavigateToHistoryPage()
    {
        // ตรวจสอบว่ามี Email หรือไม่
        if (!string.IsNullOrWhiteSpace(Email))
        {
            Debug.WriteLine($"กำลังส่ง Email ไปยังหน้าโปรไฟล์: {Email}");

            // ใช้ Shell Navigation เพื่อส่ง Email
            await Shell.Current.GoToAsync($"{nameof(HistoryPage)}?Email={Uri.EscapeDataString(Email)}");
        }
        else
        {
            // หากไม่มี Email ให้แสดงข้อความเตือน
            // ต้องใช้ Application.Current.MainPage เพื่อแสดง Alert
            await Application.Current.MainPage.DisplayAlert(
                "แจ้งเตือน",
                "ไม่พบอีเมล",
                "ตกลง"
            );
        }
    }
    [RelayCommand]
    async Task NavigateToSummaryPage()
    {
        // ตรวจสอบว่ามี Email หรือไม่
        if (!string.IsNullOrWhiteSpace(Email))
        {
            Debug.WriteLine($"กำลังส่ง Email ไปยังหน้าผลการลงทะเบียน: {Email}");

            // ใช้ Shell Navigation เพื่อส่ง Email
            await Shell.Current.GoToAsync($"{nameof(SummaryPage)}?Email={Uri.EscapeDataString(Email)}");
        }
        else
        {
            // หากไม่มี Email ให้แสดงข้อความเตือน
            // ต้องใช้ Application.Current.MainPage เพื่อแสดง Alert
            await Application.Current.MainPage.DisplayAlert(
                "แจ้งเตือน",
                "ไม่พบอีเมล",
                "ตกลง"
            );
        }
    }
    [RelayCommand]
    async Task NavigateToRegisterPage()
    {
        // ตรวจสอบว่ามี Email หรือไม่
        if (!string.IsNullOrWhiteSpace(Email))
        {
            Debug.WriteLine($"กำลังส่ง Email ไปยังหน้าลงทะเบียนเรียน {Email}");

            // ใช้ Shell Navigation เพื่อส่ง Email
            await Shell.Current.GoToAsync($"{nameof(RegisterPage)}?Email={Uri.EscapeDataString(Email)}");
        }
        else
        {
            // หากไม่มี Email ให้แสดงข้อความเตือน
            // ต้องใช้ Application.Current.MainPage เพื่อแสดง Alert
            await Application.Current.MainPage.DisplayAlert(
                "แจ้งเตือน",
                "ไม่พบอีเมล",
                "ตกลง"
            );
        }
    }
    [RelayCommand]
    async Task NavigateToRemovePage()
    {
        // ตรวจสอบว่ามี Email หรือไม่
        if (!string.IsNullOrWhiteSpace(Email))
        {
            Debug.WriteLine($"กำลังส่ง Email ไปยังหน้าถอนวิชาเรียน: {Email}");

            // ใช้ Shell Navigation เพื่อส่ง Email
            await Shell.Current.GoToAsync($"{nameof(RemovePage)}?Email={Uri.EscapeDataString(Email)}");
        }
        else
        {
            // หากไม่มี Email ให้แสดงข้อความเตือน
            // ต้องใช้ Application.Current.MainPage เพื่อแสดง Alert
            await Application.Current.MainPage.DisplayAlert(
                "แจ้งเตือน",
                "ไม่พบอีเมล",
                "ตกลง"
            );
        }
    }
    [RelayCommand]
    async Task Logout()
    {
        Debug.WriteLine($"ออกจากระบบ");
        await Shell.Current.Navigation.PopAsync();
    }

}