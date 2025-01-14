using System;
using FluentAssertions;
using Fronius.Onboarding.Filtering;
using Fronius.Onboarding.Repositories;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Fronius.Onboarding.SelfVerification;

public class Task2Test
{
    /// <summary>
    /// Task 2)
    /// Implement a ControlFilter implementation that is based on the IControlFilter Interface.
    /// It takes a List of Controls, and filters certain controls that are not supposed to be in the output
    ///
    /// Rules for the Filter:
    /// - Controls that started in the past, and finished in the past need to be removed
    /// - Controls that started in the past, but are still active (meaning having a date in the future) are NOT removed
    /// - Controls that start in the future, and will end in the future, are NOT removed
    /// - Invalid Controls (StartDate >= EndDate) are removed
    ///
    /// Extra-Points:
    /// - if possible, applicable use LINQ
    /// </summary>
    [Test]
    public void Test1()
    {
        IControlRepository? controlRepository;
        IControlFilter? controlFilter;
        PrepareTestServices(out controlRepository, out controlFilter);
        var now = DateTime.UtcNow;

        // started in past, finished in past
        controlRepository
            .AddControl(new TestControl { Begin = now.AddDays(-5), End = now.AddDays(-4), Origin = "Test" })
            .AddControl(new TestControl { Begin = now.AddDays(-3), End = now.AddDays(-2), Origin = "Test" });

        // starting in past, ending in future
        controlRepository
            .AddControl(new TestControl { Begin = now.AddDays(-2), End = now.AddDays(2), Origin = "Test" })
            .AddControl(new TestControl { Begin = now.AddHours(-1), End = now.AddDays(1), Origin = "Test" });

        // starting in future, ending in future
        controlRepository
            .AddControl(new TestControl { Begin = now.AddHours(1), End = now.AddDays(4), Origin = "Test" })
            .AddControl(new TestControl { Begin = now.AddHours(2), End = now.AddDays(5), Origin = "Test" });

        // error cases
        controlRepository
            .AddControl(new TestControl { Begin = now.AddHours(-1), End = now.AddDays(-4), Origin = "Test" })
            .AddControl(new TestControl { Begin = now.AddHours(4), End = now.AddDays(-5), Origin = "Test" });

        // Assert
        controlFilter!.Filter(controlRepository!.GetAll()).Should().NotContain(c => c.Begin <= now && c.End <= now);
        controlFilter!.Filter(controlRepository!.GetAll()).Should().Contain(c => c.Begin >= now && c.End >= now);
        controlFilter!.Filter(controlRepository!.GetAll()).Should().Contain(c => c.Begin <= now && c.End >= now);
    }

    [Test]
    public void Filter_ControlsStartedInThePastAndFinishedInThePast_ShouldBeRemoved()
    {
        IControlRepository? controlRepository;
        IControlFilter? controlFilter;
        PrepareTestServices(out controlRepository, out controlFilter);
        var now = DateTime.UtcNow;

        controlRepository
            .AddControl(new TestControl { Begin = now.AddDays(-5), End = now.AddDays(-4), Origin = "Test" })
            .AddControl(new TestControl { Begin = now.AddDays(-3), End = now.AddDays(-2), Origin = "Test" });

        var result = controlRepository!.GetAll();

        controlFilter!.Filter(result).Should().NotContain(c => c.Begin <= now && c.End <= now);
        controlFilter!.Filter(result).Count.Should().Be(0);
    }

    [Test]
    public void Filter_ControlsStartedInThePastAndFinishedInTheFuture_ShouldNotBeRemoved()
    {
        IControlRepository? controlRepository;
        IControlFilter? controlFilter;
        PrepareTestServices(out controlRepository, out controlFilter);
        var now = DateTime.UtcNow;

        controlRepository
            .AddControl(new TestControl { Begin = now.AddDays(-2), End = now.AddDays(2), Origin = "Test" })
            .AddControl(new TestControl { Begin = now.AddHours(-1), End = now.AddDays(1), Origin = "Test" });

        var result = controlRepository!.GetAll();

        controlFilter!.Filter(result).Should().Contain(c => c.Begin <= now && c.End >= now);
        controlFilter!.Filter(result).Count.Should().Be(2);
    }

    [Test]
    public void Filter_ControlsStartedInTheFutureAndFinishedInTheFuture_ShouldNotBeRemoved()
    {
        IControlRepository? controlRepository;
        IControlFilter? controlFilter;
        PrepareTestServices(out controlRepository, out controlFilter);
        var now = DateTime.UtcNow;

        controlRepository
            .AddControl(new TestControl { Begin = now.AddHours(1), End = now.AddDays(4), Origin = "Test" })
            .AddControl(new TestControl { Begin = now.AddHours(2), End = now.AddDays(5), Origin = "Test" });

        var result = controlRepository!.GetAll();

        controlFilter!.Filter(result).Should().Contain(c => c.Begin >= now && c.End >= now);
        controlFilter!.Filter(result).Count.Should().Be(2);
    }

    [Test]
    public void Filter_ControlsInvalidEndedBeforeStarted_ShouldBeRemoved()
    {
        IControlRepository? controlRepository;
        IControlFilter? controlFilter;
        PrepareTestServices(out controlRepository, out controlFilter);
        var now = DateTime.UtcNow;

        controlRepository
            .AddControl(new TestControl { Begin = now.AddHours(-1), End = now.AddDays(-4), Origin = "Test" })
            .AddControl(new TestControl { Begin = now.AddHours(4), End = now.AddDays(-5), Origin = "Test" });

        var result = controlRepository!.GetAll();

        controlFilter!.Filter(result).Should().NotContain(c => c.Begin <= c.End);
        controlFilter!.Filter(result).Count.Should().Be(0);
    }

    private void PrepareTestServices(out IControlRepository? controlRepository, out IControlFilter? controlFilter)
    {
        controlRepository = DependencyInjection
                    .BuildDependencyInjectionContainer()?
                    .BuildServiceProvider()?
                    .GetRequiredService<IControlRepository>();
        controlFilter = DependencyInjection
                    .BuildDependencyInjectionContainer()?
                    .BuildServiceProvider()?
                    .GetRequiredService<IControlFilter>();
        controlRepository.Should().NotBeNull();
        controlFilter.Should().NotBeNull();

        controlFilter!.Filter(controlRepository!.GetAll()).Should().BeEmpty();
    }
}