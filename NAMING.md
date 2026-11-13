# Naming Conventions

Name: Gamal Khaled Mohamed
Group / ID: G4

### R1 - Name Of Tests

### Test project names must follow :

New test projects must follow one of these naming patterns:

- `<ProjectName>.UnitTests`
- `<ProjectName>.ArchitectureTests`

### Test Classes
Test class names must end with `Tests`

Example: RentalMathTests

### Test Files
Test file names must match their test class names.

Example: RentalMathTests.cs



## R2 — Test Method Naming

### Naming Styles Comparison

**1. Method_Scenario_ExpectedResult**

- Shows the method being tested

- Describes the test scenario

- Makes the expected result clear

- Easy to enforce using Regex

**2. Should_X_When_Y**

- Focuses on expected behavior

- Reads like a natural sentence

- Does not always identify the method being tested

**3. Given_When_Then**

- Describes the initial condition, action, and outcome

- Useful for behavior-driven tests

- Can produce long method names

### Chosen Convention

I chose `Method_Scenario_ExpectedResult`

Reasons:

- It clearly identifies the method under test

- It describes the scenario and expected behavior

- It makes test failures easier to understand

- It can be validated using a naming test

### Examples from DriveNow

1. `CalculateAge_ValidDate_ReturnsCorrectAge`

2. `CalculateLicenseYears_ValidDate_ReturnsCorrectLicenseYears`

3. `CalculateRentalDays_WithinGracePeriod_ReturnsOneDay`



## R3 — Test Doubles and Test Data Naming

### Hand-Written Test Doubles

Hand-written test doubles must use a prefix that identifies their type.

| Type | Naming Convention | Example |
|---|---|---|
| Dummy | Dummy + DependencyName | DummyEmailSender |
| Stub | Stub + DependencyName | StubPaymentGateway |
| Spy | Spy + DependencyName | SpySmsSender |
| Mock | Mock + DependencyName | MockPaymentGateway |
| Fake | Fake + DependencyName | FakeBookingRepository |

### Bogus Fakers

Bogus data generator classes must end with `Faker`

Examples:
- `CustomerFaker`

- `VehicleFaker`

### AutoFixture Customizations

AutoFixture customization classes must end with `Customization`

Example:
- `ValidAdultCustomerCustomization`

### AutoFixture Attributes

Custom AutoFixture attribute classes must end with `Attribute`

Example:
- `AutoMoqDataAttribute`

### General Rules

- Class names must use PascalCase

- Test helper names must follow these conventions in both DriveNow and ParkSmart



## R4 — Production Code Naming

### General Rules

- All class, interface, and method names must use PascalCase

### Interfaces

- Interface names must start with `I`

- Interfaces must use PascalCase

Examples:
- `IBookingRepository`

- `IPaymentGateway`

- `ICurrencyFormatter`

### Interface Namespaces

- Domain-level interfaces must live in `<ProjectName>.Domain.Interfaces`

- Application-level interfaces must live in `<ProjectName>.Application.Interfaces`

- Interfaces must not create dependencies from Domain to Application or Infrastructure

### Repositories

- Repository class names must end with `Repository`

- Repository implementations must be placed in a `Repositories` namespace

Examples:
- `DriveNow.Infrastructure.Repositories.BookingRepository`

- `DriveNow.Infrastructure.Repositories.CustomerRepository`

### Services

- Service class names must end with `Service`

- Service classes must be placed in a `Services` namespace

Examples:
- `DriveNow.Application.Services.BookingService`

- `DriveNow.Application.Services.PricingService`

- `DriveNow.Domain.Services.RentalMathService`

### Exceptions

- Exception class names must end with `Exception`

- Domain exceptions must be placed in the `DriveNow.Domain.Exceptions` namespace

Examples:
- `BookingNotFoundException`

- `CustomerNotFoundException`

- `InvalidBookingStateException`


