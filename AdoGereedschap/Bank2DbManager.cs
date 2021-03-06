﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Data.Common;

namespace AdoGereedschap
{
    public class Bank2DbManager
    {
        private static ConnectionStringSettings conBankSetting = ConfigurationManager.ConnectionStrings["Bank2"];
        private static DbProviderFactory factory = DbProviderFactories.GetFactory(conBankSetting.ProviderName);
        public DbConnection GetConnection()
        {
            var conBank = factory.CreateConnection();
            conBank.ConnectionString = conBankSetting.ConnectionString;
            return conBank;
        }
    }
}
