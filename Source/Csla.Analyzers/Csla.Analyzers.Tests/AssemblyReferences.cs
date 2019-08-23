using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers.Tests
{
  internal static class AssemblyReferences
  {
    private static void LoadDependencies(HashSet<Assembly> loadedAssemblies, Assembly fromAssembly)
    {
      foreach (var reference in fromAssembly.GetReferencedAssemblies())
      {
        try
        {
          var assembly = Assembly.Load(reference);

          if (loadedAssemblies.Add(assembly))
          {
            LoadDependencies(loadedAssemblies, assembly);
          }
        }
        catch (FileNotFoundException) { }
        catch (FileLoadException) { }
      }
    }

    internal static MetadataReference[] GetMetadataReferences(IEnumerable<Assembly> assemblies)
    {
      var references = assemblyReferences.Value;

      foreach (var reference in assemblies.Transform())
      {
        references.Add(reference);
      }

      foreach(var assembly in assemblies)
      {
        var referencedAssemblies = new HashSet<Assembly>();
        LoadDependencies(referencedAssemblies, assembly);

        foreach(var referencedAssembly in referencedAssemblies.Transform())
        {
          references.Add(referencedAssembly);
        }
      }

      return references.ToArray();
    }

    private static readonly ConcurrentDictionary<Assembly, MetadataReference> transformedAssemblies =
       new ConcurrentDictionary<Assembly, MetadataReference>();

    private static IEnumerable<MetadataReference> Transform(this IEnumerable<Assembly> @this) =>
       @this.Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
          .Select(_ => transformedAssemblies.GetOrAdd(
             _, asm => MetadataReference.CreateFromFile(asm.Location)))
          .Cast<MetadataReference>();

    // Lifted from:
    // https://github.com/dotnet/roslyn/wiki/Runtime-code-generation-using-Roslyn-compilations-in-.NET-Core-App
    internal static Lazy<HashSet<MetadataReference>> assemblyReferences =
       new Lazy<HashSet<MetadataReference>>(() =>
       {
         var assemblies = new HashSet<Assembly>();

         var trustedPlatformAssemblies =
              (AppDomain.CurrentDomain.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string)?.Split(Path.PathSeparator);

         if (trustedPlatformAssemblies != null)
         {
           var platformAssemblyPaths = new HashSet<string>(trustedPlatformAssemblies);
           var platformAssemblyNames = platformAssemblyPaths.Select(Path.GetFileNameWithoutExtension);

           foreach (var platformAssemblyName in platformAssemblyNames)
           {
             if (!string.IsNullOrWhiteSpace(platformAssemblyName))
             {
               assemblies.Add(Assembly.Load(new AssemblyName(platformAssemblyName)));
             }
           }
         }

         foreach (var assembly in assemblies.ToList())
         {
           LoadDependencies(assemblies, assembly);
         }

         return new HashSet<MetadataReference>(assemblies.Transform());
       });
  }
}