namespace CookieClicker.Common
{
    /// <summary>
    /// Anything that exposes a normalized 0..1 progress value a view can display.
    /// </summary>
    public interface IProgressSource
    {
        float Progress { get; }

        /// <summary>False while the progress is meaningless to the player, so views hide it.</summary>
        bool IsProgressVisible { get; }
    }
}
