using OfficeOpenXml;
using WebBackOffice.ViewModels.Oficinas;

namespace WebBackOffice.Helper
{
    public class ExcelHelper
    {
        public static byte[] GenerarExcelNegocioHipotecario(
        List<ListadoHipotecarioVM> data)
        {
        

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Negocio Hipotecario");

            // Cabeceras
            ws.Cells[1, 1].Value = "Ejecutivo";
            ws.Cells[1, 2].Value = "Documento";
            ws.Cells[1, 3].Value = "Celular";
            ws.Cells[1, 4].Value = "Correo";
            ws.Cells[1, 5].Value = "Score";
            ws.Cells[1, 6].Value = "Fecha";
            ws.Cells[1, 7].Value = "Estado";

            int row = 2;
            foreach (var item in data)
            {
                ws.Cells[row, 1].Value = item.Ejecutivo;
                ws.Cells[row, 2].Value = item.Documento;
                ws.Cells[row, 3].Value = item.Celular;
                ws.Cells[row, 4].Value = item.Correo;
                ws.Cells[row, 5].Value = item.Score;
                ws.Cells[row, 6].Value = item.Fecha.ToString("dd/MM/yyyy");
                ws.Cells[row, 7].Value = item.Estado;
                row++;
            }

            ws.Cells.AutoFitColumns();

            return package.GetAsByteArray();
        }
    }
}
