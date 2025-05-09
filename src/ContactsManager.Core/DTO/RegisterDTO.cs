using System.ComponentModel.DataAnnotations;
using ContactsManager.Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Core.DTO;

public class RegisterDTO
{
    [Required(ErrorMessage = "UserName can't be blank")]
    [StringLength(100, ErrorMessage = "UserName must be at least {2} characters long.", MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "UserName can only contain letters, numbers, and underscores.")]
    public required string PersonName { get; set; }

    [Required(ErrorMessage = "Email can't be blank")]
    [EmailAddress(ErrorMessage = "Email is not valid")]
    [DataType(DataType.EmailAddress)]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email is not valid")]
    [Remote(action: "IsEmailInUse", controller: "Account")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Phone can't be blank")]
    [Phone(ErrorMessage = "Phone is not valid")]
    [DataType(DataType.PhoneNumber)]
    [RegularExpression(@"^\+?[0-9]{9,15}$", ErrorMessage = "Phone is not valid")]
    [StringLength(15, MinimumLength = 9, ErrorMessage = "Phone number must be between 9 and 15 digits.")]
    [Display(Name = "Phone Number")]
    public required string Phone { get; set; }

    [Required(ErrorMessage = "Password can't be blank")]
    [StringLength(100, ErrorMessage = "Password must be at least {2} characters long.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Confirm Password can't be blank")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    [StringLength(100, ErrorMessage = "Password must be at least {2} characters long.", MinimumLength = 8)]
    [Display(Name = "Confirm Password")]
    public required string ConfirmPassword { get; set; }

    public UserTypeOptions UserType { get; set; } = UserTypeOptions.User;
}
