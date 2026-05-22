using Oracle.ManagedDataAccess.Client;

var cs = "User Id=RM556013;Password=170305;Data Source=oracle.fiap.com.br:1521/ORCL;";
using var conn = new OracleConnection(cs);
conn.Open();
Console.WriteLine("Conectado ao Oracle FIAP.");

string[] tables = { "CONTRATACOES", "CLIENTES", "FILIAIS", "SERVICOS", "__EFMigrationsHistory" };

foreach (var t in tables)
{
    try
    {
        using var cmd = new OracleCommand($"DROP TABLE \"{t}\" CASCADE CONSTRAINTS PURGE", conn);
        cmd.ExecuteNonQuery();
        Console.WriteLine($"  ok  DROP TABLE {t}");
    }
    catch (OracleException ex) when (ex.Number == 942)
    {
        Console.WriteLine($"  --  {t} não existe");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  ERR {t}: {ex.Message}");
    }
}

Console.WriteLine("Limpeza concluída.");
