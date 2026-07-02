using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TrackNGoMati.Models;

namespace TrackNGoMati.Data
{
    public static class DbInitializer
    {
        private static string Hash(string pw) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(pw))).ToLower();

        public static void Initialize(TrackNgoDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any()) return; // Already seeded

            // ── Departments ──────────────────────────────────────────────
            var depts = new[]
            {
                new Department { DepartmentCode = "OOM",    DepartmentName = "Office of the Mayor" },
                new Department { DepartmentCode = "ADMIN",  DepartmentName = "Executive Admin Office" },
                new Department { DepartmentCode = "HRMO",   DepartmentName = "Human Resource Mgmt Office" },
                new Department { DepartmentCode = "ENG",    DepartmentName = "City Engineering Office" },
                new Department { DepartmentCode = "FIN",    DepartmentName = "City Treasurer / Finance" },
                new Department { DepartmentCode = "LEGAL",  DepartmentName = "City Legal Office" },
                new Department { DepartmentCode = "CDRR",   DepartmentName = "CDRRMO" },
                new Department { DepartmentCode = "RECORDS", DepartmentName = "Records & Receiving Office" },
                new Department { DepartmentCode = "CART",   DepartmentName = "CART Compliance Office" },
            };
            context.Departments.AddRange(depts);
            context.SaveChanges();

            var oom     = depts.First(d => d.DepartmentCode == "OOM");
            var adminDept = depts.First(d => d.DepartmentCode == "ADMIN");
            var hrmo    = depts.First(d => d.DepartmentCode == "HRMO");
            var eng     = depts.First(d => d.DepartmentCode == "ENG");
            var fin     = depts.First(d => d.DepartmentCode == "FIN");
            var records = depts.First(d => d.DepartmentCode == "RECORDS");
            var cart    = depts.First(d => d.DepartmentCode == "CART");

            // ── Document Types (with ARTA tiers) ────────────────────────
            var docTypes = new[]
            {
                new DocumentTypeConfig { TypeName = "Leave Application",          Description = "Personnel leave requests",                   DefaultProcessingDays = 3  },
                new DocumentTypeConfig { TypeName = "Travel Order",               Description = "Official travel authorization",               DefaultProcessingDays = 3  },
                new DocumentTypeConfig { TypeName = "Memorandum",                 Description = "Internal communications and directives",     DefaultProcessingDays = 3  },
                new DocumentTypeConfig { TypeName = "Letter",                     Description = "Official correspondence",                     DefaultProcessingDays = 3  },
                new DocumentTypeConfig { TypeName = "Endorsement",               Description = "Document forwarding with recommendation",    DefaultProcessingDays = 7  },
                new DocumentTypeConfig { TypeName = "Procurement Request",        Description = "Purchase and procurement requests",          DefaultProcessingDays = 7  },
                new DocumentTypeConfig { TypeName = "Executive Order",            Description = "Official executive directives from the Mayor", DefaultProcessingDays = 7  },
                new DocumentTypeConfig { TypeName = "Resolution",                 Description = "Legislative resolutions",                    DefaultProcessingDays = 7  },
                new DocumentTypeConfig { TypeName = "Building Permit",            Description = "Construction and renovation permits",        DefaultProcessingDays = 20 },
                new DocumentTypeConfig { TypeName = "Business Permit",            Description = "Business license and permit applications",   DefaultProcessingDays = 20 },
                new DocumentTypeConfig { TypeName = "Financial Report",           Description = "Fiscal and accounting reports",              DefaultProcessingDays = 20 },
                new DocumentTypeConfig { TypeName = "Research Incentive",         Description = "Applications for educational research grants", DefaultProcessingDays = 20 },
            };
            context.DocumentTypeConfigs.AddRange(docTypes);
            context.SaveChanges();

            var pw   = Hash("password123");
            var pwEx = Hash("export@TrackNGo2026"); // export password

            // ── Users (one per role + extras) ───────────────────────────
            var mayor = new User
            {
                Username = "mayor@mati.gov.ph", PasswordHash = pw, ExportPasswordHash = pwEx,
                FullName = "Hon. Mayor Alberto Cruz", Email = "mayor@mati.gov.ph",
                MobileNumber = "09171234567", Role = 4, Department = oom.DepartmentName,
                DepartmentId = oom.Id, IsActive = true, CreatedAt = DateTime.Now
            };
            var admin = new User
            {
                Username = "admin@mati.gov.ph", PasswordHash = pw, ExportPasswordHash = pwEx,
                FullName = "Maria Santos", Email = "admin@mati.gov.ph",
                MobileNumber = "09181234567", Role = 1, Department = adminDept.DepartmentName,
                DepartmentId = adminDept.Id, IsActive = true, CreatedAt = DateTime.Now
            };
            var receivingClerk = new User
            {
                Username = "records@mati.gov.ph", PasswordHash = pw, ExportPasswordHash = pwEx,
                FullName = "Juan Dela Cruz", Email = "records@mati.gov.ph",
                MobileNumber = "09191234567", Role = 2, Department = records.DepartmentName,
                DepartmentId = records.Id, IsActive = true, CreatedAt = DateTime.Now
            };
            var deptHead = new User
            {
                Username = "hrmo.head@mati.gov.ph", PasswordHash = pw, ExportPasswordHash = pwEx,
                FullName = "Dr. Ana Reyes", Email = "hrmo.head@mati.gov.ph",
                MobileNumber = "09201234567", Role = 3, Department = hrmo.DepartmentName,
                DepartmentId = hrmo.Id, IsActive = true, CreatedAt = DateTime.Now
            };
            var cartOfficer = new User
            {
                Username = "cart@mati.gov.ph", PasswordHash = pw, ExportPasswordHash = pwEx,
                FullName = "Atty. Carlos Mendoza", Email = "cart@mati.gov.ph",
                MobileNumber = "09211234567", Role = 5, Department = cart.DepartmentName,
                DepartmentId = cart.Id, IsActive = true, CreatedAt = DateTime.Now
            };
            var engHead = new User
            {
                Username = "eng.head@mati.gov.ph", PasswordHash = pw, ExportPasswordHash = pwEx,
                FullName = "Engr. Pedro Lim", Email = "eng.head@mati.gov.ph",
                MobileNumber = "09221234567", Role = 3, Department = eng.DepartmentName,
                DepartmentId = eng.Id, IsActive = true, CreatedAt = DateTime.Now
            };

            var allUsers = new[] { mayor, admin, receivingClerk, deptHead, cartOfficer, engHead };
            context.Users.AddRange(allUsers);
            context.SaveChanges();

            // Assign dept heads
            hrmo.HeadUserId = deptHead.Id;
            eng.HeadUserId  = engHead.Id;
            oom.HeadUserId  = mayor.Id;
            context.SaveChanges();

            // ── Sample Documents ─────────────────────────────────────────
            var leaveType  = docTypes.First(t => t.TypeName == "Leave Application");
            var travelType = docTypes.First(t => t.TypeName == "Travel Order");
            var eoType     = docTypes.First(t => t.TypeName == "Executive Order");
            var procType   = docTypes.First(t => t.TypeName == "Procurement Request");

            var docs = new[]
            {
                // Completed document
                new Document
                {
                    TrackingNumber = "TNG-2026-0001", Title = "Annual Leave Application - J. Dela Cruz",
                    DocumentType = leaveType.TypeName, TypeId = leaveType.Id,
                    OriginatingDepartment = hrmo.DepartmentName, DepartmentId = hrmo.Id,
                    SubmittedBy = "Juan Dela Cruz", ContactNumber = "09191234567",
                    CurrentStatus = 3, CurrentOfficeName = oom.DepartmentName,
                    CurrentStepIndex = 5, TotalSteps = 5,
                    DateFiled = DateTime.Now.AddDays(-5), DateCompleted = DateTime.Now.AddDays(-1),
                    LastUpdated = DateTime.Now.AddDays(-1),
                    ArtaprocessingDays = leaveType.DefaultProcessingDays,
                    IsEscalated = false, IsLocked = false, CreatedByUserId = receivingClerk.Id
                },
                // In Mayor approval queue
                new Document
                {
                    TrackingNumber = "TNG-2026-0002", Title = "Travel Order - Manila Conference 2026",
                    DocumentType = travelType.TypeName, TypeId = travelType.Id,
                    OriginatingDepartment = eng.DepartmentName, DepartmentId = eng.Id,
                    SubmittedBy = "Engr. Pedro Lim", ContactNumber = "09221234567",
                    CurrentStatus = 2, CurrentOfficeName = oom.DepartmentName,
                    CurrentStepIndex = 4, TotalSteps = 5,
                    DateFiled = DateTime.Now.AddDays(-2),
                    LastUpdated = DateTime.Now.AddDays(-1),
                    ArtaprocessingDays = travelType.DefaultProcessingDays,
                    IsEscalated = false, IsLocked = false, CreatedByUserId = receivingClerk.Id
                },
                // Overdue / escalated (for CART demo)
                new Document
                {
                    TrackingNumber = "TNG-2026-0003", Title = "Procurement Request - IT Equipment HRMO",
                    DocumentType = procType.TypeName, TypeId = procType.Id,
                    OriginatingDepartment = hrmo.DepartmentName, DepartmentId = hrmo.Id,
                    SubmittedBy = "Dr. Ana Reyes", ContactNumber = "09201234567",
                    CurrentStatus = 1, CurrentOfficeName = hrmo.DepartmentName,
                    CurrentStepIndex = 1, TotalSteps = 5,
                    DateFiled = DateTime.Now.AddDays(-12),
                    LastUpdated = DateTime.Now.AddDays(-12),
                    ArtaprocessingDays = procType.DefaultProcessingDays, // 7 days limit
                    IsEscalated = true, IsLocked = true, CreatedByUserId = receivingClerk.Id
                },
                // Pending at Dept Head
                new Document
                {
                    TrackingNumber = "TNG-2026-0004", Title = "Executive Order No. 12 - Mati City Clean Drive",
                    DocumentType = eoType.TypeName, TypeId = eoType.Id,
                    OriginatingDepartment = oom.DepartmentName, DepartmentId = oom.Id,
                    SubmittedBy = "Admin Office", ContactNumber = "09181234567",
                    CurrentStatus = 1, CurrentOfficeName = eng.DepartmentName,
                    CurrentStepIndex = 2, TotalSteps = 5,
                    DateFiled = DateTime.Now.AddDays(-1),
                    LastUpdated = DateTime.Now.AddDays(-1),
                    ArtaprocessingDays = eoType.DefaultProcessingDays,
                    IsEscalated = false, IsLocked = false, CreatedByUserId = receivingClerk.Id
                },
                // Returned doc
                new Document
                {
                    TrackingNumber = "TNG-2026-0005", Title = "Leave Application - P. Garcia (Incomplete Requirements)",
                    DocumentType = leaveType.TypeName, TypeId = leaveType.Id,
                    OriginatingDepartment = fin.DepartmentName, DepartmentId = fin.Id,
                    SubmittedBy = "Pedro Garcia", ContactNumber = "09351234567",
                    CurrentStatus = 4, CurrentOfficeName = hrmo.DepartmentName,
                    CurrentStepIndex = 2, TotalSteps = 5,
                    DateFiled = DateTime.Now.AddDays(-3),
                    LastUpdated = DateTime.Now.AddDays(-1),
                    ArtaprocessingDays = leaveType.DefaultProcessingDays,
                    IsEscalated = false, IsLocked = false, CreatedByUserId = receivingClerk.Id
                }
            };
            context.Documents.AddRange(docs);
            context.SaveChanges();

            // ── Seed Audit Trail entries ─────────────────────────────────
            context.AuditTrailEntries.AddRange(
                new AuditTrailEntry { Document = docs[0], User = receivingClerk, Action = "Registered",  Timestamp = docs[0].DateFiled, Ipaddress = "192.168.1.10", Details = "Document registered." },
                new AuditTrailEntry { Document = docs[0], User = deptHead,       Action = "Endorsed",    Timestamp = docs[0].DateFiled.AddHours(2), Ipaddress = "192.168.1.11", Details = "Endorsed to Mayor." },
                new AuditTrailEntry { Document = docs[0], User = mayor,          Action = "Approved",    Timestamp = docs[0].DateFiled.AddDays(1), Ipaddress = "192.168.1.12", Details = "Digitally signed & approved." },
                new AuditTrailEntry { Document = docs[1], User = receivingClerk, Action = "Registered",  Timestamp = docs[1].DateFiled, Ipaddress = "192.168.1.10", Details = "Document registered." },
                new AuditTrailEntry { Document = docs[2], User = receivingClerk, Action = "Registered",  Timestamp = docs[2].DateFiled, Ipaddress = "192.168.1.10", Details = "Document registered." },
                new AuditTrailEntry { Document = docs[2], User = cartOfficer,    Action = "Escalated",   Timestamp = DateTime.Now.AddDays(-3), Ipaddress = "192.168.1.15", Details = "ARTA limit exceeded. Auto-escalated." }
            );
            context.SaveChanges();

            // ── Seed Escalation Log ──────────────────────────────────────
            context.EscalationLogs.Add(new EscalationLog
            {
                DocumentId = docs[2].Id, EscalationLevel = "Overdue",
                EscalationReason = "Exceeded 7-day ARTA processing limit.",
                ArtaperiodDays = 7, ElapsedDays = 12,
                Artathreshold = 7, ViolatingOffice = hrmo.DepartmentName,
                EscalatedAt = DateTime.Now.AddDays(-3), IsResolved = false,
                NotifiedUserId = cartOfficer.Id
            });
            context.SaveChanges();

            // ── Seed SMS Notifications ───────────────────────────────────
            context.Smsnotifications.AddRange(
                new Smsnotification
                {
                    DocumentId = docs[0].Id, RecipientNumber = "09191234567", RecipientName = "Juan Dela Cruz",
                    MessageContent = "TrackNGo: Your Leave Application (TNG-2026-0001) has been Approved. Claim at Mayor's Office.",
                    TriggerEvent = "Approved", SentAt = docs[0].DateFiled.AddDays(1), IsDelivered = true, GatewayResponse = "MOCK_OK"
                },
                new Smsnotification
                {
                    DocumentId = docs[2].Id, RecipientNumber = "09211234567", RecipientName = "Atty. Carlos Mendoza",
                    MessageContent = "ARTA ALERT: TNG-2026-0003 exceeded 7-day limit. Elapsed: 12 days. Auto-escalated.",
                    TriggerEvent = "ARTAEscalation", SentAt = DateTime.Now.AddDays(-3), IsDelivered = true, GatewayResponse = "MOCK_OK"
                }
            );
            context.SaveChanges();
        }
    }
}
