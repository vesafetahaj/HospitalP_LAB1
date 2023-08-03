namespace HOSPITAL2.Models
{
    public class PatientRoleModel
    {
        public int PatientID { get; set; }
        public int RoleID { get; set; }

        // Navigation properties
        public PatientModel Patient { get; set; }
        public RoleModel Role { get; set; }
    }
}
