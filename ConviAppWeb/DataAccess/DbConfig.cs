using System.IO;

namespace ConviAppWeb.DataAccess
{
    public static class DbConfig
    {
        // Ruta al archivo SQLite en el mismo directorio que el ejecutable
        public static string ConnectionString =>
            $"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conviapp.db")};Version=3;";
    }
}
