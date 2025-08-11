using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace InternProject.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bumon> Bumons { get; set; }

    public virtual DbSet<EsYdenpyo> EsYdenpyos { get; set; }

    public virtual DbSet<EsYdenpyod> EsYdenpyods { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Bumon>(entity =>
        {
            entity.HasKey(e => e.Bumoncd).HasName("SYS_C008227");

            entity.ToTable("BUMON", "INTERPJ");

            entity.Property(e => e.Bumoncd)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("BUMONCD");
            entity.Property(e => e.Bumonnm)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("BUMONNM");
        });

        modelBuilder.Entity<EsYdenpyo>(entity =>
        {
            entity.HasKey(e => e.Denpyono).HasName("SYS_C008229");

            entity.ToTable("ES_YDENPYO", "INTERPJ");

            entity.Property(e => e.Denpyono)
                .HasColumnType("NUMBER")
                .HasColumnName("DENPYONO");
            entity.Property(e => e.Biko)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("BIKO");
            entity.Property(e => e.BumoncdYkanr)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("BUMONCD_YKANR");
            entity.Property(e => e.Denpyodt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DENPYODT");
            entity.Property(e => e.InsertDate)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("INSERT_DATE");
            entity.Property(e => e.InsertOpeId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("INSERT_OPE_ID");
            entity.Property(e => e.InsertPgmId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("INSERT_PGM_ID");
            entity.Property(e => e.InsertPgmPrm)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("INSERT_PGM_PRM");
            entity.Property(e => e.Kaikeind)
                .HasColumnType("NUMBER")
                .HasColumnName("KAIKEIND");
            entity.Property(e => e.Kingaku)
                .HasColumnType("NUMBER")
                .HasColumnName("KINGAKU");
            entity.Property(e => e.Shiharaidt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SHIHARAIDT");
            entity.Property(e => e.Suitokb)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SUITOKB");
            entity.Property(e => e.Uketukedt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UKETUKEDT");
            entity.Property(e => e.UpdateDate)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UPDATE_DATE");
            entity.Property(e => e.UpdateOpeId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("UPDATE_OPE_ID");
            entity.Property(e => e.UpdatePgmId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("UPDATE_PGM_ID");
            entity.Property(e => e.UpdatePgmPrm)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("UPDATE_PGM_PRM");

            entity.HasOne(d => d.BumoncdYkanrNavigation).WithMany(p => p.EsYdenpyos)
                .HasForeignKey(d => d.BumoncdYkanr)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_BUMONCD_YKANR");
        });

        modelBuilder.Entity<EsYdenpyod>(entity =>
        {
            entity.HasKey(e => new { e.Denpyono, e.Gyono });

            entity.ToTable("ES_YDENPYOD", "INTERPJ");

            entity.Property(e => e.Denpyono)
                .HasColumnType("NUMBER")
                .HasColumnName("DENPYONO");
            entity.Property(e => e.Gyono)
                .HasColumnType("NUMBER")
                .HasColumnName("GYONO");
            entity.Property(e => e.Idodt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("IDODT");
            entity.Property(e => e.InsertDate)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("INSERT_DATE");
            entity.Property(e => e.InsertOpeId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("INSERT_OPE_ID");
            entity.Property(e => e.InsertPgmId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("INSERT_PGM_ID");
            entity.Property(e => e.InsertPgmPrm)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("INSERT_PGM_PRM");
            entity.Property(e => e.Keiro)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("KEIRO");
            entity.Property(e => e.Kingaku)
                .HasColumnType("NUMBER")
                .HasColumnName("KINGAKU");
            entity.Property(e => e.MokutekiPlc)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("MOKUTEKI_PLC");
            entity.Property(e => e.ShuppatsuPlc)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SHUPPATSU_PLC");
            entity.Property(e => e.UpdateDate)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UPDATE_DATE");
            entity.Property(e => e.UpdateOpeId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UPDATE_OPE_ID");
            entity.Property(e => e.UpdatePgmId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UPDATE_PGM_ID");
            entity.Property(e => e.UpdatePgmPrm)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UPDATE_PGM_PRM");

            entity.HasOne(d => d.DenpyonoNavigation).WithMany(p => p.EsYdenpyods)
                .HasForeignKey(d => d.Denpyono)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ES_YDENPYOD");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
