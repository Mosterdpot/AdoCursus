using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoGereedschap
{
    public class Figuur
    {
        private Int32 IDValue;
        private String naamValue;
        private System.Object versieValue;
        public Int32 ID
        {
            get { return IDValue; }
            set { IDValue = value; }
        }
        public String Naam
        {
            get { return naamValue; }
            set { naamValue = value; }
        }
        public Object Versie
        {
            get { return versieValue; }
            set { versieValue = value; }
        }
        public Figuur(Int32 id, String naam, Object versie)
        {
            this.IDValue = id;
            this.Naam = naam;
            this.Versie = versie;
        }
        public Figuur() { }
    }
}
