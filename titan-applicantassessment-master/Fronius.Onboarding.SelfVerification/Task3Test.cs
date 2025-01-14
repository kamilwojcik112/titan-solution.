using System;
using FluentAssertions;
using Fronius.Onboarding.Repositories;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Fronius.Onboarding.SelfVerification;

public class Task3Test
{
    [SetUp]
    public void Setup()
    {
    }

    /// <summary>
    /// Task 3)
    /// The repository seems to work, and the developers in your team love it. However, the nice people from Testing
    /// identified that the repository does not yet meet all requirements. Sometimes controls are issued that are
    /// duplicates of previously added Controls. This is a waste of memory and breaks other logic.
    /// To solve this problem: implement deduplication logic that prevents duplicates in the Control Repository
    /// you have implemented.
    ///
    /// When are 2 Controls the same Control?
    /// Controls are the same if the Begin Time, the End Time and the Origin is exactly the same
    ///
    /// Extra-Points (optional):
    /// - In case of a newly added Control is deduplicated, log to Console that a Control was deduplicated.
    /// - You fear, that other arbitrary restrictions (that prevent Controls from being added) will be added soon,
    ///   design the solution so it's easy extensible for up to 10 other rules, without producing Spaghetti Code.
    ///   Feel free to discuss this with the people in the room with you ;)
    /// </summary>
    [Test]
    public void Test3()
    {
        IControlRepository? controlRepository = PrepareControlRepository();

        var now = DateTime.UtcNow;

        // adding differing Controls (sharing similar Origin)
        controlRepository!
            .AddControl(new TestControl { Begin = now, End = now, Origin = "1" })
            .AddControl(new TestControl { Begin = now, End = now.AddDays(1), Origin = "1" })
            .AddControl(new TestControl { Begin = now.AddDays(1), End = now.AddDays(2), Origin = "1" });

        // adding differing Controls (sharing same start Time)
        controlRepository
            .AddControl(new TestControl { Begin = now, End = now.AddDays(1), Origin = "1.1" })
            .AddControl(new TestControl { Begin = now, End = now, Origin = "1.2" })
            .AddControl(new TestControl { Begin = now, End = now, Origin = "1.3" });

        // adding conflicting controls that should be deduplicated (same ref)
        var sameReferenceControl = new TestControl { Begin = now, End = now, Origin = "dup1" };
        controlRepository.AddControl(sameReferenceControl);
        controlRepository.AddControl(sameReferenceControl);
        controlRepository.AddControl(sameReferenceControl);

        // adding conflicting controls that should be deduplicated
        controlRepository
            .AddControl(new TestControl { Begin = now, End = now, Origin = "dup2" })
            .AddControl(new TestControl { Begin = now, End = now, Origin = "dup2" })
            .AddControl(new TestControl { Begin = now, End = now, Origin = "dup2" });

        var resutl = controlRepository.GetAll();

        // Assert
        controlRepository.GetAll().Should().ContainSingle(x => x.Origin == "dup1");
        controlRepository.GetAll().Should().ContainSingle(x => x.Origin == "dup2");
    }

    [Test]
    public void GetAll_AddedDifferingControlsWithSimilarOrigin_ShouldReturnAll()
    {
        //Arrange
        IControlRepository? controlRepository = PrepareControlRepository();
        var now = DateTime.UtcNow;

        //Act
        controlRepository!
            .AddControl(new TestControl { Begin = now, End = now, Origin = "1" })
            .AddControl(new TestControl { Begin = now, End = now.AddDays(1), Origin = "1" })
            .AddControl(new TestControl { Begin = now.AddDays(1), End = now.AddDays(2), Origin = "1" });

        //Assert
        controlRepository.GetAll().Count.Equals(3);
        controlRepository.GetAll().Should().Contain(x => x.Origin == "1");
    }

    [Test]
    public void GetAll_AddedDifferingControlsWithSimilarStartTime_ShouldReturnAll()
    {
        //Arrange
        IControlRepository? controlRepository = PrepareControlRepository();
        var now = DateTime.UtcNow;

        //Act
        controlRepository
            .AddControl(new TestControl { Begin = now, End = now.AddDays(1), Origin = "1.1" })
            .AddControl(new TestControl { Begin = now, End = now, Origin = "1.2" })
            .AddControl(new TestControl { Begin = now, End = now, Origin = "1.3" });

        //Assert
        controlRepository.GetAll().Count.Equals(3);
        controlRepository.GetAll().Should().Contain(x => x.Begin == now);
    }

    [Test]
    public void GetAll_AddedSameControlMultipleTimes_ShouldReturnOneDeduplicated()
    {
        //Arrange
        IControlRepository? controlRepository = PrepareControlRepository();
        var now = DateTime.UtcNow;

        //Act
        // adding conflicting controls that should be deduplicated (same ref)
        var sameReferenceControl = new TestControl { Begin = now, End = now, Origin = "dup1" };
        controlRepository.AddControl(sameReferenceControl);
        controlRepository.AddControl(sameReferenceControl);
        controlRepository.AddControl(sameReferenceControl);

        //Assert
        controlRepository.GetAll().Count.Equals(1);
        controlRepository.GetAll().Should().ContainSingle(x => x.Origin == "dup1");
    }

    [Test]
    public void GetAll_AddedDifferingControlsWithSameFieldValues_ShouldReturnOneDeduplicated()
    {
        //Arrange
        IControlRepository? controlRepository = PrepareControlRepository();
        var now = DateTime.UtcNow;

        //Act
        controlRepository
            .AddControl(new TestControl { Begin = now, End = now, Origin = "dup2" })
            .AddControl(new TestControl { Begin = now, End = now, Origin = "dup2" })
            .AddControl(new TestControl { Begin = now, End = now, Origin = "dup2" });

        //Assert
        controlRepository.GetAll().Count.Equals(1);
        controlRepository.GetAll().Should().ContainSingle(x => x.Origin == "dup2");
    }

    private static IControlRepository? PrepareControlRepository()
    {
        var controlRepository = DependencyInjection
                    .BuildDependencyInjectionContainer()
                    .BuildServiceProvider()?
                    .GetRequiredService<IControlRepository>();
        controlRepository.Should().NotBeNull();
        return controlRepository;
    }
}