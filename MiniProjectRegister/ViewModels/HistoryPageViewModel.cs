using CommunityToolkit.Mvvm.ComponentModel;

namespace MiniProjectRegister.ViewModels;

public partial class HistoryPageViewModel : ObservableObject
{
	[ObservableProperty]
    private string email;
	public HistoryPageViewModel(string email = "")
	{
		Email = email;
	}
}