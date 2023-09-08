using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HOSPITAL2_LAB1.Model
{
    public partial class Room
    {
        public Room()
        {
            Patients = new HashSet<Patient>();
        }

        public int RoomId { get; set; }

        [Required(ErrorMessage = "Room number is required.")]
        [RegularExpression("^[A-Za-z]{1}[0-9]{3}$", ErrorMessage = "Room number must start with one letter followed by three numbers.")]
        public string? RoomNumber { get; set; }

        public virtual ICollection<Patient> Patients { get; set; }
    }
}
