using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Data;
using System.Drawing.Printing;
using System.Windows.Forms;
using AVS.Tools;
using System.Drawing.Html;
using System.Drawing;
using Microsoft.Windows.Controls;
using MongoDB.Bson.Serialization.Attributes;

namespace AVS.Tools
{
    //[name, format]
    public class PrintTemplate
    {
        public const string ItemsPlaceholder = "$items";
        public const string ParamPlaceholderFormat = "${0}";
        public const string ColumnPlaceholderFormat = "[{0}]";
        public const string SizeErrorText = "Шаблон генерирует содержимое недопустимого размера.";

        [BsonId()]
        public string Name
        {
            get;
            set;
        }
        [BsonElement("page_template")]
        public string PageTemplate
        {
            get;
            set;
        }
        [BsonElement("item_template")]
        public string ItemTemplate
        {
            get;
            set;
        }
        [BsonElement("max_items_per_page")]
        public int MaxItemsPerPage
        {
            get;
            set;
        }
    }

    public class PrintTemplateParameters : Dictionary<string, string> { }
    public class TemplatePrinter : PrintDocument
    {
        public TemplatePrinter()
        {
            Parameters = new PrintTemplateParameters();
        }
        List<string> ColumnPlaceholders
        {
            get;
            set;
        }

        string PageHtml
        {
            get;
            set;
        }

        int CurrentPage
        {
            get;
            set;
        }
        int NextRow
        {
            get;
            set;
        }

        public DataTable DataTable
        {
            get;
            set;
        }
        public PrintTemplate Template
        {
            get;
            set;
        }

        public PrintTemplateParameters Parameters
        {
            get;
            set;
        }

        public void InvokeDialog(Action action)
        {
            base.PrintPage += new PrintPageEventHandler(PrintPageHandler);
            try
            {
                action();
            }
            catch (InvalidPrinterException)
            {
                Microsoft.Windows.Controls.MessageBox.Show("На вашем компьютере нет принтеров");
            }
            finally
            {
                base.PrintPage -= new PrintPageEventHandler(PrintPageHandler);
            }
        }

        void Prepare()
        {
            CurrentPage = 1;
            NextRow = 0;
            ColumnPlaceholders = new List<string>();

            foreach (DataColumn dc in DataTable.Columns)
            {
                if (Template.ItemTemplate.Contains(string.Format(PrintTemplate.ColumnPlaceholderFormat, dc.ColumnName)))
                {
                    ColumnPlaceholders.Add(dc.ColumnName);
                }
            }

            PageHtml = Template.PageTemplate;
            if (Parameters != null)
            {
                foreach (string param in Parameters.Keys)
                {
                    PageHtml = PageHtml.Replace(string.Format(PrintTemplate.ParamPlaceholderFormat, param),
                        Parameters[param]);
                }
            }
        }

        public void Preview()
        {
            PrintPreviewDialog pd = new PrintPreviewDialog();
            pd.Document = this;
            InvokeDialog(() =>
            {
                Prepare();
                pd.ShowDialog();
            });
        }
        public new void Print()
        {
            PrintDialog pd = new PrintDialog();
            pd.Document = this;
            InvokeDialog(() =>
            {
                Prepare();
                if (pd.ShowDialog() == DialogResult.OK)
                {
                    base.Print();
                }
            });
        }
        protected void PrintPageHandler(Object sender, PrintPageEventArgs e)
        {
            string collection_html = string.Empty;
            int lastRow = NextRow + Template.MaxItemsPerPage;
            if (lastRow > DataTable.Rows.Count)
                lastRow = DataTable.Rows.Count;
            while (NextRow != lastRow)
            {
                string row_html = Template.ItemTemplate;
                foreach (var r in ColumnPlaceholders)
                {
                    var v = DataTable.Rows[NextRow][r];
                    string s = null;
                    if (v is DateTime)
                    {
                        s = ((DateTime)v).ToString("dd.MM.yyyy");
                    }
                    else
                        if (v is double)
                        {
                            s = ((double)v).ToString("0.##");
                        }
                        else
                        {
                            s = DataTable.Rows[NextRow][r].ToString();
                        }
                    row_html = row_html.Replace(
                        string.Format(PrintTemplate.ColumnPlaceholderFormat, r), s);
                }
                collection_html += row_html;
                NextRow++;
            }

            string html = PageHtml.Replace(PrintTemplate.ItemsPlaceholder, collection_html);

            var area = new RectangleF(e.MarginBounds.Location, e.MarginBounds.Size);
            HtmlRenderer.Render(e.Graphics, html, area, false);

            Font font = new Font("Verdana", 12);
            Brush black_brush = new SolidBrush(Color.Black);
            float x = e.MarginBounds.Right + (e.PageBounds.Right - e.MarginBounds.Right) / 3;
            float y = e.MarginBounds.Bottom + (e.PageBounds.Bottom - e.MarginBounds.Bottom) / 3;
            e.Graphics.DrawString(CurrentPage.ToString(), font, black_brush, x, y);

            CurrentPage++;
            e.HasMorePages = NextRow != DataTable.Rows.Count;
        }
    }
}
