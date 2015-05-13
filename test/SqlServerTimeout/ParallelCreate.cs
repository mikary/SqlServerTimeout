

using System;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Data.Entity;
using Xunit;

namespace SqlServerTimeout
{
    public class Poco
    {
    }

    public class TestContext : DbContext
    {
        public DbSet<Poco> Pocos;

        private readonly string _initialCatalog;

        public TestContext(string initialCatalog)
        {
            _initialCatalog = initialCatalog;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                new SqlConnectionStringBuilder
                    {
                        DataSource = @"(localdb)\MSSQLLocalDB",
                        MultipleActiveResultSets = true,
                        InitialCatalog = _initialCatalog,
                        IntegratedSecurity = true,
                        ConnectTimeout = 30
                    }.ConnectionString);
        }
    }

    public class ParallelCreate
    {

        public string CreateConnectionString(string initialCatalog)
        {
            return new SqlConnectionStringBuilder
                {
                    DataSource = @"(localdb)\MSSQLLocalDB",
                    MultipleActiveResultSets = true,
                    InitialCatalog = initialCatalog,
                    IntegratedSecurity = true,
                    ConnectTimeout = 30
                }.ConnectionString;
        }


        public void CreateDatabase(int id)
        {
            var name = "stratch" + id;

            using (var context = new TestContext(name))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                using (var master = new SqlConnection(CreateConnectionString("master")))
                {
                    master.Open();

                    using (var command = master.CreateCommand())
                    {
                        command.CommandTimeout = 30;

                        // SET SINGLE_USER will close any open connections that would prevent the drop
                        command.CommandText
                            = string.Format(@"IF EXISTS (SELECT * FROM sys.databases WHERE name = N'{0}')
                                          BEGIN
                                              ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                                              DROP DATABASE [{0}];
                                          END", name);

                        command.ExecuteNonQuery();

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
                }
            }
        }

        [Fact]
        public void Create_database1() => CreateDatabase(1);
        [Fact]
        public void Create_database2() => CreateDatabase(2);
        [Fact]
        public void Create_database3() => CreateDatabase(3);
        [Fact]
        public void Create_database4() => CreateDatabase(4);
        [Fact]
        public void Create_database5() => CreateDatabase(5);
        [Fact]
        public void Create_database6() => CreateDatabase(6);
        [Fact]
        public void Create_database7() => CreateDatabase(7);
        [Fact]
        public void Create_database8() => CreateDatabase(8);
        [Fact]
        public void Create_database9() => CreateDatabase(9);
        [Fact]
        public void Create_database10() => CreateDatabase(10);
        [Fact]
        public void Create_database11() => CreateDatabase(11);
        [Fact]
        public void Create_database12() => CreateDatabase(12);
        [Fact]
        public void Create_database13() => CreateDatabase(13);
        [Fact]
        public void Create_database14() => CreateDatabase(14);
        [Fact]
        public void Create_database15() => CreateDatabase(15);
        [Fact]
        public void Create_database16() => CreateDatabase(16);
        [Fact]
        public void Create_database17() => CreateDatabase(17);
        [Fact]
        public void Create_database18() => CreateDatabase(18);
        [Fact]
        public void Create_database19() => CreateDatabase(19);
    }
}
