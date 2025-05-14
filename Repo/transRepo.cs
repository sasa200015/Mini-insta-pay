using Microsoft.EntityFrameworkCore;
using Service2.Interface;
using Service2.Model;

namespace Service2.Repo
{
    public class transRepo: ITransaction
    {
        private readonly Project_Context context;

        public transRepo(Project_Context context)
        {
            this.context = context;
        }
        public void Add(Transactions transaction)
        {
            context.Transactions.Add(transaction);
        }
        public async  Task<List<Transactions>> getTransaction(Guid userId)
        {
          return await context.Transactions
                .Where(t => t.senedrId == userId || t.receiverId == userId)
                .ToListAsync();
        }
        public async Task<Transactions> getById(int id)
        {
            return await context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        public void Save()
        {
            context.SaveChanges();
        }
        public void Saveasync()
        {
            context.SaveChangesAsync();
        }
    }

}
