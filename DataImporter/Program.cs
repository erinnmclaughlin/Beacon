using CsvHelper;
using DataImporter;
using DataImporter.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

BeaconDbContext? dbContext = null;

while (dbContext is null)
{
    Console.Write("Connection string: ");
    var cs = Console.ReadLine();
    Console.WriteLine($"\nConnecting to db...");

    var options = new DbContextOptionsBuilder<BeaconDbContext>().UseSqlServer(cs).Options;

    try 
    {
        dbContext = new BeaconDbContext(options);
        //dbContext.Database.EnsureCreated();
        Console.WriteLine("Connected.");
    }
    catch
    {
        dbContext = null;
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Unable to connect to the db. Please try again.\n");
        Console.ResetColor();
    }
}

var allison = CreateUser("Allison", "aalix");
var erin = CreateUser("Erin", "emclaughlin");
var kyle = CreateUser("Kyle", "kclements");
var hunter = CreateUser("Hunter", "hforget");
var jeff = CreateUser("Jeff", "jhorsman");
var jillian = CreateUser("Jillian", "jillian");
var jordan = CreateUser("Jordan", "jordan");
var julianne = CreateUser("Julianne", "jnolasco");
var ken = CreateUser("Ken", "kvictor");
var michael = CreateUser("Michael", "mepperstein");
var page = CreateUser("Page", "page");

var users = new[] {allison, erin, kyle, hunter, jeff, jillian, jordan, julianne, michael, ken, page};

var lab = new Laboratory { Id = Guid.NewGuid(), Name = "Lighthouse Instruments" };
lab.Memberships.AddRange(new[]
{
    new Membership { Member = allison, MembershipType = "Analyst" },
    new Membership { Member = erin, MembershipType = "Admin" },
    new Membership { Member = kyle, MembershipType = "Analyst" },
    new Membership { Member = hunter, MembershipType = "Analyst" },
    new Membership { Member = jeff, MembershipType = "Analyst" },
    new Membership { Member = jillian, MembershipType = "Analyst" },
    new Membership { Member = jordan, MembershipType = "Analyst" },
    new Membership { Member = julianne, MembershipType = "Analyst" },
    new Membership { Member = ken, MembershipType = "Manager" },
    new Membership { Member = michael, MembershipType = "Analyst" },
    new Membership { Member = page, MembershipType = "Analyst" },
});

dbContext.Laboratories.Add(lab);
await dbContext.SaveChangesAsync();

using var reader = new StreamReader("projects.csv");
using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

var records = csv.GetRecords<CsvData>().ToList();

var projects = new List<Project>();
foreach (var r in records)
{
    if (ProjectCode.TryParse(r.ProjectId, out var projectCode))
    {
        projects.Add(new Project
        {
            Id = Guid.NewGuid(),
            LeadAnalystId = users.FirstOrDefault(u => u.DisplayName == r.Lead)?.Id,
            CreatedById = erin.Id,
            CustomerName = r.CustomerName,
            ProjectCode = projectCode,
            ProjectStatus = GetStatus(r.Status),
            LaboratoryId = lab.Id
        });
    }
}

dbContext.Projects.AddRange(projects);
await dbContext.SaveChangesAsync();

static User CreateUser(string displayName, string username) => new()
{
    Id = Guid.NewGuid(),
    DisplayName = displayName,
    EmailAddress = $"{username}@lighthouseinstruments.com",
    HashedPassword = new PasswordHasher().Hash($"!!{username}", out var salt),
    HashedPasswordSalt = salt
};

static string GetStatus(string initialStatus)
{
    if (initialStatus == "Cancelled")
        return "Canceled";

    if (initialStatus == "EU Lab")
        return "Expired";

    if (initialStatus == "On Hold")
        return "OnHold";

    if (initialStatus == "Current")
        return "Active";

    return initialStatus;
}