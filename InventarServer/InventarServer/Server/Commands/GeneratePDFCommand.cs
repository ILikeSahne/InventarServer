using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace InventarServer
{
    class GeneratePDFCommand : Command
    {
        private Dictionary<DocumentType, Func<Image, List<Image>>> documentsTypeFunctions;

        public GeneratePDFCommand() : base("GeneratePDF")
        {
            documentsTypeFunctions = new Dictionary<DocumentType, Func<Image, List<Image>>>();
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

            List<Image> images = documentsTypeFunctions[d](img);

            byte[] pdf = convertToPDF2(images);

            _helper.SendByteArray(pdf);

            img.Dispose();
            foreach (Image i in images)
                i.Dispose();
        }

        private List<Image> generateAbschreibungsPDF(Image _img)
        {
            List<Image> images = new List<Image>();
            for(int i = 0; i < 10; i++)
                images.Add(_img);
            return images;
        }

        private byte[] convertToPDF2(List<Image> _images)
        {
            if (!Directory.Exists("pdfs"))
                Directory.CreateDirectory("pdfs");
            string filename = "pdfs/" + DateTime.Now.ToString().Replace('/', '_').Replace(' ', '_').Replace(':', '_') + ".pdf";

            PdfDocument document = new PdfDocument();

            foreach (Image i in _images)
            {
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                DrawImage(gfx, i, page);
            }
            document.Save(filename);

            return File.ReadAllBytes(filename);
        }

        private void DrawImage(XGraphics _gfx, Image _img, PdfPage _page)
        {
            MemoryStream strm = new MemoryStream();
            _img.Save(strm, System.Drawing.Imaging.ImageFormat.Png);

            XImage xImg = XImage.FromStream(strm);

            _gfx.DrawImage(xImg, 0, 0, _page.Width, _page.Height);
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
