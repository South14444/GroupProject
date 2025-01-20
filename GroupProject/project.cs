using GroupProject.Models;
using GroupProject.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProject
{
    internal class project
    {
        static async Task Main(string[] args)
        {
            var userRegistration = new Registration();

            var newUser = new User
            {
                Name = "Имя",
                Surname = "Фамилия",
                Password = "Пароль",
                Email = "email@example.com"
            };

            try
            {
                await userRegistration.RegisterUserAsync(newUser);
                Console.WriteLine("Пользователь успешно зарегистрирован.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка регистрации: {ex.Message}");
            }
        }
    }
}
