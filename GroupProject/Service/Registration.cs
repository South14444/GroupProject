using GroupProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GroupProject.Models.Context;

namespace GroupProject.Service
{
    internal class Registration
    {
        public async Task RegisterUserAsync(User user)
        {
            using (var context = new UserDbContext())
            {
                var existingUser = await context.Users
                    .FirstOrDefaultAsync(u => u.Email == user.Email);

                if (existingUser != null)
                {
                    throw new Exception("Пользователь с таким email уже существует.");
                }

                context.Users.Add(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task<User> LoginUserAsync(string email, string password)
        {
            using (var context = new UserDbContext())
            {
                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    throw new Exception("Пользователь не найден.");
                }

                if (user.Password != password)
                {
                    throw new Exception("Неверный пароль.");
                }

                return user;
            }
        }
    }
}
