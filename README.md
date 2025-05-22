# HierarchyHub

> WPF Desktop Application do zarządzania, przetwarzania i prezentacji hierarchicznych zbiorów danych.

---

## Spis treści

* [Opis projektu](#opis-projektu)
* [Kluczowe funkcje](#kluczowe-funkcje)
* [Interfejs użytkownika](#interfejs-użytkownika)
* [Przetwarzanie danych](#przetwarzanie-danych)
* [Zaawansowane operacje](#zaawansowane-operacje)
* [Struktura projektu](#struktura-projektu)
* [Uruchomienie](#uruchomienie)
* [Kontakt](#kontakt)
* [Licencja](#licencja)

---

## Opis projektu

**HierarchyHub** to aplikacja desktopowa stworzona w technologii WPF (.NET), która umożliwia generowanie, przeglądanie, modyfikację oraz analizę rekurencyjnych struktur danych. Aplikacja demonstruje najlepsze praktyki w zakresie architektury MVVM, pracy z XML, LINQ, serializacji oraz zaawansowanej prezentacji informacji.

---

## Kluczowe funkcje

1. **CRUD dla hierarchii**

   * Generowanie dynamicznych struktur drzewiastych
   * Dodawanie i usuwanie węzłów z poziomu menu kontekstowego
2. **Model rozszerzony**

   * Relacja jeden-do-jednego między główną klasą i dodatkowym obiektem (klasą asocjacyjną)
   * Automatyczne generowanie co najmniej 50 elementów z unikatowym ID
3. **Zaawansowane zapytania**

   * LINQ: projekcje, grupowania i analizy na obiektach w pamięci
   * XPath i LINQ to XML: filtrowanie i generowanie dokumentu XHTML z tabelą danych
4. **Serializacja XML**

   * Eksport i import danych w formacie XML
   * Automatyczne przypisywanie ID podczas deserializacji
5. **Prezentacja w DataGrid**

   * Pokazywanie ObservableCollection z możliwością edycji, sortowania i filtrowania
   * Dynamiczne ładowanie właściwości (string, int) do wyszukiwarki i sortera

---

## Interfejs użytkownika

* **Menu główne**

  * Generuj dane, Pokazuj wersję, Zakończ
* **TreeView + Details Pane**

  * Lewy panel: hierarchia w TreeView
  * Prawy panel: szczegóły wybranego węzła z formularzem i widokiem plików
* **Menu kontekstowe**

  * Create, Delete, Query operations
* **Toolbar**

  * Wyszukiwarka i sortowanie kolumn w DataGrid

---

## Przetwarzanie danych

* **Model danych**: Klasa główna z powiązaną klasą pomocniczą (enum + pole liczby całkowitej)
* **Generowanie**: Konstruktor tworzy drzewo i automatycznie wypełnia listę elementów
* **Operacje LINQ**:

  * Projekcje SUM\_OF i UPPERCASE dla nieparzystych ID
  * Grupowanie i średnie wartości wyświetlane w konsoli

---

## Zaawansowane operacje

* **XPath**: Filtrowanie unikatowych wartości pola numerycznego w wygenerowanym XML
* **LINQ to XML**: Tworzenie dokumentu XHTML z tabelą obiektów
* **ObservableCollection<T>**:

  * Rozszerzona klasa wspierająca sortowanie i wyszukiwanie po wybranych właściwościach
* **DataGrid**:

  * Edycja, dodawanie, usuwanie, sortowanie kolumn, dynamiczne wyszukiwanie

---

## Struktura projektu

```
HierarchyHub/
├── bin/                      # skompilowane pliki aplikacji
├── obj/                      # pliki tymczasowe kompilatora
├── Properties/               # ustawienia projektu (AssemblyInfo, itd.)
├── App.config                # konfiguracja aplikacji (connection strings, ustawienia)
├── App.xaml                  # entry point WPF (definicja zasobów, uruchomienie)
├── App.xaml.cs               # kod-behind dla App.xaml
├── MainWindow.xaml           # główne okno aplikacji
├── MainWindow.xaml.cs        # kod-behind dla MainWindow
├── CreatePersonWindow.xaml   # okno dialogu dodawania nowego obiektu
├── CreatePersonWindow.xaml.cs# kod-behind dla CreatePersonWindow
├── Person.cs                 # definicja klasy modelu Person
├── PersonCollection.cs       # klasa ObservableCollection dla Person, z sortowaniem/wyszukiwaniem
├── StudentInfo.cs            # dodatkowy model związany (enum, pole int)
├── PT_Lab1.csproj            # plik projektu .NET
├── PT_Lab1.csproj.user       # użytkownikowe ustawienia projektu
└── README.md                 # dokumentacja
```
## Kontakt

* **Igor Tomkowicz**
* GitHub: [npnpdev](https://github.com/npnpdev)
* LinkedIn: [Igor Tomkowicz](https://www.linkedin.com/in/igor-tomkowicz-a5760b358/)
* E-mail: [npnpdev@gmail.com](mailto:npnpdev@gmail.com)

---

## Licencja

Projekt udostępniony na licencji MIT. Szczegóły w pliku [LICENSE](LICENSE).
