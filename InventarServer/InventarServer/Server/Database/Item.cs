using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        public string Status { get; set; }
        public string Notiz { get; set; }
        public List<Image> Bilder { get; set; }
        public List<string> Verlauf { get; set; }


        public Item()
        {
            ID = Guid.NewGuid().ToString();
            Anlage = "";
            Unternummer = "";
            AktuelleInventarNummer = "";
            AktivierungAm = DateTime.Now;
            Anlagenbezeichnung = "";
            Serialnummer = "";
            AnschaffungsWert = 0;
            BuchWert = 0;
            Waehrung = "€";
            KfzKennzeichen = "";
            Raum = "";
            RaumBezeichnung = "";
            Status = "";
            Notiz = "";
            Bilder = new List<Image>();
            Verlauf = new List<string>();
        }

        public Item(BsonDocument _doc)
        {
            FromBson(_doc);
            GenerateID();
        }

        public string[] ToStrings()
        {
            return new string[]
            {
                ID, 
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
                RaumBezeichnung,
                Status,
                Notiz
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
            BsonArray images = new BsonArray();
            foreach (Image i in Bilder)
            {
                using (var _memorystream = new MemoryStream())
                {
                    i.Save(_memorystream, i.RawFormat);
                    byte[] img = _memorystream.ToArray();
                    images.Add(img);
                }
            }
            BsonArray history = new BsonArray();
            foreach (string s in Verlauf)
            {
                history.Add(s);
            }
            return new BsonDocument
            {
                { "ID", ID },
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
                { "RaumBezeichnung", RaumBezeichnung },
                { "Status", Status },
                { "Notiz", Notiz },
                { "Bilder", images },
                { "Verlauf", history }
            };
        }

        public void FromBson(BsonDocument _doc)
        {
            Bilder = new List<Image>();
            Verlauf = new List<string>();
            ID = _doc.GetValue("ID").AsString;
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
            Status = _doc.GetValue("Status").AsString;
            Notiz = _doc.GetValue("Notiz").AsString;
            BsonArray images = _doc.GetValue("Bilder").AsBsonArray;
            foreach (var b in images)
            {
                byte[] img = b.AsByteArray;
                Bilder.Add(Image.FromStream(new MemoryStream(img)));
            }
            BsonArray history = _doc.GetValue("Verlauf").AsBsonArray;
            foreach (var h in history)
            {
                Verlauf.Add(h.AsString);
            }

        }

        public void GenerateID()
        {
            if (string.IsNullOrWhiteSpace(ID))
                ID = Guid.NewGuid().ToString();
        }
    }
}
