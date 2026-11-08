namespace CookieClicker.EditorTools
{
    /// <summary>
    /// A self-contained piece of the scene that can be added to an existing scene without touching anything else.
    /// </summary>
    internal interface ISceneFeature
    {
        string Name { get; }

        /// <summary>True when the open scene already has this feature, so installing is skipped.</summary>
        bool IsInstalled();

        void Install();
    }
}
