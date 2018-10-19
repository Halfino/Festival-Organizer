using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Navigation;
using iText.Layout;
using iText.Layout.Element;
using Table = iText.Layout.Element.Table;
using iText.Layout.Properties;
using iText.IO.Util;
using EvickaWPF;
using System.Collections;
using iText.Kernel.Colors;

namespace Organizer
{
    public  class PdfExport
    {
        /// <summary>
        /// Process band collection to pdf
        /// </summary>
        /// <param name="bands"></param>
        /// <param name="destination"></param>
        /// <param name="fontSize"></param>
        public static void processBandsToPdf(ArrayList bands, string destination, int fontSize)
        {
            PdfDocument pdf = createDocument(destination);
            Document doc = preparePdfDocument(fontSize, pdf);
            List<KeyValuePair<string, int>> toc = new List<KeyValuePair<string, int>>();
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            Paragraph p;
            int counter = 0;
            int iterator = 0;

            foreach (Band band in bands)
            {
                ArrayList contacts = new ArrayList();
                contacts = BandContact.findBandContacts(band);

                p = new Paragraph(band.name);
                p.SetKeepTogether(true);
                string line = band.name;
                string name = String.Format(band.name, counter++);
                p.SetFont(bold).SetFontSize(12).SetKeepWithNext(true).SetDestination(name).SetTextAlignment(TextAlignment.CENTER);
                doc.Add(p);
                toc.Add(new KeyValuePair<string, int>(line, pdf.GetNumberOfPages()));

                PdfExport.writeBandProperty(doc, "Mesto:", band.city);
                PdfExport.writeBandProperty(doc, "Styl:", band.style);

                p = new Paragraph("Slozeni a kontakty").SetFont(bold);
                doc.Add(p);
                PdfExport.processContactsToBandExport(doc, contacts);
                PdfExport.writeBandProperty(doc, "Popis:", band.description);
                PdfExport.writeBandProperty(doc, "Facebook:", band.facebook);
                PdfExport.writeBandProperty(doc, "Bandzone", band.banzone);
                PdfExport.writeBandProperty(doc, "Webová stránka:", band.website);

                if (iterator < bands.Count - 1)
                {
                    doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                }
                iterator += 1;
            }
            PdfExport.processDocumentLinkedList(toc, doc);

            doc.Close();
        }

        /// <summary>
        /// Process one band parameter, defined in method params and writes it into PDF file
        /// </summary>
        /// <param name="document"></param>
        /// <param name="nameOfProperty"></param>
        /// <param name="propertyValue"></param>
        private static void writeBandProperty(Document document, string nameOfProperty, string propertyValue)
        {
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            Paragraph p = new Paragraph(nameOfProperty).SetFont(bold);
            document.Add(p);
            p = new Paragraph(propertyValue).SetFont(font).SetMarginLeft(20).SetTextAlignment(TextAlignment.JUSTIFIED);
            document.Add(p);
        }

        /// <summary>
        /// Make table of given contacts collection and writes it into PDF file. Writes only header if the collection is empty
        /// </summary>
        /// <param name="document"></param>
        /// <param name="contacts"></param>
        private static void processContactsToBandExport(Document document, ArrayList contacts)
        {
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            //set number of table columns and their width relative to each other (that works weird, changing values has no affection)
            Table table = new Table(new float[] { 1, 1, 1, 1 });

            //table width related to page
            table.SetWidth(UnitValue.CreatePercentValue(100));

            //process header
            string headerLine = "Jméno, Funkce, Telefon, Email";
            process(table, headerLine, bold, true);

            if(contacts.Count != 0)
            {
                foreach (BandContact contact in contacts)
                {
                    string line;
                    string phone;
                    string email;

                    if (contact.phone.Count() > 1)
                    {
                        phone = contact.phone;
                    }
                    else
                    {
                        phone = "NIL";
                    }

                    if (contact.email.Count() > 3)
                    {
                        email = contact.email;
                    }
                    else
                    {
                        email = "NIL";
                    }

                    line = string.Format("{0}, {1}, {2}, {3}", contact.fName + contact.lName, contact.function, phone, email);

                    //process data rows into table
                    process(table, line, font, false);
                }
            }
            document.Add(table);
        }

        /// <summary>
        /// Process table row for PDF export
        /// </summary>
        /// <param name="table"></param>
        /// <param name="line"></param>
        /// <param name="font"></param>
        /// <param name="isHeader"></param>
        private static void process(Table table, String line, PdfFont font, Boolean isHeader)
        {
            StringTokenizer tokenizer = new StringTokenizer(line, ",");

            while (tokenizer.HasMoreTokens())
            {
                if (isHeader)
                {
                    table.AddHeaderCell(new Cell().SetBackgroundColor(WebColors.GetRGBColor("A6B8AE")).Add(new Paragraph(tokenizer.NextToken()).SetFont(font)));
                }
                else
                {
                    table.AddCell(new Cell().Add(new iText.Layout.Element.Paragraph(tokenizer.NextToken()).SetFont(font)));
                }

            }
        }

        /// <summary>
        /// Create document object by font size
        /// </summary>
        /// <param name="fontSize"></param>
        /// <param name="pdf"></param>
        /// <returns></returns>
        private static Document preparePdfDocument(int fontSize, PdfDocument pdf)
        {
            Document document = new Document(pdf, PageSize.A4);
            document.SetMargins(20, 20, 20, 20);
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            document.SetFont(font);
            document.SetFontSize(fontSize);

            return document;
        }

        /// <summary>
        /// create pdf document for export
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        private static PdfDocument createDocument(string destination)
        {
            PdfWriter writer = new PdfWriter(destination);
            PdfDocument pdf = new PdfDocument(writer);

            return pdf;
        }
        
        /// <summary>
        /// process list as last page of creating PDF
        /// </summary>
        /// <param name="toc"></param>
        /// <param name="doc"></param>
       private  static void processDocumentLinkedList(List<KeyValuePair<string, int>> toc, Document doc)
        {
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            Paragraph p = new Paragraph().SetFont(bold).Add("Obsah");
            doc.Add(p);

            List<TabStop> tabStops = new List<TabStop>();
            tabStops.Add(new TabStop(580, TabAlignment.RIGHT, new iText.Kernel.Pdf.Canvas.Draw.DottedLine()));
            foreach (KeyValuePair<string, int> entry in toc)
            {
                p = new iText.Layout.Element.Paragraph()
                    .AddTabStops(tabStops)
                    .Add(entry.Key)
                    .Add(new Tab())
                    .Add(entry.Value.ToString())
                    .SetAction(PdfAction.CreateGoTo(PdfExplicitRemoteGoToDestination.CreateFit(entry.Value)));

                doc.Add(p);
            }
        }
    }
}



