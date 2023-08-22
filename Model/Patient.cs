using System;
using System.Collections.Generic;

namespace HOSPITAL2_LAB1.Model
{
    public partial class Patient
    {
        public Patient()
        {
            Complaints = new HashSet<Complaint>();
            ContactForms = new HashSet<ContactForm>();
            PatientDoctors = new HashSet<PatientDoctor>();
            Payments = new HashSet<Payment>();
            Reports = new HashSet<Report>();
            Reservations = new HashSet<Reservation>();
        }

        public int PatientId { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? UserId { get; set; }

        public virtual AspNetUser? User { get; set; }
        public virtual Room? Room { get; set; }
        public virtual ICollection<Complaint> Complaints { get; set; }
        public virtual ICollection<ContactForm> ContactForms { get; set; }
        public virtual ICollection<PatientDoctor> PatientDoctors { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
