using IS_Kactus_Expenses.Model;
using IS_Kactus_Expenses.Service.Interface;
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
                //var responseJson = "{\"data\":{\"codEmp\":\"10198901245\",\"estadoEmp\":\"1\",\"statusOcupa\":\"\",\"idPersonal\":\"13\",\"nombreEmp1\":\"RIVERA ALBA LUISA FERNANDA\",\"nombreEmp2\":null,\"apeEmp1\":null,\"apeEmp2\":null,\"nombreJef1\":\"RIVERA ALBA LUISA FERNANDA\",\"nombreJef2\":null,\"apeJef1\":null,\"apeJef2\":null,\"codJef2\":\"\",\"codJef1\":\"10198901245\",\"correoEmp\":\"prueba123@mail.com\",\"cateHospe\":\"0005\",\"tipoHospe\":\"002\",\"pos1\":\"AUXILIAR DE SISTEMAS DE INFORMACION\",\"pos2\":\"\",\"iniVal\":\"\",\"finVal\":\"\",\"claseAbsentismo\":\"\",\"fechaNaciEmpl\":\"1993/03/13\",\"cargoEmp\":\"DIRECTORA DE TECNOLOGIA\",\"centroEmp\":\"BOGOTA\",\"telEmp\":\"1239876\",\"correoJef\":\"prueba123@mail.com\",\"cargoJef\":\"DIRECTORA DE TECNOLOGIA\",\"divPers\":\"DIGE\",\"subDivPers\":\"GERENCIA DE OPERACIONES\",\"token\":\"27y9HS4nWSHa5gE4nDsuGNqvtDz6Tzi1|KHDvspMbRAxUtRP4neSWhe/evZdIHkfH|BOBDeWalQlU=\",\"changeToken\":\"N\"},\"notificationDTO\":{\"httpstatus\":\"OK\",\"message\":\"Respuesta correcta\"}}";
                var responseJson = await _apiClient.GetUserDataAsync(user.Cedula!);
                if (string.IsNullOrEmpty(responseJson)) continue;

                var responseData = JsonSerializer.Deserialize<ResponseData>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (responseData?.Data == null) continue;

                UpdateUserFromData(user, responseData.Data);

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
                    Nit = userData.CodEmp,                      //Negociar con el cliente la estructura para usuario
                    Clave = userData.CodEmp + "Siesa",          //Negociar con el cliente la estructura para clave
                    NombreCompleto = userData.NombreEmp1,
                    RazonSocial = userData.NombreEmp1,
                    CiudadBase = userData.CentroEmp,
                    Correo = userData.CorreoEmp,
                    Direccion = "Cll 00 # 00 - 00",
                    Celular = userData.TelEmp,
                    Observaciones = userData.CargoEmp,
                    Grupo = employee.Grupo,
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
    }

}