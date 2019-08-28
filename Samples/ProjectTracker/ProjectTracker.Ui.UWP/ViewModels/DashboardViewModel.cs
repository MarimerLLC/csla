namespace UwpUI.ViewModels
{
  public class DashboardViewModel : ViewModel<ProjectTracker.Library.Dashboard>
  {
    public DashboardViewModel()
    {
      var task = RefreshAsync<ProjectTracker.Library.Dashboard>(async () =>
        await ProjectTracker.Library.Dashboard.GetDashboardAsync());
    }
  }
}
