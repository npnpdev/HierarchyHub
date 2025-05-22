using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace PT_Lab1
{
    public enum YearOfStudy
    {
        First,
        Second,
        Third,
        Fourth
    }

    [Serializable]
    public class Person
    {

        private static int _nextId = 1;
        // ID jest unikatowe i nie będzie eksportowane do XML,
        // ale musi mieć setter, byśmy mogli je przypisać przy deserializacji
        [XmlIgnore]
        public int Id { get; set; }

        // Inicjalizacja instancji Random
        private static readonly Random _rand = new Random();

        [XmlIgnore]
        public Person Parent { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }

        [XmlIgnore]
        // Data utworzenia obiektu – nie serializujemy
        public DateTime CreatedAt { get; private set; }

        // Powiązanie jeden-do-jeden: dane studenta
        public StudentInfo Info { get; set; }

        public ObservableCollection<Person> Children { get; }

        // Konstruktor domyślny używany przy deserializacji i ręcznym tworzeniu
        public Person()
        {
            // Jeżeli ID nie zostało nadane
            if (Id == 0)
                Id = _nextId++;

            Children = new ObservableCollection<Person>();
            Info = new StudentInfo
            {
                GPA = Math.Round(_rand.NextDouble() * 4.0, 2),
                CompletedProjects = _rand.Next(1, 11),
                Year = (YearOfStudy)_rand.Next(0, 4)
            };
            CreatedAt = DateTime.Now;

            // Losowanie wieku
            Age = _rand.Next(1, 100);
        }

        public Person(string name, string surname, int age) : this()
        {
            Name = name;
            Surname = surname;
            Age = age;
        }

        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }
}
