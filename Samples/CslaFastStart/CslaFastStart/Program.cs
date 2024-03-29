﻿using BusinessLayer;
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
      var services = new ServiceCollection();
      services.AddCsla();
      ServiceProvider = services.BuildServiceProvider();

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
    }
  }
}
