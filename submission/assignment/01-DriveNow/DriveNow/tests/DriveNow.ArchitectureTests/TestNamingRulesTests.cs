using System.Reflection;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace DriveNow.ArchitectureTests;

public sealed class TestNamingRulesTests
{
    private const string MethodPattern = "^[A-Z][A-Za-z0-9]*_[A-Z][A-Za-z0-9]*_[A-Z][A-Za-z0-9]*$";

    private static bool IsTestMethod(MethodInfo method) =>
        method.IsDefined(typeof(FactAttribute), inherit: true) ||
        method.IsDefined(typeof(TheoryAttribute), inherit: true);

    private static Type[] TestClasses(Assembly assembly) => assembly.GetTypes()
        .Where(type => type.IsClass && !type.IsAbstract &&
            type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Static | BindingFlags.Instance)
                .Any(IsTestMethod))
        .ToArray();

    // NAMING.md R1: new project names follow <Project>.UnitTests or <Project>.ArchitectureTests
    // The original starter's DriveNow.LegacyTests is explicitly excluded: it is not a NEW test project
    [Fact]
    public void TestProjects_NewProjectNames_FollowConvention()
    {
        // Arrange
        var assemblies = new[] { ArchitectureAssemblies.UnitTests, typeof(TestNamingRulesTests).Assembly };

        // Act
        var errors = assemblies.Select(assembly => assembly.GetName().Name!)
            .Where(name => !Regex.IsMatch(name, "^DriveNow\\.(UnitTests|ArchitectureTests)$"))
            .ToArray();

        // Assert
        errors.Should().BeEmpty("new DriveNow test projects must follow NAMING.md R1. Violations: {0}", string.Join("; ", errors));
    }

    // NAMING.md R1: every test class ends with Tests, including architecture test classes
    [Fact]
    public void TestClasses_ClassNames_EndWithTests()
    {
        // Arrange
        var assemblies = new[] { ArchitectureAssemblies.UnitTests, typeof(TestNamingRulesTests).Assembly };

        // Act
        var errors = assemblies.SelectMany(TestClasses)
            .Where(t => !t.Name.EndsWith("Tests", StringComparison.Ordinal))
            .Select(t => t.FullName ?? t.Name)
            .ToArray();

        // Assert
        errors.Should().BeEmpty("test class names must end with Tests. Violations: {0}", string.Join("; ", errors));
    }

    // NAMING.md R1: each test class must have a corresponding <ClassName>.cs source file
    [Fact]
    public void TestFiles_ClassNames_MatchSourceFileNames()
    {
        // Arrange
        var assembly = typeof(TestNamingRulesTests).Assembly;
        var testClasses = new[] { ArchitectureAssemblies.UnitTests, assembly }
            .SelectMany(TestClasses).Select(type => type.Name).ToHashSet(StringComparer.Ordinal);
        var errors = new List<string>();
        var found = new HashSet<string>(StringComparer.Ordinal);

        // Act
        foreach (var resource in assembly.GetManifestResourceNames()
                     .Where(name => name.StartsWith("NamingSources.", StringComparison.Ordinal)))
        {
            using var stream = assembly.GetManifestResourceStream(resource)!;
            using var reader = new StreamReader(stream);
            var source = reader.ReadToEnd();
            var fileName = resource.Split('.')[^2];

            foreach (Match match in Regex.Matches(source, @"\bclass\s+(\w+)"))
            {
                var className = match.Groups[1].Value;
                if (!testClasses.Contains(className))
                    continue;

                found.Add(className);
                if (className != fileName)
                    errors.Add($"{className} is declared in {fileName}.cs");
            }
        }

        errors.AddRange(testClasses.Except(found).Select(name => $"Missing embedded source for {name}"));

        // Assert
        errors.Should().BeEmpty("test source file names must match the test class names. Violations: {0}", string.Join("; ", errors));
    }

    // Task 3 R5 + NAMING.md R2 use Reflection and Regex (3 PascalCase segments)
    [Fact]
    public void UnitTestMethods_FactOrTheoryNames_FollowMethodScenarioExpectedResult()
    {
        // Arrange
        var types = ArchitectureAssemblies.UnitTests.GetTypes();

        // Act
        var errors = types
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                                 BindingFlags.Static | BindingFlags.Instance |
                                                 BindingFlags.DeclaredOnly)
                .Where(IsTestMethod)
                .Select(method => $"{type.FullName}.{method.Name}"))
            .Where(name => !Regex.IsMatch(name.Split('.').Last(), MethodPattern))
            .ToArray();

        // Assert
        errors.Should().BeEmpty("[Fact]/[Theory] methods must use Method_Scenario_ExpectedResult. Violations: {0}", string.Join("; ", errors));
    }


    // NAMING.md R3 general rule: all hand-written test helper class names use PascalCase
    [Fact]
    public void UnitTestHelpers_ClassNames_UsePascalCase()
    {
        // Arrange
        var types = ArchitectureAssemblies.UnitTests.GetTypes();

        // Act
        var errors = types
            .Where(t => t.IsClass && !t.Name.StartsWith('<') &&
                !t.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false))
            .Where(t => !Regex.IsMatch(t.Name.Split('`')[0], "^[A-Z][A-Za-z0-9]*$"))
            .Select(t => t.FullName ?? t.Name)
            .ToArray();

        // Assert
        errors.Should().BeEmpty("test helper class names must use PascalCase. Violations: {0}", string.Join("; ", errors));
    }

    // NAMING.md R3: hand-written doubles, Bogus fakers, AutoFixture customizations, attributes
    [Fact]
    public void UnitTestHelpers_DoubleAndGeneratorNames_FollowConvention()
    {
        // Arrange
        var errors = new List<string>();

        // Act
        foreach (var type in ArchitectureAssemblies.UnitTests.GetTypes()
                     .Where(t => t.IsClass && !t.IsAbstract && !t.Name.StartsWith('<')))
        {
            var codeInterfaces = type.GetInterfaces()
                .Any(i => i.Assembly.GetName().Name?.StartsWith("DriveNow.", StringComparison.Ordinal) == true
                       && !i.Assembly.GetName().Name!.EndsWith("Tests", StringComparison.Ordinal));

            var inDoubleFolder = type.Namespace?.Split('.').Any(segment =>
                segment is "TestDoubles" or "Doubles" or "Fakes" or "Mocks" or
                           "Stubs" or "Spies" or "Dummies") == true;

            if ((codeInterfaces || inDoubleFolder) &&
                !Regex.IsMatch(type.Name, "^(Dummy|Stub|Spy|Mock|Fake)[A-Z][A-Za-z0-9]*$"))
                errors.Add($"Test double: {type.FullName}");

            if (ArchitectureAssemblies.HasBaseType(type, "Bogus.Faker") &&
                !type.Name.EndsWith("Faker", StringComparison.Ordinal))
                errors.Add($"Bogus faker: {type.FullName}");

            if (type.GetInterfaces().Any(i => i.FullName == "AutoFixture.ICustomization") &&
                !type.Name.EndsWith("Customization", StringComparison.Ordinal))
                errors.Add($"AutoFixture customization: {type.FullName}");

            if (ArchitectureAssemblies.HasBaseType(type, "AutoFixture.Xunit2.AutoDataAttribute") &&
                !type.Name.EndsWith("Attribute", StringComparison.Ordinal))
                errors.Add($"AutoFixture attribute: {type.FullName}");
        }

        // Assert
        errors.Should().BeEmpty("test doubles and generators must follow NAMING.md R3. Violations: {0}", string.Join("; ", errors));
    }
}
