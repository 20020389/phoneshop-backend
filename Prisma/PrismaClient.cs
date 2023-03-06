using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PhoneShop.Model;

namespace PhoneShop.Prisma;

public partial class PrismaClient : DbContext
{
    public PrismaClient()
    {
    }

    public PrismaClient(DbContextOptions<PrismaClient> options)
        : base(options)
    {
    }

    public virtual DbSet<Phone> Phones { get; set; }

    public virtual DbSet<Phoneoffer> Phoneoffers { get; set; }

    public virtual DbSet<Phonerating> Phoneratings { get; set; }

    public virtual DbSet<Phonetostore> Phonetostores { get; set; }

    public virtual DbSet<PrismaMigration> PrismaMigrations { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<Storetotransaction> Storetotransactions { get; set; }

    public virtual DbSet<Storetouser> Storetousers { get; set; }

    public virtual DbSet<Stringtemplate> Stringtemplates { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("Host=localhost;Database=PhoneStore;Username=maianh;Password=dai;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Phone>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("phone");

            entity.HasIndex(e => e.RatingId, "Phone_ratingId_key").IsUnique();

            entity.HasIndex(e => e.Uid, "Phone_uid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(191)
                .HasColumnName("description");
            entity.Property(e => e.Detail)
                .HasMaxLength(191)
                .HasColumnName("detail");
            entity.Property(e => e.Images)
                .HasMaxLength(191)
                .HasColumnName("images");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Profile)
                .HasMaxLength(191)
                .HasColumnName("profile");
            entity.Property(e => e.RatingId)
                .HasMaxLength(191)
                .HasColumnName("ratingId");
            entity.Property(e => e.Sold).HasColumnName("sold");
            entity.Property(e => e.Tags)
                .HasMaxLength(191)
                .HasColumnName("tags");
            entity.Property(e => e.Uid)
                .HasMaxLength(191)
                .HasDefaultValueSql("'uuid()'")
                .HasColumnName("uid");

            entity.HasOne(d => d.Rating).WithOne(p => p.Phone)
                .HasPrincipalKey<Phonerating>(p => p.Uid)
                .HasForeignKey<Phone>(d => d.RatingId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Phone_ratingId_fkey");
        });

        modelBuilder.Entity<Phoneoffer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("phoneoffer");

            entity.HasIndex(e => e.PhoneId, "PhoneOffer_phoneId_fkey");

            entity.HasIndex(e => e.Uid, "PhoneOffer_uid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Color)
                .HasMaxLength(191)
                .HasColumnName("color");
            entity.Property(e => e.PhoneId)
                .HasMaxLength(191)
                .HasColumnName("phoneId");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Storage)
                .HasMaxLength(191)
                .HasColumnName("storage");
            entity.Property(e => e.Uid)
                .HasMaxLength(191)
                .HasDefaultValueSql("'uuid()'")
                .HasColumnName("uid");

            entity.HasOne(d => d.Phone).WithMany(p => p.Phoneoffers)
                .HasPrincipalKey(p => p.Uid)
                .HasForeignKey(d => d.PhoneId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("PhoneOffer_phoneId_fkey");
        });

        modelBuilder.Entity<Phonerating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("phonerating");

            entity.HasIndex(e => e.Uid, "PhoneRating_uid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Evaluated)
                .HasMaxLength(191)
                .HasColumnName("evaluated");
            entity.Property(e => e.RatingValue).HasColumnName("ratingValue");
            entity.Property(e => e.Uid)
                .HasMaxLength(191)
                .HasDefaultValueSql("'uuid()'")
                .HasColumnName("uid");
        });

        modelBuilder.Entity<Phonetostore>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("_phonetostore");

            entity.HasIndex(e => new { e.A, e.B }, "_PhoneToStore_AB_unique").IsUnique();

            entity.HasIndex(e => e.B, "_PhoneToStore_B_index");

            entity.HasOne(d => d.ANavigation).WithMany()
                .HasForeignKey(d => d.A)
                .HasConstraintName("_PhoneToStore_A_fkey");

            entity.HasOne(d => d.BNavigation).WithMany()
                .HasForeignKey(d => d.B)
                .HasConstraintName("_PhoneToStore_B_fkey");
        });

        modelBuilder.Entity<PrismaMigration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("_prisma_migrations");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .HasColumnName("id");
            entity.Property(e => e.AppliedStepsCount).HasColumnName("applied_steps_count");
            entity.Property(e => e.Checksum)
                .HasMaxLength(64)
                .HasColumnName("checksum");
            entity.Property(e => e.FinishedAt)
                .HasColumnType("datetime(3)")
                .HasColumnName("finished_at");
            entity.Property(e => e.Logs)
                .HasColumnType("text")
                .HasColumnName("logs");
            entity.Property(e => e.MigrationName)
                .HasMaxLength(255)
                .HasColumnName("migration_name");
            entity.Property(e => e.RolledBackAt)
                .HasColumnType("datetime(3)")
                .HasColumnName("rolled_back_at");
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("'CURRENT_TIMESTAMP(3)'")
                .HasColumnType("datetime(3)")
                .HasColumnName("started_at");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("store");

            entity.HasIndex(e => e.Uid, "Store_uid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Group)
                .HasMaxLength(191)
                .HasColumnName("group");
            entity.Property(e => e.Location)
                .HasMaxLength(191)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(191)
                .HasColumnName("phoneNumber");
            entity.Property(e => e.Uid)
                .HasMaxLength(191)
                .HasDefaultValueSql("'uuid()'")
                .HasColumnName("uid");
        });

        modelBuilder.Entity<Storetotransaction>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("_storetotransaction");

            entity.HasIndex(e => new { e.A, e.B }, "_StoreToTransaction_AB_unique").IsUnique();

            entity.HasIndex(e => e.B, "_StoreToTransaction_B_index");

            entity.HasOne(d => d.ANavigation).WithMany()
                .HasForeignKey(d => d.A)
                .HasConstraintName("_StoreToTransaction_A_fkey");

            entity.HasOne(d => d.BNavigation).WithMany()
                .HasForeignKey(d => d.B)
                .HasConstraintName("_StoreToTransaction_B_fkey");
        });

        modelBuilder.Entity<Storetouser>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("_storetouser");

            entity.HasIndex(e => new { e.A, e.B }, "_StoreToUser_AB_unique").IsUnique();

            entity.HasIndex(e => e.B, "_StoreToUser_B_index");

            entity.HasOne(d => d.ANavigation).WithMany()
                .HasForeignKey(d => d.A)
                .HasConstraintName("_StoreToUser_A_fkey");

            entity.HasOne(d => d.BNavigation).WithMany()
                .HasForeignKey(d => d.B)
                .HasConstraintName("_StoreToUser_B_fkey");
        });

        modelBuilder.Entity<Stringtemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("stringtemplate");

            entity.HasIndex(e => e.TransactionId, "StringTemplate_transactionId_fkey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TransactionId).HasColumnName("transactionId");
            entity.Property(e => e.Value)
                .HasMaxLength(191)
                .HasColumnName("value");

            entity.HasOne(d => d.Transaction).WithMany(p => p.Stringtemplates)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("StringTemplate_transactionId_fkey");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("transaction");

            entity.HasIndex(e => e.Uid, "Transaction_uid_key").IsUnique();

            entity.HasIndex(e => e.UserId, "Transaction_userId_fkey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Uid)
                .HasMaxLength(191)
                .HasDefaultValueSql("'uuid()'")
                .HasColumnName("uid");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Transaction_userId_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user");

            entity.HasIndex(e => e.Email, "User_email_key").IsUnique();

            entity.HasIndex(e => e.Uid, "User_uid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(191)
                .HasColumnName("email");
            entity.Property(e => e.Image)
                .HasMaxLength(191)
                .HasColumnName("image");
            entity.Property(e => e.LastLogin)
                .HasDefaultValueSql("'CURRENT_TIMESTAMP(3)'")
                .HasColumnType("datetime(3)")
                .HasColumnName("lastLogin");
            entity.Property(e => e.Name)
                .HasMaxLength(191)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(191)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(191)
                .HasColumnName("phoneNumber");
            entity.Property(e => e.Profile)
                .HasMaxLength(191)
                .HasColumnName("profile");
            entity.Property(e => e.RegisteredAt)
                .HasDefaultValueSql("'CURRENT_TIMESTAMP(3)'")
                .HasColumnType("datetime(3)")
                .HasColumnName("registeredAt");
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'DEFAULT'")
                .HasColumnType("enum('STORE','DEFAULT')")
                .HasColumnName("role");
            entity.Property(e => e.Uid)
                .HasMaxLength(191)
                .HasDefaultValueSql("'uuid()'")
                .HasColumnName("uid");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("'CURRENT_TIMESTAMP(3)'")
                .HasColumnType("datetime(3)")
                .HasColumnName("updateAt");
            entity.Property(e => e.Verified).HasColumnName("verified");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
