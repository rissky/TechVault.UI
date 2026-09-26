using System;
using System.Collections.Generic;
using System.Linq;
using TechVault.UI.Models;

namespace TechVault.UI.Data
{
    public class MockDataManager
    {
        private static List<Student> _students = new List<Student>
        {
            new Student { StudentId = 1, FirstName = "Mock", LastName = "Student1", CollegeEmail = "mock1@college.ac.uk" },
            new Student { StudentId = 2, FirstName = "Mock", LastName = "Student2", CollegeEmail = "mock2@college.ac.uk" }
        };

        private static List<Equipment> _equipment = new List<Equipment>
        {
            new Equipment { EquipmentId = 1, Type = "Mock Laptop", Model = "MockModel X", Condition = "Good" },
            new Equipment { EquipmentId = 2, Type = "Mock Camera", Model = "MockCam Y", Condition = "New" }
        };

        private static List<LoanRecord> _loans = new List<LoanRecord>
        {
            new LoanRecord {
                LoanId = 1,
                StudentId = 1,
                EquipmentId = 1,
                CheckoutDate = DateTime.Now.AddDays(-2),
                ExpectedReturn = DateTime.Now.AddDays(5),
                Status = "Active",
                StudentName = "Mock Student1",
                EquipmentDetails = "Mock Laptop - MockModel X"
            },
             new LoanRecord {
                LoanId = 2,
                StudentId = 2,
                EquipmentId = 2,
                CheckoutDate = DateTime.Now.AddDays(-10),
                ExpectedReturn = DateTime.Now.AddDays(-3),
                Status = "Returned",
                StudentName = "Mock Student2",
                EquipmentDetails = "Mock Camera - MockCam Y"
            }
        };

        public List<Student> GetStudents() => _students;

        public List<Equipment> GetAvailableEquipment() => _equipment;

        public void SaveStudent(Student student)
        {
            student.StudentId = _students.Any() ? _students.Max(s => s.StudentId) + 1 : 1;
            _students.Add(student);
        }

        public void SaveLoan(LoanRecord loan)
        {
            loan.LoanId = _loans.Any() ? _loans.Max(l => l.LoanId) + 1 : 1;

            // Populate navigation properties for the mock view
            var student = _students.FirstOrDefault(s => s.StudentId == loan.StudentId);
            var equipment = _equipment.FirstOrDefault(e => e.EquipmentId == loan.EquipmentId);

            if (student != null) loan.StudentName = $"{student.FirstName} {student.LastName}";
            if (equipment != null) loan.EquipmentDetails = $"{equipment.Type} - {equipment.Model}";

            _loans.Add(loan);
        }

        public List<LoanRecord> GetAllLoans(bool activeOnly = false)
        {
            if (activeOnly)
            {
                return _loans.Where(l => l.Status == "Active").OrderByDescending(l => l.CheckoutDate).ToList();
            }
            return _loans.OrderByDescending(l => l.CheckoutDate).ToList();
        }
    }
}