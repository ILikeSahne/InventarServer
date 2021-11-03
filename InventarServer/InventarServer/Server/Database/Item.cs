using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    public class Item   
    {
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

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(Anlage);
            builder.AppendLine(Unternummer);
            builder.AppendLine(AktuelleInventarNummer);
            builder.AppendLine(AktivierungAm.ToString());
            builder.AppendLine(Anlagenbezeichnung);
            builder.AppendLine(Serialnummer);
            builder.AppendLine(AnschaffungsWert.ToString());
            builder.AppendLine(BuchWert.ToString());
            builder.AppendLine(Waehrung);
            builder.AppendLine(KfzKennzeichen);
            builder.AppendLine(Raum);
            builder.AppendLine(RaumBezeichnung);
            return builder.ToString();
        }
    }
}
