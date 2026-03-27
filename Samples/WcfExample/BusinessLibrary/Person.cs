using Csla;
using Csla.Server;

namespace BusinessLibrary
{
    [ObjectFactory(typeof(PersonFactory))]
    [CslaImplementProperties]
    public partial class Person : BusinessBase<Person>
    {
        public partial int Id { get; set; }
        public partial string Name { get; set; }
    }
}
