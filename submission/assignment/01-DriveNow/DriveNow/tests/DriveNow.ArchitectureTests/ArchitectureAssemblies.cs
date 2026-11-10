using System.Reflection;
using DriveNow.Application.Services;
using DriveNow.Domain.Entities;
using DriveNow.Infrastructure.Repositories;

namespace DriveNow.ArchitectureTests;

internal static class ArchitectureAssemblies
{
    public static Assembly Domain => typeof(Customer).Assembly;
    public static Assembly Application => typeof(BookingService).Assembly;
    public static Assembly Infrastructure => typeof(BookingRepository).Assembly;
    public static Assembly UnitTests => Assembly.Load("DriveNow.UnitTests");

    public static Assembly ConsoleApp => Assembly.Load("DriveNow.ConsoleApp");

    public static Assembly[] Production => [Domain, Application, Infrastructure, ConsoleApp];

    public static IEnumerable<Type> ProductionTypes => Production
        .SelectMany(assembly => assembly.GetTypes())
        .Where(type => !type.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false)
            && !type.Name.StartsWith('<'));

    public static bool InNamespace(Type type, string namespacePart) =>
        type.Namespace is { } ns &&
        (ns.EndsWith('.' + namespacePart, StringComparison.Ordinal) ||
         ns.Contains('.' + namespacePart + '.', StringComparison.Ordinal));

    public static bool HasBaseType(Type type, string fullName)
    {
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            // Matches generic base types such as Bogus.Faker<T> as well.
            if (current.FullName == fullName ||
                current.FullName?.StartsWith(fullName + "`", StringComparison.Ordinal) == true)
                return true;
        }

        return false;
    }

    public static IEnumerable<string> OtherDriveNowReferences(Assembly assembly, params string[] allowed) =>
        assembly.GetReferencedAssemblies()
            .Select(a => a.Name)
            .Where(n => n is not null && n.StartsWith("DriveNow.", StringComparison.Ordinal)
                && !allowed.Contains(n, StringComparer.Ordinal))
            .Select(n => n!)
            .OrderBy(n => n);
}
