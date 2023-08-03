namespace HOSPITAL2.Models
{
    public class ContactFormModel
    {
        public int ContactID { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

        // Foreign key property and navigation property for the Patient relationship
        public int PatientID { get; set; }
        public PatientModel Patient { get; set; }
    }
}
