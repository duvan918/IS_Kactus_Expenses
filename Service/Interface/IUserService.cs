using IS_Kactus_Expenses.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Kactus_Expenses.Service.Interface
{
    public interface IUserService
    {
        Task<int> UpdateUsersAsync(int companyId);
        Task<int> CreateUsersAsync(IEnumerable<EmployeeData> employees, int companyId);


        Task CloneConfigurationsAsync(int targetUserId, int masterUserId);
    }

}
