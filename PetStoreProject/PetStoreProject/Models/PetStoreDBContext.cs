﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

public partial class PetStoreDBContext : DbContext
{
    public PetStoreDBContext(DbContextOptions<PetStoreDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Attribute> Attributes { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Consultation> Consultations { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<DiscountType> DiscountTypes { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderService> OrderServices { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductOption> ProductOptions { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<ResponseFeedback> ResponseFeedbacks { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceOption> ServiceOptions { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<TagNews> TagNews { get; set; }

    public virtual DbSet<TimeService> TimeServices { get; set; }

    public virtual DbSet<WorkingTime> WorkingTimes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.Property(e => e.IsDelete).HasDefaultValue(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts).HasConstraintName("FK_Account_Role");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.Property(e => e.IsDelete).HasDefaultValue(false);

            entity.HasOne(d => d.Account).WithMany(p => p.Admins)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Admin_Account");
        });

        modelBuilder.Entity<Attribute>(entity =>
        {
            entity.HasKey(e => e.AttributeId).HasName("PK_Taste_1");

            entity.Property(e => e.Name).IsFixedLength();
            entity.Property(e => e.Type).IsFixedLength();
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK_Brand_1");

            entity.Property(e => e.Name).IsFixedLength();
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasOne(d => d.Customer).WithMany(p => p.CartItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItem_Customer");

            entity.HasOne(d => d.ProductOption).WithMany(p => p.CartItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItem_ProductOption");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.CategoryId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Consultation>(entity =>
        {
            entity.HasOne(d => d.Employee).WithMany(p => p.Consultations).HasConstraintName("FK_Consultation_Employee");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.IsDelete).HasDefaultValue(false);

            entity.HasOne(d => d.Account).WithMany(p => p.Customers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Customer_Account");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.Property(e => e.DiscountId).ValueGeneratedNever();
            entity.Property(e => e.Code).IsFixedLength();
            entity.Property(e => e.CreatedAt).IsFixedLength();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Discounts).HasConstraintName("FK_Discount_Admin");

            entity.HasOne(d => d.DiscountType).WithMany(p => p.Discounts).HasConstraintName("FK_Discount_DiscountType");
        });

        modelBuilder.Entity<DiscountType>(entity =>
        {
            entity.Property(e => e.DiscountTypeId).ValueGeneratedNever();
            entity.Property(e => e.DiscountName).IsFixedLength();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.IsDelete).HasDefaultValue(false);

            entity.HasOne(d => d.Account).WithMany(p => p.Employees)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Account");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK_Feedback_1");

            entity.HasOne(d => d.Product).WithMany(p => p.Feedbacks).HasConstraintName("FK_Feedback_Product");

            entity.HasOne(d => d.Service).WithMany(p => p.Feedbacks).HasConstraintName("FK_Feedback_Service");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.Property(e => e.ImageId).ValueGeneratedNever();
            entity.Property(e => e.ImageUrl).IsFixedLength();

            entity.HasOne(d => d.News).WithMany(p => p.Images).HasConstraintName("FK_Image_News");

            entity.HasOne(d => d.Service).WithMany(p => p.Images).HasConstraintName("FK_Image_Service");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.Property(e => e.IsDelete).HasDefaultValue(false);

            entity.HasOne(d => d.Employee).WithMany(p => p.News)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_News_Employee");

            entity.HasOne(d => d.Tag).WithMany(p => p.News).HasConstraintName("FK_News_TagNews");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.ConsigneePhone).IsFixedLength();
            entity.Property(e => e.Email).IsFixedLength();

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders).HasConstraintName("FK_Orders_Customer");

            entity.HasOne(d => d.Discount).WithMany(p => p.Orders).HasConstraintName("FK_Orders_Discount");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_Orders");

            entity.HasOne(d => d.ProductOption).WithMany(p => p.OrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_ProductOption");

            entity.HasOne(d => d.Promotion).WithMany(p => p.OrderItems).HasConstraintName("FK_OrderItem_Promotion");
        });

        modelBuilder.Entity<OrderService>(entity =>
        {
            entity.HasOne(d => d.Customer).WithMany(p => p.OrderServices).HasConstraintName("FK_OrderService_Customer");

            entity.HasOne(d => d.Employee).WithMany(p => p.OrderServices).HasConstraintName("FK_OrderService_Employee");

            entity.HasOne(d => d.ServiceOption).WithMany(p => p.OrderServices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderService_ServiceOption");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.ProductId).ValueGeneratedNever();
            entity.Property(e => e.IsDelete).HasDefaultValue(false);

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Brand");

            entity.HasOne(d => d.ProductCate).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_ProductCategory");

            entity.HasMany(d => d.Customers).WithMany(p => p.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "FavouriteList",
                    r => r.HasOne<Customer>().WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_FavouriteList_Customer"),
                    l => l.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_FavouriteList_Product"),
                    j =>
                    {
                        j.HasKey("ProductId", "CustomerId");
                        j.ToTable("FavouriteList");
                    });
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.Property(e => e.ProductCateId).ValueGeneratedNever();
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.Name).IsFixedLength();

            entity.HasOne(d => d.Category).WithMany(p => p.ProductCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductCategory_Category");
        });

        modelBuilder.Entity<ProductOption>(entity =>
        {
            entity.Property(e => e.IsDelete).HasDefaultValue(false);

            entity.HasOne(d => d.Attribute).WithMany(p => p.ProductOptions).HasConstraintName("FK_ProductOption_Attribute");

            entity.HasOne(d => d.Image).WithMany(p => p.ProductOptions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductOption_Image");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductOptions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductOption_Product");

            entity.HasOne(d => d.Size).WithMany(p => p.ProductOptions).HasConstraintName("FK_ProductOption_Size");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.Property(e => e.PromotionId).ValueGeneratedNever();
            entity.Property(e => e.Name).IsFixedLength();

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.Promotions).HasConstraintName("FK_Promotion_Admin");
        });

        modelBuilder.Entity<ResponseFeedback>(entity =>
        {
            entity.HasOne(d => d.Employee).WithMany(p => p.ResponseFeedbacks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResponseFeedback_Employee");

            entity.HasOne(d => d.Feedback).WithMany(p => p.ResponseFeedbacks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResponseFeedback_Feedback");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleId).ValueGeneratedNever();
        });

        modelBuilder.Entity<ServiceOption>(entity =>
        {
            entity.HasKey(e => e.ServiceOptionId).HasName("PK_ServiceOption_1");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceOptions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ServiceOption_Service");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.SizeId).HasName("PK_Size_1");
        });

        modelBuilder.Entity<TagNews>(entity =>
        {
            entity.Property(e => e.TagId).ValueGeneratedNever();
        });

        modelBuilder.Entity<TimeService>(entity =>
        {
            entity.HasOne(d => d.Service).WithMany(p => p.TimeServices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TimeService_Service");

            entity.HasOne(d => d.WorkingTime).WithMany(p => p.TimeServices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TimeService_WorkingTime");
        });

        modelBuilder.Entity<WorkingTime>(entity =>
        {
            entity.Property(e => e.WorkingTimeId).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}