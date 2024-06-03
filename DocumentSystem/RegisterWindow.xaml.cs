using DocumentSystem.Models;
using DocumentSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DocumentSystem
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private DatabaseService _databaseService;

        public RegisterWindow()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            string userName = UserNameTextBox.Text;
            string password = PasswordBox.Password;
            string role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            User newUser = new User
            {
                UserName = userName,
                Password = password,
                UserRole = role
            };

            if (_databaseService.UserExists(userName))
            {
                MessageBox.Show("Пользователь с таким именем уже существует.");
            }
            else
            {
                if (_databaseService.RegisterUser(newUser))
                {
                    MessageBox.Show("Регистрация успешна!");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Ошибка при регистрации.");
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
