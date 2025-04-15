using IS_Kactus_Expenses.Model;
using IS_Kactus_Expenses.Service.Interface;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace IS_Kactus_Expenses.Service
{
    public class UserService : IUserService
    {
        private readonly IApiClient _apiClient;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public UserService(IApiClient apiClient, IUserRepository userRepository, IEmailService emailService)
        {
            _apiClient = apiClient;
            _userRepository = userRepository;
            _emailService = emailService;
        }


        public async Task<int> UpdateUsersAsync(int companyId)
        {
            int updatedUsers = 0;

            var users = await _userRepository.GetUsersByCompanyId(companyId);

            foreach (var user in users)
            {
                var responseJson = await _apiClient.GetUserDataAsync(user.Cedula!);
                if (string.IsNullOrEmpty(responseJson)) continue;

                var responseData = JsonSerializer.Deserialize<ResponseData>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (responseData?.Data == null) continue;

                UpdateUserFromData(user, responseData.Data);

                if (user.Grupo == 0 || user.Grupo == null)
                {
                    //Enviar correo
                    Console.WriteLine($"Grupo no encontrado para el valor: {user.Grupo} del documento {user.Cedula}");
                    continue;
                }
                
                var UsuarioMaestro = await _userRepository.GetMasterUserAsync((int)user.Grupo);
                if (UsuarioMaestro == null)
                {
                    //Enviar correo
                    Console.WriteLine($"No se encontró el usuario maestro para el grupo {user.Grupo} y el documento {user.Cedula}");
                    continue;
                }
                
                await CloneConfigurationsAsync(user.IdUsuario, UsuarioMaestro.IdUsuario);



                updatedUsers++;

                await _userRepository.UpdateUserAsync(user);
            }

            return updatedUsers;
        }

        private void UpdateUserFromData(Usuario user, DataReceived data)
        {
            //user.Cedula = data.CodEmp;
            user.NombreCompleto = data.NombreEmp1;
            user.RazonSocial = data.NombreEmp1;
            user.Correo = data.CorreoEmp;
            user.CiudadBase = data.CentroEmp;
            user.Celular = data.TelEmp;
            user.Observaciones = data.CargoEmp;
            user.IdPadrino = 0;                         //Revisar -> Ya debería existir el Supervisor por lo que hay que consultar usuario con ese perfil antes de... podría usarse codJefe
            user.BitEstado = data.EstadoEmp == "1";
        }


        public async Task<int> CreateUsersAsync(IEnumerable<EmployeeData> employees, int companyId)
        {
            int createdUsers = 0;

            foreach (var employee in employees)
            {

                var apiResponse = await _apiClient.GetUserDataAsync(employee.Documento);
                var userData = JsonSerializer.Deserialize<ResponseData>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })?.Data;
                if (userData == null) continue;

                Usuario? existingUser = await _userRepository.GetUserAsync(employee.Documento, companyId);
                if (existingUser != null) continue;


                var newUser = new Usuario
                {
                    IdCompania = companyId,
                    IdPerfil = employee.Perfil,
                    Cedula = userData.CodEmp,
                    Nit = userData.CodEmp,
                    Clave = userData.CodEmp + "Siesa",
                    NombreCompleto = userData.NombreEmp1,
                    RazonSocial = userData.NombreEmp1,
                    CiudadBase = userData.CentroEmp,
                    Correo = userData.CorreoEmp,
                    Direccion = "Cll 00 # 00 - 00",
                    Celular = userData.TelEmp,
                    Observaciones = userData.CargoEmp,
                    // Grupo = employee.Grupo,
                    // Grupo = userData.DivPers,
                    Grupo = Utils.Utils.DepartmentMapping.TryGetValue(userData.DivPers, out int div) 
                        ? div 
                        : throw new Exception($"Grupo no encontrado para el valor: {userData.DivPers} del documento {userData.CodEmp}"),
                    BitAprobacion = false,
                    IdPadrino = 0,                              //Revisar -> Ya debería existir el Supervisor por lo que hay que consultar usuario con ese perfil antes de... podría usarse codJefe
                    BitEstado = userData.EstadoEmp == "1"
                };

                await _userRepository.AddUserAsync(newUser);

                createdUsers++;




                //string subject = "Bienvenido a Siesa Expenses";
                //string body = $"Hola {newUser.NombreCompleto},<br/>Tu usuario ha sido creado exitosamente.<br/><br/>Usuario: {newUser.Nit}<br/>Contraseña: {newUser.Clave}";

                //await _emailService.SendEmailAsync(newUser.Correo, subject, body);

            }

            return createdUsers;
        }



        public async Task CloneConfigurationsAsync(int targetUserId, int masterUserId)
        {
            var masterConfigurations = await _userRepository.GetConfigurationsByUserIdAsync(masterUserId);

            await _userRepository.DeleteConfigurationsByUserIdAsync(targetUserId);

            foreach (var config in masterConfigurations)
            {
                var newConfig = new UsuarioConfiguracion
                {
                    IdUsuario = targetUserId,
                    IdCompania = config.IdCompania,
                    TipoDocumento = config.TipoDocumento,
                    CentroOperacion = config.CentroOperacion,
                    Servicios = config.Servicios,
                    UnidadNegocio = config.UnidadNegocio,
                    IdCondicionPago = config.IdCondicionPago,
                    Motivo = config.Motivo,
                    TipoProveedor = config.TipoProveedor,
                    CentroCostos = config.CentroCostos,
                    Moneda = config.Moneda
                };

                await _userRepository.AddConfigurationAsync(newConfig);
            }
        }
    }

}