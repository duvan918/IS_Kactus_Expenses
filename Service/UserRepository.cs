using IS_Kactus_Expenses.Data;

namespace IS_Kactus_Expenses.Service
{
    using IS_Kactus_Expenses.Model;
    using IS_Kactus_Expenses.Service.Interface;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetUsersByCompanyId(int companyId)
        {
            return await _context.Usuarios.Where(u => u.IdCompania == companyId).ToListAsync();
        }

        public async Task UpdateUserAsync(Usuario user)
        {
            _context.Usuarios.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task AddUserAsync(Usuario user)
        {
            await _context.Usuarios.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario?> GetUserAsync(string codEmp, int companyId)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Cedula == codEmp && u.IdCompania == companyId);
        }
    }


}
