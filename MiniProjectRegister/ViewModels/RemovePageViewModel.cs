using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Model;
using Newtonsoft.Json;

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
    private async Task RemoveCourse(Course course)
    {
        // Validate inputs
        if (course == null)
        {
            Debug.WriteLine("Cannot remove null course.");
            return;
        }

        if (CurrentEmail == null)
        {
            Debug.WriteLine("No user is currently selected.");
            return;
        }

        // Add confirmation alert
        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Confirm Removal",
            $"Are you sure you want to remove the course '{course.Coursename}'?",
            "Yes",
            "No");

        if (!confirm)
        {
            Debug.WriteLine("Course removal cancelled by user.");
            return;
        }

        try
        {
            // Find the user's register
            var userRegister = Registers.FirstOrDefault(r => r.UserId == CurrentEmail.UserId);
            if (userRegister == null)
            {
                Debug.WriteLine("No register found for the current user.");
                return;
            }

            // Find the term containing this course
            var termToModify = userRegister.Terms.FirstOrDefault(t => t.Courses.Contains(course.Courseid));
            if (termToModify == null)
            {
                Debug.WriteLine($"Course {course.Coursename} not found in any term for this user.");
                return;
            }

            // Remove the course from the term
            termToModify.Courses.Remove(course.Courseid);

            // Update the TermCoursesList and Courses collection
            var termCoursesToRemove = TermCoursesList.FirstOrDefault(tc => tc.TermName == termToModify.TermTerm);
            if (termCoursesToRemove != null)
            {
                termCoursesToRemove.Courses.Remove(course);
            }

            // Optional: Persist changes to JSON file
            await SaveUpdatedRegisterToJson(userRegister);

            // Refresh the view
            CombineCourses();

            // Optional: Show a success message
            Debug.WriteLine($"Course {course.Coursename} removed successfully.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error removing course: {ex.Message}");
        }
    }

    private async Task SaveUpdatedRegisterToJson(Register updatedRegister)
    {
        try
        {
            // Read existing registers
            var allRegisters = await ReadRegisterJsonAsync();

            // Find and replace the updated register
            var existingRegisterIndex = allRegisters.FindIndex(r => r.UserId == updatedRegister.UserId);
            if (existingRegisterIndex != -1)
            {
                allRegisters[existingRegisterIndex] = updatedRegister;
            }

            // Convert updated registers back to JSON
            string updatedJson = JsonConvert.SerializeObject(allRegisters, Formatting.Indented);

            // Write back to the file
            string filePath = Path.Combine(FileSystem.AppDataDirectory, "register.json");
            await File.WriteAllTextAsync(filePath, updatedJson);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error saving updated register: {ex.Message}");
        }
    }
    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.Navigation.PopAsync();
    }
}
