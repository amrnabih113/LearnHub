using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.RegularExpressions;
using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity.Events;

namespace LearnHub.Domain.Identity;


public class User : AuditableEntity
{
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string FullName => $"{FirstName} {LastName}";

    public string PasswordHash { get; private set; } = default!;
    public string? PhoneNumber { get; private set; } = default!;
    public string Email { get; private set; } = default!;

    public bool IsEmailVerified { get; private set; } = false;
    public LearnHub.Domain.Identity.Enums.AccountStatus Status { get; private set; } = LearnHub.Domain.Identity.Enums.AccountStatus.Active;
    private readonly List<UserRole> _roles = [];

    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();
    public string? ImageUrl { get; private set; } = default!;
    public DateOnly? DateOfBirth { get; private set; }
    public string? Bio { get; private set; } = default!;
    public string? Country { get; private set; } = default!;

    private User() { }

    private User(Guid id, string firstName, string lastName, string email, string passwordHash, Role role, string? phoneNumber = null, string? imageUrl = null, DateOnly? dateOfBirth = null, string? bio = null, string? country = null)
    : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        PhoneNumber = phoneNumber;
        IsEmailVerified = false;
        _roles.Add(UserRole.Create(id, role));
        ImageUrl = imageUrl;
        DateOfBirth = dateOfBirth;
        Bio = bio;
        Country = country;
    }

    public static Result<User> Create(Guid id, string firstName, string lastName, string email, string passwordHash, Role role, string? phoneNumber = null, string? imageUrl = null, DateOnly? dateOfBirth = null, string? bio = null, string? country = null)
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
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return UserErrors.PasswordHashRequired;
        }
        try
        {
            _ = new MailAddress(email);
        }
        catch (FormatException)
        {
            return UserErrors.InvalidEmail;
        }
        if (!string.IsNullOrWhiteSpace(phoneNumber) && !Regex.IsMatch(phoneNumber!, @"^\+?\d{7,15}$"))
        {
            return UserErrors.InvalidPhoneNumber;
        }

        if (!Enum.IsDefined(typeof(Role), role))
        {
            return UserErrors.InvalidRole;
        }

        var user = new User(id, firstName, lastName, email, passwordHash, role, phoneNumber, imageUrl, dateOfBirth, bio, country);
        user.AddDomainEvent(
          new UserCreatedDomainEvent(
          user.Id,
          user.Email));

        return user;
    }

    public Result<Updated> Update(string firstName, string lastName, Role role, string email, string passwordHash, string? phoneNumber = null, string? imageUrl = null, DateOnly? dateOfBirth = null, string? bio = null, string? country = null)
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
        if (!string.IsNullOrWhiteSpace(phoneNumber) && !Regex.IsMatch(phoneNumber, @"^\+?\d{7,15}$"))
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
        PasswordHash = passwordHash;
        PhoneNumber = phoneNumber;
        ImageUrl = imageUrl;
        DateOfBirth = dateOfBirth;
        Bio = bio;
        if (!_roles.Any(r => r.Role == role))
        {
            _roles.Add(UserRole.Create(Id, role));
        }
        Country = country;

        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> UpdateProfile(
        string firstName,
        string lastName,
        string? phoneNumber,
        DateOnly? dateOfBirth,
        string? bio,
        string? country)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return UserErrors.FirstNameRequired;

        if (string.IsNullOrWhiteSpace(lastName))
            return UserErrors.LastNameRequired;

        if (!string.IsNullOrWhiteSpace(phoneNumber) &&
            !Regex.IsMatch(phoneNumber, @"^\+?\d{7,15}$"))
        {
            return UserErrors.InvalidPhoneNumber;
        }

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        PhoneNumber = phoneNumber?.Trim();
        DateOfBirth = dateOfBirth;
        Bio = bio?.Trim();
        Country = country?.Trim();

        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
        {
            return UserErrors.PasswordHashRequired;
        }

        PasswordHash = newPasswordHash;
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> VerifyEmail()
    {
        if (IsEmailVerified)
        {
            return Result.Updated;
        }

        IsEmailVerified = true;
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> UpdateProfileImage(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return UserErrors.ImageUrlRequired;
        }

        ImageUrl = imageUrl;
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> AssignRole(Role role)
    {
        if (!Enum.IsDefined(role))
        {
            return UserErrors.InvalidRole;
        }

        if (_roles.Any(x => x.Role == role))
        {
            return Result.Updated;
        }

        _roles.Add(UserRole.Create(Id, role));
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> RemoveRole(Role role)
    {
        if (!Enum.IsDefined(role))
        {
            return UserErrors.InvalidRole;
        }

        var existingRole = _roles.FirstOrDefault(x => x.Role == role);
        if (existingRole is null)
        {
            return Result.Updated;
        }

        _roles.Remove(existingRole);
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> Suspend()
    {
        Status = LearnHub.Domain.Identity.Enums.AccountStatus.Suspended;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> Restore()
    {
        Status = LearnHub.Domain.Identity.Enums.AccountStatus.Active;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }
}