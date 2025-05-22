using System;
using System.Xml.Serialization;

namespace PT_Lab1
{
    [Serializable]
    public class StudentInfo : IComparable<StudentInfo>, IComparable
    {
        private int _completedProjects;

        // Średnia ocen studenta (0.0 – 4.0)
        public double GPA { get; set; }

        // Liczba ukończonych projektów (1 – 10)
        public int CompletedProjects
        {
            get => _completedProjects;
            set
            {
                if (value < 1 || value > 10)
                    throw new ArgumentOutOfRangeException(nameof(CompletedProjects),
                        "Liczba projektów musi być w zakresie 1-10.");
                _completedProjects = value;
            }
        }

        // Rok studiów
        public YearOfStudy Year { get; set; }

        // Konstruktor domyślny wykorzystywany przy deserializacji
        public StudentInfo() { }

        // Porównanie po CompletedProjects
        public int CompareTo(StudentInfo other)
        {
            if (other == null) return 1;
            return this.CompletedProjects.CompareTo(other.CompletedProjects);
        }

        // Wymagane przez interfejs IComparable
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (obj is StudentInfo otherInfo)
                return CompareTo(otherInfo);
            throw new ArgumentException("Obiekt nie jest typu StudentInfo", nameof(obj));
        }
    }
}