# HierarchyHub

[English](#english-version) | [Polski](#wersja-polska)

---

## English Version

### Project Description

**HierarchyHub** is a WPF desktop application built on .NET for managing, processing, and visualizing hierarchical data sets. The application demonstrates MVVM architecture best practices, XML handling, LINQ queries, serialization, and advanced data presentation.

### Key Features

1. **CRUD for Hierarchies**

   * Generate dynamic tree structures
   * Add and remove nodes via context menu
2. **Extended Data Model**

   * One-to-one relationship between the main class and an auxiliary object
   * Automatic generation of at least 50 items with unique IDs
3. **Advanced Queries**

   * LINQ: projections, groupings, and in-memory analysis
   * XPath and LINQ to XML: filtering and generating an XHTML document with a data table
4. **XML Serialization**

   * Export and import data in XML format
   * Automatic ID assignment during deserialization
5. **DataGrid Presentation**

   * Display an ObservableCollection with editing, sorting, and filtering capabilities
   * Dynamic loading of string and integer properties into search and sort controls

### User Interface

* **Main Menu**

  * Generate Data, Show Version, Exit
* **TreeView & Details Pane**

  * Left pane: hierarchy displayed in a TreeView
  * Right pane: details of selected node with a form and file view
* **Context Menu**

  * Create, Delete, Query operations
* **Toolbar**

  * Search box and column sorting in the DataGrid

### Data Processing

* **Data Model**: Main class with an associated helper class (enum + integer field)
* **Generation**: Constructor builds the tree and auto-populates the item list
* **LINQ Operations**:

  * `SUM_OF` and `UPPERCASE` projections for odd IDs
  * Grouping and average calculations logged to console

### Advanced Operations

* **XPath**: Filter unique numeric field values in the generated XML
* **LINQ to XML**: Create an XHTML document with a table of objects
* **ObservableCollection<T>**:

  * Extended class supporting sorting and searching by selected properties
* **DataGrid**:

  * Editing, adding, deleting rows, column sorting, dynamic searching

### Project Structure

```text
HierarchyHub/
├── bin/                     # compiled application files
├── obj/                     # temporary compiler files
├── Properties/              # project settings (AssemblyInfo, etc.)
├── App.config               # application configuration (connection strings, settings)
├── App.xaml                 # WPF entry point (resource definitions, startup)
├── App.xaml.cs              # code-behind for App.xaml
├── MainWindow.xaml          # main window definition
├── MainWindow.xaml.cs       # code-behind for MainWindow
├── CreatePersonWindow.xaml  # new item dialog definition
├── CreatePersonWindow.xaml.cs # code-behind for CreatePersonWindow
├── Person.cs                # Person model class
├── PersonCollection.cs      # ObservableCollection for Person with sort/search
├── StudentInfo.cs           # auxiliary model (enum, integer field)
├── PT_Lab1.csproj           # .NET project file
├── PT_Lab1.csproj.user      # user-specific project settings
└── README.md                # documentation
```

## Wersja polska

### Opis projektu

**HierarchyHub** to aplikacja desktopowa WPF (.NET) do zarządzania, przetwarzania i wizualizacji hierarchicznych zbiorów danych. Aplikacja demonstruje wzorzec MVVM, obsługę XML, zapytania LINQ, serializację oraz zaawansowaną prezentację danych.

### Kluczowe funkcje

1. **CRUD dla hierarchii**

   * Generowanie dynamicznych struktur drzewiastych
   * Dodawanie i usuwanie węzłów przez menu kontekstowe
2. **Rozszerzony model danych**

   * Relacja jeden‑do‑jednego między klasą główną a obiektem pomocniczym
   * Automatyczne wygenerowanie co najmniej 50 elementów z unikalnymi ID
3. **Zaawansowane zapytania**

   * LINQ: projekcje, grupowania, analiza w pamięci
   * XPath i LINQ to XML: filtrowanie oraz generowanie dokumentu XHTML z tabelą danych
4. **Serializacja XML**

   * Eksport i import danych w formacie XML
   * Automatyczne nadawanie ID podczas deserializacji
5. **Prezentacja w DataGrid**

   * Wyświetlanie ObservableCollection z możliwością edycji, sortowania i filtrowania
   * Dynamiczne ładowanie właściwości (string, int) do wyszukiwarki i sortera

### Interfejs użytkownika

* **Menu główne**

  * Generuj dane, pokaż wersję, zamknij
* **TreeView i panel szczegółów**

  * Lewy panel: hierarchia w TreeView
  * Prawy panel: formularz i widok plików dla wybranego węzła
* **Menu kontekstowe**

  * Operacje: Utwórz, Usuń, Zapytaj
* **Pasek narzędzi**

  * Pole wyszukiwania i sortowanie kolumn w DataGrid

### Przetwarzanie danych

* **Model danych**: Klasa główna z klasą pomocniczą (enum + pole int)
* **Generowanie**: Konstruktor tworzy drzewo i wypełnia listę elementów
* **Operacje LINQ**:

  * Projekcje `SUM_OF` i `UPPERCASE` dla nieparzystych ID
  * Grupowanie i średnie wypisywane w konsoli

### Zaawansowane operacje

* **XPath**: Filtrowanie unikalnych wartości pól numerycznych w XML
* **LINQ to XML**: Tworzenie dokumentu XHTML z tabelą obiektów
* **ObservableCollection<T>**:

  * Rozszerzona klasa obsługująca sortowanie i wyszukiwanie po właściwościach
* **DataGrid**:

  * Edycja, dodawanie, usuwanie wierszy, sortowanie kolumn, dynamiczne wyszukiwanie

---

## Kontakt / Contact

* **Igor Tomkowicz**
* GitHub: [npnpdev](https://github.com/npnpdev)
* LinkedIn: [Igor Tomkowicz](https://www.linkedin.com/in/igor-tomkowicz-a5760b358/)
* E-mail: [npnpdev@gmail.com](mailto:npnpdev@gmail.com)

---

## Licencja / License

Projekt udostępniony na licencji MIT. Szczegóły w pliku [LICENSE](LICENSE).
