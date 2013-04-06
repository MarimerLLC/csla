#if !CLIENTONLY
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Csla.Windows
{
  /// <summary>
  /// HostComponentDesigner is an enhanced ComponentDesigner 
  /// class used for linking a parent container control to a component
  /// marked with the HostComponentDesigner attribute 
  /// as a .NET designer service. 
  /// </summary>
  public class HostComponentDesigner : System.ComponentModel.Design.ComponentDesigner
  {
    #region Public Override Methods

    /// <summary>
    /// InitializeNewComponent() overrides the base class InitializeNewComponent() method.  This version of
    /// InitializeNewComponent() simply runs through the base class InitializeNewComponent functionality then
    /// if the associated component contains the HostPropertyAttribute attribute the conponent's host property
    /// is then set to the parent component if the component is a container control.
    /// </summary>
    /// <param name="defaultValues">The default values for initialising the new component.</param>
    public override void InitializeNewComponent(System.Collections.IDictionary defaultValues)
    {
      AttributeCollection attributes = null;
      string propertyName = string.Empty;
      int index = 0;

      // Run through the default new component initialisation process.
      base.InitializeNewComponent(defaultValues);


      // Ignore if the parent is not a container.
      if (!(ParentComponent is ContainerControl))
        return;

      // Retrieve the component's attributes.
      attributes = TypeDescriptor.GetAttributes(Component);

      // If we have attributes then find our host property attribute.
      if (attributes != null)
      {
        for (index = 0; index < attributes.Count; ++index)
        {
          // If we have a match the fetch the host property name.
          if (attributes[index] is HostPropertyAttribute)
          {
            propertyName = ((HostPropertyAttribute)attributes[index]).HostPropertyName;
            break;
          }
        }
      }

      // If there is a host property name then set the host property to be the parent container.
      if (!string.IsNullOrEmpty(propertyName))
        Component.GetType().GetProperty(propertyName).SetValue(Component, (ContainerControl)ParentComponent, null);
    }

    #endregion

  }
}
#endif