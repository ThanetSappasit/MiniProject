using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiniProjectRegister.Pages;

namespace MiniProjectRegister.ViewModels;

public partial class LoginPageViewModel : ObservableObject
{
    private string _email = "";
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    private string _password = "";
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    [RelayCommand]
    async Task Login()
    {
        // บันทึกข้อมูลสำหรับตรวจสอบ
        System.Diagnostics.Debug.WriteLine("กำลังเข้าสู่ระบบ");
        System.Diagnostics.Debug.WriteLine($"Email ที่ส่ง: {Email}");

        // ส่ง Email โดยใช้ Uri.EscapeDataString เพื่อหลีกเลี่ยงปัญหาอักขระพิเศษ
        await Shell.Current.GoToAsync($"{nameof(HomePage)}?Email={Uri.EscapeDataString(Email)}");
    }
}
