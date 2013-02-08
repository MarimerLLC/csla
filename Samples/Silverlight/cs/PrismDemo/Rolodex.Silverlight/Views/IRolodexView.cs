namespace Rolodex.Silverlight.Views
{
    public interface IRolodexView
    {
        object DataContext { get; set; }
        bool IsDirty { get; }
        bool Focus();
    }
}
