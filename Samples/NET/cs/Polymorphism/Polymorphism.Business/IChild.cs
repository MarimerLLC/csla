using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Serialization.Mobile;

namespace Polymorphism.Business
{
  // Common interface for child 
  public interface IChild : Csla.Core.IEditableBusinessObject, IMobileObject
  {
    int Id { get; set; }
    string Name { get; set; }
  }
}