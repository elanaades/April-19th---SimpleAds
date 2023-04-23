using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Simple_Ads_Auth.Data
{
    public class UserRepository
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimpleAds;Integrated Security=true;";

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddNewItem(SimpleAd item, int userid)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Ads (UserId, Date, Phone, Details) " +
                                  "VALUES (@userid, @date, @phone, @details) SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@userid", userid);
            command.Parameters.AddWithValue("@date", DateTime.Now);
            command.Parameters.AddWithValue("@phone", item.Phone);
            command.Parameters.AddWithValue("@details", item.Details);
            connection.Open();

            item.Id = (int)(decimal)command.ExecuteScalar();
        }

        public void DeleteAd(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Ads " +
                                  "WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public List<SimpleAd> GetAllAds()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT a.*, u.Name FROM Ads a " +
                "Join Users u ON u.Id = a.UserId";
            connection.Open();
            var ads = new List<SimpleAd>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new SimpleAd
                {
                    Id = (int)reader["Id"],
                    UserId = (int)reader["UserId"],
                    Details = (string)reader["Details"],
                    Date = (DateTime)reader["Date"],
                    Name = (string)reader["Name"],
                    Phone = (string)reader["Phone"]
                });
            }

            return ads;
        }

        public void AddUser(User user, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Name, Email, PasswordHash) VALUES " +
                "(@name, @email, @hash)";
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            //cmd.Parameters.AddWithValue("@phone", user.Phone);
            cmd.Parameters.AddWithValue("@hash", passwordHash);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            var isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isValid)
            {
                return null;
            }

            return user;

        }

        public User GetByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Users WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new User
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Email = (string)reader["Email"],
                PasswordHash = (string)reader["PasswordHash"]
            };
        }

        public List<SimpleAd> GetAdsByUser(int userid)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT a.*, u.Name FROM Ads a " +
                "Join Users u ON u.Id = a.UserId " +
                "WHERE a.UserId = @userid";
            cmd.Parameters.AddWithValue("@userid", userid);
            connection.Open();
            var ads = new List<SimpleAd>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new SimpleAd
                {
                    Id = (int)reader["Id"],
                    UserId = (int)reader["UserId"],
                    Details = (string)reader["Details"],
                    Date = (DateTime)reader["Date"],
                    Name = (string)reader["Name"],
                    Phone = (string)reader["Phone"]
                });
            }

            return ads;
        }
    }
}
