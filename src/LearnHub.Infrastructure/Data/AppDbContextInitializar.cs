using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Classification.Categories;
using LearnHub.Domain.Classification.Tags;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Purchasing.ValueObjects;
using LearnHub.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LearnHub.Infrastructure.Data;

public class AppDbContextInitializar(
    AppDbContext context,
    IPasswordHasher passwordHasher,
    ILogger<AppDbContextInitializar> logger)
{
    private readonly AppDbContext _context = context;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ILogger<AppDbContextInitializar> _logger = logger;

    public async Task InitializeAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }

        try
        {
            await SeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        // 1. Seed Categories if empty
        if (!await _context.Categories.AnyAsync())
        {
            var defaultCategories = new List<Category>
            {
                Category.Create(
                    id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    name: "Software Development",
                    slug: "software-development",
                    description: "Learn programming languages, frameworks, and web/mobile development.").Value,

                Category.Create(
                    id: Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    name: "Data Science & AI",
                    slug: "data-science-ai",
                    description: "Explore machine learning, data analysis, Python, and AI models.").Value,

                Category.Create(
                    id: Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    name: "Design & UX",
                    slug: "design-ux",
                    description: "Master UI/UX design, Figma, graphics, and web design.").Value,

                Category.Create(
                    id: Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    name: "Business & Marketing",
                    slug: "business-marketing",
                    description: "Digital marketing, product management, and business analytics.").Value
            };

            await _context.Categories.AddRangeAsync(defaultCategories);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded default categories.");
        }

        // 2. Seed Tags if empty
        if (!await _context.Tags.AnyAsync())
        {
            var defaultTags = new List<Tag>
            {
                Tag.Create(Guid.NewGuid(), "C#", "csharp", "C# programming language").Value,
                Tag.Create(Guid.NewGuid(), ".NET", "dotnet", ".NET framework and core").Value,
                Tag.Create(Guid.NewGuid(), "Web Development", "web-dev", "Web applications and APIs").Value,
                Tag.Create(Guid.NewGuid(), "React", "react", "React JS library").Value,
                Tag.Create(Guid.NewGuid(), "Python", "python", "Python programming language").Value
            };

            await _context.Tags.AddRangeAsync(defaultTags);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded default tags.");
        }

        // 3. Seed Subscription Plans if empty
        if (!await _context.SubscriptionPlans.AnyAsync())
        {
            var plans = new List<SubscriptionPlan>();

            var freePlan = SubscriptionPlan.Create(
                id: Guid.Parse("10101010-1010-1010-1010-101010101010"),
                name: "Free Plan",
                tier: SubscriptionTier.Free,
                billingCycle: BillingCycle.Monthly,
                price: Money.Create(0, "USD").Value).Value;

            var proPlan = SubscriptionPlan.Create(
                id: Guid.Parse("20202020-2020-2020-2020-202020202020"),
                name: "Pro Plan",
                tier: SubscriptionTier.Pro,
                billingCycle: BillingCycle.Monthly,
                price: Money.Create(19.99m, "USD").Value).Value;

            var premiumPlan = SubscriptionPlan.Create(
                id: Guid.Parse("30303030-3030-3030-3030-303030303030"),
                name: "Premium Plan",
                tier: SubscriptionTier.Premium,
                billingCycle: BillingCycle.Monthly,
                price: Money.Create(39.99m, "USD").Value).Value;

            plans.Add(freePlan);
            plans.Add(proPlan);
            plans.Add(premiumPlan);

            await _context.SubscriptionPlans.AddRangeAsync(plans);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded Subscription Plans (Free, Pro, Premium).");
        }

        // 4. Seed Instructor User if missing
        if (!await _context.Users.AnyAsync(u => u.Email == "instructor@learnhub.com"))
        {
            var hashResult = _passwordHasher.HashPassword("Instructor@123");
            if (hashResult.IsSuccess)
            {
                var instructorResult = User.Create(
                    id: Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    firstName: "John",
                    lastName: "Instructor",
                    email: "instructor@learnhub.com",
                    passwordHash: hashResult.Value,
                    role: Role.Instructor,
                    phoneNumber: "+1234567890",
                    imageUrl: "https://res.cloudinary.com/demo/image/upload/sample.jpg",
                    dateOfBirth: new DateOnly(1990, 1, 1),
                    bio: "Senior Software Engineer and Certified Instructor",
                    country: "Egypt");

                if (instructorResult.IsSuccess)
                {
                    var instructor = instructorResult.Value;
                    instructor.VerifyEmail();

                    await _context.Users.AddAsync(instructor);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Seeded default instructor user (instructor@learnhub.com / Instructor@123).");
                }
            }
        }

        // 5. Seed Admin User if missing
        if (!await _context.Users.AnyAsync(u => u.Email == "admin@learnhub.com"))
        {
            var hashResult = _passwordHasher.HashPassword("Admin@123");
            if (hashResult.IsSuccess)
            {
                var adminResult = User.Create(
                    id: Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    firstName: "Admin",
                    lastName: "User",
                    email: "admin@learnhub.com",
                    passwordHash: hashResult.Value,
                    role: Role.Admin,
                    phoneNumber: "+1234567891",
                    bio: "System Administrator",
                    country: "Egypt");

                if (adminResult.IsSuccess)
                {
                    var admin = adminResult.Value;
                    admin.VerifyEmail();

                    await _context.Users.AddAsync(admin);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Seeded default admin user (admin@learnhub.com / Admin@123).");
                }
            }
        }

        // 6. Seed Student User if missing
        if (!await _context.Users.AnyAsync(u => u.Email == "student@learnhub.com"))
        {
            var hashResult = _passwordHasher.HashPassword("Student@123");
            if (hashResult.IsSuccess)
            {
                var studentResult = User.Create(
                    id: Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    firstName: "Jane",
                    lastName: "Student",
                    email: "student@learnhub.com",
                    passwordHash: hashResult.Value,
                    role: Role.Student,
                    phoneNumber: "+1234567892",
                    bio: "Eager student learning web development",
                    country: "Egypt");

                if (studentResult.IsSuccess)
                {
                    var student = studentResult.Value;
                    student.VerifyEmail();

                    await _context.Users.AddAsync(student);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Seeded default student user (student@learnhub.com / Student@123).");
                }
            }
        }
    }
}