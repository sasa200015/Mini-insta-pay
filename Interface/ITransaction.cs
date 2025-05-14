using Service2.Model;

namespace Service2.Interface
{
    public interface ITransaction
    {
        public void Add(Transactions transaction);
        public void Save();
        public void Saveasync();
        public Task<Transactions> getById(int id);
        public Task<List<Transactions>> getTransaction(Guid userId);
    }
}
