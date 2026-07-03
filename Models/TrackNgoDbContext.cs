using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TrackNGoMati.Models;

public partial class TrackNgoDbContext : DbContext
{
    public TrackNgoDbContext()
    {
    }

    public TrackNgoDbContext(DbContextOptions<TrackNgoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditTrailEntry> AuditTrailEntries { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<DigitalSignature> DigitalSignatures { get; set; }

    public virtual DbSet<Document> Documents { get; set; }
    
    public virtual DbSet<CitizenFeedback> CitizenFeedbacks { get; set; }

    public virtual DbSet<DocumentAttachment> DocumentAttachments { get; set; }

    public virtual DbSet<DocumentComment> DocumentComments { get; set; }

    public virtual DbSet<DocumentMetadata> DocumentMetadatas { get; set; }

    public virtual DbSet<DocumentTypeConfig> DocumentTypeConfigs { get; set; }

    public virtual DbSet<EscalationLog> EscalationLogs { get; set; }

    public virtual DbSet<ExportAuditLog> ExportAuditLogs { get; set; }

    public virtual DbSet<QrcodeRecord> QrcodeRecords { get; set; }

    public virtual DbSet<ReportLog> ReportLogs { get; set; }

    public virtual DbSet<RoutingSlip> RoutingSlips { get; set; }

    public virtual DbSet<Smsnotification> Smsnotifications { get; set; }

    public virtual DbSet<SystemNotification> SystemNotifications { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WorkflowStep> WorkflowSteps { get; set; }

    public virtual DbSet<WorkflowTransition> WorkflowTransitions { get; set; }

    public virtual DbSet<DocumentTemplate> DocumentTemplates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditTrailEntry>(entity =>
        {
            entity.HasIndex(e => e.DocumentId, "IX_AuditTrailEntries_DocumentId");

            entity.HasIndex(e => e.UserId, "IX_AuditTrailEntries_UserId");

            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.Details).HasMaxLength(1000);
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.NewValue).HasMaxLength(500);
            entity.Property(e => e.OldValue).HasMaxLength(500);

            entity.HasOne(d => d.Document).WithMany(p => p.AuditTrailEntries)
                .HasForeignKey(d => d.DocumentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.User).WithMany(p => p.AuditTrailEntries)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasIndex(e => e.DepartmentCode, "IX_Departments_DepartmentCode").IsUnique();

            entity.HasIndex(e => e.HeadUserId, "IX_Departments_HeadUserId");

            entity.Property(e => e.DepartmentCode).HasMaxLength(20);
            entity.Property(e => e.DepartmentName).HasMaxLength(200);

            entity.HasOne(d => d.HeadUser).WithMany(p => p.Departments)
                .HasForeignKey(d => d.HeadUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<DigitalSignature>(entity =>
        {
            entity.HasIndex(e => e.DocumentId, "IX_DigitalSignatures_DocumentId");

            entity.HasIndex(e => e.SignedByUserId, "IX_DigitalSignatures_SignedByUserId");

            entity.Property(e => e.ActionType).HasMaxLength(50);
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.SignatureHash).HasMaxLength(128);
            entity.Property(e => e.SignatureImagePath).HasColumnType("nvarchar(max)");

            entity.HasOne(d => d.Document).WithMany(p => p.DigitalSignatures).HasForeignKey(d => d.DocumentId);

            entity.HasOne(d => d.SignedByUser).WithMany(p => p.DigitalSignatures)
                .HasForeignKey(d => d.SignedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasIndex(e => e.CreatedByUserId, "IX_Documents_CreatedByUserId");

            entity.HasIndex(e => e.CurrentStatus, "IX_Documents_CurrentStatus");

            entity.HasIndex(e => e.DateFiled, "IX_Documents_DateFiled");

            entity.HasIndex(e => e.DepartmentId, "IX_Documents_DepartmentId");

            entity.HasIndex(e => e.SubmittedByUserId, "IX_Documents_SubmittedByUserId");

            entity.HasIndex(e => e.TrackingNumber, "IX_Documents_TrackingNumber").IsUnique();

            entity.HasIndex(e => e.TypeId, "IX_Documents_TypeId");

            entity.Property(e => e.ArtaprocessingDays).HasColumnName("ARTAProcessingDays");
            entity.Property(e => e.AttachmentPath).HasMaxLength(500);
            entity.Property(e => e.ContactNumber).HasMaxLength(20);
            entity.Property(e => e.CurrentOfficeName).HasMaxLength(100);
            entity.Property(e => e.DocumentType).HasMaxLength(50);
            entity.Property(e => e.EmailAddress).HasMaxLength(100);
            entity.Property(e => e.OriginatingDepartment).HasMaxLength(100);
            entity.Property(e => e.QrcodePath)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("QRCodePath");
            entity.Property(e => e.SubmittedBy).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(300);
            entity.Property(e => e.TrackingNumber).HasMaxLength(30);

        entity.HasOne(d => d.AssignedToUser).WithMany(p => p.DocumentsAssignedToUser)
                .HasForeignKey(d => d.AssignedToUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Documents_Users_AssignedToUserId");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.DocumentCreatedByUsers)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Department).WithMany(p => p.Documents)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.SubmittedByUser).WithMany(p => p.DocumentSubmittedByUsers)
                .HasForeignKey(d => d.SubmittedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Type).WithMany(p => p.Documents)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<DocumentAttachment>(entity =>
        {
            entity.HasIndex(e => e.DocumentId, "IX_DocumentAttachments_DocumentId");

            entity.Property(e => e.FileName).HasMaxLength(200);
            entity.Property(e => e.FilePath).HasMaxLength(500);

            entity.HasOne(d => d.Document).WithMany(p => p.DocumentAttachments).HasForeignKey(d => d.DocumentId);
        });

        modelBuilder.Entity<DocumentComment>(entity =>
        {
            entity.HasIndex(e => e.DocumentId, "IX_DocumentComments_DocumentId");

            entity.HasIndex(e => e.UserId, "IX_DocumentComments_UserId");

            entity.Property(e => e.Content).HasMaxLength(2000);
            entity.Property(e => e.RemarkType).HasMaxLength(20);

            entity.HasOne(d => d.Document).WithMany(p => p.DocumentComments).HasForeignKey(d => d.DocumentId);

            entity.HasOne(d => d.User).WithMany(p => p.DocumentComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DocumentMetadata>(entity =>
        {
            entity.HasIndex(e => e.DocumentId, "IX_DocumentMetadatas_DocumentId").IsUnique();

            entity.Property(e => e.CategoryFlags).HasMaxLength(100);
            entity.Property(e => e.ConferenceName).HasMaxLength(300);
            entity.Property(e => e.Province).HasMaxLength(100);
            entity.Property(e => e.ReportNumber).HasMaxLength(50);
            entity.Property(e => e.SourceLink).HasMaxLength(500);
            entity.Property(e => e.ExtractedText).HasColumnType("nvarchar(max)");

            entity.HasOne(d => d.Document).WithOne(p => p.DocumentMetadata).HasForeignKey<DocumentMetadata>(d => d.DocumentId);
        });

        modelBuilder.Entity<DocumentTypeConfig>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<EscalationLog>(entity =>
        {
            entity.HasIndex(e => e.DocumentId, "IX_EscalationLogs_DocumentId");

            entity.HasIndex(e => e.NotifiedUserId, "IX_EscalationLogs_NotifiedUserId");

            entity.HasIndex(e => e.ResolvedByUserId, "IX_EscalationLogs_ResolvedByUserId");

            entity.Property(e => e.ArtaperiodDays).HasColumnName("ARTAPeriodDays");
            entity.Property(e => e.Artathreshold).HasColumnName("ARTAThreshold");
            entity.Property(e => e.EscalationLevel).HasMaxLength(20);
            entity.Property(e => e.EscalationReason).HasMaxLength(500);
            entity.Property(e => e.ResolutionNotes).HasMaxLength(1000);
            entity.Property(e => e.ViolatingOffice).HasMaxLength(100);

            entity.HasOne(d => d.Document).WithMany(p => p.EscalationLogs).HasForeignKey(d => d.DocumentId);

            entity.HasOne(d => d.NotifiedUser).WithMany(p => p.EscalationLogNotifiedUsers).HasForeignKey(d => d.NotifiedUserId);

            entity.HasOne(d => d.ResolvedByUser).WithMany(p => p.EscalationLogResolvedByUsers).HasForeignKey(d => d.ResolvedByUserId);
        });

        modelBuilder.Entity<ExportAuditLog>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_ExportAuditLogs_UserId");

            entity.Property(e => e.ExportScope).HasMaxLength(200);
            entity.Property(e => e.ExportType).HasMaxLength(20);
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");

            entity.HasOne(d => d.User).WithMany(p => p.ExportAuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<QrcodeRecord>(entity =>
        {
            entity.ToTable("QRCodeRecords");

            entity.HasIndex(e => e.DocumentId, "IX_QRCodeRecords_DocumentId");

            entity.HasIndex(e => e.TrackingNumber, "IX_QRCodeRecords_TrackingNumber").IsUnique();

            entity.Property(e => e.QrcodeImagePath)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("QRCodeImagePath");
            entity.Property(e => e.TrackingNumber).HasMaxLength(20);
            entity.Property(e => e.TrackingUrl).HasMaxLength(500);

            entity.HasOne(d => d.Document).WithMany(p => p.QrcodeRecords).HasForeignKey(d => d.DocumentId);
        });

        modelBuilder.Entity<ReportLog>(entity =>
        {
            entity.HasIndex(e => e.GeneratedByUserId, "IX_ReportLogs_GeneratedByUserId");

            entity.Property(e => e.ReportType).HasMaxLength(50);

            entity.HasOne(d => d.GeneratedByUser).WithMany(p => p.ReportLogs)
                .HasForeignKey(d => d.GeneratedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<RoutingSlip>(entity =>
        {
            entity.HasIndex(e => e.DocumentId, "IX_RoutingSlips_DocumentId").IsUnique();

            entity.HasIndex(e => e.NotedByUserId, "IX_RoutingSlips_NotedByUserId");

            entity.HasIndex(e => e.ReceivedByUserId, "IX_RoutingSlips_ReceivedByUserId");

            entity.HasIndex(e => e.TargetDepartmentId, "IX_RoutingSlips_TargetDepartmentId");

            entity.HasIndex(e => e.TrackingNumber, "IX_RoutingSlips_TrackingNumber").IsUnique();

            entity.Property(e => e.ActionInstruction).HasMaxLength(1000);
            entity.Property(e => e.SenderName).HasMaxLength(150);
            entity.Property(e => e.SlipStatus).HasMaxLength(30);
            entity.Property(e => e.TrackingNumber).HasMaxLength(30);

            entity.HasOne(d => d.Document).WithOne(p => p.RoutingSlip).HasForeignKey<RoutingSlip>(d => d.DocumentId);

            entity.HasOne(d => d.NotedByUser).WithMany(p => p.RoutingSlipNotedByUsers)
                .HasForeignKey(d => d.NotedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.ReceivedByUser).WithMany(p => p.RoutingSlipReceivedByUsers)
                .HasForeignKey(d => d.ReceivedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.TargetDepartment).WithMany(p => p.RoutingSlips)
                .HasForeignKey(d => d.TargetDepartmentId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Smsnotification>(entity =>
        {
            entity.ToTable("SMSNotifications");

            entity.HasIndex(e => e.DocumentId, "IX_SMSNotifications_DocumentId");

            entity.HasIndex(e => e.RecipientUserId, "IX_SMSNotifications_RecipientUserId");

            entity.Property(e => e.GatewayResponse).HasMaxLength(500);
            entity.Property(e => e.MessageContent).HasMaxLength(500);
            entity.Property(e => e.RecipientName).HasMaxLength(100);
            entity.Property(e => e.RecipientNumber).HasMaxLength(20);
            entity.Property(e => e.TriggerEvent).HasMaxLength(50);

            entity.HasOne(d => d.Document).WithMany(p => p.Smsnotifications).HasForeignKey(d => d.DocumentId);

            entity.HasOne(d => d.RecipientUser).WithMany(p => p.Smsnotifications)
                .HasForeignKey(d => d.RecipientUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.DepartmentId, "IX_Users_DepartmentId");

            entity.HasIndex(e => e.Username, "IX_Users_Username").IsUnique();

            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.MobileNumber).HasMaxLength(20);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.ProfileImagePath).HasMaxLength(250);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.DepartmentNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<WorkflowStep>(entity =>
        {
            entity.HasIndex(e => e.AssignedDepartmentId, "IX_WorkflowSteps_AssignedDepartmentId");

            entity.HasIndex(e => e.TypeId, "IX_WorkflowSteps_TypeId");

            entity.Property(e => e.AllowedActions).HasMaxLength(200);
            entity.Property(e => e.StepName).HasMaxLength(150);

            entity.HasOne(d => d.AssignedDepartment).WithMany(p => p.WorkflowSteps)
                .HasForeignKey(d => d.AssignedDepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.Type).WithMany(p => p.WorkflowSteps).HasForeignKey(d => d.TypeId);
        });

        modelBuilder.Entity<WorkflowTransition>(entity =>
        {
            entity.HasIndex(e => e.DocumentId, "IX_WorkflowTransitions_DocumentId");

            entity.HasIndex(e => e.PerformedByUserId, "IX_WorkflowTransitions_PerformedByUserId");

            entity.Property(e => e.ActionTaken).HasMaxLength(50);
            entity.Property(e => e.FromOffice).HasMaxLength(100);
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.ToOffice).HasMaxLength(100);

            entity.HasOne(d => d.Document).WithMany(p => p.WorkflowTransitions).HasForeignKey(d => d.DocumentId);

            entity.HasOne(d => d.PerformedByUser).WithMany(p => p.WorkflowTransitions)
                .HasForeignKey(d => d.PerformedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });


        modelBuilder.Entity<DocumentTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.HasOne(d => d.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(d => d.CreatedByUserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
