using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace InventarServer
{
    public class Item   
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


        public Item()
        {
            ItemCollectionName = "";
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
            BarcodeLabelOk = true;
            Bilder = new List<byte[]>();
            Verlauf = new List<string>();
            Permission = "";
        }

        public Item(BsonDocument _doc) : this()
        {
            FromBson(_doc);
            GenerateID();
        }

        public Item(object[] data) : this()
        {
            FromExcelCol(data);
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
                Notiz,
                BarcodeLabelOk.ToString(),
                Permission
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
            foreach (byte[] b in Bilder)
            {
                images.Add(b);
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
                { "BarcodeLabelOk", BarcodeLabelOk },
                { "Bilder", images },
                { "Verlauf", history },
                { "Permission", Permission }
            };
        }

        public void FromBson(BsonDocument _doc)
        {
            // Bilder = new List<byte[]>();
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
            BarcodeLabelOk = _doc.GetValue("BarcodeLabelOk").AsBoolean;
            /*BsonArray images = _doc.GetValue("Bilder").AsBsonArray;
            foreach (var b in images)
            {
                byte[] img = b.AsByteArray;
                Bilder.Add(img);
            }*/
            BsonArray history = _doc.GetValue("Verlauf").AsBsonArray;
            foreach (var h in history)
            {
                Verlauf.Add(h.AsString);
            }
            Permission = _doc.GetValue("Permission").AsString;
        }

        public void FromExcelCol(object[] data)
        {
            Anlage = data[0].ToString();
            Unternummer = data[1].ToString();
            AktuelleInventarNummer = data[2].ToString();
            AktivierungAm = DateTime.Parse(data[3].ToString());
            Anlagenbezeichnung = data[4].ToString();
            Serialnummer = data[5].ToString();
            AnschaffungsWert = double.Parse(data[6].ToString());
            BuchWert = double.Parse(data[7].ToString());
            Waehrung = data[8].ToString();
            KfzKennzeichen = data[9].ToString();
            Raum = data[10].ToString();
            RaumBezeichnung = data[11].ToString();
        }

        public void LoadImages(BsonDocument _doc)
        {
            Bilder = new List<byte[]>();
            BsonArray images = _doc.GetValue("Bilder").AsBsonArray;
            foreach (var b in images)
            {
                byte[] img = b.AsByteArray;
                Bilder.Add(img);
            }
        }

        public void GenerateID()
        {
            if (string.IsNullOrWhiteSpace(ID))
                ID = Guid.NewGuid().ToString();
        }

        public byte[] ToByteArray()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                bf.Serialize(stream, ItemCollectionName);
                bf.Serialize(stream, ID);

                bf.Serialize(stream, Anlage);
                bf.Serialize(stream, Unternummer);
                bf.Serialize(stream, AktuelleInventarNummer);
                bf.Serialize(stream, AktivierungAm);
                bf.Serialize(stream, Anlagenbezeichnung);
                bf.Serialize(stream, Serialnummer);
                bf.Serialize(stream, AnschaffungsWert);
                bf.Serialize(stream, BuchWert);
                bf.Serialize(stream, Waehrung);
                bf.Serialize(stream, KfzKennzeichen);
                bf.Serialize(stream, Raum);
                bf.Serialize(stream, RaumBezeichnung);

                bf.Serialize(stream, Status);
                bf.Serialize(stream, Notiz);
                bf.Serialize(stream, BarcodeLabelOk);
                bf.Serialize(stream, Bilder);
                bf.Serialize(stream, Verlauf);
                bf.Serialize(stream, Permission);

                return stream.ToArray();
            }
        }
    }
}
