using CommunityToolkit.Mvvm.ComponentModel;

namespace MiniProjectRegister.ViewModels;

public partial class RegisterPageViewModel : ObservableObject
{
	[ObservableProperty]
    private string email;
	public RegisterPageViewModel(string email = "")
	{
		Email = email;
	}
}