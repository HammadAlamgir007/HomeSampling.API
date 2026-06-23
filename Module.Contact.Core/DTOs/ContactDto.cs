namespace Module.Contact.Core.DTOs;

public class ContactRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class ContactResponseDto
{
    public string Message { get; set; } = string.Empty;
}
