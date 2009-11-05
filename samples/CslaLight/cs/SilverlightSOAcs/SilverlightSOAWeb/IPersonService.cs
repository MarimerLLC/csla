using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SilverlightSOAWeb
{
  [DataContract]
  public class PersonData
  {
    [DataMember]
    public int Id { get; set; }
    [DataMember]
    public string FirstName { get; set; }
    [DataMember]
    public string LastName { get; set; }
  }

  [ServiceContract]
  public interface IPersonService
  {
    [OperationContract]
    PersonData GetPerson(int id);
    [OperationContract]
    PersonData AddPerson(PersonData obj);
    [OperationContract]
    PersonData UpdatePerson(PersonData obj);
    [OperationContract]
    void DeletePerson(int id);
  }
}
