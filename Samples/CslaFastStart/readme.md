# CSLA .NET Fast Start
The focus of CSLA .NET is on the creation of a reusable business logic layer for applications. That layer consists of business domain objects created using object-oriented design concepts.

Of course using this business domain layer requires some sort of interface (human or API). And in most cases the state of the domain objects must be persisted via some sort of data access layer.

CSLA .NET is based on a five layer architecture:

1. Interface layer
2. Interface control layer
3. Business layer
4. Data access layer
5. Data storage layer

This fast start will walk you through the creation of all five layers in the simplest manner possible. The interface will be a console app, and the data storage will be a collection in memory.

This sample demonstrates modern CSLA 10 features including:

- **Source-generated properties** using `[CslaImplementProperties]` and C# partial properties
- **Dependency injection** via `[Inject]` attributes on data portal method parameters
- **Attribute-based data portal methods** (`[Create]`, `[Fetch]`, `[Insert]`, `[Update]`, `[Delete]`, `[DeleteSelf]`)
- **`IDataPortalFactory`** for obtaining data portal instances from the DI container

The fact that this code doesn't include a data access layer that interacts with a database, remote service, file on disk, Excel spreadsheet, or any other type of data storage doesn't mean they can't all be used. As you'll see, you are in total control of the code in the data access layer.

For more detailed information about using CSLA .NET please see the [Using CSLA .NET](http://store.lhotka.net/using-csla-4-all-books) book.

## Creating the solution and Interface project
Open Visual Studio and create a new _Console Application_ using .NET 9 or later. Name it `CslaFastStart`.

![](readme-images/CreateConsoleApp.png)

Add references to the following NuGet packages in your console app project:

- `Csla`
- `Microsoft.Extensions.DependencyInjection`

## Creating the Business layer project
Add a new _Class Library_ project to the existing solution. Name it `BusinessLayer`.

![](readme-images/CreateBusinessLayer.png)

Add NuGet references to the following packages in your class library project:

- `Csla`
- `Csla.Generator.AutoImplementProperties.Attributes.CSharp` (for source-generated properties)

Return to your console app project and add a project reference to your new `BusinessLayer` project.

## Creating the Data access layer project
Add a new _Class Library_  project to the existing solution. Name it `DataAccessLayer`.

![](readme-images/CreateDataAccessLayer.png)

Return to your `BusinessLayer` project and add a project reference to your new `DataAccessLayer` project.

## Adding a business domain class
Open the `BusinessLayer` project and rename `Class1` to `PersonEdit`. Then update its code as follows:

```csharp
using System;
using Csla;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer
{
  [Serializable]
  [CslaImplementProperties]
  public partial class PersonEdit : BusinessBase<PersonEdit>
  {
    public partial int Id { get; private set; }

    [Required]
    public partial string FirstName { get; set; }

    [Required]
    public partial string LastName { get; set; }
  }
}
```

This is a typical, albeit very simple, business class using CSLA 10's source-generated properties. The `[CslaImplementProperties]` attribute tells the CSLA source generator to automatically generate the `RegisterProperty` calls and `GetProperty`/`SetProperty` implementations for each partial property.

Key points:
- The class must be `partial` to allow the source generator to add the implementation
- Each property is declared as `partial` with just a getter/setter signature
- The `[Required]` attribute integrates with CSLA's rules engine for validation
- The `Id` property has a `private set` so it can only be set internally

## Implementing persistence
Before the `PersonEdit` class can be used you must add the ability for an instance to be created, retrieved, updated, or deleted.

Open the `DataAccessLayer` project and rename `Class1` to `PersonDto`. Then change its code as follows:

```csharp
namespace DataAccessLayer
{
  public class PersonDto
  {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
  }
}
```

This code provides a way to pass entity state between the domain object and the data access layer without any coupling to specific data access technologies or platforms.

Now add a new class to the `DataAccessLayer` project named `PersonDal` with the following code:

```csharp
using System;
using System.Linq;
using System.Collections.Generic;

namespace DataAccessLayer
{
  public class PersonDal
  {
    // this is our in-memory database
    private static List<PersonDto> _list = new List<PersonDto>();

    public PersonDto Create()
    {
      return new PersonDto { Id = -1 };
    }

    public PersonDto GetPerson(int id)
    {
      var entity = _list.FirstOrDefault(_ => _.Id == id);
      if (entity == null)
        throw new Exception("Index not found");
      return entity;
    }

    public int InsertPerson(PersonDto data)
    {
      var newId = 1;
      if (_list.Count > 0)
        newId = _list.Max(_ => _.Id) + 1;
      data.Id = newId;
      _list.Add(data);
      return newId;
    }

    public void UpdatePerson(PersonDto data)
    {
      var entity = GetPerson(data.Id);
      entity.FirstName = data.FirstName;
      entity.LastName = data.LastName;
    }

    public void DeletePerson(int id)
    {
      var entity = GetPerson(id);
      _list.Remove(entity);
    }
  }
}
```

This class implements the basic operations needed in any data access layer. Rather than persisting the data into a database or file, it simply maintains the data in an in-memory `static` collection.

Now that the data access layer exists the `PersonEdit` business class can be enhanced to use this functionality. Open the `PersonEdit` class in the `BusinessLayer` project and add a using statement for the data access layer, then add the data portal methods to the class:

```csharp
using System;
using Csla;
using DataAccessLayer;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer
{
  [Serializable]
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
```

CSLA 10 uses attribute-based data portal methods instead of the older `DataPortal_XYZ` naming convention:

| Attribute | Purpose |
|-----------|---------|
| `[Create]` | Initialize a new object |
| `[Fetch]` | Retrieve an existing object |
| `[Insert]` | Insert a new object's data |
| `[Update]` | Update an existing object's data |
| `[Delete]` | Delete data by criteria |
| `[DeleteSelf]` | Delete the current object's data |

The `[Inject]` attribute on method parameters tells CSLA to resolve dependencies from the DI container. This eliminates the need to manually create service instances with `new`.

At this point the business layer and data access layer are complete.

## Implementing the user interface
Open the `CslaFastStart` console app project's `Program.cs` file.

CSLA 10 uses dependency injection for the data portal. First, set up the DI container and register services:

```csharp
using BusinessLayer;
using DataAccessLayer;
using Csla;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CslaFastStart
{
  class Program
  {
    private static IServiceProvider ServiceProvider { get; set; }

    static void Main(string[] args)
    {
      // Set up dependency injection
      var services = new ServiceCollection();
      services.AddCsla();
      services.AddTransient<PersonDal>();
      ServiceProvider = services.BuildServiceProvider();

      // Get the data portal factory from DI
      var dpFactory = ServiceProvider.GetRequiredService<IDataPortalFactory>();

      Console.WriteLine("Creating a new person");
      var person = dpFactory.GetPortal<PersonEdit>().Create();
      Console.Write("Enter first name: ");
      person.FirstName = Console.ReadLine();
      Console.Write("Enter last name: ");
      person.LastName = Console.ReadLine();
      if (person.IsSavable)
      {
        person = person.Save();
        Console.WriteLine("Added person with id {0}. First name = '{1}', last name = '{2}'.", person.Id, person.FirstName, person.LastName);
      }
      else
      {
        Console.WriteLine("Invalid entry");
        foreach (var item in person.BrokenRulesCollection)
          Console.WriteLine(item.Description);
        Console.ReadKey();
        return;
      }

      Console.ReadKey();
    }
  }
}
```

Key differences from older CSLA versions:

- **DI container setup**: `services.AddCsla()` registers all CSLA services
- **Service registration**: `services.AddTransient<PersonDal>()` registers the DAL for injection
- **`IDataPortalFactory`**: Used to obtain typed data portal instances
- **`dpFactory.GetPortal<PersonEdit>().Create()`**: Replaces the static `DataPortal.Create<T>()` method

Also notice how the domain object's `IsSavable` property is used to determine whether the object can be saved. Normally if it can't be saved it is due to broken business rules, and this code shows one way to list all the broken rules that make the object invalid.

Add the following code to retrieve and update an existing domain object:

```csharp
      Console.WriteLine();
      Console.WriteLine("Updating existing person");
      person = dpFactory.GetPortal<PersonEdit>().Fetch(person.Id);
      Console.Write("Update first name [{0}]: ", person.FirstName);
      var temp = Console.ReadLine();
      if (!string.IsNullOrWhiteSpace(temp))
      {
        person.FirstName = temp;
      }
      Console.Write("Update last name [{0}]: ", person.LastName);
      temp = Console.ReadLine();
      if (!string.IsNullOrWhiteSpace(temp))
      {
        person.LastName = temp;
      }
      if (person.IsSavable)
      {
        person = person.Save();
        Console.WriteLine("Updated person with id {0}. First name = '{1}', last name = '{2}'.", person.Id, person.FirstName, person.LastName);
      }
      else
      {
        if (person.IsDirty)
        {
          Console.WriteLine("Invalid entry");
          foreach (var item in person.BrokenRulesCollection)
            Console.WriteLine(item.Description);
          Console.ReadKey();
          return;
        }
        else
        {
          Console.WriteLine("No changes, nothing to save");
        }
      }

      Console.ReadKey();
```

This code uses `dpFactory.GetPortal<PersonEdit>().Fetch()` to retrieve an existing object, and then calls the `Save` method to save any changes. The CSLA .NET framework manages the metastate for each object, so it knows whether the object needs to be inserted or updated.

Not only can an object be not savable because it is invalid, but if an object has no changed values then there'd be nothing to save. This code uses the `IsDirty` property to determine if the object was changed or is invalid due to broken rules.

Finally, add the following code to delete the data related to a domain object:

```csharp
      Console.WriteLine();
      Console.WriteLine("Deleting existing person");
      dpFactory.GetPortal<PersonEdit>().Delete(person.Id);
      try
      {
        person = dpFactory.GetPortal<PersonEdit>().Fetch(person.Id);
        Console.WriteLine("Person NOT deleted");
      }
      catch
      {
        Console.WriteLine("Person successfully deleted");
      }

      Console.ReadKey();
```

At this point you should be able to run the app, seeing how it adds, updates, and deletes the data associated with a business domain object.

## Adding business rules
One of the primary features of CSLA .NET is the rules engine that sits within the `BusinessBase` type. The rules engine is very powerful and extensible, and this fast start will only touch on the simplest validation rule concepts. Beyond this are complex validation rules, calculation and data manipulation rules, authorization rules, and much more.

CSLA .NET supports the `System.ComponentModel.DataAnnotations` attributes. These provide a very simple and limited validation rule concept that works on any platform and behind any type of interface, because the attributes are automatically integrated into the broader CSLA .NET rules engine.

With CSLA 10's source-generated properties, you simply add the validation attributes directly to the partial property declarations:

```csharp
    [Required]
    public partial string FirstName { get; set; }

    [Required]
    public partial string LastName { get; set; }
```

The `[Required]` attributes were already added to our `PersonEdit` class earlier. If you run the app and don't enter values for the first and last name properties during the first _create_ phase, you'll find that the object won't save because it is invalid, and you should see the broken rules listed as you'd expect.

## Summary of CSLA 10 Features Used

| Feature | Description |
|---------|-------------|
| `[CslaImplementProperties]` | Enables source generation for partial properties |
| `partial` properties | Declare property signature; implementation is generated |
| `[Create]`, `[Fetch]`, etc. | Attribute-based data portal method declaration |
| `[Inject]` | Dependency injection for data portal method parameters |
| `IDataPortalFactory` | DI-friendly way to obtain data portal instances |
| `services.AddCsla()` | Registers all CSLA services with the DI container |
