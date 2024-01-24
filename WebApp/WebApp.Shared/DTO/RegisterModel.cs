using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Shared.DTO
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^((?!\.)[\w_.]*[^.])(@\w+)(\.\w+(\.\w+)?[^.\W])$", ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [RegularExpression(@"^[\p{L}]*$", ErrorMessage = "First name can only contain letters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [RegularExpression(@"^[\p{L}]*$", ErrorMessage = "Last name can only contain letters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Birth date is required")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*[^\w\d\s:])([^\s]){8,32}$",
            ErrorMessage = "Password doesn't meet security rules.\n" +
            "   - number (0-9)\n" +
            "   - 1 uppercase letter\n"+
            "   - 1 lowercase letter\n" +
            "   - non-alpha-numeric character\n" +
            "   - Password length is between 8 and 32 characters with no spaces")]
        public string Password { get; set; }
    }
}
