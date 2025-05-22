using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace PT_Lab1
{
    public partial class MainWindow : Window
    {
        // private ObservableCollection<Person> _roots = new ObservableCollection<Person>();
        private PersonCollection _roots = new PersonCollection();

        private Random _rand = new Random();

        // Przechowujemy wyniki pierwszego zapytania LINQ do dalszego wykorzystania
        private List<Query1Result> _lastQuery1Results;
        // Przechowujemy kontekst ostatniego zapytania LINQ 1
        private Person _lastQuery1Root;
        private Person _lastSerializedRoot;

        public MainWindow()
        {
            InitializeComponent();

            // Podpinamy DataGrid i TreeView do tej samej kolekcji _roots
            dataGrid.ItemsSource = _roots;
            treeView.ItemsSource = _roots;
        }

        private void GenerateData_Click(object sender, RoutedEventArgs e)
        {
            // Czyścimy tylko jeśli każdy z korzeni ma nazwisko "Johnson", czyli wygenerowaliśmy dane
            if (_roots.All(p => p.Surname == "Johnson"))
            {
                _roots.Clear();
            }

            // Szerokość drzewa
            const int numRoot = 5, numChild = 5, numGrand = 5;

            for (int i = 1; i <= numRoot; i++)
            {
                var root = new Person($"Root{i}", "Johnson", _rand.Next(100));
                for (int j = 1; j <= numChild; j++)
                {
                    var child = new Person($"Child{j}", "Johnson", _rand.Next(50));
                    for (int k = 1; k <= numGrand; k++)
                    {
                        var grand = new Person($"Grand{k}", "Johnson", _rand.Next(20));
                        child.Children.Add(grand);

                        grand.Parent = child;
                    }
                    root.Children.Add(child);
                    child.Parent = root;
                }
                _roots.Add(root);
                root.Parent = null;
            }
        }

        private void Version_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                $"PT_Lab1 v1.2\nAutorzy: Igor Tomkowicz, \n Aleksander Hlebowicz \n Filip Olszewski" +
                $"\nData: {DateTime.Now:dd.MM.yyyy}",
                "Version");
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Zwraca wszystkie Person w strukturze drzewa płasko (jeden ciąg wszystkich węzłów).
        private IEnumerable<Person> Flatten(ObservableCollection<Person> nodes)
        {
            foreach (var p in nodes)
            {
                // Zwracamy bieżący węzeł
                yield return p;

                // Rekurencyjnie zwracamy dzieci
                foreach (var c in Flatten(p.Children))
                    yield return c;
            }
        }

        // Buduje tekst z podstawowymi danymi (oraz studentInfo)
        private string BuildDetails(Person p, int indent = 0)
        {
            var pad = new string(' ', indent * 4);
            var sb = new StringBuilder();

            sb.AppendLine($"{pad}Id: {p.Id}");
            sb.AppendLine($"{pad}Name: {p.Name}");
            sb.AppendLine($"{pad}Surname: {p.Surname}");
            sb.AppendLine($"{pad}Age: {p.Age}");
            sb.AppendLine($"{pad}GPA: {p.Info.GPA:F2}");
            sb.AppendLine($"{pad}Completed Projects: {p.Info.CompletedProjects}");
            sb.AppendLine($"{pad}Year of Study: {p.Info.Year}");
            sb.AppendLine($"{pad}Created At: {p.CreatedAt:yyyy-MM-dd HH:mm:ss}");

            foreach (var child in p.Children)
            {
                sb.AppendLine($"{pad}-- Child --");
                sb.Append(BuildDetails(child, indent + 1));
            }
            return sb.ToString();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeView.SelectedItem is Person p)
            {
                detailsTextBlock.Text = BuildDetails(p);
            }
            else
            {
                detailsTextBlock.Text = string.Empty;
            }
        }

        private void DeleteMenu_Click(object sender, RoutedEventArgs e)
        {
            // Próbujemy pobrać osobę, którą chcemy usunąć (z menu kontekstowego)
            MenuItem menuItem = sender as MenuItem;
            Person personToDelete = menuItem?.CommandParameter as Person;

            if (personToDelete == null)
                return; // Jeśli nie ma osoby, nic nie robimy

            // Jeśli osoba jest korzeniem
            if (_roots.Contains(personToDelete))
            {
                _roots.Remove(personToDelete);
            }
            else
            {
                // Jeśli to nie korzeń – szukamy osoby rekurencyjnie w strukturze drzewa i ją usuwamy
                RemoveFromParent(_roots, personToDelete);
            }
        }

        private bool RemoveFromParent(ObservableCollection<Person> list, Person child)
        {
            foreach (var parent in list)
            {
                if (parent.Children.Contains(child))
                {
                    parent.Children.Remove(child);
                    // Odświeżamy widok
                    detailsTextBlock.Text = BuildDetails(parent);
                    return true;
                }

                if (RemoveFromParent(parent.Children, child))
                {
                    return true;
                }
            }
            return false;
        }

        private void CreateMenu_Click(object sender, RoutedEventArgs e)
        {
            // Pobieramy osobę, dla której chcemy stworzyć dziecko
            var mi = sender as MenuItem;
            var parent = mi?.CommandParameter as Person;
            if (parent == null) return;

            var dlg = new CreatePersonWindow(parent);
            dlg.Owner = this;
            if (dlg.ShowDialog() == true)
            {
                // Dodajemy nowego potomka do rodzica
                parent.Children.Add(dlg.CreatedPerson);

                dlg.CreatedPerson.Parent = parent;
                // Odświeżamy widok
                detailsTextBlock.Text = BuildDetails(parent);
            }
        }

        // Wspólna metoda pomocnicza do zebrania danych:
        private IEnumerable<Person> GetQuerySource(object sender)
        {
            // Próbujemy pobrać Person z CommandParameter (będzie nie‐null tylko w menu kontekstowym)
            if (sender is MenuItem mi && mi.CommandParameter is Person p)
            {
                // kontekstowe: tylko ten węzeł + jego poddrzewo
                return Flatten(new ObservableCollection<Person> { p });
            }
            else
            {
                // menu główne: cała kolekcja
                return Flatten(_roots);
            }
        }

        // LINQ Query 1: filtrujemy po nieparzystym ID, projektujemy sumę pól i uppercase
        private void LinqQuery1_Click(object sender, RoutedEventArgs e)
        {
            // Ustalamy, czy pracujemy na całym drzewie (null) czy poddrzewie konkretnego węzła
            _lastQuery1Root = (sender is MenuItem mi && mi.CommandParameter is Person p)
                ? p
                : null;

            // Pobieramy źródło danych zgodnie z kontekstem
            var source = GetQuerySource(sender);

            // Wykonujemy Query1 i zapisujemy wyniki
            _lastQuery1Results = source
                .Where(x => x.Id % 2 != 0)
                .Select(x => new Query1Result
                {
                    // ID = x.Id, // opcjonalne
                    SUM_OF = x.Info.GPA + x.Info.CompletedProjects,
                    UPPERCASE = x.Info.Year.ToString().ToUpper()
                })
                .ToList();

            // Wyświetlamy wynik
            var sb = new StringBuilder();
            foreach (var r in _lastQuery1Results)
                sb.AppendLine($"SUM_OF = {r.SUM_OF:F2}, UPPERCASE = {r.UPPERCASE}");

            MessageBox.Show(sb.ToString(), "LINQ Query 1 Results");
        }


        // LINQ Query 2: grupowanie po UPPERCASE i średnia SUM_OF
        private void LinqQuery2_Click(object sender, RoutedEventArgs e)
        {
            // Sprawdzamy czy Query 1 było uruchomione
            if (_lastQuery1Results == null)
            {
                MessageBox.Show(
                    "Najpierw uruchom Query 1!",
                    "LINQ Query 2",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Ustalamy bieżący root (null = całe drzewo, nie-null = poddrzewo)
            Person currentRoot = (sender is MenuItem mi && mi.CommandParameter is Person p)
                ? p
                : null;

            // Sprawdzamy, czy kontekst się zgadza z tym z Query1
            if (currentRoot != _lastQuery1Root)
            {
                MessageBox.Show(
                    "Query 2 musi być uruchomione w tym samym kontekście co Query 1.",
                    "LINQ Query 2",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Grupujemy na wynikach z Query1
            var groups = _lastQuery1Results
                .GroupBy(r => r.UPPERCASE)
                .Select(g => new
                {
                    UPPERCASE = g.Key,
                    Average = g.Average(r => r.SUM_OF)
                });

            // Wyświetlamy rezultat
            var sb = new StringBuilder();
            foreach (var g in groups)
                sb.AppendLine($"Group = {g.UPPERCASE}, Average SUM_OF = {g.Average:F2}");

            MessageBox.Show(sb.ToString(), "LINQ Query 2 Results");
        }

        // Funkcja pomocnicza do podmiany całego poddrzewa
        private bool ReplaceSubtree(
            ObservableCollection<Person> list,
            Person oldRoot,
            Person newRoot)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == oldRoot)
                {
                    list[i] = newRoot;
                    return true;
                }
                if (ReplaceSubtree(list[i].Children, oldRoot, newRoot))
                    return true;
            }
            return false;
        }

        private void XmlSerialize_Click(object sender, RoutedEventArgs e)
        {
            // Zapamiętujemy kontekst serializacji
            _lastSerializedRoot = (sender is MenuItem mi && mi.CommandParameter is Person p)
                ? p
                : null;

            // Przygotowujemy dane do serializacji
            var toSerialize = (_lastSerializedRoot == null)
                ? _roots
                : new ObservableCollection<Person> { _lastSerializedRoot };

            try
            {
                var serializer = new XmlSerializer(typeof(ObservableCollection<Person>));
                using (var fs = new FileStream("data.xml", FileMode.Create))
                    serializer.Serialize(fs, toSerialize);

                MessageBox.Show(
                    $"Serializacja {(_lastSerializedRoot == null ? "całego drzewa" : $"poddrzewa od '{_lastSerializedRoot.Name}'")} zakończona.",
                    "XML Serialize",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd serializacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void XmlDeserialize_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("data.xml"))
            {
                MessageBox.Show("Plik data.xml nie istnieje!", "XML Deserialize",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Pobieramy kontekst kliknięcia
            Person currentRoot = (sender is MenuItem mi && mi.CommandParameter is Person p)
                ? p
                : null;

            // Wczytanie z pliku
            ObservableCollection<Person> deserialized;
            try
            {
                var serializer = new XmlSerializer(typeof(ObservableCollection<Person>));
                using (var fs = new FileStream("data.xml", FileMode.Open))
                    deserialized = (ObservableCollection<Person>)serializer.Deserialize(fs);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd deserializacji: {ex.Message}", "Błąd",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Regeneracja unikatowych Id
            int baseUnix = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
            int counter = 0;
            foreach (var x in Flatten(deserialized))
                x.Id = baseUnix + (counter++);

            // Sprawdzamy czy wczytano całe drzewo czy poddrzewo
            bool fullTreeWasSerialized = _lastSerializedRoot == null;

            if (fullTreeWasSerialized)
            {
                if (currentRoot == null)
                {
                    // deserializacja z menu głównego — nadpisujemy całe drzewo
                    _roots = new PersonCollection(deserialized);
                    // _roots = deserialized; - to wersja dla _root = Person, nie PersonCollection
                }
                else
                {
                    // komunikat ostrzegawczy
                    var warn =
                        "Plik data.xml zawiera CAŁE drzewo (serializowane z menu głównego),\n" +
                        $"a Ty wywołujesz deserializację na węźle '{currentRoot.Name}'.\n\n" +
                        "Po potwierdzeniu dzieci tego węzła zostaną zastąpione całym drzewem z pliku,\n" +
                        "inne korzenie pozostaną nienaruszone.\n\n" +
                        "Czy na pewno chcesz kontynuować?";
                    if (MessageBox.Show(warn, "Deserializacja całego drzewa na węźle",
                                        MessageBoxButton.YesNo, MessageBoxImage.Warning)
                        != MessageBoxResult.Yes)
                        return;

                    // podmieniamy dzieci wybranego węzła
                    currentRoot.Children.Clear();
                    foreach (var node in deserialized)
                        currentRoot.Children.Add(node);
                }
            }
            else
            {
                // wczytano poddrzewo z konkretnego węzła
                if (currentRoot == null)
                {
                    // OSTRZEŻENIE: poddrzewo -> deserializacja na cały zbiór
                    var warnSub =
                        $"Plik data.xml zawiera PODDRZEWO od '{_lastSerializedRoot.Name}',\n" +
                        "a Ty wywołujesz deserializację z menu głównego (całe drzewo).\n\n" +
                        "Po potwierdzeniu całe istniejące drzewo zostanie zastąpione tym poddrzewem.\n\n" +
                        "Czy na pewno chcesz kontynuować?";
                    if (MessageBox.Show(warnSub, "Deserializacja poddrzewa na całe drzewo",
                                        MessageBoxButton.YesNo, MessageBoxImage.Warning)
                        != MessageBoxResult.Yes)
                        return;

                    // nadpisujemy całe drzewo tym jednym poddrzewem
                    // _roots = new ObservableCollection<Person> { deserialized.First() }; - to wersja dla _root = Person, nie PersonCollection
                    _roots = new PersonCollection(new[] { deserialized.First() });

                }
                else
                {
                    if (currentRoot != _lastSerializedRoot)
                    {
                        var msg =
                            $"Plik zawiera poddrzewo od '{_lastSerializedRoot.Name}',\n" +
                            $"a deserializujesz na węźle '{currentRoot.Name}'.\n\n" +
                            "Po potwierdzeniu to wybrane poddrzewo zostanie zastąpione.";
                        if (MessageBox.Show(msg, "Deserializacja w niezgodnym kontekście",
                                            MessageBoxButton.YesNo, MessageBoxImage.Warning)
                            != MessageBoxResult.Yes)
                            return;
                    }
                    // podmieniamy cały węzeł
                    ReplaceSubtree(_roots, currentRoot, deserialized.First());
                }
            }

            // odświeżenie TreeView
            treeView.ItemsSource = _roots;
            if (fullTreeWasSerialized)
            {
                // próbujemy pobrać aktualnie zaznaczony obiekt
                var selected = treeView.SelectedItem as Person;

                if (selected != null)
                {
                    // odświeżamy zaznaczony obiekt
                    detailsTextBlock.Text = BuildDetails(selected);
                }
            }

            MessageBox.Show("Deserializacja zakończona pomyślnie.", "XML Deserialize",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowXPath_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("data.xml"))
            {
                MessageBox.Show("Plik data.xml nie istnieje!", "XPath");
                return;
            }

            try
            {
                var doc = XDocument.Load("data.xml");

                // Szukamy wszystkich wartości CompletedProjects
                var allProjects = doc
                    .Descendants("CompletedProjects")
                    .Select(x => int.Parse(x.Value))
                    .ToList();

                // Szukamny unikatowych wartości
                var uniqueProjects = allProjects
                    .GroupBy(x => x)
                    .Where(g => g.Count() == 1)
                    .Select(g => g.Key)
                    .ToHashSet();

                // XPath do wszystkich osób
                var persons = doc.XPathSelectElements("//Person");

                // Filtrujemy tylko te, które mają unikalną wartość CompletedProjects
                var matching = persons
                    .Where(p =>
                    {
                        var proj = p.Element("Info")?.Element("CompletedProjects");
                        return proj != null && int.TryParse(proj.Value, out int val) && uniqueProjects.Contains(val);
                    });

                // Budujemy wynik
                var sb = new StringBuilder();
                foreach (var p in matching)
                {
                    var name = p.Element("Name")?.Value;
                    var projects = p.Element("Info")?.Element("CompletedProjects")?.Value;
                    sb.AppendLine($"{name} - CompletedProjects: {projects}");
                }

                if (sb.Length == 0)
                {
                    MessageBox.Show("Brak osób z unikalną liczbą ukończonych projektów.", "XPath");
                }
                else
                {
                    MessageBox.Show(sb.ToString(), "XPath - Unikalne CompletedProjects");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd przetwarzania XPath: {ex.Message}", "Błąd");
            }
        }

        private void ExportXhtml_Click(object sender, RoutedEventArgs e)
        {
            // Ustalamy, czy mamy kontekst węzła
            Person contextNode = null;
            if (sender is MenuItem mi && mi.CommandParameter is Person p)
                contextNode = p;

            // Wybieramy źródło: cały zbiór lub pojedynczy węzeł
            IEnumerable<Person> source = contextNode == null
                ? Flatten(_roots)
                : Flatten(new ObservableCollection<Person> { contextNode });

            try
            {
                // Definicja przestrzeni nazw XHTML
                XNamespace xhtml = "http://www.w3.org/1999/xhtml";

                // Budujemy dokument XHTML
                var doc = new XDocument(
                    new XDocumentType("html", null, null, null),
                    new XElement(xhtml + "html",
                        new XElement(xhtml + "head",
                            new XElement(xhtml + "title", "Exported Persons")
                        ),
                        new XElement(xhtml + "body",
                            new XElement(xhtml + "h1",
                                contextNode == null
                                  ? "Person Collection (All)"
                                  : $"Person Subtree: {contextNode.Name}"
                            ),
                            new XElement(xhtml + "table",
                                new XElement(xhtml + "thead",
                                    new XElement(xhtml + "tr",
                                        new XElement(xhtml + "th", "Id"),
                                        new XElement(xhtml + "th", "Name"),
                                        new XElement(xhtml + "th", "Surname"),
                                        new XElement(xhtml + "th", "Age"),
                                        new XElement(xhtml + "th", "CompletedProjects"),
                                        new XElement(xhtml + "th", "GPA"),
                                        new XElement(xhtml + "th", "Year")
                                    )
                                ),
                                new XElement(xhtml + "tbody",
                                    from person in source
                                    select new XElement(xhtml + "tr",
                                        new XElement(xhtml + "td", person.Id),
                                        new XElement(xhtml + "td", person.Name),
                                        new XElement(xhtml + "td", person.Surname),
                                        new XElement(xhtml + "td", person.Age),
                                        new XElement(xhtml + "td", person.Info.CompletedProjects),
                                        new XElement(xhtml + "td", person.Info.GPA.ToString("F2")),
                                        new XElement(xhtml + "td", person.Info.Year.ToString())
                                    )
                                )
                            )
                        )
                    )
                );

                // Zapis pliku
                string path = contextNode == null ? "data_all.xhtml" : $"data_{contextNode.Name}.xhtml";
                doc.Save(path);

                MessageBox.Show(
                    $"Export do XHTML ukończony: {path}",
                    "Export to XHTML",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Błąd podczas eksportu XHTML: {ex.Message}",
                    "Błąd",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Pomocnicza klasa do przechowywania wyników LINQ Query 1
        private class Query1Result
        {
            // public int ID { get; set; } - opcjonalne
            public double SUM_OF { get; set; }
            public string UPPERCASE { get; set; }
        }

        private void ShowChildrenFromRow_Click(object sender, RoutedEventArgs e)
        {
            // jeśli to placeholder, DataContext nie będzie Person, wiec nic nie robimy
            if (!(((Button)sender).DataContext is Person person))
                return;

            if (person.Children.Any())
            {
                dataGrid.ItemsSource = person.Children;
                // Odświeżamy też treeView, żeby było zsynchronizowane
                treeView.ItemsSource = person.Children;
            }
        }
        private void GoUpFromRow_Click(object sender, RoutedEventArgs e)
        {
            if (!(((Button)sender).DataContext is Person person)) return;

            //// Debug:
            //Debug.Print($"Current node: {person.Name} (Id: {person.Id})");
            //Debug.Print($"Parent: {person.Parent?.Name}");

            var parent = person.Parent;
            if (parent == null)
            {
                // Nie ma parenta - jesteśmy na _roots
                dataGrid.ItemsSource = _roots;
                treeView.ItemsSource = _roots;
            }
            else
            {
                var grandParent = parent.Parent;
                if (grandParent == null)
                {
                    // Rodzic jest korzeniem - pokazujemy wszystkich korzeni (_roots)
                    dataGrid.ItemsSource = _roots;
                    treeView.ItemsSource = _roots;
                }
                else
                {
                    // W przeciwnym razie pokazujemy wszystkich children dziadka
                    // czyli rodzeństwo parenta
                    dataGrid.ItemsSource = grandParent.Children;
                    treeView.ItemsSource = grandParent.Children;
                }
            }
        }


        // Ładowanie nazw właściwości string/int przy otwarciu listy (event Enter)
        private void cbProperties_GotFocus(object sender, RoutedEventArgs e)
        {
            var combo = (ComboBox)sender;

            // Odłączamy handler, żeby Clear() nie wywołało ponownie GotFocus
            combo.GotFocus -= cbProperties_GotFocus;

            combo.Items.Clear();

            // 1. Własności Person
            var personProps = typeof(Person)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p =>
                    p.PropertyType == typeof(string) ||
                    p.PropertyType == typeof(int));

            // 2. Własności StudentInfo (zagnieżdżone)
            //var infoProps = typeof(StudentInfo)
            //    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            //    .Where(p =>
            //        p.PropertyType == typeof(string) ||
            //        p.PropertyType == typeof(int) ||
            //        p.PropertyType == typeof(double) ||
            //        p.PropertyType.IsEnum);

            // Dodajemy najpierw Person.Name, Person.Surname, itp.
            foreach (var p in personProps)
                combo.Items.Add(p.Name);

            // A potem Info.GPA, Info.CompletedProjects, Info.Year
            //foreach (var p in infoProps)
            //    combo.Items.Add("Info." + p.Name);

            if (combo.Items.Count > 0)
                combo.SelectedIndex = 0;
        }

        // Filtrowanie po kliknięciu "Szukaj"
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var propName = cbProperties.SelectedItem as string;
            var searchText = tbSearchValue.Text?.Trim();
            if (string.IsNullOrEmpty(propName) || string.IsNullOrEmpty(searchText))
                return;

            // Jeśli to własność typu string
            var propInfo = typeof(Person).GetProperty(propName);
            if (propInfo == null)
                return;

            ObservableCollection<Person> results;

            if (propInfo.PropertyType == typeof(string))
            {
                // wyszukiwanie po string
                var found = _roots.FindByString(propName, searchText);
                results = new ObservableCollection<Person>(found);
            }
            else if (propInfo.PropertyType == typeof(int))
            {
                // wyszukiwanie po int (tylko jeśli uda się sparsować)
                if (int.TryParse(searchText, out int intVal))
                {
                    var found = _roots.FindByInt(propName, intVal);
                    results = new ObservableCollection<Person>(found);
                }
                else
                {
                    // jeśli niepoprawny int, nic nie zmieniamy
                    return;
                }
            }
            else
            {
                // inne typy ignorujemy
                return;
            }

            // ustawiamy wynik jako nowe źródło DataGrid
            dataGrid.ItemsSource = results;
        }


        // Przywracanie domyślnego widoku po kliknięciu "Czyść filtr"
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            // Przywracamy pierwotne źródło (korzenie)
            dataGrid.ItemsSource = _roots;
            //treeView.ItemsSource = _roots;
            tbSearchValue.Clear();
        }

        // Ładowanie listy właściwości
        private void cbSortProperties_DropDownOpened(object sender, EventArgs e)
        {
            var combo = (ComboBox)sender;
            combo.Items.Clear();

            // 1. Własności Person: string i int
            var personProps = typeof(Person)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p =>
                    p.PropertyType == typeof(string) ||
                    p.PropertyType == typeof(int));

            // 2. Własności StudentInfo: string, int, double, enum
            var infoProps = typeof(StudentInfo)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p =>
                    p.PropertyType == typeof(string) ||
                    p.PropertyType == typeof(int) ||
                    p.PropertyType == typeof(double) ||
                    p.PropertyType.IsEnum);

            // Dodajemy Person.Name, Person.Surname, itp.
            foreach (var p in personProps)
                combo.Items.Add(p.Name);

            // Dodajemy Info.GPA, Info.Year, Info.CompletedProjects (z prefixem)
            foreach (var p in infoProps)
                combo.Items.Add($"Info.{p.Name}");

            // Domyślny wybór pierwszego elementu
            if (combo.Items.Count > 0)
                combo.SelectedIndex = 0;
        }

        // Obsługa przycisku Sortuj
        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            var propName = cbSortProperties.SelectedItem as string;
            if (string.IsNullOrEmpty(propName)) return;

            // 0) WYCZYSZCZENIE SORTOWANIA W DATAGRID
            // Pobieramy CollectionView używany przez DataGrid
            var view = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            // Czyścimy wszystkie SortDescriptions (te z nagłówków)
            view.SortDescriptions.Clear();
            // Resetujemy też kierunek sortowania na kolumnach (strzałki)
            foreach (var col in dataGrid.Columns)
                col.SortDirection = null;

            try
            {
                _roots.SortByProperty(propName);
                dataGrid.ItemsSource = _roots;
                treeView.ItemsSource = _roots;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Błąd sortowania: {ex.Message}", "Sortowanie", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddChildFromRow_Click(object sender, RoutedEventArgs e)
        {
            // Pobieramy osobę dla danego wiersza
            if (!(((Button)sender).DataContext is Person parent))
                return;

            // Tworzymy i konfigurujemy okno
            var dlg = new CreatePersonWindow(parent)
            {
                Owner = this
            };
            if (dlg.ShowDialog() == true)
            {
                // Dodajemy nowe dziecko
                parent.Children.Add(dlg.CreatedPerson);
                dlg.CreatedPerson.Parent = parent;

                // Odświeżamy widok - panel szczegółów
                detailsTextBlock.Text = BuildDetails(parent);
            }
        }


    }
}
