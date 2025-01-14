using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Fronius.Onboarding.Repositories;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Fronius.Onboarding.SelfVerification;

public class Task1Test
{
    /// <summary>
    /// Task 1)
    /// Implement a Control-Repository based on the "IControlRepository" interface.
    /// Use the existing DependencyInjection mechanism to register this Dependency.
    ///
    /// Developer-Hints:
    /// - The Repository will be used from multiple threads in the application
    /// </summary>

    [Test]
    public void Test1()
    {
        var controlRepository = BuildControlRepository();

        var now = DateTime.UtcNow;
        var testControl = new TestControl { Begin = now, End = now.AddMinutes(5) };

        controlRepository!.GetAll().Should().BeEmpty();
        controlRepository!.AddControl(testControl);
        controlRepository!.GetAll().Should().HaveCount(1);
        controlRepository!.GetAll().Should().Contain(e => e.Begin == testControl.Begin && e.End == testControl.End);
    }

    [Test]
    public void Add_CorrectControlValue_ShouldReturnSameControlValue()
    {
        //Arrange
        var controlRepository = BuildControlRepository();
        var now = DateTime.UtcNow;
        var testControl = new TestControl { Begin = now, End = now.AddMinutes(5) };
        controlRepository!.GetAll().Should().BeEmpty();

        //Act
        controlRepository!.AddControl(testControl);

        //Assert
        controlRepository!.GetAll().Should().HaveCount(1);
        controlRepository!.GetAll().Should().Contain(e => e.Begin == testControl.Begin && e.End == testControl.End);
    }

    [TestCase(100)]
    [TestCase(999)]
    [TestCase(1)]
    [TestCase(0)]
    public async Task Add_AddsMultipleCorrectValuesConcurrently_ShouldAddThreadSafelyAllValues(int numberOfItems)
    {
        //Arrange
        var controlRepository = BuildControlRepository();
        var tasks = PrepareAddTasks(numberOfItems, controlRepository);

        //Act
        await Task.WhenAll(tasks);

        //Assert
        controlRepository.GetAll().Count.Should().Be(numberOfItems);
    }

    public async Task GetAll_GetsMultipleValuesConcurrently_ShouldGetThreadSafelyAllValues()
    {
        //Arrange
        var controlRepository = BuildControlRepository();
        int numberOfItems = 100;
        PrepareRepositoryWithControls(numberOfItems, controlRepository);
        var tasks = PrepareGetTasks(numberOfItems, controlRepository);

        //Act
        Task testTasks = Task.WhenAll(tasks);

        try
        {
            await testTasks;
        }
        catch (Exception ex)
        {
            Assert.Fail($"One or more tasks failed with exception: {ex.Message}");
        }

        //Assert
        testTasks.IsCompletedSuccessfully.Should().BeTrue();
    }

    private List<Task> PrepareAddTasks(int numberOfItems, IControlRepository controlRepository)
    {
        var tasks = new List<Task>();
        var now = DateTime.UtcNow;

        for (int i = 0; i < numberOfItems; i++)
        {
            var testControl = new TestControl { Origin = "Test Origin", Begin = now, End = now.AddMinutes(i) };

            tasks.Add(Task.Run(() =>
            {
                controlRepository.AddControl(testControl);
            }));
        }

        return tasks;
    }

    private List<Task> PrepareGetTasks(int numberOfItems, IControlRepository controlRepository)
    {
        var tasks = new List<Task>();

        for (int i = 0; i < numberOfItems; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                controlRepository.GetAll();
            }));
        }

        return tasks;
    }

    private void PrepareRepositoryWithControls(int numberOfItems, IControlRepository controlRepository)
    {
        var tasks = new List<Task>();
        var now = DateTime.UtcNow;

        for (int i = 0; i < numberOfItems; i++)
        {
            var testControl = new TestControl { Origin = "Test Origin", Begin = now, End = now.AddMinutes(i) };
            controlRepository.AddControl(testControl);
        }
    }

    private IControlRepository BuildControlRepository()
    {
        var controlRepository = DependencyInjection
                .BuildDependencyInjectionContainer()?
                .BuildServiceProvider()?
                .GetRequiredService<IControlRepository>();

        controlRepository.Should().NotBeNull();

        return controlRepository;
    }
}