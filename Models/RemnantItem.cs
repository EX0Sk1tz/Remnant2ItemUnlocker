using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Remnant2UnlockerApp.Models;

public sealed class RemnantItem : INotifyPropertyChanged
{
    private string? _imagePath;
    private ImageSource? _image;
    private bool _isImageLoading;

    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public string Path { get; set; } = "";

    public string SummonCommand => $"summon {Path}";

    public string? ImagePath
    {
        get => _imagePath;
        set
        {
            _imagePath = value;
            Image = LoadImage(value);
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasImage));
        }
    }

    public ImageSource? Image
    {
        get => _image;
        private set
        {
            _image = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasImage));
        }
    }

    public bool HasImage => Image != null;

    public bool IsImageLoading
    {
        get => _isImageLoading;
        set
        {
            _isImageLoading = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private static ImageSource? LoadImage(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            return null;

        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.UriSource = new Uri(path, UriKind.Absolute);
        bitmap.EndInit();
        bitmap.Freeze();

        return bitmap;
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}