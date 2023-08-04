using System;
using System.Collections.Generic;

namespace HOSPITAL2_LAB1.Models
{
    public partial class Room
    {
        public int RoomId { get; set; }
        public int? Patient { get; set; }

        public virtual Patient? PatientNavigation { get; set; }
    }
}
