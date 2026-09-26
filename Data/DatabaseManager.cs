using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using TechVault.UI.Models;

namespace TechVault.UI.Data
{
    public class DatabaseManager
    {
        private readonly string _connectionString = "Data Source=TechVault.db;";

        public void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Students (
                StudentID INTEGER PRIMARY KEY AUTOINCREMENT,
                FirstName TEXT NOT NULL,
                LastName TEXT NOT NULL,
                CollegeEmail TEXT UNIQUE NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Equipment (
                EquipmentID INTEGER PRIMARY KEY AUTOINCREMENT,
                Type TEXT NOT NULL,
                Model TEXT NOT NULL,
                Condition TEXT CHECK(Condition IN ('New', 'Good', 'Fair', 'Poor')) DEFAULT 'Good'
            );

            CREATE TABLE IF NOT EXISTS Loans (
                LoanID INTEGER PRIMARY KEY AUTOINCREMENT,
                StudentID INTEGER,
                EquipmentID INTEGER,
                CheckoutDate TEXT NOT NULL,
                ExpectedReturn TEXT NOT NULL,
                Status TEXT DEFAULT 'Active',
                FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
                FOREIGN KEY (EquipmentID) REFERENCES Equipment(EquipmentID)
            );
        ";
            command.ExecuteNonQuery();
        }

        public List<Student> GetStudents()
        {
            var students = new List<Student>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT StudentID, FirstName, LastName, CollegeEmail FROM Students";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                students.Add(new Student
                {
                    StudentId = reader.GetInt32(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    CollegeEmail = reader.GetString(3)
                });
            }

            return students;
        }

        public List<Equipment> GetAvailableEquipment()
        {
            var equipment = new List<Equipment>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Simplified: Gets all equipment for demonstration. In a real app, you'd filter out currently loaned items.
            var command = connection.CreateCommand();
            command.CommandText = "SELECT EquipmentID, Type, Model, Condition FROM Equipment";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                equipment.Add(new Equipment
                {
                    EquipmentId = reader.GetInt32(0),
                    Type = reader.GetString(1),
                    Model = reader.GetString(2),
                    Condition = reader.GetString(3)
                });
            }

            return equipment;
        }

        public void SaveStudent(Student student)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            INSERT INTO Students (FirstName, LastName, CollegeEmail)
            VALUES ($firstName, $lastName,$email)";

            command.Parameters.AddWithValue("$firstName", student.FirstName);
            command.Parameters.AddWithValue("$lastName", student.LastName);
            command.Parameters.AddWithValue("$email", student.CollegeEmail);

            command.ExecuteNonQuery();
        }


        public void SaveLoan(LoanRecord loan)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            INSERT INTO Loans (StudentID, EquipmentID, CheckoutDate, ExpectedReturn, Status)
            VALUES ($studentId, $equipmentId,$checkoutDate, $expectedReturn,$status)";

            command.Parameters.AddWithValue("$studentId", loan.StudentId);
            command.Parameters.AddWithValue("$equipmentId", loan.EquipmentId);
            command.Parameters.AddWithValue("$checkoutDate", loan.CheckoutDate.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("$expectedReturn", loan.ExpectedReturn.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("$status", loan.Status);

            command.ExecuteNonQuery();
        }

        public List<LoanRecord> GetAllLoans(bool activeOnly = false)
        {
            var loans = new List<LoanRecord>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            string statusFilter = activeOnly ? "WHERE l.Status = 'Active'" : "";

            command.CommandText = $@"
            SELECT 
                l.LoanID, 
                l.StudentID, 
                l.EquipmentID, 
                l.CheckoutDate, 
                l.ExpectedReturn, 
                l.Status,
                s.FirstName || ' ' || s.LastName as StudentName,
                e.Type || ' - ' || e.Model as EquipmentDetails
            FROM Loans l
            JOIN Students s ON l.StudentID = s.StudentID
            JOIN Equipment e ON l.EquipmentID = e.EquipmentID
            {statusFilter}
            ORDER BY l.CheckoutDate DESC";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                loans.Add(new LoanRecord
                {
                    LoanId = reader.GetInt32(0),
                    StudentId = reader.GetInt32(1),
                    EquipmentId = reader.GetInt32(2),
                    CheckoutDate = DateTime.Parse(reader.GetString(3)),
                    ExpectedReturn = DateTime.Parse(reader.GetString(4)),
                    Status = reader.GetString(5),
                    StudentName = reader.GetString(6),
                    EquipmentDetails = reader.GetString(7)
                });
            }

            return loans;
        }

        // Helper to add some initial data if needed
        public void SeedDataIfEmpty()
        {
            if (GetStudents().Count == 0)
            {
                SaveStudent(new Student { FirstName = "John", LastName = "Doe", CollegeEmail = "john.doe@college.ac.uk" });
                SaveStudent(new Student { FirstName = "Jane", LastName = "Smith", CollegeEmail = "jane.smith@college.ac.uk" });

                using var connection = new SqliteConnection(_connectionString);
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
               INSERT INTO Equipment (Type, Model, Condition) VALUES ('Laptop', 'Dell XPS 13', 'Good');
               INSERT INTO Equipment (Type, Model, Condition) VALUES ('Camera', 'Canon EOS R5', 'New');
           ";
                command.ExecuteNonQuery();
            }
        }
    }
}


