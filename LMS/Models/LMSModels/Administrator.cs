using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Administrator
    {
        public string UId { get; set; } = null!;
        public uint AdminId { get; set; }
        public string FName { get; set; } = null!;
        public string LName { get; set; } = null!;
        public DateOnly Dob { get; set; }
    }
}
