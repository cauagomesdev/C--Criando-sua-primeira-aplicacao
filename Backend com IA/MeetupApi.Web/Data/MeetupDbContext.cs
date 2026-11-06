using MeetupApi.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetupApi.Web.Data;

public class MeetupDbContext : DbContext
{
    public MeetupDbContext(DbContextOptions<MeetupDbContext> options) : base(options) { }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Registration> Registrations => Set<Registration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(builder =>
        {
            builder.ToTable("eventos");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            builder.Property(e => e.Title).HasColumnName("titulo").IsRequired();
            builder.Property(e => e.Description).HasColumnName("descricao").HasColumnType("TEXT");
            builder.Property(e => e.DateTime).HasColumnName("data_hora").IsRequired();
            builder.Property(e => e.Location).HasColumnName("local");
            builder.Property(e => e.Capacity).HasColumnName("limite_vagas").IsRequired();
        });

        modelBuilder.Entity<Participant>(builder =>
        {
            builder.ToTable("participantes");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();
            builder.Property(p => p.Name).HasColumnName("nome").IsRequired();
            builder.Property(p => p.Email).HasColumnName("email").IsRequired();
            builder.Property(p => p.CreatedAt).HasColumnName("criado_em");
            builder.HasIndex(p => p.Email).IsUnique();
        });

        modelBuilder.Entity<Registration>(builder =>
        {
            builder.ToTable("inscricoes");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnName("id").ValueGeneratedOnAdd();
            builder.Property(r => r.EventId).HasColumnName("evento_id");
            builder.Property(r => r.ParticipantId).HasColumnName("participante_id");
            builder.Property(r => r.RegisteredAt).HasColumnName("inscrito_em");
            builder.Property(r => r.Status).HasColumnName("status").HasConversion<string>();

            builder.HasOne(r => r.Event)
                .WithMany(e => e.Registrations)
                .HasForeignKey(r => r.EventId);

            builder.HasOne(r => r.Participant)
                .WithMany()
                .HasForeignKey(r => r.ParticipantId);
        });
    }
}
