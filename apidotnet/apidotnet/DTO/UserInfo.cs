namespace apidotnet.DTO;
public partial class UserInfo
{
    public int UserId { get; set; }

    public string? MobileNumber { get; set; }

    public string Email { get; set; } = null!;

    public sbyte IsActive { get; set; }

    public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? ProfilePic { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string Notification { get; set; } = null!;

    public string Role { get; set; } = null!;

    public sbyte IsEmailVerify { get; set; }

    public sbyte? IsMobileNumberVeirfy { get; set; }
}
