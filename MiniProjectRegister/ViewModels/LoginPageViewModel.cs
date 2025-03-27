using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Model;
using MiniProjectRegister.Pages;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MiniProjectRegister.ViewModels;

public partial class LoginPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string email = "";

    [ObservableProperty]
    private string password = "";

    [ObservableProperty]
    private ObservableCollection<User> users = new();

    public LoginPageViewModel()
    {
        LoadDataAsync();
    }

    [RelayCommand]
    private async Task Login()
    {
        // Check if email or password is empty
        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
        {
            await Shell.Current.DisplayAlert("Login Failed", 
                "Please enter both email and password.", 
                "OK");
            return;
        }

        // Check credentials against loaded users
        var user = Users.FirstOrDefault(u => u.Email == Email && u.Password == Password);
        
        if (user != null)
        {
            Debug.WriteLine("Login successful");
            // Navigate to HomePage with email parameter
            await Shell.Current.GoToAsync($"{nameof(HomePage)}?email={Email}");
        }
        else
        {
            Debug.WriteLine("Login failed");
            await ShowLoginFailedMessage();
        }
    }

    private async void LoadDataAsync()
    {
        try
        {
            var jsonUsers = await ReadJsonAsync();
            Users = new ObservableCollection<User>(jsonUsers);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading users: {ex.Message}");
        }
    }

    private async Task<List<User>> ReadJsonAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("user.json");
            using var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();
            return User.FromJson(contents) ?? new List<User>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error reading user.json: {ex.Message}");
            return new List<User>();
        }
    }

    private async Task ShowLoginFailedMessage()
    {
        await Shell.Current.DisplayAlert("Login Failed", 
            "Email or password is incorrect.", 
            "OK");
    }
}