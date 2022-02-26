using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ChatService.Models
{
    public partial class ChatDBContext : DbContext
    {
        public ChatDBContext()
        {
        }

        public ChatDBContext(DbContextOptions<ChatDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRoom> UserRooms { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=ChatDB;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>(entity =>
            {
                entity.Property(e => e.Room).HasMaxLength(50);

                entity.Property(e => e.Sendfrom).HasMaxLength(50);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room");
            });

            modelBuilder.Entity<UserRoom>(entity =>
            {
                entity.ToTable("UserRoom");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.UserRooms)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK_UserRoom_Room");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRooms)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserRoom_Users");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
