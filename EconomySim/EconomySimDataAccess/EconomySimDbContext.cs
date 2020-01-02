using EconomySim;
using RepoDb;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomySimDataAccess
{
    public static class EconomySimDbContext
    {
        public static void DoInsert()
        {
            using (var connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=EconomySim;Integrated Security=SSPI").EnsureOpen())
            {
                var good = new Good("TestGoodID", 11.1);
                connection.Insert(good);
                Console.WriteLine("A good record has been inserted.");
            }
        }
    }
}
