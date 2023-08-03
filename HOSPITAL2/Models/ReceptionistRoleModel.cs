namespace HOSPITAL2.Models
{
    public class ReceptionistRoleModel
    {
        public int ReceptionistID { get; set; }
        public int RoleID { get; set; }

        // Navigation properties
        public ReceptionistModel Receptionist { get; set; }
        public RoleModel Role { get; set; }
    }
}
