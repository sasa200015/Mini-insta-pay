using Service3.DTO;
using Service3.Model;

namespace Service3.Interface
{
    public interface IReport
    {
        public Task<List<Report>> GetHistory(Guid userId);
        public Task<decimal> totalReceive(Guid userId);
        public Task<decimal> totalSent(Guid userId);
        public Task addReport(Report report);

        public Task Saveasync();
    }
}
