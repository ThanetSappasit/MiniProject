using CommunityToolkit.Mvvm.ComponentModel;

namespace MiniProjectRegister.ViewModels;

public partial class SummaryPageViewModel : ObservableObject
{
	[ObservableProperty]
    private string email;
	public SummaryPageViewModel(string email = "")
	{
		Email = email;
	}
}