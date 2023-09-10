using System;
using System.Collections.Generic;

namespace RevisionTool.Entity;

public partial class PointsHistory
{
    public int PointsHistoryId { get; set; }

    public int UserId { get; set; }

    public int PointId { get; set; }

    public TimeOnly TimeTakenToAnswer { get; set; }

    public DateTime AskedTime { get; set; }

    public DateTime NextTime { get; set; }

    public virtual Point Point { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
