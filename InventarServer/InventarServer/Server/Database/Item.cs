﻿using MongoDB.Bson;
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

        public BsonDocument GetItemAsBson()
        {
            return new BsonDocument
            {
                { "Anlage", Anlage },
                { "Unternummer", Unternummer },
                { "AktuelleInventarNummer", AktuelleInventarNummer },
                { "AktivierungAm", AktivierungAm },
                { "Anlagenbezeichnung", Anlagenbezeichnung },
                { "Serialnummer", Serialnummer },
                { "AnschaffungsWert", AnschaffungsWert },
                { "BuchWert", BuchWert },
                { "Waehrung", Waehrung },
                { "KfzKennzeichen", KfzKennzeichen },
                { "Raum", Raum },
                { "RaumBezeichnung", RaumBezeichnung }
            };
        }

        public void FromBson(BsonDocument _doc)
        {
            Anlage = _doc.GetValue("Anlage").AsString;
            Unternummer = _doc.GetValue("Unternummer").AsString;
            AktuelleInventarNummer = _doc.GetValue("AktuelleInventarNummer").AsString;
            AktivierungAm = _doc.GetValue("AktivierungAm").ToUniversalTime();
            Anlagenbezeichnung = _doc.GetValue("Anlagenbezeichnung").AsString;
            Serialnummer = _doc.GetValue("Serialnummer").AsString;
            AnschaffungsWert = _doc.GetValue("AnschaffungsWert").AsDouble;
            BuchWert = _doc.GetValue("BuchWert").AsDouble;
            Waehrung = _doc.GetValue("Waehrung").AsString;
            KfzKennzeichen = _doc.GetValue("KfzKennzeichen").AsString;
            Raum = _doc.GetValue("Raum").AsString;
            RaumBezeichnung = _doc.GetValue("RaumBezeichnung").AsString;
        }
    }
}