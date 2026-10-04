using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GDB.Api.Application.Dtos.Request
{
    public class CloseAccountRequestDto
    {
        [Required(ErrorMessage = "Account number is required.")]
        [RegularExpression(
        @"^\d{10}$",
        ErrorMessage = "Account number must be exactly 10 digits.")]
        public string AccountNumber { get; set; }
    }
}
