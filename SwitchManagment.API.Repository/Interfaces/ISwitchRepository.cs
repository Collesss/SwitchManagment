using SwitchManagment.API.Repository.Entities;

namespace SwitchManagment.API.Repository.Interfaces
{
    public interface ISwitchRepository
    {
        public Task<IEnumerable<SwitchEntity>> GetAll(CancellationToken cancellationToken = default);

        public Task<SwitchEntity> GetById(int id, CancellationToken cancellationToken = default);

        public Task<int> Add(SwitchEntity switchEntity, CancellationToken cancellationToken = default);

        public Task DeleteById(int id, CancellationToken cancellationToken = default);
    }
}
