using System;
using System.Collections.Generic;

namespace RevisionTool.Entity;

public partial class Module
{
    public int ModuleId { get; set; }

    public string ModuleName { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreateTime { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
