using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackNGoMati.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentTypeConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TotalSteps = table.Column<int>(type: "int", nullable: false),
                    DefaultProcessingDays = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypeConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditTrailEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrailEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DepartmentCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HeadUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProfileImagePath = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ExportPasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    StepNumber = table.Column<int>(type: "int", nullable: false),
                    StepName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AssignedDepartmentId = table.Column<int>(type: "int", nullable: true),
                    AllowedActions = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowSteps_Departments_AssignedDepartmentId",
                        column: x => x.AssignedDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WorkflowSteps_DocumentTypeConfigs_TypeId",
                        column: x => x.TypeId,
                        principalTable: "DocumentTypeConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrackingNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: true),
                    OriginatingDepartment = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    SubmittedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubmittedByUserId = table.Column<int>(type: "int", nullable: true),
                    ContactNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CurrentStatus = table.Column<int>(type: "int", nullable: false),
                    CurrentOfficeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CurrentStepIndex = table.Column<int>(type: "int", nullable: false),
                    TotalSteps = table.Column<int>(type: "int", nullable: false),
                    AttachmentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QRCodePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DateFiled = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCompleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ARTAProcessingDays = table.Column<int>(type: "int", nullable: false),
                    EscalationDeadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsEscalated = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Documents_DocumentTypeConfigs_TypeId",
                        column: x => x.TypeId,
                        principalTable: "DocumentTypeConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Documents_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Documents_Users_SubmittedByUserId",
                        column: x => x.SubmittedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ExportAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ExportType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ExportScope = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RecordCount = table.Column<int>(type: "int", nullable: false),
                    ExportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExportAuditLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReportLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GeneratedByUserId = table.Column<int>(type: "int", nullable: false),
                    ReportType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportLogs_Users_GeneratedByUserId",
                        column: x => x.GeneratedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SystemNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemNotifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DigitalSignatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    SignedByUserId = table.Column<int>(type: "int", nullable: false),
                    SignatureImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SignatureHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DigitalSignatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DigitalSignatures_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DigitalSignatures_Users_SignedByUserId",
                        column: x => x.SignedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentAttachments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsInternal = table.Column<bool>(type: "bit", nullable: false),
                    RemarkType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AnchorLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Resolved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentComments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentComments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentMetadatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    ConferenceName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SourceLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Province = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReportNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CategoryFlags = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentMetadatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentMetadatas_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EscalationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    ViolatingOffice = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ARTAThreshold = table.Column<int>(type: "int", nullable: false),
                    ARTAPeriodDays = table.Column<int>(type: "int", nullable: false),
                    ElapsedDays = table.Column<int>(type: "int", nullable: false),
                    ActualElapsedDays = table.Column<int>(type: "int", nullable: false),
                    EscalationLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NotifiedUserId = table.Column<int>(type: "int", nullable: true),
                    NotificationSent = table.Column<bool>(type: "bit", nullable: false),
                    EscalationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EscalatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    Resolved = table.Column<bool>(type: "bit", nullable: false),
                    ResolvedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscalationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EscalationLogs_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EscalationLogs_Users_NotifiedUserId",
                        column: x => x.NotifiedUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EscalationLogs_Users_ResolvedByUserId",
                        column: x => x.ResolvedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QRCodeRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    TrackingNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    QRCodeImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TrackingUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScanCount = table.Column<int>(type: "int", nullable: false),
                    LastScannedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QRCodeRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QRCodeRecords_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoutingSlips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    TrackingNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ReceivedByUserId = table.Column<int>(type: "int", nullable: false),
                    DateReceived = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ActionInstruction = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TargetDepartmentId = table.Column<int>(type: "int", nullable: true),
                    NotedByUserId = table.Column<int>(type: "int", nullable: true),
                    SlipStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutingSlips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutingSlips_Departments_TargetDepartmentId",
                        column: x => x.TargetDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RoutingSlips_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoutingSlips_Users_NotedByUserId",
                        column: x => x.NotedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RoutingSlips_Users_ReceivedByUserId",
                        column: x => x.ReceivedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SMSNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    RecipientUserId = table.Column<int>(type: "int", nullable: true),
                    RecipientNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RecipientName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MessageContent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TriggerEvent = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsDelivered = table.Column<bool>(type: "bit", nullable: false),
                    QueuedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GatewayResponse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMSNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SMSNotifications_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SMSNotifications_Users_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowTransitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    FromOffice = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ToOffice = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActionTaken = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PerformedByUserId = table.Column<int>(type: "int", nullable: false),
                    TransitionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StepNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowTransitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowTransitions_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkflowTransitions_Users_PerformedByUserId",
                        column: x => x.PerformedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrailEntries_DocumentId",
                table: "AuditTrailEntries",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrailEntries_UserId",
                table: "AuditTrailEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DepartmentCode",
                table: "Departments",
                column: "DepartmentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_HeadUserId",
                table: "Departments",
                column: "HeadUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalSignatures_DocumentId",
                table: "DigitalSignatures",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalSignatures_SignedByUserId",
                table: "DigitalSignatures",
                column: "SignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAttachments_DocumentId",
                table: "DocumentAttachments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentComments_DocumentId",
                table: "DocumentComments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentComments_UserId",
                table: "DocumentComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentMetadatas_DocumentId",
                table: "DocumentMetadatas",
                column: "DocumentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CreatedByUserId",
                table: "Documents",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CurrentStatus",
                table: "Documents",
                column: "CurrentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DateFiled",
                table: "Documents",
                column: "DateFiled");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DepartmentId",
                table: "Documents",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_SubmittedByUserId",
                table: "Documents",
                column: "SubmittedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_TrackingNumber",
                table: "Documents",
                column: "TrackingNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_TypeId",
                table: "Documents",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationLogs_DocumentId",
                table: "EscalationLogs",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationLogs_NotifiedUserId",
                table: "EscalationLogs",
                column: "NotifiedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationLogs_ResolvedByUserId",
                table: "EscalationLogs",
                column: "ResolvedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExportAuditLogs_UserId",
                table: "ExportAuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QRCodeRecords_DocumentId",
                table: "QRCodeRecords",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_QRCodeRecords_TrackingNumber",
                table: "QRCodeRecords",
                column: "TrackingNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportLogs_GeneratedByUserId",
                table: "ReportLogs",
                column: "GeneratedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutingSlips_DocumentId",
                table: "RoutingSlips",
                column: "DocumentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoutingSlips_NotedByUserId",
                table: "RoutingSlips",
                column: "NotedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutingSlips_ReceivedByUserId",
                table: "RoutingSlips",
                column: "ReceivedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutingSlips_TargetDepartmentId",
                table: "RoutingSlips",
                column: "TargetDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutingSlips_TrackingNumber",
                table: "RoutingSlips",
                column: "TrackingNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SMSNotifications_DocumentId",
                table: "SMSNotifications",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_SMSNotifications_RecipientUserId",
                table: "SMSNotifications",
                column: "RecipientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemNotifications_UserId",
                table: "SystemNotifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentId",
                table: "Users",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_AssignedDepartmentId",
                table: "WorkflowSteps",
                column: "AssignedDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_TypeId",
                table: "WorkflowSteps",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTransitions_DocumentId",
                table: "WorkflowTransitions",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTransitions_PerformedByUserId",
                table: "WorkflowTransitions",
                column: "PerformedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditTrailEntries_Documents_DocumentId",
                table: "AuditTrailEntries",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditTrailEntries_Users_UserId",
                table: "AuditTrailEntries",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Users_HeadUserId",
                table: "Departments",
                column: "HeadUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Users_HeadUserId",
                table: "Departments");

            migrationBuilder.DropTable(
                name: "AuditTrailEntries");

            migrationBuilder.DropTable(
                name: "DigitalSignatures");

            migrationBuilder.DropTable(
                name: "DocumentAttachments");

            migrationBuilder.DropTable(
                name: "DocumentComments");

            migrationBuilder.DropTable(
                name: "DocumentMetadatas");

            migrationBuilder.DropTable(
                name: "EscalationLogs");

            migrationBuilder.DropTable(
                name: "ExportAuditLogs");

            migrationBuilder.DropTable(
                name: "QRCodeRecords");

            migrationBuilder.DropTable(
                name: "ReportLogs");

            migrationBuilder.DropTable(
                name: "RoutingSlips");

            migrationBuilder.DropTable(
                name: "SMSNotifications");

            migrationBuilder.DropTable(
                name: "SystemNotifications");

            migrationBuilder.DropTable(
                name: "WorkflowSteps");

            migrationBuilder.DropTable(
                name: "WorkflowTransitions");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "DocumentTypeConfigs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
