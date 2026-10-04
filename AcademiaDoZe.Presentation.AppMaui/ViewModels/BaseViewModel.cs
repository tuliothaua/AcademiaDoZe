using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public abstract class BaseViewModel : INotifyPropertyChanged
{
    private string _title = string.Empty;
    private bool _isBusy;

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
            return false;

        storage = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
