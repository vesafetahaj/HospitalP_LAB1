using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using HOSPITAL2_LAB1.Model;

namespace HOSPITAL2_LAB1.Data
{
    public partial class HOSPITAL2Context : DbContext
    {
        public HOSPITAL2Context()
        {
        }

        public HOSPITAL2Context(DbContextOptions<HOSPITAL2Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrator> Administrators { get; set; } = null!;
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; } = null!;
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; } = null!;
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; } = null!;
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; } = null!;
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; } = null!;
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; } = null!;
        public virtual DbSet<Complaint> Complaints { get; set; } = null!;
        public virtual DbSet<ContactForm> ContactForms { get; set; } = null!;
        public virtual DbSet<Doctor> Doctors { get; set; } = null!;
        public virtual DbSet<Patient> Patients { get; set; } = null!;
        public virtual DbSet<PatientDoctor> PatientDoctors { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Receptionist> Receptionists { get; set; } = null!;
        public virtual DbSet<Report> Reports { get; set; } = null!;
        public virtual DbSet<Reservation> Reservations { get; set; } = null!;
        public virtual DbSet<Room> Rooms { get; set; } = null!;
        public virtual DbSet<Specialization> Specializations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("data source=.;initial catalog=HOSPITAL2;integrated security = True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.HasKey(e => e.AdminId)
                    .HasName("PK__Administ__719FE4E84C620078");

                entity.ToTable("Administrator");

                entity.HasIndex(e => e.UserId, "UK_Administrator_UserId")
                    .IsUnique();

                entity.Property(e => e.AdminId).HasColumnName("AdminID");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Surname)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Administrator)
                    .HasForeignKey<Administrator>(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "AspNetUserRole",
                        l => l.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                        r => r.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId");

                            j.ToTable("AspNetUserRoles");

                            j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                        });
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Complaint>(entity =>
            {
                entity.Property(e => e.ComplaintId).HasColumnName("ComplaintID");

                entity.Property(e => e.ComplaintDate).HasColumnType("date");

                entity.Property(e => e.ComplaintDetails)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.AdministratorNavigation)
                    .WithMany(p => p.Complaints)
                    .HasForeignKey(d => d.Administrator)
                    .HasConstraintName("FK__Complaint__Admin__628FA481");

                entity.HasOne(d => d.PatientNavigation)
                    .WithMany(p => p.Complaints)
                    .HasForeignKey(d => d.Patient)
                    .HasConstraintName("FK__Complaint__Patie__6383C8BA");
            });

            modelBuilder.Entity<ContactForm>(entity =>
            {
                entity.HasKey(e => e.ContactId)
                    .HasName("PK__ContactF__5C6625BB38CBCF0B");

                entity.ToTable("ContactForm");

                entity.Property(e => e.ContactId).HasColumnName("ContactID");

                entity.Property(e => e.Message)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Subject)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.AdministratorNavigation)
                    .WithMany(p => p.ContactForms)
                    .HasForeignKey(d => d.Administrator)
                    .HasConstraintName("FK__ContactFo__Admin__6477ECF3");

                entity.HasOne(d => d.PatientNavigation)
                    .WithMany(p => p.ContactForms)
                    .HasForeignKey(d => d.Patient)
                    .HasConstraintName("FK__ContactFo__Patie__656C112C");
            });

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.ToTable("Doctor");

                entity.HasIndex(e => e.UserId, "UK_Doctor_UserId")
                    .IsUnique();

                entity.Property(e => e.DoctorId).HasColumnName("DoctorID");

                entity.Property(e => e.Education)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PhotoUrl)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PhotoURL");

                entity.Property(e => e.Surname)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.SpecializationNavigation)
                    .WithMany(p => p.Doctors)
                    .HasForeignKey(d => d.Specialization)
                    .HasConstraintName("FK__Doctor__Speciali__66603565");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Doctor)
                    .HasForeignKey<Doctor>(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("Patient");

                entity.HasIndex(e => e.UserId, "UK_Patient_UserId")
                    .IsUnique();

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Birthday).HasColumnType("date");

                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Surname)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Patient)
                    .HasForeignKey<Patient>(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<PatientDoctor>(entity =>
            {
                entity.HasKey(e => new { e.PatientId, e.DoctorId })
                    .HasName("PK__PatientD__75D2C3ABECCA6DBC");

                entity.ToTable("PatientDoctor");

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.DoctorId).HasColumnName("DoctorID");

                entity.HasOne(d => d.DoctorNavigation)
                    .WithMany(p => p.PatientDoctors)
                    .HasForeignKey(d => d.Doctor)
                    .HasConstraintName("FK__PatientDo__Docto__6754599E");

                entity.HasOne(d => d.PatientNavigation)
                    .WithMany(p => p.PatientDoctors)
                    .HasForeignKey(d => d.Patient)
                    .HasConstraintName("FK__PatientDo__Patie__68487DD7");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.Property(e => e.PaymentId).HasColumnName("PaymentID");

                entity.Property(e => e.PaymentAmount)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PaymentDate).HasColumnType("date");

                entity.HasOne(d => d.PatientNavigation)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.Patient)
                    .HasConstraintName("FK__Payment__Patient__693CA210");

                entity.HasOne(d => d.ReceptionistNavigation)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.Receptionist)
                    .HasConstraintName("FK__Payment__Recepti__6A30C649");

                entity.HasOne(d => d.ReportNavigation)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.Report)
                    .HasConstraintName("FK__Payment__Report__6B24EA82");
            });

            modelBuilder.Entity<Receptionist>(entity =>
            {
                entity.ToTable("Receptionist");

                entity.HasIndex(e => e.UserId, "UK_Receptionist_UserId")
                    .IsUnique();

                entity.Property(e => e.ReceptionistId).HasColumnName("ReceptionistID");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Surname)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Receptionist)
                    .HasForeignKey<Receptionist>(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Report");

                entity.Property(e => e.ReportId).HasColumnName("ReportID");

                entity.Property(e => e.ReportDate).HasColumnType("date");

                entity.Property(e => e.ReportDescription)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ReportType)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.DoctorNavigation)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.Doctor)
                    .HasConstraintName("FK__Report__Doctor__6C190EBB");

                entity.HasOne(d => d.PatientNavigation)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.Patient)
                    .HasConstraintName("FK__Report__Patient__6D0D32F4");
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.ToTable("Reservation");

                entity.Property(e => e.ReservationId).HasColumnName("ReservationID");

                entity.Property(e => e.ReservationDate).HasColumnType("date");

                entity.HasOne(d => d.DoctorNavigation)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.Doctor)
                    .HasConstraintName("FK__Reservati__Docto__6EF57B66");

                entity.HasOne(d => d.PatientNavigation)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.Patient)
                    .HasConstraintName("FK__Reservati__Patie__6FE99F9F");

                entity.HasOne(d => d.ServiceNavigation)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.Service)
                    .HasConstraintName("FK__Reservati__Servi__70DDC3D8");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room");

                entity.HasIndex(e => e.Patient, "UQ__Room__D1860046BDBAFB10")
                    .IsUnique();

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.HasOne(d => d.PatientNavigation)
                    .WithOne(p => p.Room)
                    .HasForeignKey<Room>(d => d.Patient)
                    .HasConstraintName("FK__Room__Patient__71D1E811");
            });

            modelBuilder.Entity<Specialization>(entity =>
            {
                entity.ToTable("Specialization");

                entity.Property(e => e.SpecializationId).HasColumnName("SpecializationID");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PhotoUrl)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PhotoURL");

                entity.HasOne(d => d.AdministratorNavigation)
                    .WithMany(p => p.Specializations)
                    .HasForeignKey(d => d.Administrator)
                    .HasConstraintName("FK__Specializ__Admin__72C60C4A");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
