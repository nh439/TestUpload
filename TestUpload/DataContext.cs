using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFramework;
using MySql.EntityFrameworkCore;
using TestUpload.Models.Entity;

namespace TestUpload
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<Login> login { get; set; }
        public DbSet<FileStorage> fileStorage { get; set; }
        public DbSet<FileUpload> fileUploads { get; set; }
        public DbSet<Sessions> sessions { get; set; }
        public DbSet<Changepassword> changepassword { get; set; }
        public DbSet<History> History { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasIndex(x => x.Email).IsUnique();
            builder.Entity<FileUpload>().HasIndex(x => x.Token).IsUnique();
            builder.Entity<FileStorage>().HasIndex(x => x.Token).IsUnique();
        }
    }
}
