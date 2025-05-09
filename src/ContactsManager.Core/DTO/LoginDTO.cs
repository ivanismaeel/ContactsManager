using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.DTO;

public class LoginDTO
{
    [Required(ErrorMessage = "Email can't be blank")]
    [EmailAddress(ErrorMessage = "Email is not valid")]
    [DataType(DataType.EmailAddress)]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email is not valid")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password can't be blank")]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}
