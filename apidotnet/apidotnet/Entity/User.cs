using System;
using System.Collections.Generic;

namespace RevisionTool.Entity;

public partial class User
{
    public int UserId { get; set; }

    public string? MobileNumber { get; set; }

    public string Email { get; set; } = null!;

    public sbyte IsActive { get; set; }

    public string Password { get; set; } = null!;

    public int WrongAttempts { get; set; }

    public DateTime? LastWrongAttempt { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? ProfilePic { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string Notification { get; set; } = null!;

    public string Role { get; set; } = null!;

    public sbyte IsEmailVerify { get; set; }

    public sbyte? IsMobileNumberVeirfy { get; set; }

    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();

    public virtual ICollection<Point> Points { get; set; } = new List<Point>();

    public virtual ICollection<PointsHistory> PointsHistories { get; set; } = new List<PointsHistory>();

    public virtual ICollection<PointsInRevision> PointsInRevisions { get; set; } = new List<PointsInRevision>();

    public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();

    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();

    public virtual ICollection<Verificationcode> Verificationcodes { get; set; } = new List<Verificationcode>();
}
