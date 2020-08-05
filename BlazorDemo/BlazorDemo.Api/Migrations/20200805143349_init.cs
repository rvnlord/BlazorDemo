using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BlazorDemo.Api.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CryptographyKeys",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptographyKeys", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    EmailActivationToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    DepartmentId = table.Column<int>(nullable: false),
                    PhotoPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "IT" },
                    { 2, "HR" },
                    { 3, "Payroll" },
                    { 4, "Admin" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("83722fe7-0af9-4299-8873-f66a272aa9d6"), "24cf91c5-a5ff-48ca-8508-f444f6e060b2", "Admin", "ADMIN" },
                    { new Guid("f708c641-ccbf-4a1b-0171-08d835bdd192"), "24cb18b5-90c0-45be-a420-623fb500a958", "Mod", "MOD" },
                    { new Guid("1a14b302-84fa-419c-0de3-08d839436a4a"), "863b5180-decc-4b2a-967f-9344e8b9f270", "Tester", "TESTER" },
                    { new Guid("92f0761c-19f0-490d-dc84-08d83592c976"), "48ed9669-9dc0-400a-9920-b76e83a8e7b7", "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailActivationToken", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("c136a815-d8ba-40e6-3c6b-08d832fed663"), 0, "4cb10e98-e066-4302-8795-ec5ff960da5a", "rvnlord@gmail.com", null, true, true, new DateTimeOffset(new DateTime(2020, 8, 4, 11, 39, 5, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "RVNLORD@GMAIL.COM", "RVNLORD", "amlhUzFzczZEazU2LXhlSTVEY0pkRnVmV1cydThTR3RTMndvc2czaWZwVlZNdEJFTWZ3fE1FWUNJUURlRVd1cGN3VTNXZ1JlNFJVNXpDTEFFMWQ0T3loZVFfcVF2ZFFoeURidHFRSWhBSkRDZDd5U2QxSFY5QVpjRldLVjh1ZFY2ZmRNcjYyYVNHcEJiRV9KOWVJWg", null, false, "BJUN2FSFJT2VXPGQDWEFIXXJMJL57QJN", false, "rvnlord" },
                    { new Guid("0862cc5b-5f7b-464f-0bce-08d837e1df7d"), 0, "4f86fb85-ed25-4ea0-95f3-c7b92a034919", "koverss@gmail.com", null, true, true, null, "KOVERSS@GMAIL.COM", "KOVERSS", "QmZ1TExLX0ZhUnNJOUtvQ0hybmNSajh6UWFEeTZYSmxQcGtkblI2MWZHNF9EaUE0Y2xBfE1FWUNJUUNhVmkzM2dQMjh5dGhIN3lPaE5qWGF1WmFZVlNiR2hVUlZxV09LS3pYVHl3SWhBTkxDX0o4YjRwc05VcjJ2MDlxMEZqYWZxTlFlcFBsaDRvYS1OWl80eFljcg", null, false, "2XSYZWGZVG6EWBZO3HVQY3ZYGJ3UZQHQ", false, "koverss" },
                    { new Guid("4da563ce-95be-426e-e3c5-08d839435d17"), 0, "bcd3823c-2df9-4634-a702-244a2bbe9b25", "tester1@gmail.com", null, true, true, null, "TESTER1@GMAIL.COM", "TESTER1", "MFVyZWN4SFd1VHF5dDhqRTA3SDM2VFE0SlpQRDVyTzRVZlNNSTZibUNLN3gwTElUbnhNfE1FUUNJR2J6ejNfRDVwYUZrV0gtaFNfb1pFWW9yTl84N3VRZEVwX3hWNzNCMDRGOEFpQTBRaGlRb1pQUUk4SHBBeHplei1lT0RxM3YzdnNGYUFjRG10TTJnQmNfeHc", null, false, "2XJF3IQJ6THPX5NSI3WDL7HAR4I3OBBD", false, "tester1" },
                    { new Guid("20b4514f-c424-4f01-e3c6-08d839435d17"), 0, "fe9fd3e7-22ba-4688-a399-c722502d753b", "tester2@gmail.com", null, true, true, null, "TESTER2@GMAIL.COM", "TESTER2", "WHIyN2ROcmNldVllMXBPUlJ4N2VRQWVEYUhObEo3SFZ3ZTZDUEpYd3FFSFRWTGRxcGowfE1FUUNJSFFEb0w2blYzYVdKT1dhR2R4NHNiYnNBZHpfZUhtUkpiUlExZTVfSXBjZUFpQnVGal9GWFduN25KR0tfUFFqR3ZCZWRGc1JzRjNuN3k4Vkx4ZHdlWm12c1E", null, false, "6SPNWJWFOWPPBT7PPYHVFLDHTFSF4M32", false, "tester2" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "DateOfBirth", "DepartmentId", "Email", "FirstName", "Gender", "LastName", "PhotoPath" },
                values: new object[,]
                {
                    { 4, new DateTime(1982, 9, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "sara@test.com", "Sara", 1, "Longway", "images/sara.png" },
                    { 2, new DateTime(1981, 12, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "sam@test.com", "Sam", 0, "Galloway", "images/sam.jpg" },
                    { 3, new DateTime(1979, 11, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "mary@test.com", "Mary", 1, "Smith", "images/mary.png" },
                    { 1, new DateTime(1980, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "john@test.com", "John", 0, "Hastings", "images/john.png" }
                });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "UserId" },
                values: new object[,]
                {
                    { 8, "Test Things", "true", new Guid("20b4514f-c424-4f01-e3c6-08d839435d17") },
                    { 5, "Edit Employees", "true", new Guid("20b4514f-c424-4f01-e3c6-08d839435d17") },
                    { 6, "Test Things", "true", new Guid("c136a815-d8ba-40e6-3c6b-08d832fed663") },
                    { 4, "Edit Employees", "true", new Guid("4da563ce-95be-426e-e3c5-08d839435d17") },
                    { 3, "View Employees", "true", new Guid("4da563ce-95be-426e-e3c5-08d839435d17") },
                    { 2, "Edit Employees", "true", new Guid("0862cc5b-5f7b-464f-0bce-08d837e1df7d") },
                    { 1, "View Employees", "true", new Guid("0862cc5b-5f7b-464f-0bce-08d837e1df7d") },
                    { 7, "Test Things", "true", new Guid("4da563ce-95be-426e-e3c5-08d839435d17") }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("4da563ce-95be-426e-e3c5-08d839435d17"), new Guid("92f0761c-19f0-490d-dc84-08d83592c976") },
                    { new Guid("c136a815-d8ba-40e6-3c6b-08d832fed663"), new Guid("83722fe7-0af9-4299-8873-f66a272aa9d6") },
                    { new Guid("4da563ce-95be-426e-e3c5-08d839435d17"), new Guid("83722fe7-0af9-4299-8873-f66a272aa9d6") },
                    { new Guid("0862cc5b-5f7b-464f-0bce-08d837e1df7d"), new Guid("f708c641-ccbf-4a1b-0171-08d835bdd192") },
                    { new Guid("4da563ce-95be-426e-e3c5-08d839435d17"), new Guid("f708c641-ccbf-4a1b-0171-08d835bdd192") },
                    { new Guid("c136a815-d8ba-40e6-3c6b-08d832fed663"), new Guid("1a14b302-84fa-419c-0de3-08d839436a4a") },
                    { new Guid("4da563ce-95be-426e-e3c5-08d839435d17"), new Guid("1a14b302-84fa-419c-0de3-08d839436a4a") },
                    { new Guid("20b4514f-c424-4f01-e3c6-08d839435d17"), new Guid("1a14b302-84fa-419c-0de3-08d839436a4a") },
                    { new Guid("c136a815-d8ba-40e6-3c6b-08d832fed663"), new Guid("92f0761c-19f0-490d-dc84-08d83592c976") },
                    { new Guid("0862cc5b-5f7b-464f-0bce-08d837e1df7d"), new Guid("92f0761c-19f0-490d-dc84-08d83592c976") },
                    { new Guid("20b4514f-c424-4f01-e3c6-08d839435d17"), new Guid("92f0761c-19f0-490d-dc84-08d83592c976") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CryptographyKeys");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
