using GateKeeperWebApiV1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GateKeeperWebApiV1.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Company> Companies { get; set; }
        public DbSet<EnterpirseRequest> EnterpirseRequests { get; set; }
        public DbSet<WorkerProfile> WorkerProfiles { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Movement> Movements { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<WorkerShift> WorkerShifts { get; set; }
        public DbSet<AbsenceJustification> AbsenceJustifications { get; set; }
        public DbSet<JustificationDocument> JustificationDocuments { get; set; }
        public DbSet<OffdayVacationRequest> OffdayVacationRequests { get; set; }
        public DbSet<ShiftDays> ShiftDays { get; set; }
        /*public DbSet<WorkersTeam> WorkersTeams { get; set; }
        public DbSet<WorkerTeamMembership> WorkerTeamMemberships { get; set; }*/
        public DbSet<ShiftDaysOfWeek> ShiftDaysOfWeeks { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }










            // Configuring EnterpirseRequest - User relationship
            modelBuilder.Entity<EnterpirseRequest>()
                .HasOne(er => er.User)  // Navigation property
                .WithMany(u => u.EnterpirseRequests)  // Navigation property on User
                .HasForeignKey(er => er.UserId);  // Foreign key in EnterpirseRequest

            // Define the relationship between Company and WorkerProfile
            modelBuilder.Entity<WorkerProfile>()
                .HasOne(w => w.Company)
                .WithMany(c => c.Workers)
                .HasForeignKey(w => w.CompanyId);

            // Define the relationship between Company and Building
            modelBuilder.Entity<Building>()
                .HasOne(b => b.Company)          // A building belongs to one company
                .WithMany(c => c.Buildings)      // A company has many buildings
                .HasForeignKey(b => b.CompanyId);

            // Define relationship between Movement and WorkerProfile
            modelBuilder.Entity<Movement>()
                .HasOne(m => m.Worker)
                .WithMany(w => w.Movements)
                .HasForeignKey(m => m.WorkerId);

            // One-to-many between Shift and Building
            modelBuilder.Entity<Shift>()
                .HasOne(s => s.Building)
                .WithMany(b => b.Shifts)
                .HasForeignKey(s => s.BuildingId);

            // Many-to-many relationship between WorkerProfile and Shift via WorkerShift
            modelBuilder.Entity<WorkerShift>()
                .HasKey(ws => new { ws.WorkerId, ws.ShiftId });  // Composite key

            modelBuilder.Entity<WorkerShift>()
                .HasOne(ws => ws.Worker)
                .WithMany(w => w.WorkerShifts)
                .HasForeignKey(ws => ws.WorkerId);

            modelBuilder.Entity<WorkerShift>()
                .HasOne(ws => ws.Shift)
                .WithMany(s => s.WorkerShifts)
                .HasForeignKey(ws => ws.ShiftId);

            // One-to-many between AbsenceJustification and WorkerProfile
            modelBuilder.Entity<AbsenceJustification>()
                .HasOne(aj => aj.Worker)
                .WithMany(w => w.AbsenceJustifications)  // A worker can have multiple justifications
                .HasForeignKey(aj => aj.WorkerId);

            // One-to-many between AbsenceJustification and Company
            modelBuilder.Entity<AbsenceJustification>()
                .HasOne(aj => aj.Company)
                .WithMany(c => c.AbsenceJustifications)  // A company can have multiple justifications
                .HasForeignKey(aj => aj.CompanyId);

            // One-to-many between AbsenceJustification and JustificationDocument
            modelBuilder.Entity<JustificationDocument>()
                .HasOne(jd => jd.AbsenceJustification)
                .WithMany(aj => aj.JustificationDocuments)  // A justification can have multiple documents
                .HasForeignKey(jd => jd.AbsenceJustificationId);
            // One-to-many relationship between OffdayVacationRequest and WorkerProfile
            modelBuilder.Entity<OffdayVacationRequest>()
                .HasOne(ovr => ovr.Worker)
                .WithMany(w => w.OffdayVacationRequests)
                .HasForeignKey(ovr => ovr.WorkerId);

            // One-to-many relationship between OffdayVacationRequest and Company
            modelBuilder.Entity<OffdayVacationRequest>()
                .HasOne(ovr => ovr.Company)
                .WithMany(c => c.OffdayVacationRequests)
                .HasForeignKey(ovr => ovr.CompanyId);

            // One-to-one relationship between WorkerProfile and ApplicationUser
            modelBuilder.Entity<WorkerProfile>()
                .HasOne(wp => wp.ApplicationUser)
                .WithMany(au => au.WorkerProfiles)
                .HasForeignKey(wp => wp.ApplicationUserId)
                .IsRequired(); // Ensures that each WorkerProfile must have an ApplicationUser

            // Configure one-to-many relationship between Shift and ShiftDays
            modelBuilder.Entity<Shift>()
                .HasMany(s => s.ShiftDays)
                .WithOne(sd => sd.Shift)
                .HasForeignKey(sd => sd.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);  // Set delete behavior if needed


            /*
            // Many-to-many relationship between WorkerProfile and WorkersTeam
            modelBuilder.Entity<WorkerTeamMembership>()
                .HasKey(wtm => new { wtm.WorkerId, wtm.TeamId }); // Composite key

            modelBuilder.Entity<WorkerTeamMembership>()
                .HasOne(wtm => wtm.Worker)
                .WithMany(w => w.TeamMemberships)
                .HasForeignKey(wtm => wtm.WorkerId);

            modelBuilder.Entity<WorkerTeamMembership>()
                .HasOne(wtm => wtm.Team)
                .WithMany(t => t.TeamMemberships)
                .HasForeignKey(wtm => wtm.TeamId);

            // One-to-many relationship between Company and WorkersTeam
            modelBuilder.Entity<WorkersTeam>()
                .HasOne(wt => wt.Company)
                .WithMany(c => c.Teams)
                .HasForeignKey(wt => wt.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);*/



            // Unique constraint for repetitive shift days (ShiftId + DayOfWeek)
            modelBuilder.Entity<ShiftDaysOfWeek>()
                .HasIndex(sd => new { sd.ShiftId, sd.DayOfWeek })
                .IsUnique();

            // Configure one-to-many relationship for Shift and WorkerProfile (ShiftLeader)
            modelBuilder.Entity<Movement>()
                .HasOne(m => m.Shift)                     // A Movement optionally belongs to a Shift
                .WithMany(s => s.Movements)               // A Shift can have 0 or many Movements
                .HasForeignKey(m => m.ShiftId)            // Foreign key in Movement
                .OnDelete(DeleteBehavior.Restrict);       // Prevent cascading delete
        }
    }
}
