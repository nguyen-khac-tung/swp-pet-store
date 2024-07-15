using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

public partial class SwpProjectContext : DbContext
{
    public SwpProjectContext()
    {
    }

    public SwpProjectContext(DbContextOptions<SwpProjectContext> options)
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local); Database=SWP_PROJECT; Uid=kimduong;\nPwd=123456;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Account");

            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.Password).HasMaxLength(150);

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Account_Role");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("Admin");

            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(1000);
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.Phone).HasMaxLength(50);

            entity.HasOne(d => d.Account).WithMany(p => p.Admins)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Admin_Account");
        });

        modelBuilder.Entity<Attribute>(entity =>
        {
            entity.HasKey(e => e.AttributeId).HasName("PK_Taste_1");

            entity.ToTable("Attribute");

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .IsFixedLength();
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK_Brand_1");

            entity.ToTable("Brand");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsFixedLength();
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.ToTable("CartItem");

            entity.HasOne(d => d.Customer).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItem_Customer");

            entity.HasOne(d => d.ProductOption).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductOptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItem_ProductOption");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.CategoryId).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Consultation>(entity =>
        {
            entity.ToTable("Consultation");

            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(150);

            entity.HasOne(d => d.Employee).WithMany(p => p.Consultations)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_Consultation_Employee");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");

            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(250);
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.Phone).HasMaxLength(50);

            entity.HasOne(d => d.Account).WithMany(p => p.Customers)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Customer_Account");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.ToTable("Discount");

            entity.Property(e => e.DiscountId).ValueGeneratedNever();
            entity.Property(e => e.Code)
                .HasMaxLength(15)
                .IsFixedLength();
            entity.Property(e => e.CreatedAt)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.MaxValue).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.MinPurchase).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.Value).HasColumnType("decimal(18, 3)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Discount_Admin");

            entity.HasOne(d => d.DiscountType).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.DiscountTypeId)
                .HasConstraintName("FK_Discount_DiscountType");
        });

        modelBuilder.Entity<DiscountType>(entity =>
        {
            entity.ToTable("DiscountType");

            entity.Property(e => e.DiscountTypeId).ValueGeneratedNever();
            entity.Property(e => e.DiscountName)
                .HasMaxLength(250)
                .IsFixedLength();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employee");

            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(250);
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.Phone).HasMaxLength(50);

            entity.HasOne(d => d.Account).WithMany(p => p.Employees)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Account");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK_Feedback_1");

            entity.ToTable("Feedback");

            entity.Property(e => e.Content).HasMaxLength(500);
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(50);

            entity.HasOne(d => d.Product).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_Feedback_Product");

            entity.HasOne(d => d.Service).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_Feedback_Service");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.ToTable("Image");

            entity.Property(e => e.ImageId).ValueGeneratedNever();
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(250)
                .IsFixedLength();

            entity.HasOne(d => d.News).WithMany(p => p.Images)
                .HasForeignKey(d => d.NewsId)
                .HasConstraintName("FK_Image_News");

            entity.HasOne(d => d.Service).WithMany(p => p.Images)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_Image_Service");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.Summary).HasMaxLength(250);
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Employee).WithMany(p => p.News)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_News_Employee");

            entity.HasOne(d => d.Tag).WithMany(p => p.News)
                .HasForeignKey(d => d.TagId)
                .HasConstraintName("FK_News_TagNews");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.OrderId).ValueGeneratedNever();
            entity.Property(e => e.ConsigneeFullName).HasMaxLength(200);
            entity.Property(e => e.ConsigneePhone)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsFixedLength();
            entity.Property(e => e.FullName).HasMaxLength(250);
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.PaymetMethod).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.ShipAddress).HasMaxLength(250);

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Orders_Customer");

            entity.HasOne(d => d.Discount).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("FK_Orders_Discount");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItem");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_Orders");

            entity.HasOne(d => d.ProductOption).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductOptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_ProductOption");

            entity.HasOne(d => d.Promotion).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.PromotionId)
                .HasConstraintName("FK_OrderItem_Promotion");
        });

        modelBuilder.Entity<OrderService>(entity =>
        {
            entity.ToTable("OrderService");

            entity.Property(e => e.Message).HasMaxLength(250);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.OrderTime).HasPrecision(0);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(30);

            entity.HasOne(d => d.Customer).WithMany(p => p.OrderServices)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_OrderService_Customer");

            entity.HasOne(d => d.Employee).WithMany(p => p.OrderServices)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_OrderService_Employee");

            entity.HasOne(d => d.ServiceOption).WithMany(p => p.OrderServices)
                .HasForeignKey(d => d.ServiceOptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderService_ServiceOption");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.ProductId).ValueGeneratedNever();
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Brand");

            entity.HasOne(d => d.ProductCate).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductCateId)
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
            entity.HasKey(e => e.ProductCateId);

            entity.ToTable("ProductCategory");

            entity.Property(e => e.ProductCateId).ValueGeneratedNever();
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsFixedLength();

            entity.HasOne(d => d.Category).WithMany(p => p.ProductCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductCategory_Category");
        });

        modelBuilder.Entity<ProductOption>(entity =>
        {
            entity.ToTable("ProductOption");

            entity.Property(e => e.IsDelete).HasDefaultValue(false);

            entity.HasOne(d => d.Attribute).WithMany(p => p.ProductOptions)
                .HasForeignKey(d => d.AttributeId)
                .HasConstraintName("FK_ProductOption_Attribute");

            entity.HasOne(d => d.Image).WithMany(p => p.ProductOptions)
                .HasForeignKey(d => d.ImageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductOption_Image");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductOptions)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductOption_Product");

            entity.HasOne(d => d.Size).WithMany(p => p.ProductOptions)
                .HasForeignKey(d => d.SizeId)
                .HasConstraintName("FK_ProductOption_Size");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.ToTable("Promotion");

            entity.Property(e => e.PromotionId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasMaxLength(25);
            entity.Property(e => e.EndDate).HasMaxLength(25);
            entity.Property(e => e.Name)
                .HasMaxLength(250)
                .IsFixedLength();
            entity.Property(e => e.StartDate).HasMaxLength(25);

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.Promotions)
                .HasForeignKey(d => d.CreateBy)
                .HasConstraintName("FK_Promotion_Admin");
        });

        modelBuilder.Entity<ResponseFeedback>(entity =>
        {
            entity.HasKey(e => e.ResponseId);

            entity.ToTable("ResponseFeedback");

            entity.Property(e => e.Content).HasMaxLength(500);
            entity.Property(e => e.DateCreated).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.ResponseFeedbacks)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResponseFeedback_Employee");

            entity.HasOne(d => d.Feedback).WithMany(p => p.ResponseFeedbacks)
                .HasForeignKey(d => d.FeedbackId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResponseFeedback_Feedback");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.RoleId).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.ToTable("Service");

            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Type).HasMaxLength(30);
        });

        modelBuilder.Entity<ServiceOption>(entity =>
        {
            entity.HasKey(e => e.ServiceOptionId).HasName("PK_ServiceOption_1");

            entity.ToTable("ServiceOption");

            entity.Property(e => e.PetType).HasMaxLength(10);
            entity.Property(e => e.Weight).HasMaxLength(30);

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceOptions)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ServiceOption_Service");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.SizeId).HasName("PK_Size_1");

            entity.ToTable("Size");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<TagNews>(entity =>
        {
            entity.HasKey(e => e.TagId);

            entity.Property(e => e.TagId).ValueGeneratedNever();
            entity.Property(e => e.TagName).HasMaxLength(50);
        });

        modelBuilder.Entity<TimeService>(entity =>
        {
            entity.HasKey(e => new { e.WorkingTimeId, e.ServiceId });

            entity.ToTable("TimeService");

            entity.Property(e => e.Note).HasMaxLength(20);

            entity.HasOne(d => d.Service).WithMany(p => p.TimeServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TimeService_Service");

            entity.HasOne(d => d.WorkingTime).WithMany(p => p.TimeServices)
                .HasForeignKey(d => d.WorkingTimeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TimeService_WorkingTime");
        });

        modelBuilder.Entity<WorkingTime>(entity =>
        {
            entity.ToTable("WorkingTime");

            entity.Property(e => e.WorkingTimeId).ValueGeneratedNever();
            entity.Property(e => e.Time).HasPrecision(0);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
