using Microsoft.EntityFrameworkCore;

namespace RoutesService.Models.DBModels
{
    public partial class RoutesServiceContext : DbContext
    {
        public RoutesServiceContext()
        {
        }

        public RoutesServiceContext(DbContextOptions<RoutesServiceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Routes> Routes { get; set; }
        public virtual DbSet<Tariffs> Tariffs { get; set; }
        public virtual DbSet<Carriages> Carriages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Routes>(entity =>
            {
                entity.HasKey(e => e.RouteId).HasName("PRIMARY");

                entity.ToTable("routes");

                entity.Property(e => e.RouteId).HasColumnName("route_id");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasColumnType("nvarchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Subtitle)
                    .HasColumnName("subtitle")
                    .HasColumnType("nvarchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp");
            });

            modelBuilder.Entity<Tariffs>(entity =>
            {
                entity.HasKey(e => new { e.RouteId, e.TicketId }).HasName("PRIMARY");

                entity.ToTable("tariffs");

                entity.Property(e => e.RouteId).HasColumnName("route_id");

                entity.Property(e => e.TicketId).HasColumnName("ticket_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp");

                entity.HasOne(d => d.Route)
                    .WithMany(p => p.Tariffs)
                    .HasForeignKey(d => d.RouteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("route_tariffs_ibfk_1");
            });

            modelBuilder.Entity<Carriages>(entity =>
            {
                entity.HasKey(e => new { e.RouteId, e.VehicleId }).HasName("PRIMARY");

                entity.ToTable("carriages");

                entity.Property(e => e.RouteId).HasColumnName("route_id");

                entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp");

                entity.HasOne(d => d.Route)
                    .WithMany(p => p.Carriages)
                    .HasForeignKey(d => d.RouteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("route_carriages_ibfk_1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
