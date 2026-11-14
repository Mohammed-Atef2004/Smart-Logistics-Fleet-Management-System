using System.Reflection;
using Domain.SharedKernel;
using Infrastructure.Persistence;
using Application;
using API;
using NetArchTest.Rules;

namespace Architecture.Tests;

/// <summary>
/// Layer dependency rules for SLFMS.
///
/// Allowed dependencies:
///   Domain          -> MediatR only (DomainEvent : INotification)
///   Application     -> Domain, MediatR, FluentValidation, AutoMapper
///   Infrastructure  -> Application, Domain, EF Core, ASP.NET Identity
///   API             -> Application, Infrastructure (composition root)
///
/// Known technical debt (tracked in ROADMAP.md, "Phase 3"):
///   Application handlers still use EF Core Include() for eager loading —
///   the Application layer must NOT be granted an EF Core exemption here;
///   the dependency must be removed instead.
/// </summary>
public class LayerDependencyTests
{
    private static readonly Assembly DomainAssembly = typeof(AggregateRoot<>).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(ApplicationAssemblyMarker).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(DependencyInjection).Assembly;
    private static readonly Assembly ApiAssembly = typeof(Program).Assembly;

    [Fact]
    public void Domain_Should_Not_Depend_On_Other_Layers_Or_Frameworks()
    {
        // The Domain layer must be self-contained.
        // The ONLY sanctioned external dependency is MediatR (for DomainEvent : INotification).
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Application",
                "Infrastructure",
                "WebApi",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "AutoMapper")
            .GetResult();

        Assert.True(result.IsSuccessful, BuildFailureMessage("Domain", result));
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure_Or_Api()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Infrastructure",
                "WebApi")
            .GetResult();

        Assert.True(result.IsSuccessful, BuildFailureMessage("Application", result));
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Api()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("WebApi")
            .GetResult();

        Assert.True(result.IsSuccessful, BuildFailureMessage("Infrastructure", result));
    }

    [Fact]
    public void Application_Handlers_Should_Be_Sealed()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful, BuildFailureMessage("Application handlers", result));
    }

    [Fact]
    public void Domain_Aggregates_Should_Be_Sealed()
    {
        // Concrete aggregate roots must be sealed; the abstract base (AggregateRoot<T>) is exempt.
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .ImplementInterface(typeof(IAggregateRoot))
            .And()
            .AreNotAbstract()
            .Should()
            .BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful, BuildFailureMessage("Domain aggregates", result));
    }

    private static string BuildFailureMessage(string subject, TestResult result)
    {
        var failing = result.FailingTypeNames is { Count: > 0 }
            ? string.Join(Environment.NewLine, result.FailingTypeNames.Select(n => $"  - {n}"))
            : "  (no details reported)";

        return $"{subject} violates the architecture rules.{Environment.NewLine}{failing}";
    }
}
