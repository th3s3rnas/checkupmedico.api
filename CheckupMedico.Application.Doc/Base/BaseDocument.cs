namespace CheckupMedico.Application.Doc.Base
{
    using CheckupMedico.Application.Doc.Interface.Base;
    using CheckupMedico.Domain.Enum;
    using CheckupMedico.Transversal.Util;
    using iText.IO.Font.Constants;
    using iText.IO.Image;
    using iText.Kernel.Font;
    using iText.Kernel.Geom;
    using iText.Kernel.Pdf;
    using iText.Kernel.Pdf.Canvas;
    using iText.Kernel.Pdf.Event;
    using iText.Layout;
    using iText.Layout.Element;
    using iText.Layout.Properties;

    public abstract class BaseDocument<TEntrada> : IBaseDocument<TEntrada> where TEntrada : class
    {
        protected Document _document;
        private byte[] _documentoBytes;

        protected virtual float PageWidth { get; } = PageSize.A4.GetWidth();
        protected virtual float PageHeight { get; } = PageSize.A4.GetHeight();

        public void Build(TEntrada data)
        {
            using (var stream = new MemoryStream())
            {
                var pdfWriter = new PdfWriter(stream);
                var pdf = new PdfDocument(pdfWriter);
                _document = new Document(pdf, new PageSize(PageWidth, PageHeight));

                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterEventHandler(""));
                AddHeader();
                AddContent(data);
                _document.Close();
                _documentoBytes = stream.ToArray();
            }
        }

        public byte[] GetDoc() => _documentoBytes;

        private void AddHeader()
        {
            // Crear la imagen con el logo
            var logoImage = new Image(ImageDataFactory.Create(ResourceReader.GetImageAsByteArray(ImageTypes.Logo)))
                                .SetWidth(150)
                                .SetHeight(55);

            // Crear un párrafo para centrar la imagen
            var logoParagraph = new Paragraph().Add(logoImage).SetTextAlignment(TextAlignment.CENTER);

            // Agregar el párrafo que contiene la imagen al documento
            _document.Add(logoParagraph);

            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var paragraph = new Paragraph("")
                .SetFont(font)
                .SetFontSize(13)
                .SetTextAlignment(TextAlignment.CENTER);

            _document.Add(paragraph);
        }

        protected Paragraph AddParagraph(string titulo, float fontSize, bool negrita = false, TextAlignment alineacion = TextAlignment.LEFT)
        {
            PdfFont font = PdfFontFactory.CreateFont(negrita ? StandardFonts.HELVETICA_BOLD : StandardFonts.HELVETICA);

            var paragraph = new Paragraph(titulo)
                .SetFont(font)
                .SetFontSize(fontSize)
                .SetTextAlignment(alineacion);

            return paragraph;
        }

        protected void AddTitle(string titulo, float fontSize, bool negrita = false, TextAlignment alineacion = TextAlignment.LEFT)
        {
            PdfFont font = PdfFontFactory.CreateFont(negrita ? StandardFonts.HELVETICA_BOLD : StandardFonts.HELVETICA);

            var paragraph = new Paragraph(titulo)
                .SetFont(font)
                .SetFontSize(fontSize)
                .SetTextAlignment(alineacion);

            _document.Add(paragraph);
        }

        protected void AddTable(string[,] datos, bool bordes = true)
        {
            int rows = datos.GetLength(0);
            int cols = datos.GetLength(1);
            var table = new Table(cols);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Cell cell = new Cell().Add(new Paragraph(datos[i, j]));
                    if (bordes)
                        cell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                    table.AddCell(cell);
                }
            }

            _document.Add(table);
        }

        protected abstract void AddContent(TEntrada data);
    }

    public class FooterEventHandler : AbstractPdfDocumentEventHandler
    {
        private string _footerText;

        public FooterEventHandler(string footerText)
        {
            _footerText = footerText;
        }

        protected override void OnAcceptedEvent(AbstractPdfDocumentEvent @event)
        {
            var documentEvent = (PdfDocumentEvent)@event;
            var page = documentEvent.GetPage();

            // Obtener el canvas de la página
            var canvas = new PdfCanvas(page);

            // Establecer la fuente y tamaño
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            float fontSize = 7;

            // Obtener dimensiones del rectángulo de la página
            var pageSize = page.GetPageSize();
            float x = pageSize.GetWidth() / 2; // Centrado horizontal
            float y = 15; // Distancia desde el fondo de la página

            // Comenzar a dibujar el texto del pie de página
            canvas.BeginText();
            canvas.SetFontAndSize(font, fontSize);

            // Para centrar el texto, calcular el ancho del texto y restarlo del centro
            float textWidth = font.GetWidth(_footerText, fontSize);
            canvas.SetTextMatrix(x - textWidth / 2, y); // Ajusta x
            canvas.ShowText(_footerText); // Mostrar texto
            canvas.EndText();
        }
    }
}
