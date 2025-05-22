using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PT_Lab1
{
    // Kolekcja Person z metodami sortowania i wyszukiwania
    public class PersonCollection : ObservableCollection<Person>
    {
        public PersonCollection() { }
        public PersonCollection(IEnumerable<Person> collection)
        {
            foreach (var p in collection)
                Add(p);
        }

        // Metoda spłaszczająca całą kolekcję rekurencyjnie
        private IEnumerable<Person> Flatten()
        {
            foreach (var p in this)
            {
                yield return p;
                foreach (var child in FlattenChildren(p))
                    yield return child;
            }
        }

        // pomocnicza rekurencja po dzieciakch
        private IEnumerable<Person> FlattenChildren(Person parent)
        {
            foreach (var c in parent.Children)
            {
                yield return c;
                foreach (var gc in FlattenChildren(c))
                    yield return gc;
            }
        }

        // Sortuje po dowolnej właściwości, jeśli jej typ implementuje IComparable
        public void SortByProperty(string propertyName)
        {
            // Jeśli brak elementów – nic nie robimy
            if (!this.Any())
                return;

            // Obsługa zagnieżdżonej nazwy właściwości (np. "Info.CompletedProjects")
            var parts = propertyName.Split('.');
            PropertyInfo prop = typeof(Person).GetProperty(parts[0]);
            if (prop == null)
                throw new ArgumentException("Brak właściwości: " + parts[0]);

            // Funkcja wybierająca wartość właściwości jako IComparable
            Func<Person, IComparable> selector = p =>
            {
                object current = prop.GetValue(p);
                for (int i = 1; i < parts.Length; i++)
                {
                    if (current == null) break;
                    var pi = current.GetType().GetProperty(parts[i]);
                    if (pi == null)
                        throw new ArgumentException("Brak właściwości: " + parts[i]);
                    current = pi.GetValue(current);
                }
                return current as IComparable;
            };

            // Sprawdzamy czy typ implementuje IComparable
            var first = this.FirstOrDefault();
            var test = selector(first);
            if (test == null || test.GetType().GetInterface("IComparable") == null)
                throw new ArgumentException("Właściwość nie implementuje IComparable lub jest null.");

            var sorted = this.OrderBy(selector).ToList();
            Clear();
            foreach (var item in sorted)
                Add(item);
        }


        // Wyszukuje osoby po właściwości string
        public IEnumerable<Person> FindByString(string propertyName, string value)
        {
            var prop = typeof(Person).GetProperty(propertyName);
            if (prop == null || prop.PropertyType != typeof(string))
                throw new ArgumentException("Właściwość nie istnieje lub nie jest typu string.");

            return Flatten()
                .Where(p =>
                {
                    var v = (string)prop.GetValue(p);
                    return v != null && v.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
                })
                .ToList();
        }

        // Wyszukuje osoby po właściwości int
        public IEnumerable<Person> FindByInt(string propertyName, int value)
        {
            var prop = typeof(Person).GetProperty(propertyName);
            if (prop == null || prop.PropertyType != typeof(int))
                throw new ArgumentException("Właściwość nie istnieje lub nie jest typu int.");

            return Flatten()
                .Where(p => (int)prop.GetValue(p) == value)
                .ToList();
        }
    }
}
