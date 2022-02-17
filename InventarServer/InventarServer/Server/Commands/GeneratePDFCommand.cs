using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Text.Json;

namespace InventarServer
{
    class GeneratePDFCommand : Command
    {
        private Dictionary<DocumentType, Func<string, List<Item>, StreamHelper, byte[]>> documentsTypeFunctions;

        public GeneratePDFCommand() : base("GeneratePDF")
        {
            documentsTypeFunctions = new Dictionary<DocumentType, Func<string, List<Item>, StreamHelper, byte[]>>();
            documentsTypeFunctions.Add(DocumentType.ABSCHREIBUNG, generateAbschreibungsPDF);
            if (!Directory.Exists("pdf"))
                Directory.CreateDirectory("pdf");
        }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            if (!IsAdmin(_u, _helper))
                return;

            string doc = _helper.ReadString();
            DocumentType d = fromString(doc);

            string itemCollection = _helper.ReadString();
            ItemCollection col = _u.Database.GetItemCollection(itemCollection);

            if (!documentsTypeFunctions.ContainsKey(d))
            {
                _helper.SendString("Document type doesn't exist!");
                return;
            }

            if (col == null)
            {
                _helper.SendString("Item Collection doesn't exist!");
                return;
            }

            SendOKMessage(_helper);

            int amount = _helper.ReadInt();
            List<Item> items = new List<Item>();
            for (int i = 0; i < amount; i++)
            {
                Item it = col.GetItem(_u, _helper.ReadString());
                if(it == null)
                {
                    _helper.SendString("Item does not exist!");
                    return;
                }
                items.Add(it);
            }

            SendOKMessage(_helper);

            string filename = "DocumentTypes/" + d.ToString() + ".pdf";

            byte[] pdf = documentsTypeFunctions[d](filename, items, _helper);

            _helper.SendByteArray(pdf);
        }   

        public byte[] generateAbschreibungsPDF(string _filename, List<Item> _items, StreamHelper _helper)
        {
            string abschreibungsType = _helper.ReadString();

            string newFileName = "pdf/" + DateTime.Now.ToString().Replace('/', '_').Replace(' ', '_').Replace(':', '_') + ".pdf";

            using (var reader = new PdfReader(_filename))
            {
                using (var fileStream = new FileStream(newFileName, FileMode.Create, FileAccess.Write))
                {
                    var document = new Document(reader.GetPageSizeWithRotation(1));
                    var writer = PdfWriter.GetInstance(document, fileStream);

                    document.Open();

                    for (int i = 0; i <= _items.Count / 8; i++) {
                        document.NewPage();

                        var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                        var importedPage = writer.GetImportedPage(reader, 1);

                        var contentByte = writer.DirectContent;
                        contentByte.BeginText();
                        contentByte.SetFontAndSize(baseFont, 10);

                        int startPos = i * 8;
                        float y = 561;
                        for (int j = 0; j < Math.Min(startPos + 8, _items.Count) - startPos; j++)
                        {
                            Item it = _items[startPos + j];
                            contentByte.ShowTextAligned(PdfContentByte.ALIGN_CENTER, it.Anlage, 105, y, 0);
                            contentByte.SetFontAndSize(baseFont, 7);
                            contentByte.ShowTextAligned(PdfContentByte.ALIGN_CENTER, it.Anlagenbezeichnung, 284, y + 1.5f, 0);
                            contentByte.SetFontAndSize(baseFont, 10);
                            contentByte.ShowTextAligned(PdfContentByte.ALIGN_CENTER, it.BuchWert.ToString("0.##") + it.Waehrung, 420, y, 0);
                            string x = abschreibungsType;
                            if (it.BuchWert < 0.01)
                                x = "V";
                            contentByte.ShowTextAligned(PdfContentByte.ALIGN_CENTER, x, 503, y, 0);
                            y -= 28.1f;
                        }

                        contentByte.EndText();
                        contentByte.AddTemplate(importedPage, 0, 0);
                    }

                    document.Close();
                    writer.Close();
                }
            }
            return File.ReadAllBytes(newFileName);
        }

        private DocumentType fromString(string _s)
        {
            foreach(DocumentType d in Enum.GetValues(typeof(DocumentType)))
            {
                if (d.ToString() == _s)
                    return d;            }
            return DocumentType.UNKNOWN;
        }
    }

    enum DocumentType
    {
        UNKNOWN, ABSCHREIBUNG
    }
}
