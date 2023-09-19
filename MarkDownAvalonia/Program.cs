using Avalonia;
using Avalonia.Media;

namespace MarkDownAvalonia
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<EntryApp>()
                .UsePlatformDetect()
                .With(new FontManagerOptions
                {
                    FontFallbacks = new[]
                    {
                        new FontFallback
                        {
                            FontFamily = new FontFamily("Apple Color Emoji"),
                            UnicodeRange = UnicodeRange.Parse("U+23??, U+26??, U+2700-27BF, U+2B??, U+1F1E6-1F1FF, U+1F300-1F5FF, U+1F600-1F64F, U+1F680-1F6FF, U+1F9??")
                        }
                    }
                })
                .LogToTrace();
    }
}