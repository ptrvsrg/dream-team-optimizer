﻿// <auto-generated />
using DreamTeamOptimizer.Core.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DreamTeamOptimizer.Core.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241117171653_InsertData")]
    partial class InsertData
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0-rc.2.24474.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DreamTeamOptimizer.Core.Persistence.Entities.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Grade")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("grade");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("employees");
                });

            modelBuilder.Entity("DreamTeamOptimizer.Core.Persistence.Entities.Hackathon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<double>("Result")
                        .HasColumnType("double precision")
                        .HasColumnName("result");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.HasKey("Id");

                    b.ToTable("hackathons");
                });

            modelBuilder.Entity("DreamTeamOptimizer.Core.Persistence.Entities.HackathonEmployee", b =>
                {
                    b.Property<int>("EmployeeId")
                        .HasColumnType("integer")
                        .HasColumnName("employee_id");

                    b.Property<int>("HackathonId")
                        .HasColumnType("integer")
                        .HasColumnName("hackathon_id");

                    b.HasKey("EmployeeId", "HackathonId");

                    b.HasIndex("HackathonId");

                    b.ToTable("hackathons_employees");
                });

            modelBuilder.Entity("DreamTeamOptimizer.Core.Persistence.Entities.Preference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("DesiredEmployeeId")
                        .HasColumnType("integer")
                        .HasColumnName("desired_employee_id");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("integer")
                        .HasColumnName("employeeId_id");

                    b.Property<int>("HackathonId")
                        .HasColumnType("integer")
                        .HasColumnName("hackathon_id");

                    b.HasKey("Id");

                    b.HasIndex("DesiredEmployeeId");

                    b.HasIndex("HackathonId");

                    b.HasIndex("EmployeeId", "DesiredEmployeeId", "HackathonId")
                        .IsUnique();

                    b.ToTable("preferences");
                });

            modelBuilder.Entity("DreamTeamOptimizer.Core.Persistence.Entities.Satisfaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("EmployeeId")
                        .HasColumnType("integer")
                        .HasColumnName("employee_id");

                    b.Property<int>("HackathonId")
                        .HasColumnType("integer")
                        .HasColumnName("hackathon_id");

                    b.Property<double>("Rank")
                        .HasColumnType("double precision")
                        .HasColumnName("rank");

                    b.HasKey("Id");

                    b.HasIndex("HackathonId");

                    b.HasIndex("EmployeeId", "HackathonId")
                        .IsUnique();

                    b.ToTable("satisfactions");
                });

            modelBuilder.Entity("DreamTeamOptimizer.Core.Persistence.Entities.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("HackathonId")
                        .HasColumnType("integer")
                        .HasColumnName("hackathon_id");

                    b.Property<int>("JuniorId")
                        .HasColumnType("integer")
                        .HasColumnName("junior_id");

                    b.Property<int>("TeamLeadId")
                        .HasColumnType("integer")
                        .HasColumnName("team_lead_id");

                    b.HasKey("Id");

                    b.HasIndex("HackathonId");

                    b.HasIndex("TeamLeadId");

                    b.HasIndex("JuniorId", "TeamLeadId", "HackathonId")
                        .IsUnique();

                    b.ToTable("teams");
                });

            modelBuilder.Entity("DreamTeamOptimizer.Core.Persistence.Entities.HackathonEmployee", b =>
                {
                    b.HasOne("DreamTeamOptimizer.Core.Persistence.Entities.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DreamTeamOptimizer.Core.Persistence.Entities.Hackathon", "Hackathon")
                        .WithMany()
                        .HasForeignKey("HackathonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Hackathon");
                });

            modelBuilder.Entity("DreamTeamOptimizer.Core.Persistence.Entities.Preference", b =>
                {
                    b.HasOne("DreamTeamOptimizer.Core.Persistence.Entities.Employee", "DesiredEmployee")
                        .WithMany()
                        .HasForeignKey("DesiredEmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DreamTeamOptimizer.Core.Persistence.Entities.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DreamTeamOptimizer.Core.Persistence.Entities.Hackathon", "Hackathon")
                        .WithMany("Preferences")
                        .HasForeignKey("HackathonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DesiredEmployee");

                    b.Navigation("Employee");

                    b.Navigation("Hackathon");
                });

            modelBuilder.Entity("DreamTeamOptimizer.Core.Persistence.Entities.Satisfaction", b =>
                {
                    b.HasOne("DreamTeamOptimizer.Core.Persistence.Entities.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DreamTeamOptimizer.Core.Persistence.Entities.Hackathon", "Hackathon")
                        .WithMany("Satisfactions")
                        .HasForeignKey("HackathonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Hackathon");
                });

            modelBuilder.Entity("DreamTeamOptimizer.Core.Persistence.Entities.Team", b =>
                {
                    b.HasOne("DreamTeamOptimizer.Core.Persistence.Entities.Hackathon", "Hackathon")
                        .WithMany("Teams")
                        .HasForeignKey("HackathonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DreamTeamOptimizer.Core.Persistence.Entities.Employee", "Junior")
                        .WithMany()
                        .HasForeignKey("JuniorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DreamTeamOptimizer.Core.Persistence.Entities.Employee", "TeamLead")
                        .WithMany()
                        .HasForeignKey("TeamLeadId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hackathon");

                    b.Navigation("Junior");

                    b.Navigation("TeamLead");
                });

            modelBuilder.Entity("DreamTeamOptimizer.Core.Persistence.Entities.Hackathon", b =>
                {
                    b.Navigation("Preferences");

                    b.Navigation("Satisfactions");

                    b.Navigation("Teams");
                });
#pragma warning restore 612, 618
        }
    }
}
