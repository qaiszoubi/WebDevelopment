using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Hotel_Reservation_System.Models;

namespace Hotel_Reservation_System.Models;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Page> Pages { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=xe)));User Id=C##Qais2;Password=Test321;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("C##QAIS2")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.Hotelid).HasName("SYS_C008529");

            entity.ToTable("HOTELS");

            entity.Property(e => e.Hotelid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("HOTELID");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Imagepath)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("IMAGEPATH");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("LOCATION");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.Numberofrooms)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("NUMBEROFROOMS");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Paymentid).HasName("SYS_C008548");

            entity.ToTable("PAYMENTS");

            entity.Property(e => e.Paymentid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("PAYMENTID");
            entity.Property(e => e.Amount)
                .HasColumnType("NUMBER(18,2)")
                .HasColumnName("AMOUNT");
            entity.Property(e => e.Cardnumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CARDNUMBER");
            entity.Property(e => e.Paymentdate)
                .HasColumnType("DATE")
                .HasColumnName("PAYMENTDATE");
            entity.Property(e => e.Paymentmethod)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PAYMENTMETHOD");
            entity.Property(e => e.Reservationid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("RESERVATIONID");

            entity.HasOne(d => d.Reservation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.Reservationid)
                .HasConstraintName("SYS_C008549");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Reservationid).HasName("SYS_C008541");

            entity.ToTable("RESERVATIONS");

            entity.Property(e => e.Reservationid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("RESERVATIONID");
            entity.Property(e => e.Checkindate)
                .HasColumnType("DATE")
                .HasColumnName("CHECKINDATE");
            entity.Property(e => e.Checkoutdate)
                .HasColumnType("DATE")
                .HasColumnName("CHECKOUTDATE");
            entity.Property(e => e.Roomid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ROOMID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STATUS");
            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_ID");

            entity.HasOne(d => d.Room).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.Roomid)
                .HasConstraintName("SYS_C008543");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("SYS_C008542");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Roomid).HasName("SYS_C008535");

            entity.ToTable("ROOMS");

            entity.Property(e => e.Roomid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ROOMID");
            entity.Property(e => e.Hotelid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("HOTELID");
            entity.Property(e => e.Imagepath)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("IMAGEPATH");
            entity.Property(e => e.Isavailable)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ISAVAILABLE");
            entity.Property(e => e.Price)
                .HasColumnType("NUMBER(18,2)")
                .HasColumnName("PRICE");
            entity.Property(e => e.Roomnumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ROOMNUMBER");
            entity.Property(e => e.Roomtype)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ROOMTYPE");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.Hotelid)
                .HasConstraintName("SYS_C008536");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("SYS_C008523");

            entity.ToTable("USER_S");

            entity.HasIndex(e => e.Email, "SYS_C008524").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_ID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("PHONE");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USER_NAME");
            entity.Property(e => e.UserRole)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("USER_ROLE");
        });
        modelBuilder.Entity<Page>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("PAGES");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("TITLE");
            entity.Property(e => e.Content)
                .HasColumnType("CLOB")
                .HasColumnName("CONTENT");
            entity.Property(e => e.Slug)
                .HasMaxLength(100)
                .HasColumnName("SLUG");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public DbSet<Hotel_Reservation_System.Models.Page>? Page { get; set; }
}
