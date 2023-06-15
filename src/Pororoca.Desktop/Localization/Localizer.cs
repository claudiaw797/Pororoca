using System.ComponentModel;
using System.Text.Json;
using Avalonia.Platform;

namespace Pororoca.Desktop.Localization;

// Taken from:
// https://www.sakya.it/wordpress/avalonia-ui-framework-localization/
public class Localizer : INotifyPropertyChanged
{
    private const string IndexerName = "Item";
    private const string IndexerArrayName = "Item[]";
    public static Localizer Instance { get; set; } = new Localizer();
    public event PropertyChangedEventHandler? PropertyChanged;
    public Language Language { get; private set; }
    private readonly List<Action> _languageChangedSubscriptions = new();

    public string this[string key]
    {
        get
        {
            if (this._mappings != null && this._mappings.TryGetValue(key, out string? res))
                return res.Replace("\\n", "\n");
            else
                return $"{Language}:{key}";
        }
    }
    private IDictionary<string, string>? _mappings = null;

    public bool LoadLanguage(Language language)
    {
        Language = language;

        Uri uri = new($"avares://Pororoca.Desktop/Assets/i18n/{language.GetLanguageLCID()}.json");
        using var stringsFileUtf8Stream = AssetLoader.Open(uri);
        this._mappings = JsonSerializer.Deserialize<Dictionary<string, string>>(stringsFileUtf8Stream);
        Invalidate();

        return true;
    } // LoadLanguage

    public void Invalidate()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerArrayName));
        this._languageChangedSubscriptions.ForEach(sub => sub.Invoke());
    }

    public void SubscribeToLanguageChange(Action onLanguageChanged) =>
        this._languageChangedSubscriptions.Add(onLanguageChanged);
}