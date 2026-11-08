namespace CookieClicker.Saving
{
    /// <summary>
    /// Where the save text is kept. Swappable for tests or a cloud save later.
    /// </summary>
    public interface ISaveStorage
    {
        bool TryRead(out string contents);
        void Write(string contents);
        void Delete();
    }
}
