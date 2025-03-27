using MiniProjectRegister.ViewModels;

namespace MiniProjectRegister.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginPageViewModel();
    }
	public class LoginData
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}
}