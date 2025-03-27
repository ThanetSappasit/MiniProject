using CommunityToolkit.Mvvm.ComponentModel;

namespace MiniProjectRegister.ViewModels;

public partial class RemovePageViewModel : ObservableObject
{
	[ObservableProperty]
    private string email;
	public RemovePageViewModel(string email = "")
	{
		Email = email;
	}
}