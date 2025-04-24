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

        { "RAYS", 194 },    //Clínica
        { "REOC", 194 },    //Clínica
        { "RECE", 194 },    //Clínica
        { "RENO", 194 },    //Clínica

        { "FANO", 195 },    //Farma
        { "FAYS", 195 },    //Farma
        { "FACE", 195 },    //Farma
        { "FOCE", 195 },    //Farma
    };

    public static readonly Dictionary<string, string> DepartmentEquivalence = new Dictionary<string, string>
    {
        { "DIGE", "DIRECCION GENERAL" },

        { "REOC", "CLINICAS" },
        { "RENO", "CLINICAS" },
        { "RECE", "CLINICAS" },
        { "RAYS", "CLINICAS" },

        { "FAYS", "FARMA" },
        { "FANO", "FARMA" },
        { "FACE", "FARMA" },
        { "FOCE", "FARMA" },

    };
}
