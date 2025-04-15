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


        public async Task<IEnumerable<UsuarioConfiguracion>> GetConfigurationsByUserIdAsync(int userId)
        {
            return await _context.UsuarioConfiguraciones.Where(c => c.IdUsuario == userId).ToListAsync();
        }

        public async Task AddConfigurationAsync(UsuarioConfiguracion configuration)
        {
            // await _context.UsuarioConfiguraciones.AddAsync(configuration);
            // await _context.SaveChangesAsync();

            // Insertar usando SQL directo debido a que la entidad no tiene llave primaria
            // y no se puede usar AddAsync directamente.
            var sql = @"
                INSERT INTO Usuario_Configuracion 
                (IdUsuario, IdCompania, Tipo_Documento, Centro_Operacion, Servicios, Unidad_Negocio, id_CondicionPago, Motivo, TipoProveedor, CentroCostos, Moneda)
                VALUES 
                (@IdUsuario, @IdCompania, @TipoDocumento, @CentroOperacion, @Servicios, @UnidadNegocio, @IdCondicionPago, @Motivo, @TipoProveedor, @CentroCostos, @Moneda)";

            await _context.Database.ExecuteSqlRawAsync(sql, new[]
            {
                new Microsoft.Data.SqlClient.SqlParameter("@IdUsuario", configuration.IdUsuario ?? (object)DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@IdCompania", configuration.IdCompania ?? (object)DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@TipoDocumento", configuration.TipoDocumento ?? (object)DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@CentroOperacion", configuration.CentroOperacion ?? (object)DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@Servicios", configuration.Servicios ?? (object)DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@UnidadNegocio", configuration.UnidadNegocio ?? (object)DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@IdCondicionPago", configuration.IdCondicionPago ?? (object)DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@Motivo", configuration.Motivo ?? (object)DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@TipoProveedor", configuration.TipoProveedor ?? (object)DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@CentroCostos", configuration.CentroCostos ?? (object)DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@Moneda", configuration.Moneda ?? (object)DBNull.Value)
            });
        }

        public async Task DeleteConfigurationsByUserIdAsync(int userId)
        {
            // var configurations = _context.UsuarioConfiguraciones.Where(c => c.IdUsuario == userId);
            // _context.UsuarioConfiguraciones.RemoveRange(configurations);
            // await _context.SaveChangesAsync();

            // Eliminar usando SQL directo debido a que la entidad no tiene llave primaria
            // y no se puede usar RemoveRange directamente.
            var sql = @"
                DELETE FROM Usuario_Configuracion
                WHERE IdUsuario = @IdUsuario";

            await _context.Database.ExecuteSqlRawAsync(sql, new[]
            {
                new Microsoft.Data.SqlClient.SqlParameter("@IdUsuario", userId)
            });
        }

        public async Task<Usuario?> GetMasterUserAsync(int grupoId)
        {
            var grupoMapName = Utils.Utils.DepartmentMapping.FirstOrDefault(x => x.Value == grupoId).Key;
            var grupoName = Utils.Utils.DepartmentEquivalence.TryGetValue(grupoMapName, out string? grupo) ? grupo : throw new Exception($"Grupo no encontrado para el valor: {grupoMapName}");
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Nit == grupoName && u.Grupo == grupoId);
        }

    }
}
