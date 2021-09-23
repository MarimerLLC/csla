using Csla.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Generators.TestObjects
{

  [AutoSerializable]
  public partial class AddressPOCO
  {

    public string AddressLine1 { get; set; }

    public string AddressLine2 { get; set; }

    public string Town { get; set; }

    public string County { get; set; }

    public string Postcode { get; set; }

  }
}
