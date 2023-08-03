namespace HOSPITAL2.Models
{
    public class ReportModel
    {
        public int ReportID { get; set; }
        public string ReportType { get; set; }
        public DateTime ReportDate { get; set; }
        public string ReportDescription { get; set; }

        public int PatientID { get; set; }
        public PatientModel Patient { get; set; }

        public int DoctorID { get; set; }
        public DoctorModel Doctor { get; set; }
    }
}
