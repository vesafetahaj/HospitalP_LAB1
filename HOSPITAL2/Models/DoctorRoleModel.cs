namespace HOSPITAL2.Models
{
    public class DoctorRoleModel
    {
        public int DoctorID { get; set; }
        public int RoleID { get; set; }

        // Navigation properties
        public DoctorModel Doctor { get; set; }
        public RoleModel Role { get; set; }
    }
}
