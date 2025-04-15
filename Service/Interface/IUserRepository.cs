using IS_Kactus_Expenses.Model;

namespace IS_Kactus_Expenses.Service.Interface
{
    public interface IUserRepository
    {
        Task<IEnumerable<Usuario>> GetUsersByCompanyId(int companyId);
        Task UpdateUserAsync(Usuario user);
        Task AddUserAsync(Usuario user);
        Task<Usuario?> GetUserAsync(string codEmp, int companyId);


        Task<IEnumerable<UsuarioConfiguracion>> GetConfigurationsByUserIdAsync(int userId);
        Task AddConfigurationAsync(UsuarioConfiguracion configuration);
        Task DeleteConfigurationsByUserIdAsync(int userId);

        Task<Usuario?> GetMasterUserAsync(int grupoId);
    }
}
