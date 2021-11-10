using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    public class Item   
    {
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

        public Item()
        {

        }

        public Item(BsonDocument _doc)
        {
            FromBson(_doc);
        }

        public string[] ToStrings()
        {
            return new string[]
            {
                Anlage,
                Unternummer,
                AktuelleInventarNummer,
                AktivierungAm.ToString(),
                Anlagenbezeichnung,
                Serialnummer,
                AnschaffungsWert.ToString(),
                BuchWert.ToString(),
                Waehrung,
                KfzKennzeichen,
                Raum,
                RaumBezeichnung
            };
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (string s in ToStrings())
            {
                builder.AppendLine(s);
            }
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
            ID = _doc.GetValue("_id").AsObjectId.ToString();
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
