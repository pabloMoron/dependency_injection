using DI.Interfaces;
using DI.Models;

namespace DI.Services
{
    public class PersonRepository : IPersonRepository
    {
        List<Person> _people = new List<Person>{
            new Person(1, "Pablo Moron", new DateTime(1994, 1, 26)),
            new Person(2, "Juan Gomez", new DateTime(2010, 3, 13)),
            new Person(3, "Roberto García", new DateTime(1980, 6, 18)),
            new Person(4, "Mario Otako", new DateTime(1985, 9, 29)),
            new Person(5, "Lazio Tano", new DateTime(1990, 12, 30)),
        };
        public List<Person> getAllPeople()
        {
            return _people;
        }
        public Person? getById(int id)
        {
            return _people.Where(x => x._id == id).FirstOrDefault();
        }
        public Person? getByName(string name)
        {
            return _people.Where(x => x._name.ToLower().Contains(name.ToLower())).FirstOrDefault();
        }
    }
}
