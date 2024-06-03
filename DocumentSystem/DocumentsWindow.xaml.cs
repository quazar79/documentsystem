using DocumentSystem.Models;
using DocumentSystem.Services;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Diagnostics;
using DocumentFormat.OpenXml.Packaging;

namespace DocumentSystem
{
    /// <summary>
    /// Логика взаимодействия для DocumentsWindow.xaml
    /// </summary>
    public partial class DocumentsWindow : Window
    {
        private DatabaseService _databaseService;
       

        public DocumentsWindow()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            LoadDocuments();            
        }
        private void LoadDocuments()
        {
            try
            {
                List<Document> documents = _databaseService.GetDocuments();
                DocumentsDataGrid.ItemsSource = documents;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке документов: " + ex.Message);
            }
        }

        

        private void CreateDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создание временного файла .rtf
                string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString() + ".rtf");

                // Создание пустого документа RTF (это важно для инициализации файла)
                File.WriteAllText(tempFilePath, @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang1033{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1 \pard\sa200\sl276\slmult1\f0\fs22\lang9 This is a new document.\par}");

                // Запуск текстового редактора (например, WordPad)
                Process wordPadProcess = new Process();
                wordPadProcess.StartInfo.FileName = "write.exe"; // WordPad
                wordPadProcess.StartInfo.Arguments = tempFilePath;
                wordPadProcess.Start();
                wordPadProcess.WaitForExit();

                // Проверка наличия созданного документа
                if (File.Exists(tempFilePath))
                {
                    // Чтение содержимого документа
                    string documentContent = File.ReadAllText(tempFilePath);
                    string documentName = System.IO.Path.GetFileName(tempFilePath);

                    // Добавление документа в базу данных
                    _databaseService.AddDocument(documentName, documentContent);

                    // Удаление временного файла
                    File.Delete(tempFilePath);

                    MessageBox.Show("Документ успешно сохранен в базе данных.");
                    LoadDocuments(); // Обновление списка документов
                }
                else
                {
                    MessageBox.Show("Документ не был сохранен.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании документа: " + ex.Message);
            }
        }



        private void DocumentsDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DocumentsDataGrid.SelectedItem is Document selectedDocument)
            {
                try
                {
                    // Создание временного файла .rtf
                    string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), selectedDocument.DocumentName);

                    // Запись содержимого документа во временный файл
                    File.WriteAllText(tempFilePath, selectedDocument.DocumentContent);

                    // Запуск текстового редактора (например, WordPad)
                    Process wordPadProcess = new Process();
                    wordPadProcess.StartInfo.FileName = "write.exe"; // WordPad
                    wordPadProcess.StartInfo.Arguments = tempFilePath;
                    wordPadProcess.Start();
                    wordPadProcess.WaitForExit();

                    // Проверка наличия измененного документа
                    if (File.Exists(tempFilePath))
                    {
                        // Чтение содержимого документа
                        string updatedContent = File.ReadAllText(tempFilePath);

                        // Обновление документа в базе данных
                        _databaseService.UpdateDocument(selectedDocument.DocumentId, updatedContent);

                        // Удаление временного файла
                        File.Delete(tempFilePath);

                        MessageBox.Show("Документ успешно обновлен в базе данных.");
                        LoadDocuments(); // Обновление списка документов
                    }
                    else
                    {
                        MessageBox.Show("Документ не был обновлен.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при редактировании документа: " + ex.Message);
                }
            }
        }
        private Document selectedDocument;
       

        private void UserListButton_Click(object sender, RoutedEventArgs e)
        {
            UserListWindow userListWindow = new UserListWindow();
            userListWindow.Show();
            this.Close();
        }

        private void DocumentsDataGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = ItemsControl.ContainerFromElement(DocumentsDataGrid, e.OriginalSource as DependencyObject) as DataGridRow;
            if (row != null)
            {
                selectedDocument = row.Item as Document;
                DocumentsDataGrid.SelectedItem = selectedDocument;

                ContextMenu contextMenu = new ContextMenu();

                MenuItem renameItem = new MenuItem { Header = "Переименовать" };
                renameItem.Click += RenameDocument_Click;
                contextMenu.Items.Add(renameItem);

                
                    MenuItem deleteItem = new MenuItem { Header = "Удалить" };
                    deleteItem.Click += DeleteDocument_Click;
                    contextMenu.Items.Add(deleteItem);
                

                    contextMenu.IsOpen = true;
                
            }
        }

        private void RenameDocument_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDocument != null)
            {
                RenameDocumentWindow renameWindow = new RenameDocumentWindow(selectedDocument.DocumentName);
                if (renameWindow.ShowDialog() == true)
                {
                    string newName = renameWindow.NewDocumentName;
                    if (!string.IsNullOrEmpty(newName))
                    {
                        _databaseService.RenameDocument(selectedDocument.DocumentId, newName);
                        LoadDocuments();
                    }
                }
            }
        }
        private void DeleteDocument_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDocument != null)
            {
                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить документ '{selectedDocument.DocumentName}'?",
                    "Удаление документа", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _databaseService.DeleteDocument(selectedDocument.DocumentId);
                    LoadDocuments();
                }
            }
        }



        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
