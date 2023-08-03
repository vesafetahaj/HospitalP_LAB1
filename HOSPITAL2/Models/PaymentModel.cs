namespace HOSPITAL2.Models
{
    public class PaymentModel
    {
        public int PaymentID { get; set; }
        public string PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }

        // Foreign key properties and navigation properties for Receptionist, Patient, and Report relationships
        public int ReceptionistID { get; set; }
        public ReceptionistModel Receptionist { get; set; }

        public int PatientID { get; set; }
        public PatientModel Patient { get; set; }

        public int ReportID { get; set; }
        public ReportModel Report { get; set; }
    }
}
