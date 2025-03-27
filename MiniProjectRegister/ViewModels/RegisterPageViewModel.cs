using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MiniProjectRegister.ViewModels;

public partial class RegisterPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Course> courses = new();

    [ObservableProperty]
    private ObservableCollection<Course> selectedCourses = new();

    [ObservableProperty]
    private User currentEmail;

    [ObservableProperty]
    private ObservableCollection<Register> registers = new();

    [ObservableProperty]
    private long totalCredit;

    // Private field to store all courses
    private List<Course> _allCourses = new();

    // Public parameterless constructor
    public RegisterPageViewModel()
    {
        InitializeData();
    }

    // Constructor with email parameter
    public RegisterPageViewModel(string email)
    {
        Email = email;
        InitializeData();
    }

    private async void InitializeData()
    {
        try
        {
            Debug.WriteLine("===== เริ่มโหลดข้อมูล =====");

            var allCourses = await ReadCourseJsonAsync();
            _allCourses = allCourses;
            Courses = new ObservableCollection<Course>(allCourses);

            var users = await ReadUserJsonAsync();
            CurrentEmail = users.FirstOrDefault(u => u.Email == Email);

            var allRegisters = await ReadRegisterJsonAsync();
            Registers = new ObservableCollection<Register>(allRegisters);

            Debug.WriteLine("===== โหลดข้อมูลเสร็จสิ้น =====");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"InitializeData Error: {ex.Message}");
        }
    }

    [RelayCommand]
    private void AddCourse(Course item)
    {
        if (item == null) return;

        if (SelectedCourses.Any(c => c.Courseid == item.Courseid))
        {
            Debug.WriteLine($"Course already selected: {item.Courseid} - {item.Coursename}");
            return;
        }

        SelectedCourses.Add(item);
        TotalCredit += item.Credits;
    }

    [RelayCommand]
    private async Task Confirm()
    {
        // Add registration logic here
        await Shell.Current.Navigation.PopAsync();
    }

    // JSON reading methods
    private async Task<List<Course>> ReadCourseJsonAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("course.json");
            using var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();
            var courses = Course.FromJson(contents);
            Debug.WriteLine($"[ReadCourseJsonAsync] อ่านข้อมูลสำเร็จ: {courses.Count} รายวิชา");
            return courses;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ReadCourseJsonAsync] Error: {ex.Message}");
            return new List<Course>();
        }
    }

    private async Task<List<User>> ReadUserJsonAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("user.json");
            using var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();
            var users = User.FromJson(contents);
            Debug.WriteLine($"[ReadUserJsonAsync] อ่านข้อมูลสำเร็จ: {users.Count} ผู้ใช้");
            return users;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ReadUserJsonAsync] Error: {ex.Message}");
            return new List<User>();
        }
    }

    private async Task<List<Register>> ReadRegisterJsonAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("register.json");
            using var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();
            var registers = Register.FromJson(contents);
            Debug.WriteLine($"[ReadRegisterJsonAsync] อ่านข้อมูลสำเร็จ: {registers.Count} รายการลงทะเบียน");
            return registers;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ReadRegisterJsonAsync] Error: {ex.Message}");
            return new List<Register>();
        }
    }
}