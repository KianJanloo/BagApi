using System;
using System.ComponentModel.DataAnnotations;

namespace BagApi.Models;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
