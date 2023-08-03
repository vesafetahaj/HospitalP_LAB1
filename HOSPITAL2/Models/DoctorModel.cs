using System.ComponentModel.DataAnnotations.Schema;

namespace HOSPITAL2.Models
{
    public class DoctorModel
    {
        public int DoctorID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Education { get; set; }
        public string PhotoURL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        [ForeignKey("Specialization")]
        public int SpecializationID { get; set; }
        public SpecializationModel Specialization { get; set; }
    }
}
