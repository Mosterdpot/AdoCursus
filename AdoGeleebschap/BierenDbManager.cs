using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;

namespace AdoGereedschap
{
    public class BierenDbManager
    {
        private static ConnectionStringSettings conBierenSetting = ConfigurationManager.ConnectionStrings["Bieren"];
        private static DbProviderFactory factory = DbProviderFactories.GetFactory(conBierenSetting.ProviderName);

        public DbConnection GetConnection()
        {
            var conBieren = factory.CreateConnection();
            conBieren.ConnectionString = conBierenSetting.ConnectionString;
            return conBieren;
        }
    }

    public class BankDbManager
    {
        private static ConnectionStringSettings conBankSetting = ConfigurationManager.ConnectionStrings["Bank"];
        private static DbProviderFactory factory = DbProviderFactories.GetFactory(conBankSetting.ProviderName);

        public DbConnection GetConnection()
        {
            var conBank = factory.CreateConnection();
            conBank.ConnectionString = conBankSetting.ConnectionString;
            return conBank;
        }

    }

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
