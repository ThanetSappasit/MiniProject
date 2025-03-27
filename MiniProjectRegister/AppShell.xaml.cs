using MiniProjectRegister.Pages;

namespace MiniProjectRegister;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("HistoryPage", typeof(HistoryPage));
		Routing.RegisterRoute("HomePage", typeof(HomePage));
		Routing.RegisterRoute("LoginPage", typeof(LoginPage));
		Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
		Routing.RegisterRoute("RemovePage", typeof(RemovePage));
		Routing.RegisterRoute("SummaryPage", typeof(SummaryPage));
	}
}
