namespace DI.Models
{
    public class Person
    {
        public int _id { get; set; }
        public string _name { get; set; } 
        public DateTime _dateOfBirth { get; set; }

        public Person(int id, string name, DateTime date) { 
            this._id = id;
            this._name = name;
            this._dateOfBirth = date;
        }
    }
}
