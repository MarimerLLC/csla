using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Csla;
using System.ComponentModel;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  [CslaImplementProperties]
  public partial class ProjectEdit : CslaBaseTypes.BusinessBase<ProjectEdit>
  {
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public partial byte[] TimeStamp { get; private set; }

    [Display(Name = "Project id")]
    public partial int Id { get; private set; }

    [Display(Name = "Project name")]
    [Required]
    [StringLength(50)]
    public partial string Name { get; set; }

    public partial DateTime? Started { get; set; }

    public partial DateTime? Ended { get; set; }

    public partial string Description { get; set; }

    public partial ProjectResources Resources { get; private set; }

    public override string ToString()
    {
      return Id.ToString();
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      //BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(NameProperty));
      BusinessRules.AddRule(
        new StartDateGTEndDate { 
          PrimaryProperty = StartedProperty, 
          AffectedProperties = { EndedProperty } });
      BusinessRules.AddRule(
        new StartDateGTEndDate { 
          PrimaryProperty = EndedProperty, 
          AffectedProperties = { StartedProperty } });

      BusinessRules.AddRule(
        new Csla.Rules.CommonRules.IsInRole(
          Csla.Rules.AuthorizationActions.WriteProperty, 
          NameProperty, 
          "ProjectManager"));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(
        Csla.Rules.AuthorizationActions.WriteProperty, StartedProperty, Security.Roles.ProjectManager));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(
        Csla.Rules.AuthorizationActions.WriteProperty, EndedProperty, Security.Roles.ProjectManager));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(
        Csla.Rules.AuthorizationActions.WriteProperty, DescriptionProperty, Security.Roles.ProjectManager));
      BusinessRules.AddRule(new NoDuplicateResource { PrimaryProperty = ResourcesProperty });
    }

    [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [ObjectAuthorizationRules]
    public static void AddObjectAuthorizationRules()
    {
      Csla.Rules.BusinessRules.AddRule(
        typeof(ProjectEdit), 
        new Csla.Rules.CommonRules.IsInRole(
          Csla.Rules.AuthorizationActions.CreateObject, 
          "ProjectManager"));
      Csla.Rules.BusinessRules.AddRule(typeof(ProjectEdit), 
        new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, Security.Roles.ProjectManager));
      Csla.Rules.BusinessRules.AddRule(typeof(ProjectEdit), 
        new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, Security.Roles.ProjectManager, Security.Roles.Administrator));
    }

    protected override void OnChildChanged(Csla.Core.ChildChangedEventArgs e)
    {
      if (e.ChildObject is ProjectResources)
      {
        BusinessRules.CheckRules(ResourcesProperty);
        OnPropertyChanged(ResourcesProperty);
      }
      base.OnChildChanged(e);
    }

    private class NoDuplicateResource : Csla.Rules.BusinessRule
    {
      protected override void Execute(Csla.Rules.IRuleContext context)
      {
        if (context.Target is not ProjectEdit target)
          return;
        try
        {
          var resources = target.Resources;
          if (resources is null)
            return;
          foreach (var item in resources)
          {
            var count = resources.Count(r => r.ResourceId == item.ResourceId);
            if (count > 1)
            {
              context.AddErrorResult("Duplicate resources not allowed");
              return;
            }
          }
        }
        catch
        {
          // Resources may not be loaded yet
        }
      }
    }

    private class StartDateGTEndDate : Csla.Rules.BusinessRule
    {
      protected override void Execute(Csla.Rules.IRuleContext context)
      {
        if (context.Target is not ProjectEdit target)
          return;

        var started = target.ReadProperty(StartedProperty);
        var ended = target.ReadProperty(EndedProperty);
        if ((started.HasValue && ended.HasValue && started.Value > ended.Value) || (!started.HasValue && ended.HasValue))
          context.AddErrorResult("Start date can't be after end date");
      }
    }

    [Create]
    [RunLocal]
    private void Create([Inject]IChildDataPortal<ProjectResources> portal)
    {
      LoadProperty(ResourcesProperty, portal!.CreateChild()!);
      BusinessRules.CheckRules();
    }

    [Fetch]
    private void Fetch(int id, [Inject] IProjectDal dal, [Inject] IChildDataPortal<ProjectResources> portal)
    {
      var data = dal.Fetch(id) ?? throw new DataNotFoundException("Project");
      using (BypassPropertyChecks)
      {
        Id = data.Id;
        Name = data.Name ?? string.Empty;
        Description = data.Description ?? string.Empty;
        Started = data.Started;
        Ended = data.Ended;
        TimeStamp = data.LastChanged ?? Array.Empty<byte>();
        Resources = portal!.FetchChild(id)!;
      }
    }

    [Insert]
    private void Insert([Inject] IProjectDal dal)
    {
      using (BypassPropertyChecks)
      {
        var item = new ProjectTracker.Dal.ProjectDto
        {
          Name = this.Name,
          Description = this.Description,
          Started = this.Started,
          Ended = this.Ended
        };
        dal.Insert(item);
        Id = item.Id;
        TimeStamp = item.LastChanged;
      }
      FieldManager.UpdateChildren(this);
    }

    [Update]
    private void Update([Inject] IProjectDal dal)
    {
      using (BypassPropertyChecks)
      {
        var item = new ProjectTracker.Dal.ProjectDto
        {
          Id = this.Id,
          Name = this.Name,
          Description = this.Description,
          Started = this.Started,
          Ended = this.Ended,
          LastChanged = this.TimeStamp
        };
        dal.Update(item);
        TimeStamp = item.LastChanged;
      }
      FieldManager.UpdateChildren(this.Id);
    }

    [DeleteSelf]
    private void DeleteSelf([Inject] IProjectDal dal)
    {
      using (BypassPropertyChecks)
      {
        Resources?.Clear();
        FieldManager.UpdateChildren(this);
        Delete(this.Id, dal);
      }
    }

    [Delete]
    private void Delete(int id, [Inject] IProjectDal dal)
    {
      dal.Delete(id);
    }
  }
}