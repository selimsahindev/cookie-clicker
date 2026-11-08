namespace CookieClicker.Saving
{
    /// <summary>
    /// Implemented by any component that has state worth keeping between sessions.
    /// The save manager finds every ISaveable in the scene, so new systems only need to implement this.
    /// </summary>
    public interface ISaveable
    {
        void Capture(SaveData data);
        void Restore(SaveData data);
    }
}
