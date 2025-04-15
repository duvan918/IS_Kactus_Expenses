using System.Text;
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

        public async Task AddUsersAsync(IEnumerable<Usuario> users)
        {
            await _context.Usuarios.AddRangeAsync(users);
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

        public async Task AddConfigurationsAsync(IEnumerable<UsuarioConfiguracion> configurations)
        {
            var sql = new StringBuilder("INSERT INTO Usuario_Configuracion (IdUsuario, IdCompania, Tipo_Documento, Centro_Operacion, Servicios, Unidad_Negocio, id_CondicionPago, Motivo, TipoProveedor, CentroCostos, Moneda) VALUES ");

            var parameters = new List<Microsoft.Data.SqlClient.SqlParameter>();
            int index = 0;

            foreach (var config in configurations)
            {
                sql.Append($"(@IdUsuario{index}, @IdCompania{index}, @TipoDocumento{index}, @CentroOperacion{index}, @Servicios{index}, @UnidadNegocio{index}, @IdCondicionPago{index}, @Motivo{index}, @TipoProveedor{index}, @CentroCostos{index}, @Moneda{index}),");

                parameters.Add(new Microsoft.Data.SqlClient.SqlParameter($"@IdUsuario{index}", config.IdUsuario ?? (object)DBNull.Value));
                parameters.Add(new Microsoft.Data.SqlClient.SqlParameter($"@IdCompania{index}", config.IdCompania ?? (object)DBNull.Value));
                parameters.Add(new Microsoft.Data.SqlClient.SqlParameter($"@TipoDocumento{index}", config.TipoDocumento ?? (object)DBNull.Value));
                parameters.Add(new Microsoft.Data.SqlClient.SqlParameter($"@CentroOperacion{index}", config.CentroOperacion ?? (object)DBNull.Value));
                parameters.Add(new Microsoft.Data.SqlClient.SqlParameter($"@Servicios{index}", config.Servicios ?? (object)DBNull.Value));
                parameters.Add(new Microsoft.Data.SqlClient.SqlParameter($"@UnidadNegocio{index}", config.UnidadNegocio ?? (object)DBNull.Value));
                parameters.Add(new Microsoft.Data.SqlClient.SqlParameter($"@IdCondicionPago{index}", config.IdCondicionPago ?? (object)DBNull.Value));
                parameters.Add(new Microsoft.Data.SqlClient.SqlParameter($"@Motivo{index}", config.Motivo ?? (object)DBNull.Value));
                parameters.Add(new Microsoft.Data.SqlClient.SqlParameter($"@TipoProveedor{index}", config.TipoProveedor ?? (object)DBNull.Value));
                parameters.Add(new Microsoft.Data.SqlClient.SqlParameter($"@CentroCostos{index}", config.CentroCostos ?? (object)DBNull.Value));
                parameters.Add(new Microsoft.Data.SqlClient.SqlParameter($"@Moneda{index}", config.Moneda ?? (object)DBNull.Value));

                index++;
            }

            sql.Length--; // Eliminar la última coma
            await _context.Database.ExecuteSqlRawAsync(sql.ToString(), parameters.ToArray());
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
