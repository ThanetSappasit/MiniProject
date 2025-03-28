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
    private List<Course> _allCourses = new();
    // รายการ Term และ Year ที่มีให้เลือก
    [ObservableProperty]
    private ObservableCollection<string> availableTerms = new();

    [ObservableProperty]
    private ObservableCollection<string> availableYears = new();
    // Term และ Year ที่เลือก
    [ObservableProperty]
    private string selectedTerm;
    [ObservableProperty]
    private string selectedYear;
    [ObservableProperty]
    private string searchResultText;
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
            var users = await ReadUserJsonAsync();
            CurrentEmail = users.FirstOrDefault(u => u.Email == Email);
            
            var allCourses = await ReadCourseJsonAsync();
            _allCourses = allCourses;
            Courses = new ObservableCollection<Course>(allCourses);
            // โหลด Term และ Year ที่ไม่ซ้ำจาก allCourses
            AvailableTerms = new ObservableCollection<string>(allCourses.Select(c => c.Term).Distinct().OrderBy(t => t).Select(t => t.ToString()));
            AvailableYears = new ObservableCollection<string>(allCourses.Select(c => c.Year).Distinct().OrderBy(y => y).Select(y => y.ToString()));

            // ตั้งค่าเริ่มต้น
            SelectedTerm = "0";
            SelectedYear = "0";
            // SelectedTerm = AvailableTerms.FirstOrDefault();
            // SelectedYear = AvailableYears.FirstOrDefault();

            var allRegisters = await ReadRegisterJsonAsync();
            Registers = new ObservableCollection<Register>(allRegisters);

            // UpdateFilteredCourses();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"InitializeData Error: {ex.Message}");
        }
    }
    // อัปเดต Courses เมื่อ Term หรือ Year เปลี่ยน
    // partial void OnSelectedTermChanged(string value)
    // {
    //     UpdateFilteredCourses();
    // }

    // partial void OnSelectedYearChanged(string value)
    // {
    //     UpdateFilteredCourses();
    // }

    // private void UpdateFilteredCourses()
    // {
    //     var filtered = _allCourses.AsEnumerable();

    //     if (!string.IsNullOrEmpty(SelectedTerm))
    //         filtered = filtered.Where(c => c.Term.ToString() == SelectedTerm);

    //     if (!string.IsNullOrEmpty(SelectedYear))
    //         filtered = filtered.Where(c => c.Year.ToString() == SelectedYear);

    //     Courses.Clear();
    //     foreach (var course in filtered)
    //     {
    //         Courses.Add(course);
    //     }

    //     SearchResultText = $"ผลการค้นหา: {Courses.Count} รายการ";
    // }

    [RelayCommand]
    private void AddCourse(Course item)
    {
        if (item == null) return;

        if (SelectedCourses.Any(c => c.Courseid == item.Courseid && c.Term == item.Term))
        {
            Debug.WriteLine($"Course already selected: {item.Courseid} - {item.Coursename}");
            return;
        }

        SelectedCourses.Add(item);
        TotalCredit += item.Credits;
    }

    [RelayCommand]
    private void RemoveCourse(Course item)
    {
        if (item == null) return;

        if (!SelectedCourses.Contains(item))
        {
            Debug.WriteLine($"Course not found in selected courses: {item.Courseid} - {item.Coursename}");
            return;
        }

        SelectedCourses.Remove(item);
        TotalCredit -= item.Credits;
        Debug.WriteLine($"Course removed: {item.Courseid} - {item.Coursename}. New Total Credits: {TotalCredit}");
    }

    [RelayCommand]
    private async Task Confirm()
    {
        var confirm = await Application.Current.MainPage.DisplayAlert(
            "Confirm Registration",
            "Are you sure you want to confirm your registration?",
            "Yes",
            "No"
        );

        if (!confirm)
        {
            await Shell.Current.Navigation.PopAsync();
            return;
        }

        try
        {
            // Add your confirmation logic here, e.g., save data or proceed with registration
            Debug.WriteLine("Registration confirmed by user.");

            // Example: Navigate to a success page or pop the current page
            await Shell.Current.Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during confirmation: {ex.Message}");
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                $"An error occurred: {ex.Message}",
                "OK"
            );
        }
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

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.Navigation.PopAsync();
    }

}