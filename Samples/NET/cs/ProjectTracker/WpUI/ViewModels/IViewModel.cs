namespace WpUI.ViewModels
{
  public interface IViewModel
  {
    void Initialize();
    void NavigatingTo();
    void NavigatedAway();
  }
}
