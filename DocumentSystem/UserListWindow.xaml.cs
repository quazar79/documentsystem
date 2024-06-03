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
    /// Логика взаимодействия для UserListWindow.xaml
    /// </summary>
    public partial class UserListWindow : Window
    {
        private DatabaseService _databaseService;

        public UserListWindow()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            LoadUsers();
        }

        private void LoadUsers()
        {
            List<User> users = _databaseService.GetAllUsers();
            UsersDataGrid.ItemsSource = users;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            DocumentsWindow documentsWindow = new DocumentsWindow();
            //MainWindow mainWindow = new MainWindow();
            documentsWindow.Show();
            this.Close();
        }
    }
}
