using System;
using System.Collections.Generic;

namespace RevisionTool.Entity;

public partial class Verificationcode
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string Code { get; set; } = null!;

    public DateTime? ExpirationTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
