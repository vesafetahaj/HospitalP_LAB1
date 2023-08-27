using System;
using System.Collections.Generic;

namespace HOSPITAL2_LAB1.Model
{
    public partial class Specialization
    {
        public Specialization()
        {
            Doctors = new HashSet<Doctor>();
        }

        public int SpecializationId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? PhotoUrl { get; set; }
        public int? Administrator { get; set; }

        public virtual Administrator? AdministratorNavigation { get; set; }
        public virtual ICollection<Doctor> Doctors { get; set; }
    }
}
