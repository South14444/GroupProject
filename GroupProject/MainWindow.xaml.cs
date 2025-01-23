using GroupProject.Models;
using GroupProject.Service;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace GroupProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var userRegistration = new Registration();

            string email = EmailLoginTextBox.Text;
            string password = PasswordLoginBox.Password;

            try
            {
                var loggedInUser = await userRegistration.LoginUserAsync(email, password);
                MessageBox.Show($"Добро пожаловать, {loggedInUser.Name} {loggedInUser.Surname}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                Application_1 application_1 = new Application_1();
                application_1.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка входа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var userRegistration = new Registration();

            string firstName = FirstNameTextBox.Text;
            string lastName = LastNameTextBox.Text;

            // Проверка на буквы
            if (!IsValidName(firstName))
            {
                MessageBox.Show("Имя может содержать только буквы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsValidName(lastName))
            {
                MessageBox.Show("Фамилия может содержать только буквы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newUser = new User
            {
                Name = firstName,
                Surname = lastName,
                Password = PasswordRegisterBox.Password,
                Email = EmailRegisterTextBox.Text,
            };

            try
            {
                await userRegistration.RegisterUserAsync(newUser);
                MessageBox.Show("Пользователь успешно зарегистрирован.");

                Application_1 application_1 = new Application_1();
                application_1.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}");
            }
        }

        // Метод для проверки, что строка состоит только из букв
        private bool IsValidName(string name)
        {
            return !string.IsNullOrEmpty(name) && Regex.IsMatch(name, @"^[a-zA-Zа-яА-ЯёЁ]+$");
        }
    }
}
