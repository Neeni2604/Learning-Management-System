using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Course
    {
        public Course()
        {
            Classes = new HashSet<Class>();
        }

        public uint CId { get; set; }
        public string Name { get; set; } = null!;
        public uint Num { get; set; }
        public uint DId { get; set; }

        public virtual Department DIdNavigation { get; set; } = null!;
        public virtual ICollection<Class> Classes { get; set; }
    }
}
