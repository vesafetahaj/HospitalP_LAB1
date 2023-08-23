using System;
using System.Collections.Generic;

namespace HOSPITAL2_LAB1.Model
{
    public partial class ContactForm
    {
        public int ContactId { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
        public int? Patient { get; set; }

        public virtual Patient? PatientNavigation { get; set; }
    }
}
