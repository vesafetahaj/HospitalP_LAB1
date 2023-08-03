namespace HOSPITAL2.Models
{
    public class ReservationModel
    {
        public int ReservationID { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan ReservationTime { get; set; }

        // Foreign key properties and navigation properties for Patient, Doctor, and Specialization relationships
        public int PatientID { get; set; }
        public PatientModel Patient { get; set; }

        public int DoctorID { get; set; }
        public DoctorModel Doctor { get; set; }

        public int SpecializationID { get; set; }
        public SpecializationModel Specialization { get; set; }
    }
}
