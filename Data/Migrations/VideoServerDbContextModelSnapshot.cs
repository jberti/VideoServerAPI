﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VideoServerAPI.Data;

namespace VideoServerAPI.Data.Migrations
{
    [DbContext(typeof(VideoServerDbContext))]
    partial class VideoServerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.3");

            modelBuilder.Entity("VideoServerAPI.Models.Server", b =>
                {
                    b.Property<Guid>("ServerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Port")
                        .HasColumnType("INTEGER");

                    b.HasKey("ServerId");

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("VideoServerAPI.Models.Video", b =>
                {
                    b.Property<Guid>("VideoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ServerId")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("VideoContent")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.HasKey("VideoId");

                    b.HasIndex("ServerId");

                    b.ToTable("Videos");
                });

            modelBuilder.Entity("VideoServerAPI.Models.Video", b =>
                {
                    b.HasOne("VideoServerAPI.Models.Server", "Server")
                        .WithMany("Videos")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("VideoServerAPI.Models.Server", b =>
                {
                    b.Navigation("Videos");
                });
#pragma warning restore 612, 618
        }
    }
}
