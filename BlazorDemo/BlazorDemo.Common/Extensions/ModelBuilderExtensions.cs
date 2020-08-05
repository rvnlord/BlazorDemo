using System;
using System.Collections.Generic;
using BlazorDemo.Common.Models;
using BlazorDemo.Common.Models.Account;
using BlazorDemo.Common.Models.EmployeeManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorDemo.Common.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder mb)
        {
            mb.Entity<User>().HasData(new List<User>
            {
                new User
                {
                    Id = Guid.Parse("c136a815-d8ba-40e6-3c6b-08d832fed663"),
                    UserName = "rvnlord",
                    NormalizedUserName = "RVNLORD",
                    Email = "rvnlord@gmail.com",
                    NormalizedEmail = "RVNLORD@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = "amlhUzFzczZEazU2LXhlSTVEY0pkRnVmV1cydThTR3RTMndvc2czaWZwVlZNdEJFTWZ3fE1FWUNJUURlRVd1cGN3VTNXZ1JlNFJVNXpDTEFFMWQ0T3loZVFfcVF2ZFFoeURidHFRSWhBSkRDZDd5U2QxSFY5QVpjRldLVjh1ZFY2ZmRNcjYyYVNHcEJiRV9KOWVJWg",
                    SecurityStamp = "BJUN2FSFJT2VXPGQDWEFIXXJMJL57QJN",
                    ConcurrencyStamp = "4cb10e98-e066-4302-8795-ec5ff960da5a",
                    PhoneNumber = null,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnd = new DateTimeOffset(new DateTime(2020, 8, 4, 11, 39, 5)),
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    EmailActivationToken = null
                },
                new User
                {
                    Id = Guid.Parse("0862cc5b-5f7b-464f-0bce-08d837e1df7d"),
                    UserName = "koverss",
                    NormalizedUserName = "KOVERSS",
                    Email = "koverss@gmail.com",
                    NormalizedEmail = "KOVERSS@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = "QmZ1TExLX0ZhUnNJOUtvQ0hybmNSajh6UWFEeTZYSmxQcGtkblI2MWZHNF9EaUE0Y2xBfE1FWUNJUUNhVmkzM2dQMjh5dGhIN3lPaE5qWGF1WmFZVlNiR2hVUlZxV09LS3pYVHl3SWhBTkxDX0o4YjRwc05VcjJ2MDlxMEZqYWZxTlFlcFBsaDRvYS1OWl80eFljcg",
                    SecurityStamp = "2XSYZWGZVG6EWBZO3HVQY3ZYGJ3UZQHQ",
                    ConcurrencyStamp = "4f86fb85-ed25-4ea0-95f3-c7b92a034919",
                    PhoneNumber = null,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnd = null,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    EmailActivationToken = null
                },
                new User
                {
                    Id = Guid.Parse("4da563ce-95be-426e-e3c5-08d839435d17"),
                    UserName = "tester1",
                    NormalizedUserName = "TESTER1",
                    Email = "tester1@gmail.com",
                    NormalizedEmail = "TESTER1@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = "MFVyZWN4SFd1VHF5dDhqRTA3SDM2VFE0SlpQRDVyTzRVZlNNSTZibUNLN3gwTElUbnhNfE1FUUNJR2J6ejNfRDVwYUZrV0gtaFNfb1pFWW9yTl84N3VRZEVwX3hWNzNCMDRGOEFpQTBRaGlRb1pQUUk4SHBBeHplei1lT0RxM3YzdnNGYUFjRG10TTJnQmNfeHc",
                    SecurityStamp = "2XJF3IQJ6THPX5NSI3WDL7HAR4I3OBBD",
                    ConcurrencyStamp = "bcd3823c-2df9-4634-a702-244a2bbe9b25",
                    PhoneNumber = null,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnd = null,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    EmailActivationToken = null
                },
                new User
                {
                    Id = Guid.Parse("20b4514f-c424-4f01-e3c6-08d839435d17"),
                    UserName = "tester2",
                    NormalizedUserName = "TESTER2",
                    Email = "tester2@gmail.com",
                    NormalizedEmail = "TESTER2@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = "WHIyN2ROcmNldVllMXBPUlJ4N2VRQWVEYUhObEo3SFZ3ZTZDUEpYd3FFSFRWTGRxcGowfE1FUUNJSFFEb0w2blYzYVdKT1dhR2R4NHNiYnNBZHpfZUhtUkpiUlExZTVfSXBjZUFpQnVGal9GWFduN25KR0tfUFFqR3ZCZWRGc1JzRjNuN3k4Vkx4ZHdlWm12c1E",
                    SecurityStamp = "6SPNWJWFOWPPBT7PPYHVFLDHTFSF4M32",
                    ConcurrencyStamp = "fe9fd3e7-22ba-4688-a399-c722502d753b",
                    PhoneNumber = null,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnd = null,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    EmailActivationToken = null
                }
            });

            mb.Entity<IdentityRole<Guid>>().HasData(new List<IdentityRole<Guid>>
            {
                new IdentityRole<Guid> 
                {
                    Id = Guid.Parse("83722fe7-0af9-4299-8873-f66a272aa9d6"),
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "24cf91c5-a5ff-48ca-8508-f444f6e060b2"
                },
                new IdentityRole<Guid> 
                {
                    Id = Guid.Parse("f708c641-ccbf-4a1b-0171-08d835bdd192"),
                    Name = "Mod",
                    NormalizedName = "MOD",
                    ConcurrencyStamp = "24cb18b5-90c0-45be-a420-623fb500a958"
                },
                new IdentityRole<Guid> 
                {
                    Id = Guid.Parse("1a14b302-84fa-419c-0de3-08d839436a4a"),
                    Name = "Tester",
                    NormalizedName = "TESTER",
                    ConcurrencyStamp = "863b5180-decc-4b2a-967f-9344e8b9f270"
                },
                new IdentityRole<Guid> {
                    Id = Guid.Parse("92f0761c-19f0-490d-dc84-08d83592c976"),
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "48ed9669-9dc0-400a-9920-b76e83a8e7b7"
                }
            });

            mb.Entity<IdentityUserRole<Guid>>().HasData(new List<IdentityUserRole<Guid>>
            {
                new IdentityUserRole<Guid>
                {
                    UserId = Guid.Parse("c136a815-d8ba-40e6-3c6b-08d832fed663"),
                    RoleId = Guid.Parse("92f0761c-19f0-490d-dc84-08d83592c976")
                },
                new IdentityUserRole<Guid>
                {
                    UserId = Guid.Parse("0862cc5b-5f7b-464f-0bce-08d837e1df7d"),
                    RoleId = Guid.Parse("92f0761c-19f0-490d-dc84-08d83592c976")
                },
                new IdentityUserRole<Guid>
                {
                    UserId = Guid.Parse("4da563ce-95be-426e-e3c5-08d839435d17"),
                    RoleId = Guid.Parse("92f0761c-19f0-490d-dc84-08d83592c976")
                },
                new IdentityUserRole<Guid>
                {
                    UserId = Guid.Parse("20b4514f-c424-4f01-e3c6-08d839435d17"),
                    RoleId = Guid.Parse("92f0761c-19f0-490d-dc84-08d83592c976")
                },
                new IdentityUserRole<Guid>
                {
                    UserId = Guid.Parse("0862cc5b-5f7b-464f-0bce-08d837e1df7d"),
                    RoleId = Guid.Parse("f708c641-ccbf-4a1b-0171-08d835bdd192")
                },
                new IdentityUserRole<Guid>
                {
                    UserId = Guid.Parse("4da563ce-95be-426e-e3c5-08d839435d17"),
                    RoleId = Guid.Parse("f708c641-ccbf-4a1b-0171-08d835bdd192")
                },
                new IdentityUserRole<Guid>
                {
                    UserId = Guid.Parse("c136a815-d8ba-40e6-3c6b-08d832fed663"),
                    RoleId = Guid.Parse("1a14b302-84fa-419c-0de3-08d839436a4a")
                },
                new IdentityUserRole<Guid>
                {
                    UserId = Guid.Parse("4da563ce-95be-426e-e3c5-08d839435d17"),
                    RoleId = Guid.Parse("1a14b302-84fa-419c-0de3-08d839436a4a")
                },
                new IdentityUserRole<Guid>
                {
                    UserId = Guid.Parse("20b4514f-c424-4f01-e3c6-08d839435d17"),
                    RoleId = Guid.Parse("1a14b302-84fa-419c-0de3-08d839436a4a")
                },
                new IdentityUserRole<Guid>
                {
                    UserId = Guid.Parse("c136a815-d8ba-40e6-3c6b-08d832fed663"),
                    RoleId = Guid.Parse("83722fe7-0af9-4299-8873-f66a272aa9d6")
                },
                new IdentityUserRole<Guid>
                {
                    UserId = Guid.Parse("4da563ce-95be-426e-e3c5-08d839435d17"),
                    RoleId = Guid.Parse("83722fe7-0af9-4299-8873-f66a272aa9d6")
                }
            });

            mb.Entity<IdentityUserClaim<Guid>>().HasData(new List<IdentityUserClaim<Guid>>
            {
                new IdentityUserClaim<Guid>
                {
                    Id = 1,
                    UserId = Guid.Parse("0862cc5b-5f7b-464f-0bce-08d837e1df7d"),
                    ClaimType = "View Employees",
                    ClaimValue = "true",
                },
                new IdentityUserClaim<Guid>
                {
                    Id = 2,
                    UserId = Guid.Parse("0862cc5b-5f7b-464f-0bce-08d837e1df7d"),
                    ClaimType = "Edit Employees",
                    ClaimValue = "true",
                },
                new IdentityUserClaim<Guid>
                {
                    Id = 3,
                    UserId = Guid.Parse("4da563ce-95be-426e-e3c5-08d839435d17"),
                    ClaimType = "View Employees",
                    ClaimValue = "true",
                },
                new IdentityUserClaim<Guid>
                {
                    Id = 4,
                    UserId = Guid.Parse("4da563ce-95be-426e-e3c5-08d839435d17"),
                    ClaimType = "Edit Employees",
                    ClaimValue = "true",
                },
                new IdentityUserClaim<Guid>
                {
                    Id = 5,
                    UserId = Guid.Parse("20b4514f-c424-4f01-e3c6-08d839435d17"),
                    ClaimType = "Edit Employees",
                    ClaimValue = "true",
                },
                new IdentityUserClaim<Guid>
                {
                    Id = 6,
                    UserId = Guid.Parse("c136a815-d8ba-40e6-3c6b-08d832fed663"),
                    ClaimType = "Test Things",
                    ClaimValue = "true",
                },
                new IdentityUserClaim<Guid>
                {
                    Id = 7,
                    UserId = Guid.Parse("4da563ce-95be-426e-e3c5-08d839435d17"),
                    ClaimType = "Test Things",
                    ClaimValue = "true",
                },
                new IdentityUserClaim<Guid>
                {
                    Id = 8,
                    UserId = Guid.Parse("20b4514f-c424-4f01-e3c6-08d839435d17"),
                    ClaimType = "Test Things",
                    ClaimValue = "true",
                },
            });

            mb.Entity<Department>().HasData(new Department(DepartmentType.IT));
            mb.Entity<Department>().HasData(new Department(DepartmentType.HR));
            mb.Entity<Department>().HasData(new Department(DepartmentType.Payroll));
            mb.Entity<Department>().HasData(new Department(DepartmentType.Admin));

            mb.Entity<Employee>().HasData(new Employee
            {
                Id = 1,
                FirstName = "John",
                LastName = "Hastings",
                Email = "john@test.com",
                DateOfBirth = new DateTime(1980, 10, 5),
                Gender = Gender.Male,
                DepartmentId = 1,
                PhotoPath = "images/john.png"
            });

            mb.Entity<Employee>().HasData(new Employee
            {
                Id = 2,
                FirstName = "Sam",
                LastName = "Galloway",
                Email = "sam@test.com",
                DateOfBirth = new DateTime(1981, 12, 22),
                Gender = Gender.Male,
                DepartmentId = 2,
                PhotoPath = "images/sam.jpg"
            });

            mb.Entity<Employee>().HasData(new Employee
            {
                Id = 3,
                FirstName = "Mary",
                LastName = "Smith",
                Email = "mary@test.com",
                DateOfBirth = new DateTime(1979, 11, 11),
                Gender = Gender.Female,
                DepartmentId = 1,
                PhotoPath = "images/mary.png"
            });

            mb.Entity<Employee>().HasData(new Employee
            {
                Id = 4,
                FirstName = "Sara",
                LastName = "Longway",
                Email = "sara@test.com",
                DateOfBirth = new DateTime(1982, 9, 23),
                Gender = Gender.Female,
                DepartmentId = 3,
                PhotoPath = "images/sara.png"
            });
        }

        public static void RenameIdentityTables(this ModelBuilder mb)
        {
            mb.Entity<User>().ToTable("Users");
            mb.Entity<IdentityRole<Guid>>().ToTable("Roles");
            mb.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            mb.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            mb.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            mb.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
            mb.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        }

    }
}
