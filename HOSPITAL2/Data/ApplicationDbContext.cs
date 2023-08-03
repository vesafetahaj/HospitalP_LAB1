using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HOSPITAL2.Models;

namespace HOSPITAL2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
  
        }
        public DbSet<PatientModel> Patient { get; set; }
        public DbSet<SpecializationModel> Specialization { get; set; }
        public DbSet<ComplaintsModel> Complaints { get; set; }
        public DbSet<ContactFormModel> ContactForm { get; set; }
        public DbSet<DoctorModel> Doctor { get; set; }
        public DbSet<DoctorRoleModel> DoctorRole { get; set; }
        public DbSet<PatientDoctorModel> PatientDoctor { get; set; }
        public DbSet<PatientRoleModel> PatientRole { get; set; }
        public DbSet<PaymentModel> Payment { get; set; }
        public DbSet<ReceptionistModel> Receptionist { get; set; }
        public DbSet<ReceptionistRoleModel> ReceptionistRole { get; set; }
        public DbSet<ReportModel> Report { get; set; }
        public DbSet<ReservationModel> Reservation { get; set; }
        public DbSet<RoleModel> Role { get; set; }
        public DbSet<RoomModel> Room { get; set; }


    }
}