using System;
using FluentAssertions;
using Domain.Claims.ValueObjects;
using Xunit;

public class ClaimAmountTests
{
    // =========================
    // Creation
    // =========================

    [Fact]
    public void Of_Should_Create_Valid_Amount()
    {
        var amount = ClaimAmount.Of(100, "USD");

        amount.Value.Should().Be(100);
        amount.Currency.Should().Be("USD");
    }

    [Fact]
    public void Of_Should_Throw_When_Currency_Is_Empty()
    {
        Action act = () => ClaimAmount.Of(100, "");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Currency cannot be empty*");
    }

    [Fact]
    public void Of_Should_Throw_When_Value_Is_Negative()
    {
        Action act = () => ClaimAmount.Of(-10, "USD");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Amount cannot be negative*");
    }

    [Fact]
    public void Of_Should_Normalize_Currency_To_Upper()
    {
        var amount = ClaimAmount.Of(50, "usd");

        amount.Currency.Should().Be("USD");
    }

    // =========================
    // IsPositive
    // =========================

    [Fact]
    public void IsPositive_Should_Return_True_When_Value_Greater_Than_Zero()
    {
        var amount = ClaimAmount.Of(10, "USD");

        amount.IsPositive().Should().BeTrue();
    }

    [Fact]
    public void IsPositive_Should_Return_False_When_Zero()
    {
        var amount = ClaimAmount.Of(0, "USD");

        amount.IsPositive().Should().BeFalse();
    }

    // =========================
    // Comparisons
    // =========================

    [Fact]
    public void IsGreaterThan_Should_Return_True_When_Larger()
    {
        var a = ClaimAmount.Of(200, "USD");
        var b = ClaimAmount.Of(100, "USD");

        a.IsGreaterThan(b).Should().BeTrue();
    }

    [Fact]
    public void IsGreaterThan_Should_Throw_When_Different_Currency()
    {
        var a = ClaimAmount.Of(200, "USD");
        var b = ClaimAmount.Of(100, "EGP");

        Action act = () => a.IsGreaterThan(b);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Currency mismatch*");
    }

    // =========================
    // Add / Subtract
    // =========================

    [Fact]
    public void Add_Should_Return_Sum_When_Same_Currency()
    {
        var a = ClaimAmount.Of(100, "USD");
        var b = ClaimAmount.Of(50, "USD");

        var result = a.Add(b);

        result.Value.Should().Be(150);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Add_Should_Throw_When_Currency_Differs()
    {
        var a = ClaimAmount.Of(100, "USD");
        var b = ClaimAmount.Of(50, "EGP");

        Action act = () => a.Add(b);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Currency mismatch*");
    }

    [Fact]
    public void Subtract_Should_Work_When_Valid()
    {
        var a = ClaimAmount.Of(100, "USD");
        var b = ClaimAmount.Of(40, "USD");

        var result = a.Subtract(b);

        result.Value.Should().Be(60);
    }

    [Fact]
    public void Subtract_Should_Throw_When_Result_Negative()
    {
        var a = ClaimAmount.Of(50, "USD");
        var b = ClaimAmount.Of(100, "USD");

        Action act = () => a.Subtract(b);

        act.Should().Throw<InvalidOperationException>();
    }

    
}