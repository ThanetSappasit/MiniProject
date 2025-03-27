using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using MiniProjectRegister.ViewModels;
using System.Diagnostics;

namespace MiniProjectRegister.Pages;

// ใช้ QueryProperty กับ Email
[QueryProperty(nameof(Email), "email")]
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
        BindingContext = new HomePageViewModel(); // ตรวจสอบให้แน่ใจว่า ViewModel ถูกตั้งค่า
    }

    private void UpdateBindingContext()
    {
        if (BindingContext is HomePageViewModel viewModel)
        {
            Debug.WriteLine($"กำลังอัปเดต Email ใน ViewModel: {Email}");
            viewModel.Email = Email;
        }
    }
}