﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Transactions;

namespace AdoGereedschap
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
            var dbManager = new BankDbManager();
            using (var conBank = dbManager.GetConnection())
            {
                using (var comStorten = conBank.CreateCommand())
                {
                    comStorten.CommandText = "Storten";
                    comStorten.CommandType = CommandType.StoredProcedure;

                    DbParameter parTeStorten = comStorten.CreateParameter();
                    parTeStorten.ParameterName = "@teStorten";
                    parTeStorten.DbType = DbType.Currency;
                    parTeStorten.Value = teStorten;
                    comStorten.Parameters.Add(parTeStorten);

                    DbParameter parRekening = comStorten.CreateParameter();
                    parRekening.ParameterName = "@rekeningNr";
                    parRekening.Value = rekeningNr;
                    //parRekening.DbType = DbType.String;
                    comStorten.Parameters.Add(parRekening);
                    conBank.Open();
                    return comStorten.ExecuteNonQuery() != 0;
                }
            }
        }

        public bool StortenWithSQL(Decimal teStorten, String rekeningNr)
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

        public void Overschrijven(Decimal bedrag, String vanRekening, String naarRekening)
        {
            var dbManager = new BankDbManager();
            var dbManager2 = new Bank2DbManager();
            var opties = new TransactionOptions();
            opties.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            using (var traOverschrijven = new TransactionScope(TransactionScopeOption.Required, opties))
            {
                using (var conBank = dbManager.GetConnection())
                {
                    using (var comAftrekken = conBank.CreateCommand())
                    {
                        comAftrekken.CommandType = CommandType.Text;
                        comAftrekken.CommandText = "update Rekeningen set Saldo=Saldo-@bedrag where RekeningNr=@reknr";
                        var parBedrag = comAftrekken.CreateParameter();
                        parBedrag.ParameterName = "@bedrag";
                        parBedrag.Value = bedrag;
                        comAftrekken.Parameters.Add(parBedrag);
                        var parRekNr = comAftrekken.CreateParameter();
                        parRekNr.ParameterName = "@reknr";
                        parRekNr.Value = vanRekening;
                        comAftrekken.Parameters.Add(parRekNr);
                        conBank.Open();
                        if (comAftrekken.ExecuteNonQuery() == 0)
                        {
                            throw new Exception("Van rekening bestaat niet");
                        }
                    } // using comAftrekken
                } // using conBank
                using (var conBank = dbManager2.GetConnection())
                {
                    using (var comBijtellen = conBank.CreateCommand())
                    {
                        comBijtellen.CommandType = CommandType.Text;
                        comBijtellen.CommandText = "update Rekeningen set Saldo=Saldo+@bedrag where RekeningNr=@reknr";
                        var parBedrag = comBijtellen.CreateParameter();
                        parBedrag.ParameterName = "@bedrag";
                        parBedrag.Value = bedrag;
                        comBijtellen.Parameters.Add(parBedrag);
                        var parRekNr = comBijtellen.CreateParameter();
                        parRekNr.ParameterName = "@reknr";
                        parRekNr.Value = naarRekening;
                        comBijtellen.Parameters.Add(parRekNr);
                        conBank.Open();
                        if (comBijtellen.ExecuteNonQuery() == 0)
                        {
                            throw new Exception("Naar rekening bestaat niet");
                        }
                        traOverschrijven.Complete();
                    } // using comBijtellen
                } // using conBank
            } // using traOverschrijven
        }

        public void OverschrijvenWithTransaction(Decimal bedrag, String vanRekening, String naarRekening)
        {
            var dbManager = new BankDbManager();
            using (var conBank = dbManager.GetConnection())
            {
                conBank.Open();
                using (var traOverschrijven = conBank.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    using (var comAftrekken = conBank.CreateCommand())
                    {
                        comAftrekken.Transaction = traOverschrijven;
                        comAftrekken.CommandType = CommandType.Text;
                        comAftrekken.CommandText = "update Rekeningen set Saldo=Saldo-@bedrag where RekeningNr=@reknr";
                        var parBedrag = comAftrekken.CreateParameter();
                        parBedrag.ParameterName = "@bedrag";
                        parBedrag.Value = bedrag;
                        comAftrekken.Parameters.Add(parBedrag);
                        var parRekNr = comAftrekken.CreateParameter();
                        parRekNr.ParameterName = "@reknr";
                        parRekNr.Value = vanRekening;
                        comAftrekken.Parameters.Add(parRekNr);
                        if (comAftrekken.ExecuteNonQuery() == 0)
                        {
                            traOverschrijven.Rollback();
                            throw new Exception("Van rekening bestaat niet");
                        }
                    } // using comAftrekken
                    using (var comBijtellen = conBank.CreateCommand())
                    {
                        comBijtellen.Transaction = traOverschrijven;
                        comBijtellen.CommandType = CommandType.Text;
                        comBijtellen.CommandText = "update Rekeningen set Saldo=Saldo+@bedrag where RekeningNr=@reknr";
                        var parBedrag = comBijtellen.CreateParameter();
                        parBedrag.ParameterName = "@bedrag";
                        parBedrag.Value = bedrag;
                        comBijtellen.Parameters.Add(parBedrag);
                        var parRekNr = comBijtellen.CreateParameter();
                        parRekNr.ParameterName = "@reknr";
                        parRekNr.Value = naarRekening;
                        comBijtellen.Parameters.Add(parRekNr);
                        if (comBijtellen.ExecuteNonQuery() == 0)
                        {
                            traOverschrijven.Rollback();
                            throw new Exception("Naar rekening bestaat niet");
                        }
                    } // using comBijtellen
                    traOverschrijven.Commit();
                } // using traOverschrijven
            } // using conBank
        }

        public Decimal SaldoRekeningRaadplegen(String rekeningNr)
        {
            var dbManager = new BankDbManager();
            using (var conBank = dbManager.GetConnection())
            {
                using (var comSaldo = conBank.CreateCommand())
                {
                    comSaldo.CommandType = CommandType.StoredProcedure;
                    comSaldo.CommandText = "SaldoRekeningRaadplegen";
                    var parRekNr = comSaldo.CreateParameter();
                    parRekNr.ParameterName = "@rekeningNr";
                    parRekNr.Value = rekeningNr;
                    comSaldo.Parameters.Add(parRekNr);
                    conBank.Open();
                    Object resultaat = comSaldo.ExecuteScalar();
                    if (resultaat == null)
                    {
                        throw new Exception("Rekening bestaat niet");
                    }
                    else
                    {
                        return (Decimal)resultaat;
                    }
                }
            }
        }

        public RekeningInfo RekeningInfoRaadplegen(String rekeningNr)
        {
            var dbManager = new BankDbManager();
            using (var conBank = dbManager.GetConnection())
            {
                using (var comSaldo = conBank.CreateCommand())
                {
                    comSaldo.CommandType = CommandType.StoredProcedure;
                    comSaldo.CommandText = "RekeningInfoRaadplegen";
                    var parRekNr = comSaldo.CreateParameter();
                    parRekNr.ParameterName = "@rekeningNr";
                    parRekNr.Value = rekeningNr;
                    comSaldo.Parameters.Add(parRekNr);
                    var parSaldo = comSaldo.CreateParameter();
                    parSaldo.ParameterName = "@Saldo";
                    parSaldo.DbType = DbType.Currency;
                    parSaldo.Direction = ParameterDirection.Output;
                    comSaldo.Parameters.Add(parSaldo);
                    var parKlantNaam = comSaldo.CreateParameter();
                    parKlantNaam.ParameterName = "@KlantNaam";
                    parKlantNaam.DbType = DbType.String;
                    parKlantNaam.Size = 50;
                    parKlantNaam.Direction = ParameterDirection.Output;
                    comSaldo.Parameters.Add(parKlantNaam);
                    conBank.Open();
                    comSaldo.ExecuteNonQuery();
                    if (parSaldo.Value.Equals(DBNull.Value))
                    {
                        throw new Exception("Rekening bestaat niet");
                    }
                    else
                    {
                        return new RekeningInfo((Decimal)parSaldo.Value, (String)parKlantNaam.Value);
                    }
                }
            }
        }
    }
}
