﻿using AdoGereedschap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

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
            using (var conBank = dbManager.GetConnection())
            {
                conBank.Open();
                using (var traOverschrijven = conBank.BeginTransaction(IsolationLevel.ReadCommitted))
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

        public void OverschrijvenWithTransaction(Decimal bedrag, String vanRekening, String naarRekening)
        {
            var dbManager = new BankDbManager();
            using (var conBank = dbManager.GetConnection())
            {
                conBank.Open();
                using (var traOverschrijven = conBank.BeginTransaction(IsolationLevel.ReadCommitted))
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
    }
}
