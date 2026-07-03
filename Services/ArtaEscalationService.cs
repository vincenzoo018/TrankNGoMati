using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrackNGoMati.Models;

namespace TrackNGoMati.Services
{
    public class ArtaEscalationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ArtaEscalationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessEscalations();
                
                // Run every 1 hour
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task ProcessEscalations()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TrackNgoDbContext>();
            
            // Get documents that are not completed, rejected, or locked
            var activeDocs = await context.Documents
                .Where(d => d.CurrentStatus == 1 && !d.IsLocked && d.CurrentStepIndex < 5)
                .ToListAsync();

            foreach (var doc in activeDocs)
            {
                var elapsedDays = CalculateBusinessDays(doc.DateFiled, DateTime.Now);
                
                int currentThreshold = doc.IsUrgent ? Math.Max(1, doc.ArtaprocessingDays / 2) : doc.ArtaprocessingDays;
                
                // ARTA Breach condition
                if (elapsedDays > currentThreshold && !doc.IsEscalated)
                {
                    doc.IsEscalated = true;
                    
                    // Log the escalation
                    context.EscalationLogs.Add(new EscalationLog
                    {
                        DocumentId = doc.Id,
                        EscalatedAt = DateTime.Now,
                        EscalationReason = $"SLA Breach: Document has been processing for {elapsedDays} days, exceeding the ARTA limit of {doc.ArtaprocessingDays} days.",
                        IsResolved = false,
                        ViolatingOffice = doc.CurrentOfficeName,
                        Artathreshold = doc.ArtaprocessingDays,
                        ArtaperiodDays = doc.ArtaprocessingDays,
                        ElapsedDays = elapsedDays,
                        EscalationLevel = "Warning"
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        private int CalculateBusinessDays(DateTime start, DateTime end)
        {
            int days = 0;
            DateTime current = start.Date;
            
            while (current <= end.Date)
            {
                if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
                {
                    days++;
                }
                current = current.AddDays(1);
            }
            return days;
        }
    }
}
