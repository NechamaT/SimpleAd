using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;

namespace FreePost.Data
{
    public class Post
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class FreebayDb
    {
        private readonly string _connectionString;

        public FreebayDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddPost(Post post, bool name)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();
            if (name)
            {
                cmd.CommandText = @"INSERT INTO Listings
                                VALUES(@DateCreated, @Text, @Name, @PhoneNumber)";
            }
            else
            {
                cmd.CommandText = @"INSERT INTO Listings(DateCreated, Text, Phonenumber)
                                    VALUES(@DateCreated, @Text, @PhoneNumber)";
            }
            cmd.CommandText += "SELECT SCOPE_IDENTITY()";
            conn.Open();
            cmd.Parameters.AddWithValue("@DateCreated", DateTime.Now);
            cmd.Parameters.AddWithValue("@Text", post.Description);
            cmd.Parameters.AddWithValue("@PhoneNumber", post.PhoneNumber);
            if (name) { cmd.Parameters.AddWithValue("@Name", post.Name); }
            post.Id =  (int)(decimal)cmd.ExecuteScalar();
        }

        public List<Post> GetAllPosts()
        {
            var connection = new SqlConnection(_connectionString);
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Listings";
            connection.Open();
            var results = new List<Post>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                results.Add(new Post
                {
                    Id = (int)reader["Id"],
                    DateCreated = (DateTime)reader["DateCreated"],
                    Name = reader.GetOrNull<string>("Name"),
                    Description = (string)reader["Text"],
                    PhoneNumber = (string)reader["PhoneNumber"]
,
                }); 
            }
            return results;
        }

        public void Delete(int id)
        {
            var connection = new SqlConnection(_connectionString);
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM Listings
                              WHERE Id = @Id";
            connection.Open();
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }
    }

    public static class Extensions
    {
        public static T GetOrNull<T>(this SqlDataReader reader, string column)
        {
            object value = reader[column];
            if (value == DBNull.Value)
            {
                return default(T);
            }
            return (T)value;
        }
    }
}
