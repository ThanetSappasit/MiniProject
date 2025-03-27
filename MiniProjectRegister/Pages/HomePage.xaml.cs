using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using MiniProjectRegister.ViewModels;

namespace MiniProjectRegister.Pages;

// ใช้ QueryProperty กับ Email
[QueryProperty(nameof(Email), "Email")]
public partial class HomePage : ContentPage
{
    private string _email = string.Empty;
    public string Email 
    {
        get => _email;
        set 
        {
            // ตรวจสอบค่าที่เปลี่ยนแปลง
            if (_email != value)
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
                UpdateBindingContext();
            }
        }
    }

    public HomePage()
    {
        InitializeComponent();
    }

    private void UpdateBindingContext()
    {
        // บันทึกข้อมูล Email สำหรับตรวจสอบ
        System.Diagnostics.Debug.WriteLine($"กำลังตั้งค่า BindingContext ด้วย Email: {Email}");
        BindingContext = new HomePageViewModel(Email);
    }
}