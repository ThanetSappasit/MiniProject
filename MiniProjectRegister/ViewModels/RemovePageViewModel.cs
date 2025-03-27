using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using MauiApp1.Model;

namespace MiniProjectRegister.ViewModels;

public partial class RemovePageViewModel : ObservableObject
{
	[ObservableProperty]
    private string email = string.Empty;
	[ObservableProperty]
    private User currentEmail;
	[ObservableProperty]
    private ObservableCollection<Course> courses = new ObservableCollection<Course>();
	[ObservableProperty]
    private ObservableCollection<Register> registers = new ObservableCollection<Register>();
	public RemovePageViewModel(string email = "")
	{
		Email = email;
		LoadDataAsync();
	}
	async Task LoadDataAsync()
    {
        var allCourses = await ReadCourseJsonAsync();
        Courses = new ObservableCollection<Course>(allCourses);

        var users = await ReadUserJsonAsync();
        CurrentEmail = users.FirstOrDefault(u => u.Email == Email);

        var allRegisters = await ReadRegisterJsonAsync();
        Registers = new ObservableCollection<Register>(allRegisters);
    }
	async Task<List<Course>> ReadCourseJsonAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("course.json");
            using var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();
            var courses = Course.FromJson(contents);
            return courses;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ReadCourseJsonAsync] Error: {ex.Message}");
            return new List<Course>();
        }
    }

    async Task<List<User>> ReadUserJsonAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("user.json");
            using var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();
            var users = User.FromJson(contents);
            return users;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ReadUserJsonAsync] Error: {ex.Message}");
            return new List<User>();
        }
    }

    async Task<List<Register>> ReadRegisterJsonAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("register.json");
            using var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();
            var registers = Register.FromJson(contents);
            return registers;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ReadRegisterJsonAsync] Error: {ex.Message}");
            return new List<Register>();
        }
    }
}