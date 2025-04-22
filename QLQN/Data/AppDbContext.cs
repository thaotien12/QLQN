using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QLQN.Models;
using System.Security.Cryptography;
using System.Text;

namespace QLQN.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }

        public DbSet<Computer> Computers { get; set; }
        public DbSet<UsageSession> UsageSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình quan hệ 1-1 giữa Account và User
            modelBuilder.Entity<Account>()
                .HasOne(a => a.User)
                .WithOne(u => u.Account)
                .HasForeignKey<User>(u => u.AccountId);

            // Seed dữ liệu mẫu
            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = HashPassword("admin123"),
                    Role = "admin"
                });

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    AccountId = 1,
                    FullName = "Quản trị viên",
                    Phone = "0123456789",
                    
                });
        }

        // Hàm mã hóa mật khẩu SHA256
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
