using System.Reflection;
using System.Text.RegularExpressions;

namespace ParkSmart.ArchitectureTests;

public sealed class ProductionNamingRulesTests
{
    private const string PascalCasePattern = "^[A-Z][A-Za-z0-9]*$";

    [Fact]
    public void ProductionTypes_Names_UsePascalCase()
    {
        // Arrange
        var types = ArchitectureAssemblies.ProductionTypes;

        // Act
        var errors = types
            .Where(type => (type.IsClass || type.IsInterface || type.IsEnum) &&
                !Regex.IsMatch(type.Name.Split('`')[0], PascalCasePattern))
            .Select(type => type.FullName ?? type.Name)
            .ToArray();

        // Assert
        errors.Should().BeEmpty("production type names must follow NAMING.md R4");
    }

    [Fact]
    public void ProductionMethods_Names_UsePascalCase()
    {
        // Arrange
        var methods = ArchitectureAssemblies.ProductionTypes
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                                 BindingFlags.Static | BindingFlags.Instance |
                                                 BindingFlags.DeclaredOnly));

        // Act
        var errors = methods
            .Where(method => !method.IsSpecialName && !method.Name.Contains('.') &&
                !method.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false) &&
                !Regex.IsMatch(method.Name, PascalCasePattern))
            .Select(method => $"{method.DeclaringType!.FullName}.{method.Name}")
            .ToArray();

        // Assert
        errors.Should().BeEmpty("production method names must follow NAMING.md R4");
    }
}
