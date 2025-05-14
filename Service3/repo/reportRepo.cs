using Microsoft.EntityFrameworkCore;
using Service3.Interface;
using Service3.Model;

namespace Service3.repo
{
    public class reportRepo: IReport
    {
        private readonly ProjectContext context;

        public reportRepo(ProjectContext context)
        {
            this.context = context;
        }
        public async Task<List<Report>> GetHistory(Guid userId)
        {
            return await context.Report
                .Where(r => r.SenderId == userId || r.ReceiverId == userId)
                .OrderByDescending(r => r.transactionDate)
                .ToListAsync();
        }
        public async Task<decimal> totalSent(Guid userId)
        {
           return await context.Report
                .Where(r => r.SenderId == userId && r.transactionType == "Send")
                .SumAsync(r => r.amount);            
        }
        public async Task<decimal> totalReceive(Guid userId)
        {
            return await context.Report
                .Where(r => r.ReceiverId == userId && r.transactionType == "Receive")
                .SumAsync(r => r.amount);
        }
        public async Task addReport(Report report)
        {
           await context.Report.AddAsync(report);
        }
        public async Task Saveasync()
        {
           await context.SaveChangesAsync();
        }
    }
}
