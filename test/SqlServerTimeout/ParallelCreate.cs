

using System.Data.SqlClient;
using Microsoft.Data.Entity;
using Xunit;

namespace SqlServerTimeout
{
    public class TestContext : DbContext
    {
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

        public void CreateDatabase(int id)
        {
            using (var context = new TestContext("stratch" + id))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
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
