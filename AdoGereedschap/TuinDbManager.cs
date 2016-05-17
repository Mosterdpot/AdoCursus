using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Data.Common;

namespace AdoGereedschap
{
    public class TuinDbManager
    {
        private static ConnectionStringSettings conTuinSetting = ConfigurationManager.ConnectionStrings["Tuin"];
        private static DbProviderFactory factory = DbProviderFactories.GetFactory(conTuinSetting.ProviderName);

        public DbConnection GetConnection()
        {
            var conTuin = factory.CreateConnection();
            conTuin.ConnectionString = conTuinSetting.ConnectionString;
            return conTuin;
        }

    }
}
