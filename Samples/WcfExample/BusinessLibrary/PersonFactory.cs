using Csla;
using Csla.Server;

namespace BusinessLibrary
{
    public class PersonFactory(ApplicationContext applicationContext) : ObjectFactory(applicationContext)
    {
        public Person Fetch(int id)
        {
            var person = ApplicationContext.CreateInstanceDI<Person>();

            using (BypassPropertyChecks(person))
            {
                person.Id = id;
                person.Name = "Clark Kent";
            }

            MarkOld(person);

            return person;
        }
    }
}