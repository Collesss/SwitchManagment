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

        public async Task<int> Add(SwitchEntity add)
        {
            var result = await _context.AddAsync(add);

            await _context.SaveChangesAsync();

            return result.Entity.Id;
        }

        public async Task DeleteById(int id)
        {
            _context.Remove(new SwitchEntity { Id = id });

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SwitchEntity>> GetAll() =>
            await _context.Switches.ToListAsync();

        public async Task<SwitchEntity> GetById(int id) =>
            await _context.Switches.FindAsync(id);
    }
}
