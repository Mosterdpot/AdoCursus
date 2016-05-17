using AdoGereedschap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace AdoWPF
{
    public class RekeningenManager
    {
        public Int32 SaldoBonus()
        {
            var dbManager = new BankDbManager();
            using (var conBank = dbManager.GetConnection())
            {
                using (var comBonus = conBank.CreateCommand())
                {
                    comBonus.CommandType = CommandType.Text;
                    comBonus.CommandText = "update Rekeningen set Saldo=Saldo*1.1";
                    conBank.Open();
                    return comBonus.ExecuteNonQuery();
                }
            }
        }

        public Boolean Storten(Decimal teStorten, String rekeningNr)
        {
            BankDbManager dbManager = new BankDbManager();
            using (var conBank = dbManager.GetConnection())
            {
                using (var comStorten = conBank.CreateCommand())
                {
                    comStorten.CommandText = "Storten";
                    comStorten.CommandType = CommandType.StoredProcedure;

                    DbParameter parTeStorten = comStorten.CreateParameter();
                    parTeStorten.ParameterName = "@teStorten";
                    parTeStorten.DbType = DbType.Currency;
                    comStorten.Parameters.Add(parTeStorten);

                    DbParameter parRekening = comStorten.CreateParameter();
                    parRekening.ParameterName = "@rekningNr";
                    parRekening.DbType = DbType.String;
                    comStorten.Parameters.Add(parRekening);

                    return comStorten.ExecuteNonQuery() != 0;
                }
            }
            return false;
        }

        public bool Storten2(Decimal teStorten, String rekeningNr)
        {
            var dbManager = new BankDbManager();
            using (var conBank = dbManager.GetConnection())
            {
                using (var comStorten = conBank.CreateCommand())
                {
                    comStorten.CommandType = CommandType.Text;
                    comStorten.CommandText = "update Rekeningen set Saldo=Saldo+@teStorten where RekeningNr=@RekeningNr";
                    DbParameter parTeStorten = comStorten.CreateParameter();
                    parTeStorten.ParameterName = "@teStorten";
                    parTeStorten.Value = teStorten;
                    comStorten.Parameters.Add(parTeStorten);
                    DbParameter parRekeningNr = comStorten.CreateParameter();
                    parRekeningNr.ParameterName = "@RekeningNr";
                    parRekeningNr.Value = rekeningNr;
                    comStorten.Parameters.Add(parRekeningNr);
                    conBank.Open();
                    return comStorten.ExecuteNonQuery() != 0;
                }
            }
        }
    }
}
