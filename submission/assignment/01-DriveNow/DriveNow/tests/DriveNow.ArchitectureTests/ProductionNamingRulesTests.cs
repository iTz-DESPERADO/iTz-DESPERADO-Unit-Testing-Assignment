using System.Reflection;
using System.Text.RegularExpressions;
using FluentAssertions;
using NetArchTest.Rules;

namespace DriveNow.ArchitectureTests;

public sealed class ProductionNamingRulesTests
{
    private const string PascalCasePattern = "^[A-Z][A-Za-z0-9]*$";

    // NAMING.md R4: interfaces must start with I and PascalCase
    [Fact]
    public void ProductionInterfaces_Names_StartWithCapitalI()
    {
        // Arrange
        var errors = new List<string>();

        // Act

        foreach (var assembly in ArchitectureAssemblies.Production)
        {
            var result = Types.InAssembly(assembly)
                .That().AreInterfaces()
                .Should().HaveNameStartingWith("I")
                .GetResult();

            if (!result.IsSuccessful)
                errors.AddRange(result.FailingTypes?.Select(t => t.FullName ?? t.Name) ?? []);
        }

        errors.AddRange(ArchitectureAssemblies.ProductionTypes
            .Where(t => t.IsInterface && !Regex.IsMatch(t.Name, "^I[A-Z][A-Za-z0-9]*$"))
            .Select(t => t.FullName ?? t.Name));

        // Assert
        errors.Distinct().Should().BeEmpty("interface names must follow I + PascalCase. Violations: {0}", string.Join("; ", errors.Distinct()));
    }

    // NAMING.md R4: interfaces belong in <Layer>.Interfaces
    [Fact]
    public void ProductionInterfaces_Namespaces_MatchOwningLayer()
    {
        // Arrange
        var types = ArchitectureAssemblies.ProductionTypes;

        // Act
        var errors = types
            .Where(t => t.IsInterface &&
                (t.Assembly == ArchitectureAssemblies.Domain ||
                 t.Assembly == ArchitectureAssemblies.Application))
            .Where(t => t.Namespace !=
                (t.Assembly == ArchitectureAssemblies.Domain
                    ? "DriveNow.Domain.Interfaces"
                    : "DriveNow.Application.Interfaces"))
            .Select(t => $"{t.FullName} (expected " +
                (t.Assembly == ArchitectureAssemblies.Domain
                    ? "DriveNow.Domain.Interfaces)"
                    : "DriveNow.Application.Interfaces)"))
            .ToArray();

        // Assert
        errors.Should().BeEmpty("Domain and Application interfaces must be in their Interfaces namespace. Violations: {0}", string.Join("; ", errors.Distinct()));
    }

    // NAMING.md R4: repository classes must end in Repository and be in a Repositories namespace
    [Fact]
    public void ProductionRepositories_NamesAndNamespaces_FollowConvention()
    {
        // Arrange
        var candidates = ArchitectureAssemblies.ProductionTypes
            .Where(t => t.IsClass &&
                (ArchitectureAssemblies.InNamespace(t, "Repositories") ||
                 t.Name.EndsWith("Repository", StringComparison.Ordinal) ||
                 t.Name.EndsWith("Repo", StringComparison.Ordinal)))
            .ToArray();

        // Act
        var errors = candidates
            .Where(t => !t.Name.EndsWith("Repository", StringComparison.Ordinal) ||
                        !ArchitectureAssemblies.InNamespace(t, "Repositories"))
            .Select(t => t.FullName ?? t.Name)
            .ToList();

        foreach (var assembly in ArchitectureAssemblies.Production)
        {
            var result = Types.InAssembly(assembly)
                .That().ResideInNamespaceEndingWith("Repositories")
                .Should().HaveNameEndingWith("Repository")
                .GetResult();
            if (!result.IsSuccessful)
                errors.AddRange(result.FailingTypes?.Select(t => t.FullName ?? t.Name) ?? []);
        }

        // Assert
        errors.Distinct().Should().BeEmpty("repository classes must end in Repository and live under Repositories. Violations: {0}", string.Join("; ", errors.Distinct()));
    }

    // NAMING.md R4: service classes must end in Service and be in a Services namespace
    [Fact]
    public void ProductionServices_NamesAndNamespaces_FollowConvention()
    {
        // Arrange
        var candidates = ArchitectureAssemblies.ProductionTypes
            .Where(t => t.IsClass &&
                (ArchitectureAssemblies.InNamespace(t, "Services") ||
                 t.Name.EndsWith("Service", StringComparison.Ordinal)))
            .ToArray();

        // Act
        var errors = candidates
            .Where(t => !t.Name.EndsWith("Service", StringComparison.Ordinal) ||
                        !ArchitectureAssemblies.InNamespace(t, "Services"))
            .Select(t => t.FullName ?? t.Name)
            .ToList();

        foreach (var assembly in ArchitectureAssemblies.Production)
        {
            var result = Types.InAssembly(assembly)
                .That().ResideInNamespaceEndingWith("Services")
                .Should().HaveNameEndingWith("Service")
                .GetResult();
            if (!result.IsSuccessful)
                errors.AddRange(result.FailingTypes?.Select(t => t.FullName ?? t.Name) ?? []);
        }

        // Assert
        errors.Distinct().Should().BeEmpty("service classes must end in Service and live under Services. Violations: {0}", string.Join("; ", errors.Distinct()));
    }

    // NAMING.md R4: exception names and Domain exception namespaces
    [Fact]
    public void ProductionExceptions_NamesAndNamespaces_FollowConvention()
    {
        // Arrange
        var types = ArchitectureAssemblies.ProductionTypes;

        // Act
        var errors = types
            .Where(t => t.IsClass && t != typeof(Exception) && typeof(Exception).IsAssignableFrom(t))
            .Where(t => !t.Name.EndsWith("Exception", StringComparison.Ordinal) ||
                        (t.Assembly == ArchitectureAssemblies.Domain &&
                         t.Namespace != "DriveNow.Domain.Exceptions"))
            .Select(t => t.FullName ?? t.Name)
            .ToArray();

        // Assert
        errors.Should().BeEmpty("exceptions must end in Exception; Domain exceptions live in Domain.Exceptions. Violations: {0}", string.Join("; ", errors.Distinct()));
    }

    // NAMING.md R4: production classes, interfaces and declared methods use PascalCase
    [Fact]
    public void ProductionIdentifiers_ClassInterfaceAndMethodNames_UsePascalCase()
    {
        // Arrange
        var errors = new List<string>();

        // Act
        foreach (var type in ArchitectureAssemblies.ProductionTypes)
        {
            if ((type.IsClass || type.IsInterface) && !Regex.IsMatch(
                    type.Name.Split('`')[0], PascalCasePattern))
                errors.Add($"Type: {type.FullName}");

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                          BindingFlags.Static | BindingFlags.Instance |
                                          BindingFlags.DeclaredOnly);

            foreach (var method in methods.Where(m => !m.IsSpecialName && !m.Name.Contains('.') &&
                                                       !m.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false)))
            {
                if (!Regex.IsMatch(method.Name, PascalCasePattern))
                    errors.Add($"Method: {type.FullName}.{method.Name}");
            }
        }

        // Assert
        errors.Should().BeEmpty("production class, interface and method identifiers use PascalCase. Violations: {0}", string.Join("; ", errors.Distinct()));
    }
}
