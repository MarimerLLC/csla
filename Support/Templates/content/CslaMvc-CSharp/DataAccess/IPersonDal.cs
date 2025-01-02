using System;
using System.Collections.Generic;

namespace CslaMvc.DataAccess
{
  public interface IPersonDal
  {
    bool Exists(int id);
    PersonEntity Get(int id);
    List<PersonEntity> Get();
    PersonEntity Insert(PersonEntity person);
    PersonEntity Update(PersonEntity person);
    bool Delete(int id);
  }
}
