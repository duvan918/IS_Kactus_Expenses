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
                string responseJson;
                ResponseData? responseData;

                responseJson = await _apiClient.GetUserDataAsync(user.Cedula!);
                responseData = JsonSerializer.Deserialize<ResponseData>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if ((responseData?.NotificationDTO?.HttpStatus == "BAD_REQUEST" || responseData?.NotificationDTO?.HttpStatus != "OK") && responseData?.NotificationDTO.Message != "Respuesta correcta")
                {
                    responseJson = await _apiClient.GetUserDataAsync(user.Cedula!);
                    responseData = JsonSerializer.Deserialize<ResponseData>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                if (responseData?.Data == null) continue;

                Usuario? existingSupervisor = await _userRepository.GetUserAsync(responseData.Data.CodJef1, companyId);

                user.NombreCompleto = responseData.Data.NombreEmp1;
                user.RazonSocial = responseData.Data.NombreEmp1;
                user.Correo = responseData.Data.CorreoEmp;
                user.CiudadBase = responseData.Data.CentroEmp;
                user.Celular = responseData.Data.TelEmp;
                user.Observaciones = responseData.Data.CargoEmp;
                user.Grupo = Utils.Utils.DepartmentMapping.TryGetValue(responseData.Data.DivPers, out int div)
                    ? div
                    : throw new Exception($"Grupo no encontrado para el valor: {responseData.Data.DivPers} del documento {responseData.Data.CodEmp}");
                user.IdPadrino = user.IdPerfil == Utils.Utils.RoleMapping["Aprobador"] ? 0 : existingSupervisor?.IdUsuario ?? 0;
                user.BitEstado = responseData.Data.EstadoEmp == "1";

                if (user.Grupo == 0 || user.Grupo == null)
                {
                    // Enviar correo
                    Console.WriteLine($"Grupo no encontrado para el valor: {user.Grupo} del documento {user.Cedula}");
                    continue;
                }

                var UsuarioMaestro = await _userRepository.GetMasterUserAsync((int)user.Grupo);
                if (UsuarioMaestro == null)
                {
                    // Enviar correo
                    Console.WriteLine($"No se encontró el usuario maestro para el grupo {user.Grupo} y el documento {user.Cedula}");
                    continue;
                }

                await CloneConfigurationsAsync(user.IdUsuario, UsuarioMaestro.IdUsuario);

                updatedUsers++;

                await _userRepository.UpdateUserAsync(user);
            }

            return updatedUsers;
        }


        public async Task<int> CreateUsersAsync(IEnumerable<EmployeeData> employees, int companyId)
        {
            int createdUsers = 0;

            var groupedEmployees = employees
                .GroupBy(e => e.Perfil)
                .OrderByDescending(g => g.Key);

            foreach (var group in groupedEmployees)
            {
                var newUsers = new List<Usuario>();

                foreach (var employee in group)
                {
                    string apiResponse;
                    ResponseData? userData;

                    apiResponse = await _apiClient.GetUserDataAsync(employee.Documento);
                    userData = JsonSerializer.Deserialize<ResponseData>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if ((userData?.NotificationDTO?.HttpStatus == "BAD_REQUEST" || userData?.NotificationDTO?.HttpStatus != "OK") && userData?.NotificationDTO.Message != "Respuesta correcta")
                    {
                        apiResponse = await _apiClient.GetUserDataAsync(employee.Documento);
                        userData = JsonSerializer.Deserialize<ResponseData>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }

                    if (userData?.Data == null) continue;

                    Usuario? existingUser = await _userRepository.GetUserAsync(employee.Documento, companyId);
                    if (existingUser != null) continue;

                    Usuario? existingSupervisor = await _userRepository.GetUserAsync(userData.Data.CodJef1, companyId);

                    var newUser = new Usuario
                    {
                        IdCompania = companyId,
                        IdPerfil = employee.Perfil,
                        Cedula = userData.Data.CodEmp,
                        Nit = userData.Data.CodEmp,
                        Clave = userData.Data.CodEmp + "Siesa",
                        NombreCompleto = userData.Data.NombreEmp1,
                        RazonSocial = userData.Data.NombreEmp1,
                        CiudadBase = userData.Data.CentroEmp,
                        Correo = userData.Data.CorreoEmp,
                        Direccion = "Cll 00 # 00 - 00",
                        Celular = userData.Data.TelEmp,
                        Observaciones = userData.Data.CargoEmp,
                        Grupo = Utils.Utils.DepartmentMapping.TryGetValue(userData.Data.DivPers, out int div)
                            ? div
                            : throw new Exception($"Grupo no encontrado para el valor: {userData.Data.DivPers} del documento {userData.Data.CodEmp}"),
                        BitAprobacion = false,
                        IdPadrino = employee.Perfil == Utils.Utils.RoleMapping["Aprobador"] ? 0 : existingSupervisor?.IdUsuario ?? 0,
                        BitEstado = userData.Data.EstadoEmp == "1",
                        Cupo = 5000000,
                    };

                    newUsers.Add(newUser);
                    createdUsers++;
                }

                if (newUsers.Any())
                {
                    await _userRepository.AddUsersAsync(newUsers);
                }
            }

            return createdUsers;
        }



        public async Task CloneConfigurationsAsync(int targetUserId, int masterUserId)
        {
            var masterConfigurations = await _userRepository.GetConfigurationsByUserIdAsync(masterUserId);

            await _userRepository.DeleteConfigurationsByUserIdAsync(targetUserId);

            var newConfigurations = new List<UsuarioConfiguracion>();

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

                newConfigurations.Add(newConfig);
            }

            if (newConfigurations.Any())
            {
                await _userRepository.AddConfigurationsAsync(newConfigurations);
            }
        }
    }

}