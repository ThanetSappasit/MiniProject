using MiniProjectRegister.ViewModels;

namespace MiniProjectRegister.Pages;

[QueryProperty(nameof(Email), "Email")]
public partial class RegisterPage : ContentPage
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
	public RegisterPage()
	{
		InitializeComponent();
	}
	private void UpdateBindingContext()
    {
        System.Diagnostics.Debug.WriteLine($"Setting BindingContext with Email: {Email}");
		BindingContext = new RegisterPageViewModel(Email);
    }
}