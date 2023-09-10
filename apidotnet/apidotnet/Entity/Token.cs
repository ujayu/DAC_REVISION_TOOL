using System;
using System.Collections.Generic;

namespace RevisionTool.Entity;

public partial class Token
{
    public int TokenId { get; set; }

    public int UserId { get; set; }

    public string RefreshToken { get; set; } = null!;

    public DateTime ExpireTime { get; set; }

    public sbyte RememberMe { get; set; }

    public virtual User User { get; set; } = null!;
}
