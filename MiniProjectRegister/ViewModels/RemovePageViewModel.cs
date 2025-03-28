using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using MauiApp1.Model;

namespace MiniProjectRegister.ViewModels;

public class TermCourses
{
    public string TermName { get; set; }
    public ObservableCollection<Course> Courses { get; set; } = new ObservableCollection<Course>();
}

public partial class RemovePageViewModel : ObservableObject
{
    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private User currentEmail;

    [ObservableProperty]
    private ObservableCollection<Course> courses = new();

    [ObservableProperty]
    private ObservableCollection<Register> registers = new();

    [ObservableProperty]
    private ObservableCollection<TermCourses> termCoursesList = new();

    [ObservableProperty]
    private string fullName = string.Empty;

    public RemovePageViewModel(string email = "")
    {
        Email = email;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var allCourses = await ReadCourseJsonAsync();
            Courses = new ObservableCollection<Course>(allCourses);

            var users = await ReadUserJsonAsync();
            CurrentEmail = users.FirstOrDefault(u => u.Email == Email);


            var allRegisters = await ReadRegisterJsonAsync();
            Registers = new ObservableCollection<Register>(allRegisters);

            CombineCourses();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[LoadDataAsync] Error: {ex.Message}");
        }
    }

    private async Task<List<Course>> ReadCourseJsonAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("course.json");
            using var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();
            var courses = Course.FromJson(contents);
            return courses ?? new List<Course>();
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
            return users ?? new List<User>();
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
            return registers ?? new List<Register>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ReadRegisterJsonAsync] Error: {ex.Message}");
            return new List<Register>();
        }
    }

    private void CombineCourses()
    {
        // Clear existing TermCoursesList
        TermCoursesList.Clear();

        if (CurrentEmail == null)
        {
            Debug.WriteLine("\nCurrentEmail is null.");
            return;
        }

        var userRegister = Registers.FirstOrDefault(r => r.UserId == CurrentEmail.UserId);
        if (userRegister == null)
        {
            Debug.WriteLine("\nNo register found for the current user.");
            return;
        }

        Debug.WriteLine("\nUser Register:");
        Debug.WriteLine($"User ID: {userRegister.UserId}");

        if (userRegister.Terms == null || !userRegister.Terms.Any())
        {
            Debug.WriteLine("  No terms found for the current user.");
            return;
        }

        var groupedCourses = new Dictionary<string, List<Course>>();
        foreach (var term in userRegister.Terms)
        {
            // Debug.WriteLine($"  Term: {term.TermTerm}");
            // Debug.WriteLine($"  Course IDs: {string.Join(", ", term.Courses)}");

            var termCourses = Courses.Where(course => term.Courses.Contains(course.Courseid)).ToList();
            groupedCourses[term.TermTerm] = termCourses;
            if (termCourses.Any())
            {
                // Debug.WriteLine("  Course Details:");
                foreach (var course in termCourses)
                {
                    // Debug.WriteLine($"    Course ID: {course.Courseid}, Name: {course.Coursename}, Instructor: {course.Instructor}, Credits: {course.Credits}");
                }
            }
            else
            {
                // Debug.WriteLine("    No matching courses found in the Courses collection.");
            }
        }
        foreach (var group in groupedCourses)
        {
            var termCourses = new TermCourses
            {
                TermName = group.Key,
                Courses = new ObservableCollection<Course>(group.Value)
            };
            TermCoursesList.Add(termCourses);
        }

        // Optional: Log the final TermCoursesList for debugging
        Debug.WriteLine("\nFinal TermCoursesList:");
        foreach (var term in TermCoursesList)
        {
            Debug.WriteLine($"Term: {term.TermName}");
            foreach (var course in term.Courses)
            {
                Debug.WriteLine($"  Course ID: {course.Courseid}, Name: {course.Coursename}, Credits: {course.Credits}");
                Debug.WriteLine($"  Instructor: {course.Instructor}, Year: {course.Year}");
            }
        }
    }
}
