using System;
using System.Collections.Generic;

namespace RevisionTool.Entity;

public partial class Topic
{
    public int TopicId { get; set; }

    public int ModuleId { get; set; }

    public string TopicName { get; set; } = null!;

    public int CreateBy { get; set; }

    public DateTime CreateTime { get; set; }

    public virtual User CreateByNavigation { get; set; } = null!;

    public virtual Module Module { get; set; } = null!;

    public virtual ICollection<Point> Points { get; set; } = new List<Point>();
}
