using GroupProject.Models;
using GroupProject.Service;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

            var newUser = new User
            {
                Name = FirstNameTextBox.Text,
                Surname = LastNameTextBox.Text,
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
    }
}