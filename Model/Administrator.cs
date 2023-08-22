using System;
using System.Collections.Generic;

namespace HOSPITAL2_LAB1.Model
{
    public partial class Administrator
    {
        public Administrator()
        {
            ContactForms = new HashSet<ContactForm>();
            Specializations = new HashSet<Specialization>();
        }

        public int AdminId { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? UserId { get; set; }

        public virtual AspNetUser? User { get; set; }
        public virtual ICollection<ContactForm> ContactForms { get; set; }
        public virtual ICollection<Specialization> Specializations { get; set; }
    }
}
