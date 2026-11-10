using FluentAssertions;
using FluentAssertions.Execution;
using NetArchTest.Rules;

namespace DriveNow.ArchitectureTests;

public sealed class DependencyRulesTests
{
    // R1: Domain must be independent of every other DriveNow project
    [Fact]
    public void Domain_Dependencies_ExcludeOtherDriveNowProjects()
    {
        // Arrange
        var assembly = ArchitectureAssemblies.Domain;

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "DriveNow.Application", "DriveNow.Infrastructure", "DriveNow.ConsoleApp")
            .GetResult();

        var assemblyReferences = ArchitectureAssemblies.OtherDriveNowReferences(
            ArchitectureAssemblies.Domain, "DriveNow.Domain").ToArray();

        // Assert
        using var scope = new AssertionScope();
        assemblyReferences.Should().BeEmpty("Domain must not reference other DriveNow assemblies");
        result.IsSuccessful.Should().BeTrue(
            $"R1 offending types: {string.Join(", ", result.FailingTypes?.Select(t => t.FullName) ?? [])}");
    }

    // R2: Application may reference Domain, but not Infrastructure or ConsoleApp
    [Fact]
    public void Application_Dependencies_IncludeOnlyDomain()
    {
        // Arrange
        var assembly = ArchitectureAssemblies.Application;

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny("DriveNow.Infrastructure", "DriveNow.ConsoleApp")
            .GetResult();

        var assemblyReferences = ArchitectureAssemblies.OtherDriveNowReferences(
            ArchitectureAssemblies.Application, "DriveNow.Application", "DriveNow.Domain").ToArray();

        // Assert
        using var scope = new AssertionScope();
        assemblyReferences.Should().BeEmpty("Application can reference Domain, but not Infrastructure or ConsoleApp");
        result.IsSuccessful.Should().BeTrue(
            $"R2 offending types: {string.Join(", ", result.FailingTypes?.Select(t => t.FullName) ?? [])}");
    }

    // R3: Infrastructure must not depend on ConsoleApp
    [Fact]
    public void Infrastructure_Dependencies_ExcludeConsoleApp()
    {
        // Arrange
        var assembly = ArchitectureAssemblies.Infrastructure;

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOn("DriveNow.ConsoleApp")
            .GetResult();

        var offendingReferences = ArchitectureAssemblies.Infrastructure
            .GetReferencedAssemblies()
            .Where(a => a.Name == "DriveNow.ConsoleApp")
            .Select(a => a.Name!)
            .ToArray();

        // Assert
        using var scope = new AssertionScope();
        offendingReferences.Should().BeEmpty("Infrastructure must not reference ConsoleApp");
        result.IsSuccessful.Should().BeTrue(
            $"R3 offending types: {string.Join(", ", result.FailingTypes?.Select(t => t.FullName) ?? [])}");
    }
}
