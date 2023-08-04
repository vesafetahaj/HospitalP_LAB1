using System;
using System.Collections.Generic;

namespace HOSPITAL2_LAB1.Models
{
    public partial class Report
    {
        public Report()
        {
            Payments = new HashSet<Payment>();
        }

        public int ReportId { get; set; }
        public string? ReportType { get; set; }
        public DateTime? ReportDate { get; set; }
        public string? ReportDescription { get; set; }
        public int? Patient { get; set; }
        public int? Doctor { get; set; }

        public virtual Doctor? DoctorNavigation { get; set; }
        public virtual Patient? PatientNavigation { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
