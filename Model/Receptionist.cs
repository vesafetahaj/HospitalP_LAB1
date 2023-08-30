using System;
using System.Collections.Generic;

namespace HOSPITAL2_LAB1.Model
{
    public partial class Receptionist
    {
        public int ReceptionistId { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? UserId { get; set; }

        public virtual AspNetUser? User { get; set; }
    }
}
