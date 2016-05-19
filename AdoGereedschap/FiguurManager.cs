using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoGereedschap
{
    public class FiguurManager
    {
        public List<Figuur> GetFiguren()
        {
            List<Figuur> figuren = new List<Figuur>();
            var manager = new StripDBManager();
            using (var conStrip = manager.GetConnection())
            {
                using (var comFiguren = conStrip.CreateCommand())
                {
                    comFiguren.CommandType = CommandType.Text;
                    comFiguren.CommandText = "select * from Figuren";
                    conStrip.Open();
                    using (var rdrFiguren = comFiguren.ExecuteReader())
                    {
                        Int32 IDPos = rdrFiguren.GetOrdinal("ID");
                        Int32 NaamPos = rdrFiguren.GetOrdinal("Naam");
                        Int32 VersiePos = rdrFiguren.GetOrdinal("Versie");
                        while (rdrFiguren.Read())
                        {
                            figuren.Add(new Figuur(rdrFiguren.GetInt32(IDPos),
                            rdrFiguren.GetString(NaamPos),
                            rdrFiguren.GetValue(VersiePos)));
                        } // do while
                    } // using rdrFiguren
                } // using comFiguren
            } // using conStrip
            return figuren;
        }
    }
}
