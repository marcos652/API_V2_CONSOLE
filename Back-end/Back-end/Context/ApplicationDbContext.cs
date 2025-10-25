using Back_end.Entities;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;


namespace Back_end.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<TemplateProcess> TemplateProcesses { get; set; }
        public DbSet<FormSubmission> FormSubmissions { get; set; }
        public DbSet<Proposal> Propostas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<TemplateProcess>()
                .HasOne(tp => tp.Template)
                .WithMany(static t => t.Processes)
                .HasForeignKey(tp => tp.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}