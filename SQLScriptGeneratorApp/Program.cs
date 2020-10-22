using System;
using System.Linq;
using System.IO;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using SMO = Microsoft.SqlServer.Management.Smo;

namespace SQLScriptGeneratorApp
{
    class Program
    {
        static void Main()
        {
            var fileName         = @"C:\Zaki\Citius Tech\Study\DotNet\Core\Console\SQLScriptGeneratorApp\SQLScriptGeneratorApp\backup.sql";
            var connectionString = $"Data Source=ZAKIS-MSD\\SQLSERVER2017; Database=foo_db; User ID=sa; Password=password_123;";
            var databaseName     = "foo_db";
            var schemaName       = "dbo";

            if (File.Exists(fileName))
                File.Delete(fileName);

            try
            {
                var server    = new SMO.Server(new ServerConnection(new SqlConnection(connectionString)));
                var options   = new SMO.ScriptingOptions();
                var databases = server.Databases[databaseName];

                options.FileName                = fileName;
                options.EnforceScriptingOptions = true;
                options.WithDependencies        = true;
                options.IncludeHeaders          = true;
                options.ScriptDrops             = false;
                options.AppendToFile            = true;
                options.ScriptSchema            = true;
                options.ScriptData              = true;
                options.Indexes                 = true;

                var tableEnum     = databases.Tables.Cast<SMO.Table>().Where(i => i.Schema == schemaName);
                var viewEnum      = databases.Views.Cast<SMO.View>().Where(i => i.Schema == schemaName);
                var procedureEnum = databases.StoredProcedures.Cast<SMO.StoredProcedure>().Where(i => i.Schema == schemaName);

                Console.WriteLine("SQL Script Generator");

                Console.WriteLine("\nTable Scripts:");
                foreach (SMO.Table table in tableEnum)
                {
                    databases.Tables[table.Name, schemaName].EnumScript(options);
                    Console.WriteLine(table.Name);
                }

                options.ScriptData       = false;
                options.WithDependencies = false;

                Console.WriteLine("\nView Scripts:");
                foreach (SMO.View view in viewEnum)
                {
                    databases.Views[view.Name, schemaName].Script(options);
                    Console.WriteLine(view.Name);
                }

                Console.WriteLine("\nStored Procedure Scripts:");
                foreach (SMO.StoredProcedure procedure in procedureEnum)
                {
                    databases.StoredProcedures[procedure.Name, schemaName].Script(options);
                    Console.WriteLine(procedure.Name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured: " + ex.Message);
            }
        }
    }
}
