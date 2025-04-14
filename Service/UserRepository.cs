using IS_Kactus_Expenses.Data;
using IS_Kactus_Expenses.Model;
using IS_Kactus_Expenses.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace IS_Kactus_Expenses.Service
{
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






        // Métodos para Usuario_Configuracion
        public async Task<IEnumerable<UsuarioConfiguracion>> GetConfigurationsByUserIdAsync(int userId)
        {
            return await _context.UsuarioConfiguraciones.Where(c => c.IdUsuario == userId).ToListAsync();
        }

        public async Task AddConfigurationAsync(UsuarioConfiguracion configuration)
        {
            await _context.UsuarioConfiguraciones.AddAsync(configuration);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteConfigurationsByUserIdAsync(int userId)
        {
            var configurations = _context.UsuarioConfiguraciones.Where(c => c.IdUsuario == userId);
            _context.UsuarioConfiguraciones.RemoveRange(configurations);
            await _context.SaveChangesAsync();
        }

        // Obtener usuarios maestros
        public async Task<IEnumerable<Usuario>> GetMasterUsersAsync(int idPerfil, int grupo)
        {
            return await _context.Usuarios.Where(u => u.IdPerfil == idPerfil && u.Grupo == grupo).ToListAsync();
        }
    }
}
