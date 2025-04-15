namespace IS_Kactus_Expenses.Utils;

public static class Utils
{
    public static readonly Dictionary<string, int> RoleMapping = new Dictionary<string, int>
    {
        { "Usuario", 3 },
        { "Supervisor", 6 },
        { "Aprobador", 5 }
    };

    public static readonly Dictionary<string, int> DepartmentMapping = new Dictionary<string, int>
    {
        { "DIGE", 196 },    //Dirección General
        { "FAYS", 195 },    //Farma
        { "RAYS", 194 }     //Clínica
    };

    public static readonly Dictionary<string, string> DepartmentEquivalence = new Dictionary<string, string>
    {
        { "DIGE", "DIRECCION GENERAL" },
        { "FAYS", "FARMA" },
        { "RAYS", "CLINICAS" }
    };
}
