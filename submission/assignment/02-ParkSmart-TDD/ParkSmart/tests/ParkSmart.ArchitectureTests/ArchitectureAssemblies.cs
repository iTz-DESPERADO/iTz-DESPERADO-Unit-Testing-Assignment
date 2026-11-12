using System.Reflection;
using ParkSmart.Domain;

namespace ParkSmart.ArchitectureTests;

internal static class ArchitectureAssemblies
{
    public static Assembly Domain => typeof(ParkingSession).Assembly;
    public static Assembly Application => Assembly.Load("ParkSmart.Application");
    public static Assembly Infrastructure => Assembly.Load("ParkSmart.Infrastructure");
    public static Assembly ConsoleApp => Assembly.Load("ParkSmart.ConsoleApp");

    public static Assembly[] Production => [Domain, Application, Infrastructure, ConsoleApp];

    public static IEnumerable<Type> ProductionTypes => Production
        .SelectMany(assembly => assembly.GetTypes())
        .Where(type => !type.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false)
            && !type.Name.StartsWith('<'));

    public static IEnumerable<string> OtherParkSmartReferences(Assembly assembly, params string[] allowed) =>
        assembly.GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => name is not null && name.StartsWith("ParkSmart.", StringComparison.Ordinal)
                && !allowed.Contains(name, StringComparer.Ordinal))
            .Select(name => name!)
            .OrderBy(name => name);
}
