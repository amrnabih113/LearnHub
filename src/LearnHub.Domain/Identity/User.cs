using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.RegularExpressions;
using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Identity;


public class User : AuditableEntity
{
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string FullName => $"{FirstName} {LastName}";

    public string PhoneNumber { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public List<Role> Roles { get; private set; } = [];
    public string? ImageUrl { get; private set; } = default!;
    public DateOnly? DateOfBirth { get; private set; }
    public string? Bio { get; private set; } = default!;
    public string? Country { get; private set; } = default!;

    private User() { }

    private User(Guid id, string firstName, string lastName, string email, string phoneNumber, Role role, string? imageUrl = null, DateOnly? dateOfBirth = null, string? bio = null, string? country = null)
    : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Roles.Add(role);
        ImageUrl = imageUrl;
        DateOfBirth = dateOfBirth;
        Bio = bio;
        Country = country;
    }

    public static Result<User> Create(Guid id, string firstName, string lastName, string email, string phoneNumber, Role role, string? imageUrl = null, DateOnly? dateOfBirth = null, string? bio = null, string? country = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return UserErrors.FirstNameRequired;
        }
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return UserErrors.LastNameRequired;
        }
        if (string.IsNullOrWhiteSpace(email))
        {
            return UserErrors.EmailRequired;
        }
        try
        {
            _ = new MailAddress(email);
        }
        catch (FormatException)
        {
            return UserErrors.InvalidEmail;
        }
        if (string.IsNullOrWhiteSpace(phoneNumber) || !Regex.IsMatch(phoneNumber, @"^\+?\d{7,15}$"))
        {
            return UserErrors.PhoneNumberRequired;
        }

        if (!Enum.IsDefined(typeof(Role), role))
        {
            return UserErrors.InvalidRole;
        }
        return new User(id, firstName, lastName, email, phoneNumber, role, imageUrl, dateOfBirth, bio, country);
    }

    public Result<Updated> Update(string firstName, string lastName, Role role, string email, string phoneNumber, string? imageUrl = null, DateOnly? dateOfBirth = null, string? bio = null, string? country = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return UserErrors.FirstNameRequired;
        }
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return UserErrors.LastNameRequired;
        }
        if (string.IsNullOrWhiteSpace(email))
        {
            return UserErrors.EmailRequired;
        }
        try
        {
            _ = new MailAddress(email);
        }
        catch (FormatException)
        {
            return UserErrors.InvalidEmail;
        }
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return UserErrors.PhoneNumberRequired;
        }
        if (!Regex.IsMatch(phoneNumber, @"^\+?\d{7,15}$"))
        {
            return UserErrors.InvalidPhoneNumber;
        }

        if (!Enum.IsDefined(typeof(Role), role))
        {
            return UserErrors.InvalidRole;
        }

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        ImageUrl = imageUrl;
        DateOfBirth = dateOfBirth;
        Bio = bio;
        Roles.Add(role);
        Country = country;

        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> AssignRole(Role role)
    {
        if (!Enum.IsDefined(typeof(Role), role))
        {
            return UserErrors.InvalidRole;
        }

        if (!Roles.Contains(role))
        {
            Roles.Add(role);
            UpdatedAtUtc = DateTimeOffset.UtcNow;
        }

        return Result.Updated;
    }
}