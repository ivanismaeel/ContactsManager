using System.Linq.Expressions;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;

namespace Repositories;

public class PersonsRepository(ApplicationDbContext db, ILogger<PersonsRepository> logger) : IPersonsRepository
{
    public async Task<Person> AddPerson(Person person)
    {
        db.Persons.Add(person);
        await db.SaveChangesAsync();

        return person;
    }

    public async Task<bool> DeletePersonByPersonID(Guid personID)
    {
        db.Persons.RemoveRange(db.Persons.Where(p => p.PersonID == personID));
        int rowsDeleted = await db.SaveChangesAsync();

        return rowsDeleted > 0;
    }

    public async Task<List<Person>> GetAllPersons()
    {
        return await db.Persons.Include("Country").ToListAsync();
    }

    public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
    {
        logger.LogInformation("GetFilteredPersons() called of PersonsRepository");

        return await db.Persons.Include("Country").Where(predicate).ToListAsync();
    }

    public async Task<Person?> GetPersonByPersonID(Guid personID)
    {
        return await db.Persons.Include("Country")
            .FirstOrDefaultAsync(p => p.PersonID == personID);
    }

    public async Task<Person> UpdatePerson(Person person)
    {
        var existingPerson = await GetPersonByPersonID(person.PersonID);
        if (existingPerson == null) return person;

        existingPerson.PersonName = person.PersonName;
        existingPerson.Email = person.Email;
        existingPerson.DateOfBirth = person.DateOfBirth;
        existingPerson.Gender = person.Gender;
        existingPerson.CountryID = person.CountryID;
        existingPerson.Address = person.Address;
        existingPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;

        await db.SaveChangesAsync();

        return existingPerson;
    }
}
