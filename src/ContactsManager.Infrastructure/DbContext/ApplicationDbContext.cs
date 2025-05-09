using System.Text.Json;
using ContactsManager.Core.Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Entities;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<Person> Persons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Country>().ToTable("Countries");
        modelBuilder.Entity<Person>().ToTable("Persons", table =>
    {
        // constraint using the new configuration style
        table.HasCheckConstraint("CHK_TIN", "LEN([TaxIdentificationNumber]) = 8");
    });

        var countriesJson = File.ReadAllText("countries.json");
        var countries = JsonSerializer.Deserialize<List<Country>>(countriesJson)!;
        foreach (var country in countries)
        {
            modelBuilder.Entity<Country>().HasData(country);
        }

        var personsJson = File.ReadAllText("persons.json");
        var persons = JsonSerializer.Deserialize<List<Person>>(personsJson)!;
        foreach (var person in persons)
        {
            modelBuilder.Entity<Person>().HasData(person);
        }

        modelBuilder.Entity<Person>().Property(p => p.TIN)
            .HasColumnName("TaxIdentificationNumber")
            .HasColumnType("varchar(8)")
            .HasDefaultValue("ABC12345");

        // modelBuilder.Entity<Person>()
        //     .HasIndex(p => p.TIN)
        //     .IsUnique();

        // modelBuilder.Entity<Person>(entity=>{
        //     entity.HasOne<Country>(p=>p.Country)
        //         .WithMany(c=>c.Persons)
        //         .HasForeignKey(p=>p.CountryID)
        //         .OnDelete(DeleteBehavior.Restrict);
        // });
    }

    public List<Person> Sp_GetAllPersons()
    {
        return [.. Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]")];
    }

    public int Sp_InsertPerson(Person person)
    {
        SqlParameter[] parameters =
        [
            new("@PersonID", person.PersonID),
            new("@PersonName", person.PersonName),
            new("@Email", person.Email),
            new("@DateOfBirth", person.DateOfBirth),
            new("@Gender", person.Gender),
            new("@CountryID", person.CountryID),
            new("@Address", person.Address),
            new("@ReceiveNewsLetters", person.ReceiveNewsLetters),
            new("@TIN", person.TIN)
        ];

        return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReceiveNewsLetters, @TIN",
             parameters);
    }
}
