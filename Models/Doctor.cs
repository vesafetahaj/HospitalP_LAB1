using System;
using System.Collections.Generic;

namespace HOSPITAL2_LAB1.Models
{
    public partial class Doctor
    {
        public Doctor()
        {
            PatientDoctors = new HashSet<PatientDoctor>();
            Reports = new HashSet<Report>();
            Reservations = new HashSet<Reservation>();
        }

        public int DoctorId { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Education { get; set; }
        public string? PhotoUrl { get; set; }
        public int? Specialization { get; set; }
        public string? UserId { get; set; }

        public virtual Specialization? SpecializationNavigation { get; set; }
        public virtual AspNetUser? User { get; set; }
        public virtual ICollection<PatientDoctor> PatientDoctors { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
