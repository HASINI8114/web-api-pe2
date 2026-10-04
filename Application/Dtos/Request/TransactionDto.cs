using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB.Api.Application.Dtos.Request
{
    public class TransactionDto
    {
        [RegularExpression(
        @"^\d{10}$",
        ErrorMessage = "Account number must be exactly 10 digits.")]
        public string? AccountNumber { get; set; }

        [RegularExpression(
        @"^\d{10}$",
        ErrorMessage = "From Account number must be exactly 10 digits.")]
        public string? FromAccount { get; set; }

        [RegularExpression(
        @"^\d{10}$",
        ErrorMessage = "To Account number must be exactly 10 digits.")]
        public string? ToAccount { get; set; }


        [Required(ErrorMessage = "Amount is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be a non-negative value.")]

        public decimal Amount { get; set; }

        [StringLength(4, MinimumLength = 4, ErrorMessage = "PIN must be exactly 4 characters.")]
        public string? Pin { get; set; }
    }
    
}
