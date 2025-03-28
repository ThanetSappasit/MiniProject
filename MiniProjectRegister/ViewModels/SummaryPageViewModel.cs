using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Model;

namespace MiniProjectRegister.ViewModels;

public partial class SummaryPageViewModel : ObservableObject
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
	public SummaryPageViewModel(string email = "")
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

        if (userRegister.Terms == null || !userRegister.Terms.Any())
        {
            Debug.WriteLine("  No terms found for the current user.");
            return;
        }

        var groupedCourses = new Dictionary<string, List<Course>>();
        foreach (var term in userRegister.Terms)
        {
            // Use Distinct with a custom comparer to remove duplicate courses
            var termCourses = Courses
                .Where(course => term.Courses.Contains(course.Courseid))
                .Distinct(new CourseComparer())
                .ToList();

            groupedCourses[term.TermTerm] = termCourses;
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

        // Log the final TermCoursesList for debugging
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

    private class CourseComparer : IEqualityComparer<Course>
    {
        public bool Equals(Course x, Course y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;

            return x.Courseid == y.Courseid &&
                   x.Coursename == y.Coursename &&
                   x.Instructor == y.Instructor &&
                   x.Credits == y.Credits &&
                   x.Year == y.Year;
        }

        public int GetHashCode(Course obj)
        {
            if (obj is null) return 0;

            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + (obj.Courseid?.GetHashCode() ?? 0);
                hash = hash * 23 + (obj.Coursename?.GetHashCode() ?? 0);
                hash = hash * 23 + (obj.Instructor?.GetHashCode() ?? 0);
                hash = hash * 23 + obj.Credits.GetHashCode();
                hash = hash * 23 + obj.Year.GetHashCode();
                return hash;
            }
        }
    }
    
    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.Navigation.PopAsync();
    }
}