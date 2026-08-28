using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Assessments;
using LearnHub.Domain.Assessments.Enums;
using LearnHub.Domain.Assessments.Questions;
using LearnHub.Domain.Assessments.Questions.Choices;
using LearnHub.Domain.Classification.Categories;
using LearnHub.Domain.Classification.Tags;
using LearnHub.Domain.Courses;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Courses.Sections;
using LearnHub.Domain.Courses.Sections.Lessons;
using LearnHub.Domain.Enrollments;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Instructor;
using LearnHub.Domain.LearningPaths;
using LearnHub.Domain.Purchasing.Orders;
using LearnHub.Domain.Purchasing.ValueObjects;
using LearnHub.Domain.Reviews.CourseReviews;
using LearnHub.Domain.Reviews.InstructorReviews;
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
        // Category Guids
        var catSoftwareDevId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var catDataAiId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var catDesignUxId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var catBusinessId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var catCloudDevOpsId = Guid.Parse("88888888-8888-8888-8888-888888888888");
        var catSecurityId = Guid.Parse("99999999-9999-9999-9999-999999999999");
        var catMobileId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var catGameDevId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var catQaTestingId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        var catDatabaseId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        var catWeb3Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
        var catCertificationsId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
        var catAiLlmsId = Guid.Parse("10101010-1010-1010-1010-101010101001");
        var catSystemDesignId = Guid.Parse("20202020-2020-2020-2020-202020202002");
        var catAgileId = Guid.Parse("30303030-3030-3030-3030-303030303003");

        // Course Guids (25 Courses)
        var c1Id = Guid.Parse("c0101010-0101-0101-0101-010101010101");
        var c2Id = Guid.Parse("c0202020-0202-0202-0202-020202020202");
        var c3Id = Guid.Parse("c0303030-0303-0303-0303-030303030303");
        var c4Id = Guid.Parse("c0404040-0404-0404-0404-040404040404");
        var c5Id = Guid.Parse("c0505050-0505-0505-0505-050505050505");
        var c6Id = Guid.Parse("c0606060-0606-0606-0606-060606060606");
        var c7Id = Guid.Parse("c0707070-0707-0707-0707-070707070707");
        var c8Id = Guid.Parse("c0808080-0808-0808-0808-080808080808");
        var c9Id = Guid.Parse("c0909090-0909-0909-0909-090909090909");
        var c10Id = Guid.Parse("c1010101-1010-1010-1010-101010101010");
        var c11Id = Guid.Parse("c1111111-2222-3333-4444-555555555555");
        var c12Id = Guid.Parse("c1212121-2222-3333-4444-555555555555");
        var c13Id = Guid.Parse("c1313131-2222-3333-4444-555555555555");
        var c14Id = Guid.Parse("c1414141-2222-3333-4444-555555555555");
        var c15Id = Guid.Parse("c1515151-2222-3333-4444-555555555555");
        var c16Id = Guid.Parse("c1616161-2222-3333-4444-555555555555");
        var c17Id = Guid.Parse("c1717171-2222-3333-4444-555555555555");
        var c18Id = Guid.Parse("c1818181-2222-3333-4444-555555555555");
        var c19Id = Guid.Parse("c1919191-2222-3333-4444-555555555555");
        var c20Id = Guid.Parse("c2020202-2222-3333-4444-555555555555");
        var c21Id = Guid.Parse("c2121212-2222-3333-4444-555555555555");
        var c22Id = Guid.Parse("c2222222-3333-4444-5555-666666666666");
        var c23Id = Guid.Parse("c2323232-3333-4444-5555-666666666666");
        var c24Id = Guid.Parse("c2424242-3333-4444-5555-666666666666");
        var c25Id = Guid.Parse("c2525252-3333-4444-5555-666666666666");

        // User Guids
        var adminId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var instJohnId = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var instSarahId = Guid.Parse("55555555-5555-5555-5555-555555555556");
        var instAlexId = Guid.Parse("55555555-5555-5555-5555-555555555557");
        var instElenaId = Guid.Parse("55555555-5555-5555-5555-555555555558");
        var instDavidId = Guid.Parse("55555555-5555-5555-5555-555555555559");
        var instMarcusId = Guid.Parse("55555555-5555-5555-5555-555555555560");

        var studJaneId = Guid.Parse("77777777-7777-7777-7777-777777777777");
        var studMichaelId = Guid.Parse("77777777-7777-7777-7777-777777777778");
        var studEmilyId = Guid.Parse("77777777-7777-7777-7777-777777777779");
        var studDavidBId = Guid.Parse("77777777-7777-7777-7777-777777777780");
        var studSophiaId = Guid.Parse("77777777-7777-7777-7777-777777777781");
        var studLiamId = Guid.Parse("77777777-7777-7777-7777-777777777782");
        var studNoahId = Guid.Parse("77777777-7777-7777-7777-777777777783");

        // 1. Seed Categories (15 Categories)
        if (!await _context.Categories.AnyAsync())
        {
            var defaultCategories = new List<Category>
            {
                Category.Create(catSoftwareDevId, "Software Development", "software-development", "Learn programming languages, frameworks, clean architecture, and web/mobile development.").Value,
                Category.Create(catDataAiId, "Data Science & AI", "data-science-ai", "Explore machine learning, data analysis, Python, PyTorch, and artificial intelligence.").Value,
                Category.Create(catDesignUxId, "Design & UX", "design-ux", "Master UI/UX design, Figma, responsive design, wireframing, and visual aesthetics.").Value,
                Category.Create(catBusinessId, "Business & Marketing", "business-marketing", "Digital marketing, product management, entrepreneurship, and business analytics.").Value,
                Category.Create(catCloudDevOpsId, "Cloud & DevOps", "cloud-devops", "Docker, Kubernetes, AWS, Azure, CI/CD pipelines, and cloud architecture.").Value,
                Category.Create(catSecurityId, "Cyber Security", "cyber-security", "Ethical hacking, network security, penetration testing, and secure coding.").Value,
                Category.Create(catMobileId, "Mobile Development", "mobile-development", "Flutter, iOS, Android, and cross-platform app development.").Value,
                Category.Create(catGameDevId, "Game Development", "game-development", "Unity, Unreal Engine 5, C#, and 3D graphics.").Value,
                Category.Create(catQaTestingId, "Software Testing & QA", "qa-testing", "Automated testing, Playwright, Selenium, and unit testing.").Value,
                Category.Create(catDatabaseId, "Database Systems & SQL", "databases-sql", "PostgreSQL, SQL Server, Redis, and database architecture.").Value,
                Category.Create(catWeb3Id, "Blockchain & Web3", "blockchain-web3", "Smart contracts, Solidity, Ethereum, and decentralized apps.").Value,
                Category.Create(catCertificationsId, "IT Certifications", "it-certifications", "AWS Certified Solutions Architect, CISSP, CompTIA, and PMP.").Value,
                Category.Create(catAiLlmsId, "Generative AI & LLMs", "ai-llms", "Prompt engineering, LangChain, RAG architecture, and OpenAI APIs.").Value,
                Category.Create(catSystemDesignId, "System Design & Architecture", "system-design", "High availability, distributed caching, load balancing, and microservices.").Value,
                Category.Create(catAgileId, "Agile & Project Management", "agile-project-mgmt", "Scrum framework, Kanban, Jira, and project delivery.").Value
            };

            await _context.Categories.AddRangeAsync(defaultCategories);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded 15 categories.");
        }

        // 2. Seed Tags (30 Tags)
        if (!await _context.Tags.AnyAsync())
        {
            var defaultTags = new List<Tag>
            {
                Tag.Create(Guid.NewGuid(), "C#", "csharp", "C# programming language").Value,
                Tag.Create(Guid.NewGuid(), ".NET 9", "dotnet-9", ".NET 9 framework and ecosystem").Value,
                Tag.Create(Guid.NewGuid(), "Clean Architecture", "clean-architecture", "Clean Architecture and CQRS design patterns").Value,
                Tag.Create(Guid.NewGuid(), "React 19", "react-19", "React frontend library").Value,
                Tag.Create(Guid.NewGuid(), "Next.js 15", "nextjs-15", "Next.js React Framework").Value,
                Tag.Create(Guid.NewGuid(), "TypeScript", "typescript", "TypeScript typed JavaScript").Value,
                Tag.Create(Guid.NewGuid(), "Python", "python", "Python programming language").Value,
                Tag.Create(Guid.NewGuid(), "Machine Learning", "machine-learning", "Machine Learning models and algorithms").Value,
                Tag.Create(Guid.NewGuid(), "Deep Learning", "deep-learning", "Deep neural networks and PyTorch").Value,
                Tag.Create(Guid.NewGuid(), "Figma", "figma", "Figma design tool").Value,
                Tag.Create(Guid.NewGuid(), "UI/UX", "ui-ux", "User Interface and User Experience design").Value,
                Tag.Create(Guid.NewGuid(), "Docker", "docker", "Containerization technology").Value,
                Tag.Create(Guid.NewGuid(), "Kubernetes", "kubernetes", "Container orchestration").Value,
                Tag.Create(Guid.NewGuid(), "Flutter", "flutter", "Flutter cross-platform SDK").Value,
                Tag.Create(Guid.NewGuid(), "Dart", "dart", "Dart programming language").Value,
                Tag.Create(Guid.NewGuid(), "Cyber Security", "cybersecurity", "Security standards and ethical hacking").Value,
                Tag.Create(Guid.NewGuid(), "Penetration Testing", "pen-testing", "Penetration testing and security auditing").Value,
                Tag.Create(Guid.NewGuid(), "AWS", "aws", "Amazon Web Services").Value,
                Tag.Create(Guid.NewGuid(), "Azure", "azure", "Microsoft Azure cloud platform").Value,
                Tag.Create(Guid.NewGuid(), "GraphQL", "graphql", "GraphQL query language").Value,
                Tag.Create(Guid.NewGuid(), "Unity 3D", "unity-3d", "Unity Game Engine").Value,
                Tag.Create(Guid.NewGuid(), "PostgreSQL", "postgresql", "Relational database system").Value,
                Tag.Create(Guid.NewGuid(), "Playwright", "playwright", "End-to-end automation testing").Value,
                Tag.Create(Guid.NewGuid(), "Solidity", "solidity", "Smart contract development").Value,
                Tag.Create(Guid.NewGuid(), "Generative AI", "generative-ai", "LLMs and GenAI applications").Value,
                Tag.Create(Guid.NewGuid(), "RAG", "rag", "Retrieval-Augmented Generation").Value,
                Tag.Create(Guid.NewGuid(), "Microservices", "microservices", "Distributed microservices architecture").Value,
                Tag.Create(Guid.NewGuid(), "Scrum", "scrum", "Agile Scrum framework").Value,
                Tag.Create(Guid.NewGuid(), "Redis", "redis", "In-memory caching and data store").Value,
                Tag.Create(Guid.NewGuid(), "iOS", "ios", "Apple iOS Swift development").Value
            };

            await _context.Tags.AddRangeAsync(defaultTags);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded 30 tags.");
        }

        // 3. Seed Subscription Plans
        if (!await _context.SubscriptionPlans.AnyAsync())
        {
            var plans = new List<SubscriptionPlan>
            {
                SubscriptionPlan.Create(Guid.Parse("10101010-1010-1010-1010-101010101010"), "Free Plan", SubscriptionTier.Free, BillingCycle.Monthly, Money.Create(0, "USD").Value).Value,
                SubscriptionPlan.Create(Guid.Parse("20202020-2020-2020-2020-202020202020"), "Pro Plan", SubscriptionTier.Pro, BillingCycle.Monthly, Money.Create(19.99m, "USD").Value).Value,
                SubscriptionPlan.Create(Guid.Parse("30303030-3030-3030-3030-303030303030"), "Premium Plan", SubscriptionTier.Premium, BillingCycle.Monthly, Money.Create(39.99m, "USD").Value).Value
            };

            await _context.SubscriptionPlans.AddRangeAsync(plans);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded Subscription Plans.");
        }

        // 4. Seed Users (Admin, Instructors, Students)
        var defaultPasswordHash = _passwordHasher.HashPassword("Password@123").Value;

        if (!await _context.Users.AnyAsync(u => u.Email == "admin@learnhub.com"))
        {
            var admin = User.Create(adminId, "System", "Administrator", "admin@learnhub.com", _passwordHasher.HashPassword("Admin@123").Value, Role.Admin, phoneNumber: "+1000000000", bio: "Platform Administrator", country: "Egypt").Value;
            admin.VerifyEmail();
            await _context.Users.AddAsync(admin);
        }

        if (!await _context.Users.AnyAsync(u => u.Email == "instructor@learnhub.com"))
        {
            var instJohn = User.Create(instJohnId, "John", "Doe", "instructor@learnhub.com", _passwordHasher.HashPassword("Instructor@123").Value, Role.Instructor, phoneNumber: "+1234567890", imageUrl: "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=500", dateOfBirth: new DateOnly(1988, 5, 12), bio: "Principal .NET Architect & Cloud Specialist with 12+ years experience.", country: "United States").Value;
            instJohn.VerifyEmail();
            await _context.Users.AddAsync(instJohn);
        }

        if (!await _context.Users.AnyAsync(u => u.Email == "sarah.instructor@learnhub.com"))
        {
            var instSarah = User.Create(instSarahId, "Sarah", "Connor", "sarah.instructor@learnhub.com", defaultPasswordHash, Role.Instructor, phoneNumber: "+1234567894", imageUrl: "https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=500", dateOfBirth: new DateOnly(1992, 8, 24), bio: "Senior Frontend Lead & UX Engineer specializing in React, Next.js, and Design Systems.", country: "Canada").Value;
            instSarah.VerifyEmail();
            await _context.Users.AddAsync(instSarah);
        }

        if (!await _context.Users.AnyAsync(u => u.Email == "alex.instructor@learnhub.com"))
        {
            var instAlex = User.Create(instAlexId, "Alex", "Rivera", "alex.instructor@learnhub.com", defaultPasswordHash, Role.Instructor, phoneNumber: "+1234567895", imageUrl: "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=500", dateOfBirth: new DateOnly(1990, 3, 15), bio: "AI Research Scientist & Machine Learning Engineer.", country: "Germany").Value;
            instAlex.VerifyEmail();
            await _context.Users.AddAsync(instAlex);
        }

        if (!await _context.Users.AnyAsync(u => u.Email == "elena.instructor@learnhub.com"))
        {
            var instElena = User.Create(instElenaId, "Elena", "Rostova", "elena.instructor@learnhub.com", defaultPasswordHash, Role.Instructor, phoneNumber: "+1234567899", imageUrl: "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=500", dateOfBirth: new DateOnly(1991, 11, 4), bio: "Senior Mobile Developer specializing in Flutter, Dart, and iOS.", country: "Spain").Value;
            instElena.VerifyEmail();
            await _context.Users.AddAsync(instElena);
        }

        if (!await _context.Users.AnyAsync(u => u.Email == "david.instructor@learnhub.com"))
        {
            var instDavid = User.Create(instDavidId, "David", "Miller", "david.instructor@learnhub.com", defaultPasswordHash, Role.Instructor, phoneNumber: "+1234567893", imageUrl: "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=500", dateOfBirth: new DateOnly(1985, 7, 19), bio: "Certified Ethical Hacker (CEH) & Cyber Security Specialist.", country: "United Kingdom").Value;
            instDavid.VerifyEmail();
            await _context.Users.AddAsync(instDavid);
        }

        if (!await _context.Users.AnyAsync(u => u.Email == "marcus.instructor@learnhub.com"))
        {
            var instMarcus = User.Create(instMarcusId, "Marcus", "Thorne", "marcus.instructor@learnhub.com", defaultPasswordHash, Role.Instructor, phoneNumber: "+1234567888", imageUrl: "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=500", dateOfBirth: new DateOnly(1987, 2, 10), bio: "Lead Game Developer & Unity Specialist.", country: "Sweden").Value;
            instMarcus.VerifyEmail();
            await _context.Users.AddAsync(instMarcus);
        }

        if (!await _context.Users.AnyAsync(u => u.Email == "student@learnhub.com"))
        {
            var studJane = User.Create(studJaneId, "Jane", "Smith", "student@learnhub.com", _passwordHasher.HashPassword("Student@123").Value, Role.Student, phoneNumber: "+1234567892", bio: "Aspiring Full-Stack Software Developer.", country: "United Kingdom").Value;
            studJane.VerifyEmail();
            await _context.Users.AddAsync(studJane);
        }

        if (!await _context.Users.AnyAsync(u => u.Email == "michael.student@learnhub.com"))
        {
            var studMichael = User.Create(studMichaelId, "Michael", "Scott", "michael.student@learnhub.com", defaultPasswordHash, Role.Student, phoneNumber: "+1234567896", bio: "Learning AI and Data Science to automate enterprise applications.", country: "United States").Value;
            studMichael.VerifyEmail();
            await _context.Users.AddAsync(studMichael);
        }

        if (!await _context.Users.AnyAsync(u => u.Email == "emily.student@learnhub.com"))
        {
            var studEmily = User.Create(studEmilyId, "Emily", "Watson", "emily.student@learnhub.com", defaultPasswordHash, Role.Student, phoneNumber: "+1234567897", bio: "Passionate UI/UX Designer expanding web development knowledge.", country: "Australia").Value;
            studEmily.VerifyEmail();
            await _context.Users.AddAsync(studEmily);
        }

        if (!await _context.Users.AnyAsync(u => u.Email == "david.student@learnhub.com"))
        {
            var studDavidB = User.Create(studDavidBId, "David", "Beckham", "david.student@learnhub.com", defaultPasswordHash, Role.Student, phoneNumber: "+1234567898", bio: "Software Engineering student building practical projects.", country: "United Kingdom").Value;
            studDavidB.VerifyEmail();
            await _context.Users.AddAsync(studDavidB);
        }

        if (!await _context.Users.AnyAsync(u => u.Email == "sophia.student@learnhub.com"))
        {
            var studSophia = User.Create(studSophiaId, "Sophia", "Martinez", "sophia.student@learnhub.com", defaultPasswordHash, Role.Student, phoneNumber: "+1234567881", bio: "Mobile App Enthusiast & Flutter Learner.", country: "Spain").Value;
            studSophia.VerifyEmail();
            await _context.Users.AddAsync(studSophia);
        }

        if (!await _context.Users.AnyAsync(u => u.Email == "liam.student@learnhub.com"))
        {
            var studLiam = User.Create(studLiamId, "Liam", "Johnson", "liam.student@learnhub.com", defaultPasswordHash, Role.Student, phoneNumber: "+1234567882", bio: "Cybersecurity Engineer in training.", country: "United States").Value;
            studLiam.VerifyEmail();
            await _context.Users.AddAsync(studLiam);
        }

        if (!await _context.Users.AnyAsync(u => u.Email == "noah.student@learnhub.com"))
        {
            var studNoah = User.Create(studNoahId, "Noah", "Williams", "noah.student@learnhub.com", defaultPasswordHash, Role.Student, phoneNumber: "+1234567883", bio: "Game Development student.", country: "Sweden").Value;
            studNoah.VerifyEmail();
            await _context.Users.AddAsync(studNoah);
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded users.");

        // 5. Seed Instructor Profiles
        if (!await _context.InstructorProfiles.AnyAsync())
        {
            var profileJohn = InstructorProfile.Create(instJohnId, "Principal .NET Architect", "Building Scalable Enterprise Systems with C# & .NET 9", "Over 12 years of hands-on experience designing cloud-native microservices and domain-driven systems.").Value;
            profileJohn.Approve();
            profileJohn.UpdateProfileImage("https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=500");
            profileJohn.AddSkill(InstructorSkill.Create(Guid.NewGuid(), profileJohn.Id, "C# 12").Value);
            profileJohn.AddSkill(InstructorSkill.Create(Guid.NewGuid(), profileJohn.Id, ".NET 9").Value);
            profileJohn.AddExperience(InstructorExperience.Create(Guid.NewGuid(), profileJohn.Id, "Principal Solutions Architect", "TechCorp Global", "Leading cloud transformation.", new DateOnly(2020, 1, 1), null, true, "New York, USA").Value);

            var profileSarah = InstructorProfile.Create(instSarahId, "Senior Frontend & UX Lead", "Crafting Beautiful, Accessible, and High-Performance Web Apps", "Passionate about modern JavaScript, React 19, Next.js, and intuitive interface design.").Value;
            profileSarah.Approve();
            profileSarah.UpdateProfileImage("https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=500");

            var profileAlex = InstructorProfile.Create(instAlexId, "AI Research Scientist", "Demystifying Machine Learning, Deep Learning, and Python", "Specialized in computer vision, natural language processing, and deep neural networks.").Value;
            profileAlex.Approve();
            profileAlex.UpdateProfileImage("https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=500");

            var profileElena = InstructorProfile.Create(instElenaId, "Senior Mobile Engineer", "Cross-Platform Mobile Apps with Flutter 3.24 & Dart", "Building top-rated iOS & Android applications with clean state management.").Value;
            profileElena.Approve();
            profileElena.UpdateProfileImage("https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=500");

            var profileDavid = InstructorProfile.Create(instDavidId, "Cyber Security Lead", "Ethical Hacking, Penetration Testing & Defense", "Certified CEH & CISSP specialist protecting critical web infrastructure.").Value;
            profileDavid.Approve();
            profileDavid.UpdateProfileImage("https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=500");

            var profileMarcus = InstructorProfile.Create(instMarcusId, "Lead Game Director", "3D Game Architecture with Unity & Unreal Engine 5", "Creating immersive game experiences and high-performance C# physics engines.").Value;
            profileMarcus.Approve();
            profileMarcus.UpdateProfileImage("https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=500");

            await _context.InstructorProfiles.AddRangeAsync([profileJohn, profileSarah, profileAlex, profileElena, profileDavid, profileMarcus]);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded Instructor Profiles.");
        }

        // 6. Seed 25 Courses with 3 Sections, 5 Lessons per Section, Section Quizzes, and Final Exams!
        if (!await _context.Courses.AnyAsync())
        {
            var coursesList = new List<Course>();
            var sectionsList = new List<Section>();
            var lessonsList = new List<Lesson>();
            var quizzesList = new List<Quiz>();
            var questionsList = new List<Question>();
            var choicesList = new List<Choice>();

            var courseDefs = new (Guid Id, string Title, string Description, Guid InstructorId, Guid CategoryId, string Img, CourseLevel Level, CourseStatus Status, decimal Price, bool InSub, SubscriptionTier Tier, string Lang, string Country, string[] SecTitles)[]
            {
                (c1Id, "Mastering C# 12 & .NET 9 Clean Architecture", "Enterprise software development using C# 12, .NET 9, Domain-Driven Design (DDD), CQRS with MediatR, and EF Core 9.", instJohnId, catSoftwareDevId, "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?w=800", CourseLevel.Intermediate, CourseStatus.Published, 49.99m, true, SubscriptionTier.Pro, "en", "United States", new[] { "Domain-Driven Design Fundamentals", "CQRS & MediatR Pipeline Patterns", "EF Core 9 High Performance Persistence" }),
                (c2Id, "Complete React 19 & Next.js 15 Masterclass", "Build modern web applications using React 19, Next.js 15 App Router, Server Components, and TailwindCSS.", instSarahId, catSoftwareDevId, "https://images.unsplash.com/photo-1633356122544-f134324a6cee?w=800", CourseLevel.Beginner, CourseStatus.Published, 0.00m, true, SubscriptionTier.Free, "en", "Canada", new[] { "React 19 Core Hooks & State", "Next.js 15 Server Components & Routing", "TailwindCSS Responsive Design" }),
                (c3Id, "Practical Data Science & AI Bootcamp 2026", "Python, NumPy, Pandas, Scikit-Learn, and PyTorch to build real-world machine learning models.", instAlexId, catDataAiId, "https://images.unsplash.com/photo-1526374965328-7f61d4dc18c5?w=800", CourseLevel.Beginner, CourseStatus.Published, 149.99m, true, SubscriptionTier.Premium, "en", "Germany", new[] { "Python Data Analysis with Pandas", "Supervised & Unsupervised Machine Learning", "Deep Learning Neural Networks with PyTorch" }),
                (c4Id, "Modern UI/UX Design with Figma: Zero to Hero", "Design pixel-perfect user interfaces, interactive prototypes, and design systems with Figma.", instSarahId, catDesignUxId, "https://images.unsplash.com/photo-1581291518633-83b4ebd1d83e?w=800", CourseLevel.Beginner, CourseStatus.Published, 19.99m, true, SubscriptionTier.Free, "en", "Canada", new[] { "Figma Tool Foundations & Canvas", "Auto Layout & Component Variant Systems", "Interactive Prototyping & User Testing" }),
                (c5Id, "Enterprise Cloud Architecture & DevOps Masterclass", "Kubernetes, Docker, Terraform, and GitHub Actions CI/CD for enterprise cloud deployments.", instJohnId, catCloudDevOpsId, "https://images.unsplash.com/photo-1667372393119-3d4c48d07fc9?w=800", CourseLevel.Advanced, CourseStatus.Published, 199.99m, false, SubscriptionTier.Premium, "en", "United States", new[] { "Docker Multi-Stage Containerization", "Kubernetes Pod Orchestration & Secrets", "Terraform Infrastructure as Code" }),
                (c6Id, "Complete Flutter 3.24 & Dart Mobile Development", "Build cross-platform iOS & Android mobile applications using Flutter 3.24, Bloc, and Firebase.", instElenaId, catMobileId, "https://images.unsplash.com/photo-1512941937669-90a1b58e7e9c?w=800", CourseLevel.Beginner, CourseStatus.Published, 34.99m, true, SubscriptionTier.Pro, "en", "Spain", new[] { "Dart 3 Fundamentals & Async Streams", "Flutter Stateful Widgets & Layout Math", "Bloc State Management & Firebase Auth" }),
                (c7Id, "Ethical Hacking & Cybersecurity Fundamentals", "Web application security, penetration testing, Wireshark, and OWASP Top 10 defenses.", instDavidId, catSecurityId, "https://images.unsplash.com/photo-1550751827-4bd374c3f58b?w=800", CourseLevel.Intermediate, CourseStatus.Published, 89.99m, true, SubscriptionTier.Premium, "en", "United Kingdom", new[] { "Network Protocols & Packet Analysis", "OWASP Top 10 Exploitation & Hardening", "Penetration Testing Methodology" }),
                (c8Id, "3D Game Development with Unity & C#", "Create 3D games, physics interactions, particle effects, and character animations in Unity 2026.", instMarcusId, catGameDevId, "https://images.unsplash.com/photo-1550745165-9bc0b252726f?w=800", CourseLevel.Intermediate, CourseStatus.Published, 59.99m, true, SubscriptionTier.Pro, "en", "Sweden", new[] { "Unity GameEngine Architecture & Physics", "Character Controllers & Animations", "Shaders, Particles, & Post Processing" }),
                (c9Id, "Automated Web Testing with Playwright & TypeScript", "Master end-to-end automated testing, API mocking, and CI/CD test execution with Playwright.", instSarahId, catQaTestingId, "https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=800", CourseLevel.Beginner, CourseStatus.Published, 14.99m, true, SubscriptionTier.Free, "en", "Canada", new[] { "Playwright Test Runner Setup & Selectors", "API Mocking & Fixture Management", "CI/CD Integration with GitHub Actions" }),
                (c10Id, "High-Performance PostgreSQL & Database Indexing", "Optimize SQL queries, B-Tree indexes, partitioning, and database performance tuning.", instJohnId, catDatabaseId, "https://images.unsplash.com/photo-1544383835-bda2bc66a55d?w=800", CourseLevel.Intermediate, CourseStatus.Published, 29.99m, true, SubscriptionTier.Pro, "en", "United States", new[] { "PostgreSQL Relational Schema Design", "B-Tree, GIN, & GiST Indexing Strategies", "EXPLAIN ANALYZE & Query Execution" }),
                (c11Id, "Ethereum Smart Contracts with Solidity & Web3.js", "Build decentralized applications (dApps), ERC-20 tokens, and Solidity smart contracts.", instAlexId, catWeb3Id, "https://images.unsplash.com/photo-1639762681485-074b7f938ba0?w=800", CourseLevel.Intermediate, CourseStatus.Published, 79.99m, true, SubscriptionTier.Premium, "en", "Germany", new[] { "Solidity Language Fundamentals", "ERC-20 & ERC-721 Token Standards", "Testing Smart Contracts with Hardhat" }),
                (c12Id, "AWS Certified Solutions Architect Exam Prep", "Comprehensive prep for AWS SAA-C03 certification covering EC2, VPC, IAM, S3, and RDS.", instJohnId, catCertificationsId, "https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=800", CourseLevel.Intermediate, CourseStatus.Published, 99.99m, true, SubscriptionTier.Premium, "en", "United States", new[] { "IAM & Core Security Architecture", "VPC Networking, Subnets, & Gateways", "High Availability & Auto-Scaling" }),
                (c13Id, "Generative AI Engineering: LangChain & RAG", "Build AI agents, vector database search (Pinecone/Chroma), and RAG pipelines with LLMs.", instAlexId, catAiLlmsId, "https://images.unsplash.com/photo-1677442136019-21780efad99a?w=800", CourseLevel.Advanced, CourseStatus.Published, 129.99m, true, SubscriptionTier.Premium, "en", "Germany", new[] { "LLM API Integration & Prompt Engineering", "Vector Embeddings & Chroma DB Search", "RAG Pipeline Architecture & LangChain" }),
                (c14Id, "System Design & Distributed Microservices", "Design scalable systems handling millions of requests: Caching, Kafka, & Sharding.", instJohnId, catSystemDesignId, "https://images.unsplash.com/photo-1558494949-ef010cbdcc31?w=800", CourseLevel.Advanced, CourseStatus.Published, 119.99m, true, SubscriptionTier.Premium, "en", "United States", new[] { "Scalability Fundamentals & Load Balancing", "Distributed Caching with Redis & Memcached", "Message Queues with Kafka & Event Streaming" }),
                (c15Id, "Agile Product Management & Scrum Leadership", "Master Scrum ceremonies, user stories, sprint planning, and product backlog refinement.", instSarahId, catAgileId, "https://images.unsplash.com/photo-1531403009284-440f080d1e12?w=800", CourseLevel.Beginner, CourseStatus.Published, 0.00m, true, SubscriptionTier.Free, "en", "Canada", new[] { "Scrum Roles, Events, & Artifacts", "Writing Effective User Stories & Acceptance Criteria", "Sprint Velocity & Burndown Analytics" }),
                (c16Id, "TypeScript 5.4 Advanced Generic Patterns", "Generics, conditional types, template literal types, and type-safe API builders.", instSarahId, catSoftwareDevId, "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=800", CourseLevel.Intermediate, CourseStatus.Published, 24.99m, true, SubscriptionTier.Pro, "en", "Canada", new[] { "Advanced Type System & Utility Types", "Generics & Conditional Types", "Type-Safe Library Architecture" }),
                (c17Id, "Python Automation & Web Scraping with BeautifulSoup", "Automate tedious office tasks, parse HTML with BeautifulSoup and Playwright Python.", instAlexId, catDataAiId, "https://images.unsplash.com/photo-1526379095098-d400fd0bf935?w=800", CourseLevel.Beginner, CourseStatus.Published, 9.99m, true, SubscriptionTier.Free, "en", "Germany", new[] { "Python File I/O & Script Automation", "Parsing Web Pages with BeautifulSoup4", "Handling Dynamic JavaScript Pages" }),
                (c18Id, "iOS 18 App Development with Swift 6 & SwiftUI", "Build native iPhone & iPad apps using Swift 6, SwiftUI 6, and SwiftData local persistence.", instElenaId, catMobileId, "https://images.unsplash.com/photo-1512941937669-90a1b58e7e9c?w=800", CourseLevel.Intermediate, CourseStatus.Published, 69.99m, true, SubscriptionTier.Pro, "en", "Spain", new[] { "Swift 6 Concurrency & Syntax", "SwiftUI Views, Modifiers, & Navigation", "SwiftData & CloudKit Synchronization" }),
                (c19Id, "API Security & OWASP Penetration Testing", "Secure REST & GraphQL APIs against JWT forgery, CORS misconfigurations, and rate limiting.", instDavidId, catSecurityId, "https://images.unsplash.com/photo-1563986768609-322da13575f3?w=800", CourseLevel.Advanced, CourseStatus.Published, 79.99m, true, SubscriptionTier.Premium, "en", "United Kingdom", new[] { "Authentication & Token Security (JWT/OAuth2)", "API Gateway Security & Rate Limiting", "Penetration Testing API Endpoints" }),
                (c20Id, "Distributed Caching & Real-Time Apps with Redis", "Implement Redis pub/sub, caching strategies, sliding window rate limiters, and session storage.", instJohnId, catDatabaseId, "https://images.unsplash.com/photo-1558494949-ef010cbdcc31?w=800", CourseLevel.Intermediate, CourseStatus.Published, 39.99m, true, SubscriptionTier.Pro, "en", "United States", new[] { "Redis Data Structures & Memory Storage", "Cache-Aside, Write-Through, & Eviction Policies", "Pub/Sub Messaging & Real-time WebSockets" }),
                (c21Id, "Unreal Engine 5 C# & C++ Game Mechanics", "Build AAA environment lighting, Nanite geometry, and C++ character movement systems.", instMarcusId, catGameDevId, "https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800", CourseLevel.Advanced, CourseStatus.Published, 89.99m, true, SubscriptionTier.Premium, "en", "Sweden", new[] { "Unreal Engine 5 Interface & Nanite", "C++ Gameplay Framework & Blueprints", "Lumen Lighting & Niagara Visual Effects" }),
                (c22Id, "Microservices Event-Driven Messaging with Apache Kafka", "Architect event streaming pipelines, Kafka producers, consumer groups, and schema registries.", instJohnId, catCloudDevOpsId, "https://images.unsplash.com/photo-1518770660439-4636190af475?w=800", CourseLevel.Advanced, CourseStatus.Published, 99.99m, false, SubscriptionTier.Premium, "en", "United States", new[] { "Kafka Broker Architecture & Topics", "Producers, Consumers, & Partitioning", "Kafka Streams & Event Sourcing" }),
                (c23Id, "GraphQL API Design with Node.js & Apollo Server", "Build schema-first GraphQL APIs, custom resolvers, dataloader batching, and subscriptions.", instSarahId, catSoftwareDevId, "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=800", CourseLevel.Intermediate, CourseStatus.Published, 29.99m, true, SubscriptionTier.Pro, "en", "Canada", new[] { "GraphQL Schema Definition & Types", "Writing Resolvers & DataLoader Batching", "GraphQL Subscriptions & WebSockets" }),
                (c24Id, "Kubernetes Administration & Helm Chart Deployment", "Cluster setup, ingress controllers, persistent volumes, and Helm chart deployment.", instJohnId, catCloudDevOpsId, "https://images.unsplash.com/photo-1667372393119-3d4c48d07fc9?w=800", CourseLevel.Advanced, CourseStatus.Published, 109.99m, true, SubscriptionTier.Premium, "en", "United States", new[] { "Kubernetes Cluster Architecture & kubectl", "Deployments, Services, & Ingress NGINX", "Managing Packages with Helm Charts" }),
                (c25Id, "Building RAG AI Agents with Python & OpenAI", "Develop autonomous AI agents using OpenAI Function Calling, LangGraph, and Vector Databases.", instAlexId, catAiLlmsId, "https://images.unsplash.com/photo-1677442136019-21780efad99a?w=800", CourseLevel.Advanced, CourseStatus.Published, 139.99m, true, SubscriptionTier.Premium, "en", "Germany", new[] { "OpenAI Function Calling & Tool Use", "LangGraph State Machine AI Agents", "Deploying AI Services to Production" })
            };

            foreach (var def in courseDefs)
            {
                var course = Course.Create(def.Id, def.Title, def.Description, def.InstructorId, def.CategoryId, def.Img, def.Level, def.Status, Money.Create(def.Price, "USD").Value, def.InSub, def.Tier, def.Lang, "English", def.Country).Value;
                coursesList.Add(course);

                // Create 3 Sections per course
                for (int sIndex = 0; sIndex < 3; sIndex++)
                {
                    string secTitle = def.SecTitles.Length > sIndex ? def.SecTitles[sIndex] : $"Section {sIndex + 1}: Advanced Topics";
                    var section = Section.Create(Guid.NewGuid(), secTitle, $"In-depth exploration of {secTitle}.", sIndex + 1, def.Id).Value;
                    sectionsList.Add(section);

                    // Create 5 Lessons per Section
                    for (int lIndex = 0; lIndex < 5; lIndex++)
                    {
                        bool isPreview = (sIndex == 0 && lIndex == 0);
                        string lesTitle = $"Lesson {lIndex + 1}: Core Concepts of {secTitle}";
                        var lesson = Lesson.Create(
                            Guid.NewGuid(),
                            lesTitle,
                            $"Step-by-step practical guide covering {lesTitle}.",
                            "https://vimeo.com/sample_video",
                            isPreview,
                            $"Comprehensive lesson text content for {lesTitle}.",
                            15 + (lIndex * 5),
                            lIndex + 1,
                            section.Id).Value;

                        lessonsList.Add(lesson);
                    }

                    // Section Exam (Quiz) for EVERY Section
                    var secQuiz = Quiz.CreateSectionQuiz(
                        Guid.NewGuid(),
                        def.Id,
                        section.Id,
                        $"{secTitle} Knowledge Check",
                        $"Assessment quiz to test your mastery of {secTitle}.",
                        15,
                        3,
                        70).Value;

                    var q1 = Question.Create(Guid.NewGuid(), $"What is the main objective of {secTitle}?", QuestionType.MultipleChoice, 10, 1).Value;
                    var c1 = Choice.Create(Guid.NewGuid(), "Apply industry best practices and maintain clean architecture", true).Value;
                    var c2 = Choice.Create(Guid.NewGuid(), "Ignore performance bottlenecks and security rules", false).Value;
                    choicesList.AddRange([c1, c2]);
                    questionsList.Add(q1);

                    secQuiz.Publish();
                    quizzesList.Add(secQuiz);
                }

                // Final Exam (Quiz) for EVERY Course
                var finalExam = Quiz.Create(
                    Guid.NewGuid(),
                    def.Id,
                    $"{def.Title} Final Certification Exam",
                    $"Comprehensive final exam covering all modules of {def.Title}.",
                    45,
                    2,
                    80).Value;

                var fq1 = Question.Create(Guid.NewGuid(), $"Which architectural principle is most vital in {def.Title}?", QuestionType.MultipleChoice, 20, 1).Value;
                var fc1 = Choice.Create(Guid.NewGuid(), "High cohesion, low coupling, and clear boundary separation", true).Value;
                var fc2 = Choice.Create(Guid.NewGuid(), "Monolithic tight coupling across all layers", false).Value;
                choicesList.AddRange([fc1, fc2]);
                questionsList.Add(fq1);

                finalExam.Publish();
                quizzesList.Add(finalExam);
            }

            await _context.Courses.AddRangeAsync(coursesList);
            await _context.Sections.AddRangeAsync(sectionsList);
            await _context.Lessons.AddRangeAsync(lessonsList);
            await _context.Quizzes.AddRangeAsync(quizzesList);
            await _context.Questions.AddRangeAsync(questionsList);
            await _context.Choices.AddRangeAsync(choicesList);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded 25 Courses, 75 Sections, 375 Lessons, 100 Quizzes with Exams.");
        }

        // 7. Seed Learning Paths
        if (!await _context.LearningPaths.AnyAsync())
        {
            var path1 = LearningPath.Create(Guid.NewGuid(), "Full-Stack .NET & React Engineer", "full-stack-dotnet-react", "From C# 12 & .NET 9 Clean Architecture to React 19 & Next.js 15 App Router and PostgreSQL.", "Complete backend and frontend developer roadmap.", "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?w=800", CourseLevel.Intermediate, instJohnId).Value;
            path1.AddCourse(c1Id, 1, true);
            path1.AddCourse(c2Id, 2, true);
            path1.AddCourse(c10Id, 3, false);
            path1.Publish();

            var path2 = LearningPath.Create(Guid.NewGuid(), "Cloud-Native AI & DevOps Specialist", "cloud-native-ai-devops", "Combine Data Science, Machine Learning with PyTorch, AWS Cloud Architecture, and Docker/Kubernetes.", "Comprehensive AI engineering & cloud infrastructure roadmap.", "https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=800", CourseLevel.Advanced, instAlexId).Value;
            path2.AddCourse(c3Id, 1, true);
            path2.AddCourse(c5Id, 2, true);
            path2.Publish();

            var path3 = LearningPath.Create(Guid.NewGuid(), "Modern Mobile & UI/UX Product Designer", "mobile-uiux-product-designer", "Design pixel-perfect interfaces in Figma and build cross-platform mobile apps using Flutter 3.24.", "End-to-end design and mobile app creation path.", "https://images.unsplash.com/photo-1512941937669-90a1b58e7e9c?w=800", CourseLevel.Beginner, instSarahId).Value;
            path3.AddCourse(c4Id, 1, true);
            path3.AddCourse(c6Id, 2, true);
            path3.Publish();

            await _context.LearningPaths.AddRangeAsync([path1, path2, path3]);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded Learning Paths.");
        }

        // 8. Seed Enrollments
        if (!await _context.Enrollments.AnyAsync())
        {
            var enrollments = new List<Enrollment>
            {
                Enrollment.Create(Guid.NewGuid(), studJaneId, c1Id).Value,
                Enrollment.Create(Guid.NewGuid(), studJaneId, c2Id).Value,
                Enrollment.Create(Guid.NewGuid(), studJaneId, c6Id).Value,
                Enrollment.Create(Guid.NewGuid(), studMichaelId, c1Id).Value,
                Enrollment.Create(Guid.NewGuid(), studMichaelId, c3Id).Value,
                Enrollment.Create(Guid.NewGuid(), studMichaelId, c5Id).Value,
                Enrollment.Create(Guid.NewGuid(), studEmilyId, c2Id).Value,
                Enrollment.Create(Guid.NewGuid(), studEmilyId, c4Id).Value,
                Enrollment.Create(Guid.NewGuid(), studEmilyId, c9Id).Value,
                Enrollment.Create(Guid.NewGuid(), studDavidBId, c3Id).Value,
                Enrollment.Create(Guid.NewGuid(), studDavidBId, c7Id).Value,
                Enrollment.Create(Guid.NewGuid(), studDavidBId, c10Id).Value,
                Enrollment.Create(Guid.NewGuid(), studSophiaId, c6Id).Value,
                Enrollment.Create(Guid.NewGuid(), studSophiaId, c4Id).Value,
                Enrollment.Create(Guid.NewGuid(), studLiamId, c7Id).Value,
                Enrollment.Create(Guid.NewGuid(), studLiamId, c5Id).Value,
                Enrollment.Create(Guid.NewGuid(), studNoahId, c8Id).Value,
                Enrollment.Create(Guid.NewGuid(), studNoahId, c10Id).Value
            };

            await _context.Enrollments.AddRangeAsync(enrollments);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded Enrollments.");
        }

        // 9. Seed Paid Orders & Financial Transactions
        if (!await _context.Orders.AnyAsync())
        {
            var now = DateTimeOffset.UtcNow;
            var orders = new List<Order>();

            var o1 = Order.Create(Guid.NewGuid(), studJaneId, "USD").Value;
            o1.AddItem(c1Id, "Mastering C# 12 & .NET 9 Clean Architecture", Money.Create(49.99m, "USD").Value);
            o1.Checkout(now.AddMonths(-5));
            o1.MarkPaid("TXN_101", now.AddMonths(-5));
            orders.Add(o1);

            var o2 = Order.Create(Guid.NewGuid(), studMichaelId, "USD").Value;
            o2.AddItem(c3Id, "Practical Data Science & AI Bootcamp 2026", Money.Create(149.99m, "USD").Value);
            o2.Checkout(now.AddMonths(-4));
            o2.MarkPaid("TXN_102", now.AddMonths(-4));
            orders.Add(o2);

            var o3 = Order.Create(Guid.NewGuid(), studEmilyId, "USD").Value;
            o3.AddItem(c4Id, "Modern UI/UX Design with Figma: Zero to Hero", Money.Create(19.99m, "USD").Value);
            o3.Checkout(now.AddMonths(-3));
            o3.MarkPaid("TXN_103", now.AddMonths(-3));
            orders.Add(o3);

            var o4 = Order.Create(Guid.NewGuid(), studDavidBId, "USD").Value;
            o4.AddItem(c5Id, "Enterprise Cloud Architecture & DevOps Masterclass", Money.Create(199.99m, "USD").Value);
            o4.Checkout(now.AddMonths(-2));
            o4.MarkPaid("TXN_104", now.AddMonths(-2));
            orders.Add(o4);

            var o5 = Order.Create(Guid.NewGuid(), studSophiaId, "USD").Value;
            o5.AddItem(c6Id, "Complete Flutter 3.24 & Dart Mobile Development", Money.Create(34.99m, "USD").Value);
            o5.Checkout(now.AddMonths(-1));
            o5.MarkPaid("TXN_105", now.AddMonths(-1));
            orders.Add(o5);

            var o6 = Order.Create(Guid.NewGuid(), studLiamId, "USD").Value;
            o6.AddItem(c7Id, "Ethical Hacking & Cybersecurity Fundamentals", Money.Create(89.99m, "USD").Value);
            o6.Checkout(now.AddDays(-10));
            o6.MarkPaid("TXN_106", now.AddDays(-10));
            orders.Add(o6);

            var o7 = Order.Create(Guid.NewGuid(), studNoahId, "USD").Value;
            o7.AddItem(c8Id, "3D Game Development with Unity & C#", Money.Create(59.99m, "USD").Value);
            o7.AddItem(c10Id, "High-Performance PostgreSQL & Database Indexing", Money.Create(29.99m, "USD").Value);
            o7.Checkout(now.AddDays(-2));
            o7.MarkPaid("TXN_107", now.AddDays(-2));
            orders.Add(o7);

            await _context.Orders.AddRangeAsync(orders);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded Orders.");
        }

        // 10. Seed Reviews
        if (!await _context.CourseReviews.AnyAsync())
        {
            var reviews = new List<CourseReview>
            {
                CourseReview.Create(Guid.NewGuid(), c1Id, studJaneId, 5, "Outstanding course! Clean Architecture and CQRS explanations with MediatR are top-notch.").Value,
                CourseReview.Create(Guid.NewGuid(), c1Id, studMichaelId, 5, "Best C# and .NET 9 course on the platform. Extremely practical.").Value,
                CourseReview.Create(Guid.NewGuid(), c2Id, studEmilyId, 5, "Sarah is an amazing teacher. React 19 Server Components are crystal clear now!").Value,
                CourseReview.Create(Guid.NewGuid(), c3Id, studMichaelId, 4, "Great introduction to data science and machine learning using Python and PyTorch.").Value,
                CourseReview.Create(Guid.NewGuid(), c6Id, studSophiaId, 5, "Loved the Flutter 3.24 hands-on projects! Super responsive UI designs.").Value,
                CourseReview.Create(Guid.NewGuid(), c7Id, studLiamId, 5, "David's cybersecurity and penetration testing explanations are second to none.").Value,
                CourseReview.Create(Guid.NewGuid(), c8Id, studNoahId, 5, "Marcus makes Unity 3D physics and game architecture so easy to understand.").Value
            };

            foreach (var r in reviews) r.Publish();

            await _context.CourseReviews.AddRangeAsync(reviews);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded Course Reviews.");
        }

        if (!await _context.InstructorReviews.AnyAsync())
        {
            var instReviews = new List<InstructorReview>
            {
                InstructorReview.Create(Guid.NewGuid(), instJohnId, studJaneId, c1Id, 5, "John is a world-class instructor and software architect!").Value,
                InstructorReview.Create(Guid.NewGuid(), instSarahId, studEmilyId, c2Id, 5, "Sarah's code examples and explanations are clear and modern.").Value,
                InstructorReview.Create(Guid.NewGuid(), instElenaId, studSophiaId, c6Id, 5, "Elena explains mobile development with such clarity and patience.").Value,
                InstructorReview.Create(Guid.NewGuid(), instDavidId, studLiamId, c7Id, 5, "David's security insight is priceless!").Value,
                InstructorReview.Create(Guid.NewGuid(), instMarcusId, studNoahId, c8Id, 5, "Awesome game development instructor!").Value
            };

            foreach (var r in instReviews) r.Publish();

            await _context.InstructorReviews.AddRangeAsync(instReviews);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded Instructor Reviews.");
        }
    }
}