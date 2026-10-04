using System.ComponentModel.DataAnnotations;

namespace GDB.Api.Application.Dtos.Request
{
    public class ViewRecentTransactionsRequestDto
    {
        [Required(ErrorMessage = "Account number is required.")]
        [RegularExpression(
        @"^\d{10}$",
        ErrorMessage = "Account number must be exactly 10 digits.")]
        public string AccountNumber { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Page number must be a positive integer.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
        public int PageSize { get; set; } = 10;
    }
}
