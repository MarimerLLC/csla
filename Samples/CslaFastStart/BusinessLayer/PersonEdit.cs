using System;
using Csla;
using DataAccessLayer;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer
{
  [CslaImplementProperties]
  public partial class PersonEdit : BusinessBase<PersonEdit>
  {
    public partial int Id { get; private set; }

    [Required]
    public partial string FirstName { get; set; }

    [Required]
    public partial string LastName { get; set; }

    [Create]
    private void Create([Inject] PersonDal dal)
    {
      var dto = dal.Create();
      using (BypassPropertyChecks)
      {
        Id = dto.Id;
        FirstName = dto.FirstName;
        LastName = dto.LastName;
      }
      BusinessRules.CheckRules();
    }

    [Fetch]
    private void Fetch(int id, [Inject] PersonDal dal)
    {
      var dto = dal.GetPerson(id);
      using (BypassPropertyChecks)
      {
        Id = dto.Id;
        FirstName = dto.FirstName;
        LastName = dto.LastName;
      }
    }

    [Insert]
    private void Insert([Inject] PersonDal dal)
    {
      using (BypassPropertyChecks)
      {
        var dto = new PersonDto
        {
          FirstName = this.FirstName,
          LastName = this.LastName
        };
        Id = dal.InsertPerson(dto);
      }
    }

    [Update]
    private void Update([Inject] PersonDal dal)
    {
      using (BypassPropertyChecks)
      {
        var dto = new PersonDto
        {
          Id = this.Id,
          FirstName = this.FirstName,
          LastName = this.LastName
        };
        dal.UpdatePerson(dto);
      }
    }

    [Delete]
    private void Delete(int id, [Inject] PersonDal dal)
    {
      using (BypassPropertyChecks)
      {
        dal.DeletePerson(id);
      }
    }

    [DeleteSelf]
    private void DeleteSelf([Inject] PersonDal dal)
    {
      using (BypassPropertyChecks)
      {
        dal.DeletePerson(Id);
      }
    }
  }
}
