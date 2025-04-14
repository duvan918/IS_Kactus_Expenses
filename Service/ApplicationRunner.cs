using ClosedXML.Excel;
using IS_Kactus_Expenses.Model;
using IS_Kactus_Expenses.Service.Interface;
using Microsoft.Extensions.Configuration;

namespace IS_Kactus_Expenses.Service
{
    public class ApplicationRunner
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public ApplicationRunner(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        public async Task UpdateUsersAsync()
        {
            try
            {
                int companyId = int.Parse(_configuration["ApplicationSettings:CompanyId"]!);

                int updatedUsers = await _userService.UpdateUsersAsync(companyId);

                Console.WriteLine($"Se actualizaron {updatedUsers} usuarios");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error durante la actualización: {ex.Message}");
            }
        }

        public async Task CreateUsersAsync(string filePath)
        {
            try
            {
                var employeeList = ReadEmployeeDataFromExcel(filePath);

                int companyId = int.Parse(_configuration["ApplicationSettings:CompanyId"]!);

                int createdUsers = await _userService.CreateUsersAsync(employeeList, companyId);

                Console.WriteLine($"Se crearon {createdUsers} de {employeeList.Count} usuarios desde el archivo Excel.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error durante la actualización: {ex.Message}");
            }
        }

        public async Task CloneUserConfigurationsAsync(int targetUserId, int masterUserId)
        {
            try
            {
                await _userService.CloneConfigurationsAsync(targetUserId, masterUserId);
                Console.WriteLine($"Se clonaron las configuraciones del usuario maestro {masterUserId} al usuario {targetUserId}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al clonar configuraciones: {ex.Message}");
            }
        }

        private List<EmployeeData> ReadEmployeeDataFromExcel(string filePath)
        {
            var employees = new List<EmployeeData>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1); // Asume que los datos están en la primera hoja
                var rows = worksheet.RangeUsed()!.RowsUsed().Skip(1); // Salta el encabezado

                foreach (var row in rows)
                {
                    var employee = new EmployeeData
                    {
                        Documento = row.Cell(1).GetString(),
                        Perfil = Utils.Utils.RoleMapping.TryGetValue(row.Cell(2).GetString(), out int perfil) 
                            ? perfil 
                            : throw new Exception($"El perfil '{row.Cell(3).GetString()}' no está mapeado.")
                    };
                    employees.Add(employee);
                }
            }

            return employees;
        }
    }
}
