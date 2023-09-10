using System;
using System.Collections.Generic;

namespace RevisionTool.Entity;

public partial class PointsInRevision
{
    public int PointsInRevisionId { get; set; }

    public int UserId { get; set; }

    public int PointId { get; set; }

    public sbyte IsActive { get; set; }

    public virtual Point Point { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
