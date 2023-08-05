using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.API.Services;
using Beacon.Common.Models;
using Beacon.Common.Services;
using DataImporter;
using Microsoft.EntityFrameworkCore;

var importContext = new SessionContext
{
    CurrentLab = new() { Id = Guid.NewGuid(), Name = "Lighthouse Instruments", MembershipType = LaboratoryMembershipType.Admin },
    CurrentUser = default!
};

BeaconDbContext? dbContext = null;

while (dbContext is null)
{
    Console.Write("Connection string: ");
    var cs = Console.ReadLine();
    Console.WriteLine($"\nConnecting to db...");

    var options = new DbContextOptionsBuilder<BeaconDbContext>().UseSqlServer(cs).Options;

    try 
    {
        dbContext = new BeaconDbContext(options, importContext);
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
var hunter = CreateUser("Hunter", "hunter");
var jeff = CreateUser("Jeff", "jhorsman");
var jillian = CreateUser("Jillian", "jillian");
var jordan = CreateUser("Jordan", "jordan");
var julianne = CreateUser("Julianne", "jnolasco");
var ken = CreateUser("Ken", "kvictor");
var michael = CreateUser("Michael", "mepperstein");
var page = CreateUser("Page", "page");

var users = new[] {allison, erin, kyle, hunter, jeff, jillian, jordan, julianne, michael, ken, page};

var lab = new Laboratory { Id = importContext.CurrentLab.Id, Name = importContext.CurrentLab.Name };
lab.Memberships.AddRange(new[]
{
    new Membership { Member = allison, MembershipType = LaboratoryMembershipType.Analyst },
    new Membership { Member = erin, MembershipType = LaboratoryMembershipType.Admin },
    new Membership { Member = kyle, MembershipType = LaboratoryMembershipType.Analyst },
    new Membership { Member = hunter, MembershipType = LaboratoryMembershipType.Analyst },
    new Membership { Member = jeff, MembershipType = LaboratoryMembershipType.Analyst },
    new Membership { Member = jillian, MembershipType = LaboratoryMembershipType.Analyst },
    new Membership { Member = jordan, MembershipType = LaboratoryMembershipType.Analyst },
    new Membership { Member = julianne, MembershipType = LaboratoryMembershipType.Analyst },
    new Membership { Member = ken, MembershipType = LaboratoryMembershipType.Manager },
    new Membership { Member = michael, MembershipType = LaboratoryMembershipType.Analyst },
    new Membership { Member = page, MembershipType = LaboratoryMembershipType.Analyst },
});

dbContext.Laboratories.Add(lab);
await dbContext.SaveChangesAsync();

var instruments = LaboratoryInstrumentCsvReader.GetLaboratoryInstruments(lab.Id).ToArray();
dbContext.LaboratoryInstruments.AddRange(instruments);
await dbContext.SaveChangesAsync();

var projects = ProjectCsvReader.GetProjects(users).ToArray();
dbContext.Projects.AddRange(projects);
await dbContext.SaveChangesAsync();

var contacts = ProjectContactCsvReader.GetProjectContacts(projects);
dbContext.ProjectContacts.AddRange(contacts);
await dbContext.SaveChangesAsync();

var events = ProjectEventCsvReader.GetProjectEvents(projects, instruments);
dbContext.ProjectEvents.AddRange(events);
await dbContext.SaveChangesAsync();

static User CreateUser(string displayName, string username) => new()
{
    Id = Guid.NewGuid(),
    DisplayName = displayName,
    EmailAddress = $"{username}@lighthouseinstruments.com",
    HashedPassword = new PasswordHasher().Hash($"!!{username}", out var salt),
    HashedPasswordSalt = salt
};

