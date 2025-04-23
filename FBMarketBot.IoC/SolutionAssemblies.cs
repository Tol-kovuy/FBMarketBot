using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FBMarketBot.IoC
{
    /// <summary>
    /// Provides methods to retrieve and filter assemblies within the solution.
    /// </summary>
    public class SolutionAssemblies
    {
        /// <summary>
        /// Retrieves all assemblies referenced by the entry assembly and their dependencies.
        /// </summary>
        /// <returns>A list of all discovered assemblies.</returns>
        public static IList<Assembly> GetAll()
        {
            var returnAssemblies = new List<Assembly>();
            var loadedAssemblies = new HashSet<string>();
            var assembliesToCheck = new Queue<Assembly>();

            var entryAssembly = Assembly.GetEntryAssembly();
            assembliesToCheck.Enqueue(entryAssembly);

            while (assembliesToCheck.Any())
            {
                var assemblyToCheck = assembliesToCheck.Dequeue();

                foreach (var reference in assemblyToCheck.GetReferencedAssemblies())
                {
                    if (!loadedAssemblies.Contains(reference.FullName))
                    {
                        try
                        {
                            var assembly = Assembly.Load(reference);
                            assembliesToCheck.Enqueue(assembly);
                            loadedAssemblies.Add(reference.FullName);
                            returnAssemblies.Add(assembly);
                        }
                        catch (FileNotFoundException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        catch (FileLoadException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        catch (BadImageFormatException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }

            return returnAssemblies;
        }

        /// <summary>
        /// Filters a list of assemblies to include only those whose names contain the current assembly's root name.
        /// </summary>
        /// <param name="assemblies">The list of assemblies to filter.</param>
        /// <returns>A list of filtered assemblies.</returns>
        public static IList<Assembly> GetFiltred(IList<Assembly> assemblies)
        {
            var filtredAssemblies = new List<Assembly>();
            foreach (var assembly in assemblies)
            {
                if (assembly.FullName == null)
                {
                    continue;
                }
                if (assembly.FullName.Contains(GetCurrent()))
                {
                    filtredAssemblies.Add(assembly);
                }
            }
            return filtredAssemblies;
        }

        /// <summary>
        /// Retrieves the root name of the current assembly, truncated at the first dot.
        /// </summary>
        /// <returns>The root name of the current assembly.</returns>
        private static string GetCurrent()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            string assamblyName = currentAssembly.GetName().Name;
            int index = assamblyName.IndexOf('.');

            if (index > 0)
            {
                assamblyName = assamblyName.Substring(0, index);
            }
            return assamblyName;
        }
    }
}