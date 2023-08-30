using System;
using System.Collections.Generic;

namespace HOSPITAL2_LAB1.Model
{
    public partial class Room
    {
        public Room()
        {
            Patients = new HashSet<Patient>();
        }

        public int RoomId { get; set; }
        public string? RoomNumber { get; set; }

        public virtual ICollection<Patient> Patients { get; set; }
    }
}
