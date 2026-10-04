using GDB.Api.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB.Api.Application.Dtos.Request
{
    public class CreateAccountRequestDto
    {
        [Required(ErrorMessage = "Account number is required.")]
        [RegularExpression(
        @"^\d{10}$",
        ErrorMessage = "Account number must be exactly 10 digits.")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 3,
        ErrorMessage = "Name must be between 3 and 100 characters.")]
        [RegularExpression(
        @"^[a-zA-Z\s]+$",
        ErrorMessage = "Name can contain only letters and spaces.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(0, 150, ErrorMessage = "Age must be a positive integer.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Balance is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Balance must be a non-negative value.")]
        public decimal Balance { get; set; }

        [Required(ErrorMessage = "PIN is required.")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "PIN must be exactly 4 characters.")]
        public string Pin { get; set; }

        [Required(ErrorMessage = "Account type is required.")]
        public AccountType AccountType { get; set; }

        [Required(ErrorMessage = "Account status is required.")]
        public AccountStatus Status { get; set; }

        [Required(ErrorMessage = "Account privilege is required.")]
        public AccountPrivilege Privilege { get; set; }

        public decimal OverdraftLimit { get; set; }
        public int TenureMonths { get; set; }
        public double InterestRate { get; set; }
        public decimal MinimumBalance { get; set; }

        [StringLength(100, MinimumLength = 3,
        ErrorMessage = "Employer Name must be between 3 and 100 characters.")]
        [RegularExpression(
        @"^[a-zA-Z\s]+$",
        ErrorMessage = "Employer Name can contain only letters and spaces.")]
        public string EmployerName { get; set; }
    }
}
