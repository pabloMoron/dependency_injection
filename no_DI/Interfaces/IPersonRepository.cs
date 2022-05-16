using DI.Models;

namespace DI.Interfaces
{
    public interface IPersonRepository
    {
        List<Person> getAllPeople();
        Person? getByName(string name);
        Person? getById(int id);
    }
}
