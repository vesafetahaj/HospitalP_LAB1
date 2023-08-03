namespace HOSPITAL2.Models
{
    public class RoomModel
    {
        public int RoomID { get; set; }
        public int PatientID { get; set; }
        public PatientModel Patient { get; set; }
    }
}
