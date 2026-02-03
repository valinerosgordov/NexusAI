using NexusAI.Domain.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NexusAI.Application.Services;

public sealed class SessionContext : INotifyPropertyChanged
{
    private AppMode _currentMode = AppMode.Professional;

    public AppMode CurrentMode
    {
        get => _currentMode;
        set
        {
            if (_currentMode != value)
            {
                _currentMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsStudentMode));
                OnPropertyChanged(nameof(IsProfessionalMode));
                OnPropertyChanged(nameof(ModeDisplayName));
                OnPropertyChanged(nameof(ModeDescription));
            }
        }
    }

    public bool IsStudentMode => CurrentMode == AppMode.Student;
    public bool IsProfessionalMode => CurrentMode == AppMode.Professional;

    public string ModeDisplayName => CurrentMode switch
    {
        AppMode.Professional => "Professional",
        AppMode.Student => "Student",
        _ => "Professional"
    };

    public string ModeDescription => CurrentMode switch
    {
        AppMode.Professional => "Executive Assistant - Concise and business-focused",
        AppMode.Student => "Socratic Tutor - Educational and explanatory",
        _ => "Executive Assistant"
    };

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void SwitchMode()
    {
        CurrentMode = CurrentMode == AppMode.Professional
            ? AppMode.Student
            : AppMode.Professional;
    }
}
