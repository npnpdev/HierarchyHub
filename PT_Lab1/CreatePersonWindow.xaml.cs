using System;
using System.Windows;

namespace PT_Lab1
{
    public partial class CreatePersonWindow : Window
    {
        private Random _rand = new Random();
        public Person CreatedPerson {
            // każdy może odczytać wartość
            get;
            // tylko kod w tej klasie może ustawić wartość
            private set; 
        }

        public CreatePersonWindow(Person parent)
        {
            InitializeComponent();
            Title = $"Create child for {parent.Name} (Id: {parent.Id})";
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            NameTextBox.Text = $"Child{_rand.Next(1000)}";
            SurnameTextBox.Text = "Johnson";
            AgeTextBox.Text = _rand.Next(1, 100).ToString();
        }

        // Walidacja wpisanych danych
        private void Ok_Click(object sender, RoutedEventArgs e){
            // Puste pola
            if (string.IsNullOrWhiteSpace(NameTextBox.Text)
                || string.IsNullOrWhiteSpace(SurnameTextBox.Text)
                || !int.TryParse(AgeTextBox.Text, out int age))
            {
                MessageBox.Show("Podaj poprawne dane we wszystkich polach.",
                                "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(age < 0)
            {
                MessageBox.Show("Wiek nie może być ujemny.",
                                "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CreatedPerson = new Person(NameTextBox.Text, SurnameTextBox.Text, age);
            DialogResult = true;
        }
    }
}
