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
            builder.HasKey(@event => @event.Id);
            builder.Property(@event => @event.Id).HasColumnName("id").ValueGeneratedOnAdd();
            builder.Property(@event => @event.Title).HasColumnName("titulo").IsRequired();
            builder.Property(@event => @event.Description).HasColumnName("descricao").HasColumnType("TEXT");
            builder.Property(@event => @event.DateTime).HasColumnName("data_hora").IsRequired();
            builder.Property(@event => @event.Location).HasColumnName("local");
            builder.Property(@event => @event.Capacity).HasColumnName("limite_vagas").IsRequired();
        });

        modelBuilder.Entity<Participant>(builder =>
        {
            builder.ToTable("participantes");
            builder.HasKey(participant => participant.Id);
            builder.Property(participant => participant.Id).HasColumnName("id").ValueGeneratedOnAdd();
            builder.Property(participant => participant.Name).HasColumnName("nome").IsRequired();
            builder.Property(participant => participant.Email).HasColumnName("email").IsRequired();
            builder.Property(participant => participant.CreatedAt).HasColumnName("criado_em");
            builder.HasIndex(participant => participant.Email).IsUnique();
        });

        modelBuilder.Entity<Registration>(builder =>
        {
            builder.ToTable("inscricoes");
            builder.HasKey(registration => registration.Id);
            builder.Property(registration => registration.Id).HasColumnName("id").ValueGeneratedOnAdd();
            builder.Property(registration => registration.EventId).HasColumnName("evento_id");
            builder.Property(registration => registration.ParticipantId).HasColumnName("participante_id");
            builder.Property(registration => registration.RegisteredAt).HasColumnName("inscrito_em");
            builder.Property(registration => registration.Status).HasColumnName("status").HasConversion<string>();

            builder.HasOne(registration => registration.Event)
                .WithMany(@event => @event.Registrations)
                .HasForeignKey(registration => registration.EventId);

            builder.HasOne(registration => registration.Participant)
                .WithMany()
                .HasForeignKey(registration => registration.ParticipantId);
        });
    }
}
