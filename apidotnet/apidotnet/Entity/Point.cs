using System;
using System.Collections.Generic;

namespace RevisionTool.Entity;

public partial class Point
{
    public int PointId { get; set; }

    public int TopicId { get; set; }

    public string Point1 { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int CreateBy { get; set; }

    public DateTime CreateTime { get; set; }

    public virtual User CreateByNavigation { get; set; } = null!;

    public virtual ICollection<PointsHistory> PointsHistories { get; set; } = new List<PointsHistory>();

    public virtual ICollection<PointsInRevision> PointsInRevisions { get; set; } = new List<PointsInRevision>();

    public virtual Topic Topic { get; set; } = null!;
}
