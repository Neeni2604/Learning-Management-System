using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submission
    {
        public uint SubId { get; set; }
        public uint SId { get; set; }
        public uint AId { get; set; }
        public string Contents { get; set; } = null!;
        public uint Score { get; set; }
        public DateTime SubmissionTime { get; set; }

        public virtual Assignment AIdNavigation { get; set; } = null!;
        public virtual Student SIdNavigation { get; set; } = null!;
    }
}
