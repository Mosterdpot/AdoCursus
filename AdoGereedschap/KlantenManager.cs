using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoGereedschap
{
    public class KlantenManager
    {
        public Int64 NieuweKlant(String naam)
        {
            var manager = new BankDbManager();
            using (var conBank = manager.GetConnection())
            {
                using (var comToevoegen = conBank.CreateCommand())
                {
                    comToevoegen.CommandType = CommandType.StoredProcedure;
                    comToevoegen.CommandText = "NieuweKlant";
                    var parNaam = comToevoegen.CreateParameter();
                    parNaam.ParameterName = "@Naam";
                    parNaam.Value = naam;
                    comToevoegen.Parameters.Add(parNaam);
                    conBank.Open();
                    Int64 klantNr = Convert.ToInt64(comToevoegen.ExecuteScalar());
                    return klantNr;
                } // using comToevoegen
            } // using conBank
        }

        private void Test()
        {
            //MessageBox.Shox();

        }
    }
}
