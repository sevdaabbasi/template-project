using Modules.Identity.Domain.Entities;
using Modules.Identity.Domain.Enums;
using Modules.Identity.Domain.Events;
using NUnit.Framework;

namespace Modules.Identity.Tests;

[TestFixture]
public class UserTests
{
    [Test]
    public void Create_WithFullRegistrationDetails_ShouldCreateActiveUserAndRaiseDomainEvent()
    {
        // Arrange
        var firstName = "Ahmet";
        var lastName = "Yılmaz";
        var phone = "05551234567";
        var email = "ahmet.yilmaz@example.com";
        var birthDate = new DateOnly(1995, 5, 20);
        var gender = Gender.Male;

        // Act
        var user = User.Create(firstName, lastName, phone, email, birthDate, gender);

        // Assert
        Assert.That(user.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(user.FirstName, Is.EqualTo("Ahmet"));
        Assert.That(user.LastName, Is.EqualTo("Yılmaz"));
        Assert.That(user.FullName, Is.EqualTo("Ahmet Yılmaz"));
        Assert.That(user.PhoneNumber, Is.EqualTo("+905551234567"));
        Assert.That(user.Email, Is.EqualTo("ahmet.yilmaz@example.com"));
        Assert.That(user.DateOfBirth, Is.EqualTo(birthDate));
        Assert.That(user.Gender, Is.EqualTo(Gender.Male));
        Assert.That(user.IsOnboarded, Is.False);
        Assert.That(user.IsActive, Is.True);
        Assert.That(user.DomainEvents.Count, Is.EqualTo(1));
        Assert.That(user.DomainEvents[0], Is.TypeOf<UserRegisteredDomainEvent>());
    }

    [TestCase("05321112233", "+905321112233")]
    [TestCase("5321112233", "+905321112233")]
    [TestCase("905321112233", "+905321112233")]
    [TestCase("+90 532 111 22 33", "+905321112233")]
    public void NormalizePhoneNumber_ShouldFormatToStandardTrFormat(string rawInput, string expected)
    {
        // Act
        var normalized = User.NormalizePhoneNumber(rawInput);

        // Assert
        Assert.That(normalized, Is.EqualTo(expected));
    }

    [Test]
    public void CompleteOnboarding_ShouldSetCityInterestsAndRaiseDomainEvent()
    {
        // Arrange
        var user = User.Create("Mehmet", "Demir", "5551112233", "mehmet@example.com", new DateOnly(1998, 1, 1), Gender.Male);
        user.ClearDomainEvents();

        var cityId = 34; // İstanbul
        var interestIds = new List<int> { 1, 2, 3 }; // Doğa, Kültür, Spor

        // Act
        user.CompleteOnboarding(cityId, interestIds);

        // Assert
        Assert.That(user.IsOnboarded, Is.True);
        Assert.That(user.CityId, Is.EqualTo(34));
        Assert.That(user.UserInterests.Count, Is.EqualTo(3));
        Assert.That(user.DomainEvents.Count, Is.EqualTo(1));
        Assert.That(user.DomainEvents[0], Is.TypeOf<UserOnboardedDomainEvent>());
    }

    [Test]
    public void UpdateProfile_ShouldModifyNameAndBioAndGender()
    {
        // Arrange
        var user = User.Create("Ali", "Kaya", "5559998877", "ali@example.com", new DateOnly(2000, 3, 15), Gender.Male);

        // Act
        user.UpdateProfile("Ali Veli", "Kaya", "Doğa aşığı gezgin", new DateOnly(2000, 3, 15), Gender.Male);

        // Assert
        Assert.That(user.FirstName, Is.EqualTo("Ali Veli"));
        Assert.That(user.LastName, Is.EqualTo("Kaya"));
        Assert.That(user.FullName, Is.EqualTo("Ali Veli Kaya"));
        Assert.That(user.Bio, Is.EqualTo("Doğa aşığı gezgin"));
    }

    [Test]
    public void FollowCounters_ShouldIncrementAndDecrementCorrectly()
    {
        // Arrange
        var user = User.Create("Zeynep", "Çelik", "5550001122", "zeynep@example.com", new DateOnly(1997, 7, 7), Gender.Female);

        // Act & Assert
        user.IncrementFollowers();
        user.IncrementFollowing();
        Assert.That(user.FollowersCount, Is.EqualTo(1));
        Assert.That(user.FollowingCount, Is.EqualTo(1));

        user.DecrementFollowers();
        user.DecrementFollowing();
        Assert.That(user.FollowersCount, Is.EqualTo(0));
        Assert.That(user.FollowingCount, Is.EqualTo(0));
    }
}
