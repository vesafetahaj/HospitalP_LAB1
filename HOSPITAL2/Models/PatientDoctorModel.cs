using System.ComponentModel.DataAnnotations.Schema;

namespace HOSPITAL2.Models
{
    public class PatientDoctorModel
    {
        
        public int PatientID { get; set; }
        public int DoctorID { get; set; }

        [ForeignKey("PatientID")]
        public PatientModel Patient { get; set; }

        [ForeignKey("DoctorID")]
        public DoctorModel Doctor { get; set; }
    }
}
