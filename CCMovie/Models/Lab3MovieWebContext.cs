using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CCMovie.Models;

public partial class Lab3MovieWebContext :  DbContext
{
    public Lab3MovieWebContext()
    {
    }

    public Lab3MovieWebContext(DbContextOptions<Lab3MovieWebContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {  
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=.;Database=lab3_movie_web;User ID=sa;Password=qq12345665;TrustServerCertificate=true;");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C0CA02455");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
