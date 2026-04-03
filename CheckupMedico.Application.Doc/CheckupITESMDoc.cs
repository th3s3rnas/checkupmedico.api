namespace CheckupMedico.Application.Doc
{
    using CheckupMedico.Application.Doc.Base;
    using CheckupMedico.Application.Doc.Interface;
    using CheckupMedico.Application.Dto.Checkup;
    using iText.Kernel.Colors;
    using iText.Layout.Borders;
    using iText.Layout.Element;
    using iText.Layout.Properties;
    using System.Globalization;

    public class CheckupITESMDoc : BaseDocument<CheckupITESMDto>, ICheckupITESMDoc
    {
        protected override void AddContent(CheckupITESMDto data)
        {
            AddTopParagraph(data);
            AddGeneralTable(data);
            AddSecondaryTable(data);
        }

        private void AddTopParagraph(CheckupITESMDto data)
        {
            CultureInfo cultura = new CultureInfo("es-ES");
            AddTitle("", 9, true, TextAlignment.RIGHT);
            AddTitle("", 9, true, TextAlignment.RIGHT);
            AddTitle($"{data.City}, {data.State}, {DateTime.Now.ToString("d 'de' MMMM 'del' yyyy", cultura)}", 9, true, TextAlignment.RIGHT);

            AddTitle("", 9, true, TextAlignment.RIGHT);
            AddTitle("", 9, true, TextAlignment.RIGHT);
            AddTitle("", 9, true, TextAlignment.RIGHT);
            AddTitle("", 9, true, TextAlignment.RIGHT);
            AddTitle("", 9, true, TextAlignment.RIGHT);

            var titleTable = new Table(1);
            titleTable.SetWidth(UnitValue.CreatePercentValue(100));
            titleTable.SetBorder(Border.NO_BORDER);
            titleTable.SetFixedLayout();

            titleTable.AddCell(new Cell().Add(AddParagraph("Atn.", 9, false, TextAlignment.LEFT)).SetBorder(Border.NO_BORDER));
            titleTable.AddCell(new Cell().Add(AddParagraph($"Hospital: {data.Hospital}, Sucursal:{data.Campus}", 9, false, TextAlignment.LEFT)).SetBorder(Border.NO_BORDER));
            titleTable.AddCell(new Cell().Add(AddParagraph($"Responsable: {data.Responsible}", 9, false, TextAlignment.LEFT)).SetBorder(Border.NO_BORDER));
            titleTable.AddCell(new Cell().Add(AddParagraph("", 9, false, TextAlignment.LEFT)).SetBorder(Border.NO_BORDER));
            titleTable.AddCell(new Cell().Add(
                AddParagraph(
                    $"A nombre y representación del Grupo Educativo Tecnológico de Monterrey, solicitamos proporcionar los servicios de checkup{Environment.NewLine}" +
                    $" ejecutivo conforme al programa de salud establecido y al precio vigente acordado.", 9, false, TextAlignment.LEFT)).SetBorder(Border.NO_BORDER));
            titleTable.AddCell(new Cell().Add(AddParagraph("", 9, false, TextAlignment.LEFT)).SetBorder(Border.NO_BORDER));
            titleTable.AddCell(new Cell().Add(AddParagraph("O.", 9, false, TextAlignment.LEFT)).SetBorder(Border.NO_BORDER));
            titleTable.AddCell(new Cell().Add(AddParagraph("", 9, false, TextAlignment.LEFT)).SetBorder(Border.NO_BORDER));
            titleTable.AddCell(new Cell().Add(
                AddParagraph(
                    $"Autorizamos a {data.Hospital}, {data.Campus} a prestar sus servicios de acuerdo con el contrato celebrado entre {data.FullName} y la Empresa al Titular mencionado.", 9, false, TextAlignment.LEFT)).SetBorder(Border.NO_BORDER));

            _document.Add(titleTable);
            AddTitle("", 9, true, TextAlignment.RIGHT);
            AddTitle("", 9, true, TextAlignment.RIGHT);
        }

        private void AddGeneralTable(CheckupITESMDto data)
        {
            var table = new Table(2);
            table.SetWidth(UnitValue.CreatePercentValue(100));
            table.SetBorder(new SolidBorder(new DeviceRgb(0, 0, 0), 1));
            table.SetFixedLayout();

            // Primera fila
            table.AddCell(new Cell()
                .Add(AddParagraph("Nombre de la Institución:", 9, false, TextAlignment.LEFT)));

            table.AddCell(new Cell()
                .Add(AddParagraph($" {data.Institute}", 9, true, TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));

            // Segunda fila
            table.AddCell(new Cell()
                .Add(AddParagraph("Nómina:", 9, false, TextAlignment.LEFT)));

            table.AddCell(new Cell()
                .Add(AddParagraph($" {data.PayrollId}", 9, true, TextAlignment.LEFT)));

            // Tercera fila
            table.AddCell(new Cell()
                .Add(AddParagraph("Nombre del colaborador:", 9, false, TextAlignment.LEFT)));

            table.AddCell(new Cell()
                .Add(AddParagraph($" {data.Name}", 9, true, TextAlignment.LEFT)));

            // Cuarta fila
            table.AddCell(new Cell()
                .Add(AddParagraph("Fecha de nacimiento:", 9, false, TextAlignment.LEFT)));

            table.AddCell(new Cell()
                .Add(AddParagraph($" {data.BirthDate.ToString("dd/MM/yyyy")}", 9, true, TextAlignment.LEFT)));

            // Quinta fila
            table.AddCell(new Cell()
                .Add(AddParagraph("Paquete:", 9, false, TextAlignment.LEFT)));

            table.AddCell(new Cell()
                .Add(AddParagraph($" {data.Kit}", 9, true, TextAlignment.LEFT)));

            // Sexta fila
            table.AddCell(new Cell()
                .Add(AddParagraph("Vigencia:", 9, false, TextAlignment.LEFT)));

            table.AddCell(new Cell()
                .Add(AddParagraph($" {DateTime.Now.ToString("dd/MM/yyyy")} a {DateTime.Now.AddDays(30).ToString("dd/MM/yyyy")}", 9, true, TextAlignment.LEFT)));

            // Añadir tabla completa al documento
            _document.Add(table);
        }

        private void AddSecondaryTable(CheckupITESMDto data)
        {
            AddTitle("", 9, true, TextAlignment.RIGHT);
            AddTitle("Datos de sucursal", 9, true, TextAlignment.LEFT);

            var table = new Table(2);
            table.SetWidth(UnitValue.CreatePercentValue(100));
            table.SetBorder(new SolidBorder(new DeviceRgb(0, 0, 0), 1));
            table.SetFixedLayout();

            // Primera fila
            table.AddCell(new Cell()
                .Add(AddParagraph("Dirección, horario y teléfonos de contacto:", 9, false, TextAlignment.LEFT)));

            table.AddCell(new Cell()
                .Add(AddParagraph($"{data.LocationDetails} {data.ContactDetails}", 9, true, TextAlignment.LEFT)));

            // Segunda fila
            table.AddCell(new Cell()
                .Add(AddParagraph("Correo electrónico:", 9, false, TextAlignment.LEFT)));

            table.AddCell(new Cell()
                .Add(AddParagraph(data.Email, 9, true, TextAlignment.LEFT)));

            // Añadir tabla completa al documento
            _document.Add(table);

            AddTitle("", 9, true, TextAlignment.LEFT); 
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle(
                $"Manifestamos expresamente que nos obligamos a cubrir la factura que resulte de dicho servicio mediante el proceso centralizado{Environment.NewLine}" +
                $" de cobranza con el que contamos (crédito a la empresa) y que el monto del paquete autorizado no deberá ser por ningún motivo{Environment.NewLine}" +
                $" cobrado al colaborador(a).", 9, false, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle("", 9, true, TextAlignment.LEFT);
            AddTitle($"*En caso de requerirse estudios o tratamientos adicionales el paciente deberá adquirirlos y pagarlos directamente en la caja del hospital", 8, true, TextAlignment.LEFT);

        }
    }
}
