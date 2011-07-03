
using System;
using System.Data;
using ExcelLibrary.SpreadSheet;
using ExcelLibrary;
public static class DataSetToExcel
{
    public static void ExportToExcel(DataSet dataSet, string outputPath)
    {
        Workbook workbook = new Workbook();
        foreach (DataTable dt in dataSet.Tables)
        {
            Worksheet worksheet = new Worksheet(dt.TableName);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                // Add column header
                worksheet.Cells[0, i] = new Cell(dt.Columns[i].ColumnName);

                // Populate row data
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    if (dt.Rows[j][i] == DBNull.Value)
                        worksheet.Cells[j + 1, i] = new Cell(string.Empty);
                    else
                        if (dt.Columns[i].DataType == typeof(DateTime))
                        {
                            worksheet.Cells[j + 1, i] = new Cell(dt.Rows[j][i], new CellFormat(CellFormatType.Date, "DD.MM.YYYY"));
                        }
                        else
                            worksheet.Cells[j + 1, i] = new Cell(dt.Rows[j][i]);
                }
            }
            workbook.Worksheets.Add(worksheet);
        }
        workbook.Save(outputPath);
    }
}