using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarAPI
{
    [Serializable]
    public class ItemStorage
    {
        public string ItemCollectionName { get; set; }
        public string ID { get; set; }

        public string Anlage { get; set; }
        public string Unternummer { get; set; }
        public string AktuelleInventarNummer { get; set; }
        public DateTime AktivierungAm { get; set; }
        public string Anlagenbezeichnung { get; set; }
        public string Serialnummer { get; set; }
        public double AnschaffungsWert { get; set; }
        public double BuchWert { get; set; }
        public string Waehrung { get; set; }
        public string KfzKennzeichen { get; set; }
        public string Raum { get; set; }
        public string RaumBezeichnung { get; set; }

        public string Status { get; set; }
        public string Notiz { get; set; }
        public bool BarcodeLabelOk { get; set; }
        public List<byte[]> Bilder { get; set; }
        public List<string> Verlauf { get; set; }
        public string Permission { get; set; }
    }
}
