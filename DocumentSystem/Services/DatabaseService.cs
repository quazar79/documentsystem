using DocumentSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSystem.Services
{
    public class DatabaseService
    {
        private string connectionString = "Data Source=localhost;Initial Catalog=DocumentManagement_DB;Integrated Security=True";


        public bool UserExists(string userName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM Users WHERE UserName = @UserName";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserName", userName);

                connection.Open();
                int result = (int)command.ExecuteScalar();
                return result > 0;
            }
        }

        public bool RegisterUser(User newUser)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Users (UserName, Password, UserRole) VALUES (@UserName, @Password, @UserRole)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserName", newUser.UserName);
                command.Parameters.AddWithValue("@Password", newUser.Password);
                command.Parameters.AddWithValue("@UserRole", newUser.UserRole);

                connection.Open();
                int result = command.ExecuteNonQuery();
                return result > 0;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM Users WHERE UserName = @UserName AND Password = @Password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserName", userName);
                command.Parameters.AddWithValue("@Password", password);

                connection.Open();
                int result = (int)command.ExecuteScalar();
                return result > 0;
            }
        }

        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT UserName, UserRole FROM Users";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    User user = new User
                    {
                        UserName = reader["UserName"].ToString(),
                        UserRole = reader["UserRole"].ToString()
                    };
                    users.Add(user);
                }
            }

            return users;
        }

        public List<Document> GetDocuments()
        {
            List<Document> documents = new List<Document>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT DocumentId, DocumentName, DocumentContent FROM Documents";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Document document = new Document
                            {
                                DocumentId = reader.GetInt32(0),
                                DocumentName = reader.GetString(1),
                                DocumentContent = reader.GetString(2)
                            };
                            documents.Add(document);
                        }
                    }
                }
            }
            return documents;
        }

        public void UpdateDocument(int documentId, string documentContent)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Documents SET DocumentContent = @DocumentContent WHERE DocumentId = @DocumentId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DocumentContent", documentContent);
                    command.Parameters.AddWithValue("@DocumentId", documentId);
                    command.ExecuteNonQuery();
                }
            }
        }



        public void AddDocument(string documentName, string documentContent)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Documents (DocumentName, DocumentContent) VALUES (@DocumentName, @DocumentContent)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DocumentName", documentName);
                    command.Parameters.AddWithValue("@DocumentContent", documentContent);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void RenameDocument(int documentId, string newName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Documents SET DocumentName = @NewName WHERE DocumentId = @DocumentId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NewName", newName);
                    command.Parameters.AddWithValue("@DocumentId", documentId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteDocument(int documentId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Documents WHERE DocumentId = @DocumentId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DocumentId", documentId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
