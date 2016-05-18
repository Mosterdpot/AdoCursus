using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoGereedschap
{
    public class BrouwerManager
    {

        public List<String> GetPostCodes()
        {
            List<string> postnummers = new List<string>();
            var manager = new BierenDbManager();
            using (var conBrouwer = manager.GetConnection())
            {
                using (var comPostCodes = conBrouwer.CreateCommand())
                {
                    comPostCodes.CommandType = CommandType.StoredProcedure;
                    comPostCodes.CommandText = "PostCodes";
                    conBrouwer.Open();
                    using (var rdrPostCodes = comPostCodes.ExecuteReader())
                    {
                        Int32 postcodePos = rdrPostCodes.GetOrdinal("PostCode");
                        while (rdrPostCodes.Read())
                        {
                            postnummers.Add(rdrPostCodes.GetInt16(postcodePos).ToString());
                        }
                    } // using rdrPostCodes
                } // comPostCodes
            } // conBrouwer
            return postnummers;
        }

        public List<Brouwer> GetBrouwersBeginNaam(String beginNaam)
        {
            List<Brouwer> brouwers = new List<Brouwer>();
            var manager = new BierenDbManager();
            using (var conBieren = manager.GetConnection())
            {
                using (var comBrouwers = conBieren.CreateCommand())
                {
                    comBrouwers.CommandType = CommandType.Text;
                    if (beginNaam != string.Empty)
                    {
                        comBrouwers.CommandText = "select * from Brouwers where BrNaam like @zoals order by BrNaam";
                        var parZoals = comBrouwers.CreateParameter();
                        parZoals.ParameterName = "@zoals";
                        parZoals.Value = beginNaam + "%";
                        comBrouwers.Parameters.Add(parZoals);
                    }
                    else
                        comBrouwers.CommandText = "select * from Brouwers";

                    conBieren.Open();
                    using (var rdrBrouwers = comBrouwers.ExecuteReader())
                    {
                        Int32 brouwerNrPos = rdrBrouwers.GetOrdinal("BrouwerNr");
                        Int32 brNaamPos = rdrBrouwers.GetOrdinal("BrNaam");
                        Int32 adresPos = rdrBrouwers.GetOrdinal("Adres");
                        Int32 postcodePos = rdrBrouwers.GetOrdinal("Postcode");
                        Int32 gemeentePos = rdrBrouwers.GetOrdinal("Gemeente");
                        Int32 omzetPos = rdrBrouwers.GetOrdinal("Omzet");
                        Int32? omzet;
                        while (rdrBrouwers.Read())
                        {
                            if (rdrBrouwers.IsDBNull(omzetPos))
                            {
                                omzet = null;
                            }
                            else
                            {
                                omzet = rdrBrouwers.GetInt32(omzetPos);
                            }
                            brouwers.Add(new Brouwer(rdrBrouwers.GetInt32(brouwerNrPos),
                                                        rdrBrouwers.GetString(brNaamPos),
                                                        rdrBrouwers.GetString(adresPos),
                                                        rdrBrouwers.GetInt16(postcodePos),
                                                        rdrBrouwers.GetString(gemeentePos),
                                                        omzet));

                        } // do while
                    } // using rdrBrouwers
                } // using comBrouwers
            } // using conBieren
            return brouwers;
        }
    }
}
