using BCrypt.Net;
using HW_4_23_SimpleAdsWLogin.data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW_4_23_SimpleAdsWLogin.data
{
    public class AdsRepository
    {
        private readonly string _connectionString;

        public AdsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddAd(Ad ad)
        {

            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Ads (UserId, PhoneNumber, Description, DatePosted) " +
                "VALUES (@userId, @phoneNumber, @description, @date)";
            cmd.Parameters.AddWithValue("@userId", ad.UserId);
            cmd.Parameters.AddWithValue("@phoneNumber", ad.PhoneNumber);
            cmd.Parameters.AddWithValue("@description", ad.Description);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public List<Ad> GetAllAds()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Ads";
            connection.Open();
            var reader = cmd.ExecuteReader();

            List<Ad> ads = new();
            while (reader.Read())
            {
                var a = new Ad()
                {
                    Id = (int)reader["Id"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Description = (string)reader["Description"],
                    UserId = (int)reader["UserId"],
                    DatePosted = (DateTime)reader["DatePosted"]
                };

                var userRepo = new UserRepository(_connectionString);
                var user = userRepo.GetUserById(a.UserId);
                if (user != null)
                {
                    a.UserName = $"{user.FirstName} {user.LastName}";
                    a.UserEmail = user.Email;
                    ads.Add(a);
                }
            }

            return ads;
        }

        public List<Ad> GetUsersAds(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Ads WHERE UserId = @id";
            cmd.Parameters.AddWithValue("@id", user.Id);
            connection.Open();
            var reader = cmd.ExecuteReader();

            List<Ad> ads = new();
            while (reader.Read())
            {
                var a = new Ad()
                {
                    Id = (int)reader["Id"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Description = (string)reader["Description"],
                    UserId = (int)reader["UserId"],
                    DatePosted = (DateTime)reader["DatePosted"]
                };
                a.UserName = $"{user.FirstName} {user.LastName}";
                a.UserEmail = user.Email;
                ads.Add(a);
            }

            return ads;
        }

        public void DeleteAd(int adId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Ads WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", adId);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
