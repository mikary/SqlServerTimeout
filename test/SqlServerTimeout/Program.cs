
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading;

namespace SqlServerTimeout
{
    public class Program
    {
        private static long _count = 0;
        private static int _commandTimeout = 30;

        public void Main(string[] args)
        {
            ThreadPool.SetMinThreads(100, 100);

            for (var index = 0; index < 200; index++)
            {
                Interlocked.Increment(ref _count);

                var closureIndex = index;
                ThreadPool.QueueUserWorkItem(delegate
                {
                    try
                    {
                        var start = DateTime.UtcNow;
                        RunTest(closureIndex);
                        Console.WriteLine("done {0} - {1}", closureIndex, (DateTime.UtcNow - start).TotalSeconds);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    Interlocked.Decrement(ref _count);
                }, null);
            }

            while (Interlocked.Read(ref _count) != 0) ;

            Console.WriteLine("done");
        }

        public static string CreateConnectionString(string initialCatalog)
        {
            return new SqlConnectionStringBuilder
            {
                DataSource = @"(localdb)\MSSQLLocalDB",
                MultipleActiveResultSets = true,
                InitialCatalog = initialCatalog,
                IntegratedSecurity = true,
                ConnectTimeout = 5
            }.ConnectionString;
        }

        public static void RunTest(int id)
        {
            var name = "stratch" + id;
            Delete(name);
            Create(name);
            Delete(name);
        }

        private static void Delete(string name)
        {
            using (var master = new SqlConnection(CreateConnectionString("master")))
            {
                master.Open();

                using (var command = master.CreateCommand())
                {
                    command.CommandTimeout = _commandTimeout;

                    // SET SINGLE_USER will close any open connections that would prevent the drop
                    command.CommandText
                        = string.Format(@"IF EXISTS (SELECT * FROM sys.databases WHERE name = N'{0}')
                                          BEGIN
                                              ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                                              DROP DATABASE [{0}];
                                          END", name);

                    command.ExecuteNonQuery();
                }
            }

            var userFolder = Environment.GetEnvironmentVariable("USERPROFILE");
            try
            {
                File.Delete(Path.Combine(userFolder, name + ".mdf"));
            }
            catch (Exception)
            {
            }

            try
            {
                File.Delete(Path.Combine(userFolder, name + "_log.ldf"));
            }
            catch (Exception)
            {
            }
        }

        private static void Create(string name)
        {
            using (var master = new SqlConnection(CreateConnectionString("master")))
            {
                master.Open();
                using (var command = master.CreateCommand())
                {
                    command.CommandText = $"{Environment.NewLine}CREATE DATABASE [{name}]";

                    command.CommandTimeout = _commandTimeout;

                    command.ExecuteNonQuery();

                    WaitForExists(name);
                }
            }
        }

        private static void WaitForExists(string name)
        {
            var connection = new SqlConnection(CreateConnectionString(name));

            var retryCount = 0;
            while (true)
            {
                try
                {
                    connection.Open();

                    connection.Close();

                    return;
                }
                catch (SqlException e)
                {
                    if (++retryCount >= 30
                        || (e.Number != 233 && e.Number != -2 && e.Number != 4060))
                    {
                        throw;
                    }

                    SqlConnection.ClearPool(connection);

                    Thread.Sleep(100);
                }
            }
        }
    }
}
