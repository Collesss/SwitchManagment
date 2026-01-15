using SwitchManagment.API.Repository.Entities;

namespace SwitchManagment.API.Repository.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        public Task<IEnumerable<T>> GetAll();

        public Task<T> GetById(int id);

        public Task<int> Add(T add);

        public Task DeleteById(int id);
    }
}
