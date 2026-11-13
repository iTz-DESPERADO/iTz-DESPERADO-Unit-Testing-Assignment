using FluentAssertions.Execution;
using NetArchTest.Rules;

namespace ParkSmart.ArchitectureTests;

public sealed class DependencyRulesTests
{
    [Fact]
    public void Domain_Dependencies_ExcludeOtherParkSmartProjects()
    {
        // Arrange
        var assembly = ArchitectureAssemblies.Domain;

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "ParkSmart.Application", "ParkSmart.Infrastructure", "ParkSmart.ConsoleApp")
            .GetResult();
        var references = ArchitectureAssemblies.OtherParkSmartReferences(assembly, "ParkSmart.Domain").ToArray();

        // Assert
        using var scope = new AssertionScope();
        references.Should().BeEmpty("Domain must not reference another ParkSmart project");
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Dependencies_ExcludeInfrastructureAndConsoleApp()
    {
        // Arrange
        var assembly = ArchitectureAssemblies.Application;

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny("ParkSmart.Infrastructure", "ParkSmart.ConsoleApp")
            .GetResult();
        var references = ArchitectureAssemblies.OtherParkSmartReferences(
            assembly, "ParkSmart.Application", "ParkSmart.Domain").ToArray();

        // Assert
        using var scope = new AssertionScope();
        references.Should().BeEmpty("Application can reference Domain only");
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_Dependencies_ExcludeConsoleApp()
    {
        // Arrange
        var assembly = ArchitectureAssemblies.Infrastructure;

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOn("ParkSmart.ConsoleApp")
            .GetResult();
        var references = ArchitectureAssemblies.OtherParkSmartReferences(
            assembly, "ParkSmart.Infrastructure", "ParkSmart.Application", "ParkSmart.Domain").ToArray();

        // Assert
        using var scope = new AssertionScope();
        references.Should().BeEmpty("Infrastructure must not reference ConsoleApp");
        result.IsSuccessful.Should().BeTrue();
    }
}
