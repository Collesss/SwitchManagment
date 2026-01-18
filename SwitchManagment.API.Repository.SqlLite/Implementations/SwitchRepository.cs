using Microsoft.EntityFrameworkCore;
using SwitchManagment.API.Repository.Entities;
using SwitchManagment.API.Repository.Interfaces;

namespace SwitchManagment.API.Repository.SqlLite.Implementations
{
    public class SwitchRepository : ISwitchRepository
    {
        private readonly ApplicationContext _context;

        public SwitchRepository(ApplicationContext context) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> Add(SwitchEntity switchEntity, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _context.AddAsync(switchEntity, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return result.Entity.Id;
            }
            catch(DbUpdateException e)
            {
                throw;
            }
        }

        public async Task DeleteById(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Remove(new SwitchEntity { Id = id });

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException e) 
            {
                throw;
            }
        }

        public async Task<IEnumerable<SwitchEntity>> GetAll(CancellationToken cancellationToken = default) =>
            await _context.Switches.ToListAsync(cancellationToken);

        public async Task<SwitchEntity> GetById(int id, CancellationToken cancellationToken = default) =>
            await _context.Switches.FindAsync([id], cancellationToken);
    }
}
