using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace linco_core.Models;

public partial class LincoDbContext : DbContext
{
    public LincoDbContext()
    {
    }

    public LincoDbContext(DbContextOptions<LincoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AiCumleAsistani> AiCumleAsistanis { get; set; }

    public virtual DbSet<GenelSozluk> GenelSozluks { get; set; }

    public virtual DbSet<KullaniciKelimeleri> KullaniciKelimeleris { get; set; }

    public virtual DbSet<KullaniciSeviyeIlerleme> KullaniciSeviyeIlerlemes { get; set; }

    public virtual DbSet<Kullanicilar> Kullanicilars { get; set; }

    public virtual DbSet<OyunSkorlari> OyunSkorlaris { get; set; }

    public virtual DbSet<Oyunlar> Oyunlars { get; set; }

    public virtual DbSet<SinavSonuclari> SinavSonuclaris { get; set; }

    public virtual DbSet<Yoneticiler> Yoneticilers { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AiCumleAsistani>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AI_Cumle__3214EC07B77B2424");

            entity.ToTable("AI_Cumle_Asistani");

            entity.HasIndex(e => new { e.KelimeGirilen, e.SecilenSeviye }, "UQ_AI_Asistan_Kayit").IsUnique();

            entity.Property(e => e.KelimeGirilen).HasMaxLength(100);
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SecilenSeviye).HasMaxLength(2);
        });

        modelBuilder.Entity<GenelSozluk>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Genel_So__3214EC070D5CF3B9");

            entity.ToTable("Genel_Sozluk");

            entity.HasIndex(e => e.Ingilizce, "UQ_Sozluk_Ingilizce").IsUnique();

            entity.Property(e => e.Harf)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.Ingilizce).HasMaxLength(100);
            entity.Property(e => e.Okunus).HasMaxLength(100);
            entity.Property(e => e.Seviye).HasMaxLength(2);
            entity.Property(e => e.TelaffuzUrl)
                .HasMaxLength(255)
                .HasColumnName("Telaffuz_Url");
            entity.Property(e => e.Turkce).HasMaxLength(100);
        });

        modelBuilder.Entity<KullaniciKelimeleri>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Kullanic__3214EC0778630EAD");

            entity.ToTable("Kullanici_Kelimeleri");

            entity.HasIndex(e => new { e.KullaniciId, e.KelimeId }, "UQ_Kullanici_Kelime_Eslesme").IsUnique();

            entity.Property(e => e.Durum).HasDefaultValue((byte)0);
            entity.Property(e => e.EklenmeTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Kelime).WithMany(p => p.KullaniciKelimeleris)
                .HasForeignKey(d => d.KelimeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KullaniciKelimeleri_Kelime");

            entity.HasOne(d => d.Kullanici).WithMany(p => p.KullaniciKelimeleris)
                .HasForeignKey(d => d.KullaniciId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KullaniciKelimeleri_Kullanici");
        });

        modelBuilder.Entity<KullaniciSeviyeIlerleme>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Kullanic__3214EC07C081198C");

            entity.ToTable("Kullanici_Seviye_Ilerleme");

            entity.HasIndex(e => new { e.KullaniciId, e.Harf, e.Seviye }, "UQ_Kullanici_Ilerleme_Benzersiz").IsUnique();

            entity.Property(e => e.Harf)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.Seviye).HasMaxLength(2);

            entity.HasOne(d => d.Kullanici).WithMany(p => p.KullaniciSeviyeIlerlemes)
                .HasForeignKey(d => d.KullaniciId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ilerleme_Kullanici");

            entity.HasOne(d => d.SonKelime).WithMany(p => p.KullaniciSeviyeIlerlemes)
                .HasForeignKey(d => d.SonKelimeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ilerleme_Kelime");
        });

        modelBuilder.Entity<Kullanicilar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Kullanic__3214EC070B94E278");

            entity.ToTable("Kullanicilar");

            entity.HasIndex(e => e.Eposta, "UQ_Kullanici_Eposta").IsUnique();

            entity.HasIndex(e => e.KullaniciAdi, "UQ_Kullanici_KullaniciAdi").IsUnique();

            entity.Property(e => e.Eposta).HasMaxLength(100);
            entity.Property(e => e.KayitTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.KullaniciAdi).HasMaxLength(50);
            entity.Property(e => e.MevcutSeviye)
                .HasMaxLength(2)
                .HasDefaultValue("A1");
            entity.Property(e => e.Sifre).HasMaxLength(255);
            entity.Property(e => e.SonGirisTarihi).HasColumnType("datetime");
            entity.Property(e => e.ToplamXp)
                .HasDefaultValue(0)
                .HasColumnName("ToplamXP");
        });

        modelBuilder.Entity<OyunSkorlari>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Oyun_Sko__3214EC07AA87CE3A");

            entity.ToTable("Oyun_Skorlari");

            entity.Property(e => e.AlinanSkor).HasDefaultValue(0);
            entity.Property(e => e.OynamaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Kullanici).WithMany(p => p.OyunSkorlaris)
                .HasForeignKey(d => d.KullaniciId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OyunSkorlari_Kullanici");

            entity.HasOne(d => d.Oyun).WithMany(p => p.OyunSkorlaris)
                .HasForeignKey(d => d.OyunId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OyunSkorlari_Oyun");
        });

        modelBuilder.Entity<Oyunlar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Oyunlar__3214EC079683CEE2");

            entity.ToTable("Oyunlar");

            entity.HasIndex(e => e.OyunAdi, "UQ_Oyunlar_OyunAdi").IsUnique();

            entity.Property(e => e.OyunAdi).HasMaxLength(100);
        });

        modelBuilder.Entity<SinavSonuclari>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sinav_So__3214EC070F9D268D");

            entity.ToTable("Sinav_Sonuclari");

            entity.Property(e => e.SinavKonusu).HasMaxLength(100);
            entity.Property(e => e.TestTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Kullanici).WithMany(p => p.SinavSonuclaris)
                .HasForeignKey(d => d.KullaniciId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SinavSonuclari_Kullanici");
        });

        modelBuilder.Entity<Yoneticiler>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Yonetici__3214EC07AC8E651F");

            entity.ToTable("Yoneticiler");

            entity.HasIndex(e => e.KullaniciAdi, "UQ_Yonetici_KullaniciAdi").IsUnique();

            entity.Property(e => e.KullaniciAdi).HasMaxLength(50);
            entity.Property(e => e.Sifre).HasMaxLength(255);
            entity.Property(e => e.YetkiSeviyesi).HasDefaultValue((byte)1);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
