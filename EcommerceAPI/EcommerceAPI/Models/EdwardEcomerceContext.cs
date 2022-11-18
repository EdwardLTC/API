using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EcommerceAPI.Models
{
    public partial class EdwardEcomerceContext : DbContext
    {
        public EdwardEcomerceContext()
        {
        }

        public EdwardEcomerceContext(DbContextOptions<EdwardEcomerceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Bill> Bills { get; set; } = null!;
        public virtual DbSet<BillDetail> BillDetails { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Clothe> Clothes { get; set; } = null!;
        public virtual DbSet<ClothesProperty> ClothesProperties { get; set; } = null!;
        public virtual DbSet<Favorite> Favorites { get; set; } = null!;
        public virtual DbSet<ImgUrl> ImgUrls { get; set; } = null!;
        public virtual DbSet<Person> People { get; set; } = null!;
        public virtual DbSet<Voucher> Vouchers { get; set; } = null!;

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("data source= Edward.kynalab.com;initial catalog=EdwardEcomerce;user id=Edward; password= dinhnt24@;MultipleActiveResultSets=True;App=EntityFramework");
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Edward");

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.ToTable("Bill");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateCreate).HasColumnType("date");

                entity.Property(e => e.DateReceived).HasColumnType("date");

                entity.Property(e => e.Iduser).HasColumnName("IDUser");

                entity.Property(e => e.Idvoucher).HasColumnName("IDvoucher");

                entity.Property(e => e.Status).HasMaxLength(256);

                entity.HasOne(d => d.IduserNavigation)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.Iduser)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Bill_People");

                entity.HasOne(d => d.IdvoucherNavigation)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.Idvoucher)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Bill_Voucher");
            });

            modelBuilder.Entity<BillDetail>(entity =>
            {
                entity.HasKey(e => new { e.Idbill, e.Idclothes });

                entity.ToTable("BillDetail");

                entity.Property(e => e.Idbill).HasColumnName("IDBill");

                entity.Property(e => e.Idclothes).HasColumnName("IDClothes");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.HasOne(d => d.IdbillNavigation)
                    .WithMany(p => p.BillDetails)
                    .HasForeignKey(d => d.Idbill)
                    .HasConstraintName("FK_BillDetail_Bill");

                entity.HasOne(d => d.IdclothesNavigation)
                    .WithMany(p => p.BillDetails)
                    .HasForeignKey(d => d.Idclothes)
                    .HasConstraintName("FK_BillDetail_Clothes_Properties");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Clothe>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Des)
                    .HasMaxLength(256)
                    .HasColumnName("des");

                entity.Property(e => e.IdCategory).HasColumnName("idCategory");

                entity.Property(e => e.Idseller).HasColumnName("IDseller");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.IdCategoryNavigation)
                    .WithMany(p => p.Clothes)
                    .HasForeignKey(d => d.IdCategory)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Clothes_Category");

                entity.HasOne(d => d.IdsellerNavigation)
                    .WithMany(p => p.Clothes)
                    .HasForeignKey(d => d.Idseller)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Clothes_People");
            });

            modelBuilder.Entity<ClothesProperty>(entity =>
            {
                entity.ToTable("Clothes_Properties");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Idclothes).HasColumnName("IDClothes");

                entity.Property(e => e.Size)
                    .HasMaxLength(50)
                    .HasColumnName("size");
            });

            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Idclothes).HasColumnName("IDClothes");

                entity.Property(e => e.Iduser).HasColumnName("IDUser");

                entity.HasOne(d => d.IdclothesNavigation)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.Idclothes)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Favorites_Clothes");
            });

            modelBuilder.Entity<ImgUrl>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Idclothes).HasColumnName("IDClothes");

                entity.Property(e => e.ImgUrl1)
                    .HasMaxLength(256)
                    .HasColumnName("ImgUrl");

                entity.HasOne(d => d.IdclothesNavigation)
                    .WithMany(p => p.ImgUrls)
                    .HasForeignKey(d => d.Idclothes)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ImgUrls_Clothes");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.ImgUrl).HasMaxLength(256);

                entity.Property(e => e.Mail).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.PhoneNum).HasMaxLength(50);

                entity.Property(e => e.Psw).HasMaxLength(256);
            });

            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.ToTable("Voucher");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Idseller).HasColumnName("IDSeller");

                entity.Property(e => e.Ratio).HasColumnName("ratio");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
