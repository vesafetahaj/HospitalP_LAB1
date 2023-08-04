using System;
using System.Collections.Generic;

namespace HOSPITAL2_LAB1.Models
{
    public partial class Complaint
    {
        public int ComplaintId { get; set; }
        public DateTime? ComplaintDate { get; set; }
        public string? ComplaintDetails { get; set; }
        public int? Patient { get; set; }
        public int? Administrator { get; set; }

        public virtual Administrator? AdministratorNavigation { get; set; }
        public virtual Patient? PatientNavigation { get; set; }
    }
}
