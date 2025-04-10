using IS_Kactus_Expenses.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Kactus_Expenses.Service.Interface
{
    public interface IApiClient
    {
        Task<string> GetUserDataAsync(string userCode);
    }

}
