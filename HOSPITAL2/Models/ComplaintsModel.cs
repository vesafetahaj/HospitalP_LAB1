namespace HOSPITAL2.Models
{
    public class ComplaintsModel
    {
        public int ComplaintID { get; set; }
        public DateTime ComplaintDate { get; set; }
        public string ComplaintDetails { get; set; }

        // Foreign key property and navigation property for the Patient relationship
        public int PatientID { get; set; }
        public PatientModel Patient { get; set; }
    }
}
