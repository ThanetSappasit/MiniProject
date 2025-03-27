using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using MauiApp1.Model;

namespace MiniProjectRegister.ViewModels;

public partial class SummaryPageViewModel : ObservableObject
{
	[ObservableProperty]
    private string email;
	[ObservableProperty]
    private User currentEmail;
	[ObservableProperty]
    private ObservableCollection<Course> courses;
	[ObservableProperty]
    private ObservableCollection<Register> registers;
	public SummaryPageViewModel(string email = "")
	{
		Email = email;
		LoadDataAsync();
	}
	async Task LoadDataAsync()
    {
        Debug.WriteLine("===== เริ่มโหลดข้อมูล =====");

        var allCourses = await ReadCourseJsonAsync();
        Courses = new ObservableCollection<Course>(allCourses);
        Debug.WriteLine($"โหลด Courses สำเร็จ! จำนวน: {Courses.Count}");

        var users = await ReadUserJsonAsync();
        CurrentEmail = users.FirstOrDefault(u => u.Email == Email);
        Debug.WriteLine(CurrentEmail != null 
            ? $"พบผู้ใช้ที่มีอีเมล {Email}" 
            : $"ไม่พบผู้ใช้ที่มีอีเมล {Email}");

        var allRegisters = await ReadRegisterJsonAsync();
        Registers = new ObservableCollection<Register>(allRegisters);
        Debug.WriteLine($"โหลด Registers สำเร็จ! จำนวน: {Registers.Count}");

        Debug.WriteLine("===== โหลดข้อมูลเสร็จสิ้น =====");
    }
	async Task<List<Course>> ReadCourseJsonAsync()
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

    async Task<List<User>> ReadUserJsonAsync()
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

    async Task<List<Register>> ReadRegisterJsonAsync()
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