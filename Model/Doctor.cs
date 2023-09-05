using System;
using System.Collections.Generic;

namespace HOSPITAL2_LAB1.Model
{
    public partial class Doctor
    {
        public Doctor()
        {
            Reports = new HashSet<Report>();
            Reservations = new HashSet<Reservation>();
            Patients = new HashSet<Patient>();
        }

        public int DoctorId { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Education { get; set; }
        public string? PhotoUrl { get; set; }
        public int? Specialization { get; set; }
        public string? UserId { get; set; }
        public string FullName => $"{Name} {Surname}";
        public virtual Specialization? SpecializationNavigation { get; set; }
        public virtual AspNetUser? User { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }

        public virtual ICollection<Patient> Patients { get; set; }
    }
}
