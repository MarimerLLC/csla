namespace WpUI.ViewModels
{
  public interface IViewModel
  {
    void NavigatingTo();
    void NavigatingBackTo();
    void NavigatedAway();
  }
}
