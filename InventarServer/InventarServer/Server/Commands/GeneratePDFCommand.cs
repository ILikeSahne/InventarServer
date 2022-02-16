using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using iTextSharp.text.pdf;

namespace InventarServer
{
    class GeneratePDFCommand : Command
    {
        private Dictionary<DocumentType, Func<Image, Image>> documentsTypeFunctions;

        public GeneratePDFCommand() : base("GeneratePDF")
        {
            documentsTypeFunctions = new Dictionary<DocumentType, Func<Image, Image>>();
            documentsTypeFunctions.Add(DocumentType.ABSCHREIBUNG, generateAbschreibungsPDF);
        }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            if (!IsAdmin(_u, _helper))
                return;

            string doc = _helper.ReadString();
            DocumentType d = fromString(doc);

            if(!documentsTypeFunctions.ContainsKey(d))
            {
                _helper.SendString("Document type doesn't exist!");
                return;
            }

            SendOKMessage(_helper); 

            Image img = Image.FromFile("DocumentTypeImages/" + d.ToString() + ".png");

            img = documentsTypeFunctions[d](img);

            byte[] pdf = convertToPDF(img);
            _helper.SendByteArray(pdf);
        }

        private Image generateAbschreibungsPDF(Image _img)
        {
            return _img;
        }

        private byte[] convertToPDF(Image _img)
        {
            string filename = "pdfs/" + DateTime.Now.ToString().Replace('/', '_').Replace(' ', '_').Replace(':', '_') + ".pdf";

            iTextSharp.text.Document document = new iTextSharp.text.Document();
            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                PdfWriter.GetInstance(document, stream);
                document.Open();
                document.Add(iTextSharp.text.Image.GetInstance(_img, iTextSharp.text.BaseColor.WHITE));
                document.Close();
            }

            return File.ReadAllBytes(filename);
        }

        private DocumentType fromString(string _s)
        {
            foreach(DocumentType d in Enum.GetValues(typeof(DocumentType)))
            {
                if (d.ToString() == _s)
                    return d;
            }
            return DocumentType.UNKNOWN;
        }
    }

    enum DocumentType
    {
        UNKNOWN, ABSCHREIBUNG
    }
}
