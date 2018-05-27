using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetModel;
using VetService.VetBindingModels;
using VetService.VetInterfaces;
using VetService.VetViewModels;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.IO;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Excel;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using System.Threading;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace VetService.VetImplementations
{
    public class ReportService : IReportService
    {
        private AbstractDbContext context;

        public ReportService(AbstractDbContext context)
        {
            this.context = context;
        }
        public async Task<List<ClientCreaditViewModel>> GetClientCredits(ReportBindingModel model)
        {
            return await context.Clients.GroupJoin(
                context.Orders.Where(rec => rec.OrderStatus != OrderStatus.Оплачен)
                .Where(rec=>rec.DateCreate>=model.DateFrom && rec.DateCreate<=model.DateTo).Include(rec=>rec.Client)
                .Include(rec => rec.Pays).Include(rec => rec.ServiceOrders),
                client => client,//возможно требуется include
                order => order.Client,
                (client, orderList) =>
                new ClientCreaditViewModel
                {
                    ClientId = client.Id,
                    ClientFIO = client.ClientFIO,
                    Mail = client.UserName,
                    OrderCredits = orderList.Select(rec => new OrderCreditViewModel
                    {
                        OrderId = rec.Id,
                        Services = rec.ServiceOrders.Select(recO => new ServiceOrderViewModel {
                            ServiceName = recO.Service.ServiceName,
                            Count = recO.Count,
                            Price = recO.Price,
                            Total = recO.Count * recO.Price
                        }).ToList(),
                        DateCreate = SqlFunctions.DateName("dd", rec.DateCreate) + " " +
                                            SqlFunctions.DateName("mm", rec.DateCreate) + " " +
                                            SqlFunctions.DateName("yyyy", rec.DateCreate),
                        Total = rec.ServiceOrders.Select(recO => recO.Price * recO.Count).DefaultIfEmpty(0).Sum(),
                        TotalPaid = rec.Pays.Select(recO => recO.Summ).DefaultIfEmpty(0).Sum(),
                        Credit = rec.ServiceOrders.Select(recO => recO.Price * recO.Count).DefaultIfEmpty(0).Sum() - rec.Pays.Select(recO => recO.Summ).DefaultIfEmpty(0).Sum()
                    }).ToList()
                }).Where(rec => rec.OrderCredits.Count > 0).ToListAsync();
        }

        public async System.Threading.Tasks.Task SendClientCreditDoc(ClientCreaditViewModel model, string TempPath)
        {
            var ms = (DateTime.Now - DateTime.MinValue).TotalMilliseconds;
            TempPath += model.ClientFIO + "_Credits_" + ms + ".doc";

            var winword = new Microsoft.Office.Interop.Word.Application();
            try
            {
                object missing = System.Reflection.Missing.Value;
                //создаем документ
                Microsoft.Office.Interop.Word.Document document =
                    winword.Documents.Add(ref missing, ref missing, ref missing, ref missing);
                //получаем ссылку на параграф
                var paragraph = document.Paragraphs.Add(missing);
                var range = paragraph.Range;
                //задаем текст
                range.Text = "Дорогой Пездюк " + model.ClientFIO + ", тiкай з городу";
                //задаем настройки шрифта
                var font = range.Font;
                font.Size = 16;
                font.Name = "Times New Roman";
                font.Bold = 1;
                //задаем настройки абзаца
                var paragraphFormat = range.ParagraphFormat;
                paragraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                paragraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
                paragraphFormat.SpaceAfter = 10;
                paragraphFormat.SpaceBefore = 0;
                //добавляем абзац в документ
                range.InsertParagraphAfter();

                foreach (var order in model.OrderCredits)
                {
                    paragraph = document.Paragraphs.Add(missing);
                    range = paragraph.Range;

                    range.Text = "Дата: " + order.DateCreate;

                    font = range.Font;
                    font.Size = 12;
                    font.Name = "Times New Roman";

                    paragraphFormat = range.ParagraphFormat;
                    paragraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    paragraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
                    paragraphFormat.SpaceAfter = 10;
                    paragraphFormat.SpaceBefore = 10;

                    range.InsertParagraphAfter();
                    //создаем таблицу
                    var paragraphTable = document.Paragraphs.Add(Type.Missing);
                    var rangeTable = paragraphTable.Range;
                    var table = document.Tables.Add(rangeTable, order.Services.Count + 4, 5, ref missing, ref missing);

                    font = table.Range.Font;
                    font.Size = 14;
                    font.Name = "Times New Roman";

                    var paragraphTableFormat = table.Range.ParagraphFormat;
                    paragraphTableFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
                    paragraphTableFormat.SpaceAfter = 0;
                    paragraphTableFormat.SpaceBefore = 0;
                    //шабка
                    table.Cell(1, 1).Range.Text = "№";
                    table.Cell(1, 2).Range.Text = "Услуга";
                    table.Cell(1, 3).Range.Text = "Количество";
                    table.Cell(1, 4).Range.Text = "Цена";
                    table.Cell(1, 5).Range.Text = "Сумма";

                    for (int i = 0; i < order.Services.Count; ++i)
                    {
                        if (order.Services[i] != null)
                        {

                            table.Cell(i + 2, 1).Range.Text = (i+1).ToString();
                            table.Cell(i + 2, 2).Range.Text = order.Services[i].ServiceName;
                            table.Cell(i + 2, 3).Range.Text = order.Services[i].Count.ToString();
                            table.Cell(i + 2, 4).Range.Text = order.Services[i].Price.ToString();
                            table.Cell(i + 2, 5).Range.Text = order.Services[i].Total.ToString();
                        }
                    }
                    //font = table.Cell(order.Services.Count + 2, 4).Range.Font;
                    //font.Bold = 1;
                    table.Cell(order.Services.Count + 2, 4).Range.Text = "Итого:";
                    table.Cell(order.Services.Count + 2, 5).Range.Text = order.Total.ToString();
                    table.Cell(order.Services.Count + 3, 4).Range.Text = "Оплачено:";
                    table.Cell(order.Services.Count + 3, 5).Range.Text = order.TotalPaid.ToString();
                    table.Cell(order.Services.Count + 4, 4).Range.Text = "Кредит:";
                    table.Cell(order.Services.Count + 4, 5).Range.Text = order.Credit.ToString();
                    //задаем границы таблицы
                    table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleInset;
                    table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

                    range.InsertParagraphAfter();
                }
                //сохраняем
                object fileFormat = WdSaveFormat.wdFormatXMLDocument;
                document.SaveAs(TempPath, ref fileFormat, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing);
                document.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                winword.Quit();
            }
            await SendMail(model.Mail, "Долги по услугам в ветеринарной клинике Mentula", "Отчет по долгам " + model.ClientFIO + " за " + DateTime.Now.ToLongDateString(), TempPath);
            File.Delete(TempPath);
        }

        private async System.Threading.Tasks.Task<OrderViewModel> GetOrder(int orderId)
        {
            return await context.Orders.Where(rec => rec.Id == orderId).Include(rec => rec.Client).Include(rec => rec.ServiceOrders)
                .Select(rec => new OrderViewModel
                {
                    Id = rec.Id,
                    ClientFIO = rec.Client.ClientFIO,
                    DateCreate = SqlFunctions.DateName("dd", rec.DateCreate) + " " +
                                            SqlFunctions.DateName("mm", rec.DateCreate) + " " +
                                            SqlFunctions.DateName("yyyy", rec.DateCreate),
                    Sum = rec.ServiceOrders.Select(r=>r.Count*r.Price).DefaultIfEmpty(0).Sum(),
                    Mail = rec.Client.UserName,
                    ServiceOrders = rec.ServiceOrders.Select(r => new ServiceOrderViewModel
                    {
                        ServiceName = r.Service.ServiceName,
                        Count = r.Count,
                        Price = r.Price,
                        Total = r.Count * r.Price
                    }).ToList()

                }).FirstOrDefaultAsync();
        }

        public async System.Threading.Tasks.Task SendClientAccountXls(ReportBindingModel model)
        {
            OrderViewModel order = await GetOrder(model.OrderId);

            var ms = (DateTime.Now - DateTime.MinValue).TotalMilliseconds;
            model.FileName += order.ClientFIO + "_Account_№" + order.Id + "_" + ms + ".xls";

            var excel = new Microsoft.Office.Interop.Excel.Application();
            try
            {
                if (File.Exists(model.FileName))
                {
                    excel.Workbooks.Open(model.FileName, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing);
                }
                else
                {
                    excel.SheetsInNewWorkbook = 1;
                    excel.Workbooks.Add(Type.Missing);
                    excel.Workbooks[1].SaveAs(model.FileName, XlFileFormat.xlExcel8, Type.Missing,
                        Type.Missing, false, false, XlSaveAsAccessMode.xlNoChange, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                }

                Sheets excelsheets = excel.Workbooks[1].Worksheets;
                //Получаем ссылку на лист
                var excelworksheet = (Worksheet)excelsheets.get_Item(1);
                //очищаем ячейки
                excelworksheet.Cells.Clear();
                //настройки страницы
                excelworksheet.PageSetup.Orientation = XlPageOrientation.xlLandscape;
                excelworksheet.PageSetup.CenterHorizontally = true;
                excelworksheet.PageSetup.CenterVertically = true;
                
                Microsoft.Office.Interop.Excel.Range excelcells = excelworksheet.get_Range("A1", "E1");
                //объединяем их
                excelcells.Merge(Type.Missing);
                //задаем текст, настройки шрифта и ячейки
                excelcells.Font.Bold = true;
                excelcells.Value2 = "Счет на оплату № " + order.Id + " от " + order.DateCreate;
                excelcells.RowHeight = 25;
                excelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                excelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                excelcells.Font.Name = "Times New Roman";
                excelcells.Font.Size = 14;

                if (order.ServiceOrders != null)
                {
                    excelcells = excelworksheet.get_Range("A3", "A3");
                    //выделение таблицы
                    var excelTable =
                                excelworksheet.get_Range(excelcells,
                                            excelcells.get_Offset(order.ServiceOrders.Count(), 4));
                    excelTable.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    excelTable.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;
                    excelTable.HorizontalAlignment = Constants.xlCenter;
                    excelTable.VerticalAlignment = Constants.xlCenter;
                    excelTable.BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous,
                                            Microsoft.Office.Interop.Excel.XlBorderWeight.xlMedium,
                                            Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic,
                                            1);

                    var excelHead =
                                excelworksheet.get_Range(excelcells,
                                            excelcells.get_Offset(0, 4));
                    excelHead.Font.Bold = true;

                    excelcells.ColumnWidth = 15;
                    excelcells.Value2 = "№";
                    excelcells = excelcells.get_Offset(0, 1);
                    excelcells.ColumnWidth = 5;
                    excelcells.Value2 = "Наименование услуг";
                    excelcells = excelcells.get_Offset(0, 1);
                    excelcells.ColumnWidth = 20;
                    excelcells.Value2 = "Кол-во";
                    excelcells = excelcells.get_Offset(0, 1);
                    excelcells.ColumnWidth = 15;
                    excelcells.Value2 = "Цена";
                    excelcells = excelcells.get_Offset(0, 1);
                    excelcells.ColumnWidth = 15;
                    excelcells.Value2 = "Сумма";
                    excelcells = excelcells.get_Offset(1, -4);

                    for(int i = 0; i < order.ServiceOrders.Count; i++)
                    {
                        excelcells.ColumnWidth = 5;
                        excelcells.Value2 = (i + 1);
                        excelcells = excelcells.get_Offset(0, 1);
                        excelcells.ColumnWidth = 20;
                        excelcells.Value2 = order.ServiceOrders[i].ServiceName;
                        excelcells = excelcells.get_Offset(0, 1);
                        excelcells.ColumnWidth = 15;
                        excelcells.Value2 = order.ServiceOrders[i].Count;
                        excelcells = excelcells.get_Offset(0, 1);
                        excelcells.ColumnWidth = 15;
                        excelcells.Value2 = order.ServiceOrders[i].Price;
                        excelcells = excelcells.get_Offset(0, 1);
                        excelcells.ColumnWidth = 15;
                        excelcells.Value2 = order.ServiceOrders[i].Total;
                        excelcells = excelcells.get_Offset(1, -4);
                    }
                    excelcells = excelcells.get_Offset(0, 3);
                    excelcells.ColumnWidth = 15;
                    excelcells.Value2 = "Итого:";
                    excelcells = excelcells.get_Offset(0, 1);
                    excelcells.ColumnWidth = 15;
                    excelcells.Value2 = order.Sum;
                }
                //сохраняем
                excel.Workbooks[1].Save();
                excel.Workbooks[1].Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //закрываем
                excel.Quit();
            }
            await SendMail(order.Mail, "Счет на оплату в ветеринарной клинике Mentula", "Счет на оплату № " + order.Id, model.FileName);
            File.Delete(model.FileName);
        }

        public async System.Threading.Tasks.Task SendClientAccountDoc(ReportBindingModel model)
        {
            OrderViewModel order = await GetOrder(model.OrderId);

            var ms = (DateTime.Now - DateTime.MinValue).TotalMilliseconds;
            model.FileName += order.ClientFIO + "_Account_№" + order.Id + "_" + ms + ".doc";

            var winword = new Microsoft.Office.Interop.Word.Application();
            try
            {
                object missing = System.Reflection.Missing.Value;
                //создаем документ
                Microsoft.Office.Interop.Word.Document document =
                    winword.Documents.Add(ref missing, ref missing, ref missing, ref missing);
                //получаем ссылку на параграф
                var paragraph = document.Paragraphs.Add(missing);
                var range = paragraph.Range;
                //задаем текст
                range.Text = "Счет на оплату № " + order.Id + " от " + order.DateCreate;
                //задаем настройки шрифта
                var font = range.Font;
                font.Size = 16;
                font.Name = "Times New Roman";
                font.Bold = 1;
                //задаем настройки абзаца
                var paragraphFormat = range.ParagraphFormat;
                paragraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                paragraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
                paragraphFormat.SpaceAfter = 10;
                paragraphFormat.SpaceBefore = 0;
                //добавляем абзац в документ
                range.InsertParagraphAfter();

                //создаем таблицу
                var paragraphTable = document.Paragraphs.Add(Type.Missing);
                var rangeTable = paragraphTable.Range;
                var table = document.Tables.Add(rangeTable, order.ServiceOrders.Count + 2, 5, ref missing, ref missing);

                font = table.Range.Font;
                font.Size = 14;
                font.Name = "Times New Roman";

                var paragraphTableFormat = table.Range.ParagraphFormat;
                paragraphTableFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
                paragraphTableFormat.SpaceAfter = 0;
                paragraphTableFormat.SpaceBefore = 0;
                //шабка
                table.Cell(1, 1).Range.Text = "№";
                table.Cell(1, 2).Range.Text = "Услуга";
                table.Cell(1, 3).Range.Text = "Количество";
                table.Cell(1, 4).Range.Text = "Цена";
                table.Cell(1, 5).Range.Text = "Сумма";

                for (int i = 0; i < order.ServiceOrders.Count; ++i)
                {
                    if (order.ServiceOrders[i] != null)
                    {
                        table.Cell(i + 2, 1).Range.Text = (i+1).ToString();
                        table.Cell(i + 2, 2).Range.Text = order.ServiceOrders[i].ServiceName;
                        table.Cell(i + 2, 3).Range.Text = order.ServiceOrders[i].Count.ToString();
                        table.Cell(i + 2, 4).Range.Text = order.ServiceOrders[i].Price.ToString();
                        table.Cell(i + 2, 5).Range.Text = order.ServiceOrders[i].Total.ToString();
                    }
                }
                table.Cell(order.ServiceOrders.Count + 2, 4).Range.Text = "Итого:";
                table.Cell(order.ServiceOrders.Count + 2, 5).Range.Text = order.Sum.ToString();
                //задаем границы таблицы
                table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleInset;
                table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                range.InsertParagraphAfter();
                //сохраняем
                object fileFormat = WdSaveFormat.wdFormatXMLDocument;
                document.SaveAs(model.FileName, ref fileFormat, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing);
                document.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                winword.Quit();
            }
            await SendMail(order.Mail, "Долги по услугам в ветеринарной клинике Mentula", "Отчет по долгам " + order.ClientFIO + " за " + DateTime.Now.ToLongDateString(), model.FileName);
            File.Delete(model.FileName);
        }

        public async System.Threading.Tasks.Task SendClientsCredits(ReportBindingModel model)
        {
            var list = await GetClientCredits(model);
            await System.Threading.Tasks.Task.Run(() => StartSendCredits(list, model.FileName));
        }

        public System.Threading.Tasks.Task StartSendCredits(List<ClientCreaditViewModel> list, string TempPath)
        {
            CountdownEvent countdown = new CountdownEvent(1);
            foreach (var client in list)
            {
                countdown.AddCount();

                System.Threading.Tasks.Task.Run(async() =>
                {
                    await SendClientCreditDoc(client, TempPath);
                    countdown.Signal();
                });
            }
            countdown.Signal();

            countdown.Wait();

            return System.Threading.Tasks.Task.Run(() => true);
        }

        public async Task<List<PayViewModel>> GetPays(ReportBindingModel model)
        {
            return await context.Pays.Where(rec => rec.DateCreate >= model.DateFrom && rec.DateCreate <= model.DateTo).Include(rec => rec.Order.Client)
                .Select(rec => new PayViewModel
                {
                    ClientFIO = rec.Order.Client.ClientFIO,
                    OrderId  = rec.OrderId,
                    DateCreate = SqlFunctions.DateName("dd", rec.DateCreate) + " " +
                                            SqlFunctions.DateName("mm", rec.DateCreate) + " " +
                                            SqlFunctions.DateName("yyyy", rec.DateCreate),
                    Sum = rec.Summ
                }).ToListAsync();
        }

        public async System.Threading.Tasks.Task SendMail(string mailto, string caption, string message, string path = null)
        {
            System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
            SmtpClient stmpClient = null;
            try
            {
                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["MailLogin"]);
                mailMessage.To.Add(new MailAddress(mailto));
                mailMessage.Subject = caption;
                mailMessage.Body = message;
                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                if (path != null)
                {
                    mailMessage.Attachments.Add(new Attachment(path));
                }

                stmpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(ConfigurationManager.AppSettings["MailLogin"].Split('@')[0],
                    ConfigurationManager.AppSettings["MailPassword"])
                };
                await stmpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                mailMessage.Dispose();
                mailMessage = null;
                stmpClient = null;
            }
        }

        public async System.Threading.Tasks.Task SavePays(ReportBindingModel model)
        {
            var list = await GetPays(model);

            //открываем файл для работы
            FileStream fs = new FileStream(model.FileName, FileMode.OpenOrCreate, FileAccess.Write);
            //создаем документ, задаем границы, связываем документ и поток
            iTextSharp.text.Document doc = new iTextSharp.text.Document();
            doc.SetMargins(0.5f, 0.5f, 0.5f, 0.5f);
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            doc.Open();
            BaseFont baseFont = BaseFont.CreateFont(model.FontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            //вставляем заголовок
            var phraseTitle = new Phrase("Отчет по оплатам",
                new iTextSharp.text.Font(baseFont, 16, iTextSharp.text.Font.BOLD));
            iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph(phraseTitle)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 12
            };
            doc.Add(paragraph);

            var phrasePeriod = new Phrase("c " + model.DateFrom.ToShortDateString() +
                                    " по " + model.DateTo.ToShortDateString(),
                                    new iTextSharp.text.Font(baseFont, 14, iTextSharp.text.Font.BOLD));
            paragraph = new iTextSharp.text.Paragraph(phrasePeriod)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 12
            };
            doc.Add(paragraph);

            //вставляем таблицу, задаем количество столбцов, и ширину колонок
            PdfPTable table = new PdfPTable(3)
            {
                TotalWidth = 800F
            };
            table.SetTotalWidth(new float[] { 160, 160, 140});
            //вставляем шапку
            PdfPCell cell = new PdfPCell();
            var fontForCellBold = new iTextSharp.text.Font(baseFont, 10, iTextSharp.text.Font.BOLD);
            table.AddCell(new PdfPCell(new Phrase("ФИО клиента", fontForCellBold))
            {
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            table.AddCell(new PdfPCell(new Phrase("Дата", fontForCellBold))
            {
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            table.AddCell(new PdfPCell(new Phrase("Сумма", fontForCellBold))
            {
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            
            var fontForCells = new iTextSharp.text.Font(baseFont, 10);
            for (int i = 0; i < list.Count; i++)
            {
                cell = new PdfPCell(new Phrase(list[i].ClientFIO, fontForCells));
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(list[i].DateCreate, fontForCells));
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(list[i].Sum.ToString(), fontForCells))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                table.AddCell(cell);
            }
            //вставляем итого
            cell = new PdfPCell(new Phrase("Итого:", fontForCellBold))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Colspan = 1,
                Border = 0
            };
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(list.Select(rec=>rec.Sum).DefaultIfEmpty(0).Sum().ToString(), fontForCellBold))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = 0
            };
            table.AddCell(cell);
            /*cell = new PdfPCell(new Phrase("", fontForCellBold))
            {
                Border = 0
            };
            table.AddCell(cell);*/
            //вставляем таблицу
            doc.Add(table);

            doc.Close();
        }

        public void SaveTourPriceW(ReportBindingModel model)
        {
            if (File.Exists(model.FileName))
            {
                File.Delete(model.FileName);
            }

            var winword = new Microsoft.Office.Interop.Word.Application();
            try
            {
                object missing = System.Reflection.Missing.Value;
                //создаем документ
                Microsoft.Office.Interop.Word.Document document =
                    winword.Documents.Add(ref missing, ref missing, ref missing, ref missing);
                //получаем ссылку на параграф
                var paragraph = document.Paragraphs.Add(missing);
                var range = paragraph.Range;
                //задаем текст
                range.Text = "Список туров";
                //задаем настройки шрифта
                var font = range.Font;
                font.Size = 16;
                font.Name = "Times New Roman";
                font.Bold = 1;
                //задаем настройки абзаца
                var paragraphFormat = range.ParagraphFormat;
                paragraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                paragraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
                paragraphFormat.SpaceAfter = 10;
                paragraphFormat.SpaceBefore = 0;
                //добавляем абзац в документ
                range.InsertParagraphAfter();

                var tours = context.Services.ToList();
                //создаем таблицу
                var paragraphTable = document.Paragraphs.Add(Type.Missing);
                var rangeTable = paragraphTable.Range;
                var table = document.Tables.Add(rangeTable, tours.Count, 2, ref missing, ref missing);

                font = table.Range.Font;
                font.Size = 14;
                font.Name = "Times New Roman";

                var paragraphTableFormat = table.Range.ParagraphFormat;
                paragraphTableFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
                paragraphTableFormat.SpaceAfter = 0;
                paragraphTableFormat.SpaceBefore = 0;

                for (int i = 0; i < tours.Count; ++i)
                {
                    table.Cell(i + 1, 1).Range.Text = tours[i].ServiceName;
                    table.Cell(i + 1, 2).Range.Text = tours[i].Price.ToString();
                }
                //задаем границы таблицы
                table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleInset;
                table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

                paragraph = document.Paragraphs.Add(missing);
                range = paragraph.Range;

                range.Text = "Дата: " + DateTime.Now.ToLongDateString();

                font = range.Font;
                font.Size = 12;
                font.Name = "Times New Roman";

                paragraphFormat = range.ParagraphFormat;
                paragraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
                paragraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
                paragraphFormat.SpaceAfter = 10;
                paragraphFormat.SpaceBefore = 10;

                range.InsertParagraphAfter();
                //сохраняем
                object fileFormat = WdSaveFormat.wdFormatXMLDocument;
                document.SaveAs(@"D:/Doc.docx", ref fileFormat, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing);
                document.Close(ref missing, ref missing, ref missing);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                winword.Quit();
            }
        }

        public void SaveTourPriceE(ReportBindingModel model)
        {
            var excel = new Microsoft.Office.Interop.Excel.Application();
            try
            {
                //или создаем excel-файл, или открываем существующий
                if (File.Exists(model.FileName))
                {
                    excel.Workbooks.Open(@"D:/Exel.xlsx", Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing);
                }
                else
                {
                    excel.SheetsInNewWorkbook = 1;
                    excel.Workbooks.Add(Type.Missing);
                    excel.Workbooks[1].SaveAs(@"D:/Exel.xlsx", XlFileFormat.xlExcel8, Type.Missing,
                        Type.Missing, false, false, XlSaveAsAccessMode.xlNoChange, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                }

                Sheets excelsheets = excel.Workbooks[1].Worksheets;
                //Получаем ссылку на лист
                var excelworksheet = (Worksheet)excelsheets.get_Item(1);
                //очищаем ячейки
                excelworksheet.Cells.Clear();
                //настройки страницы
                excelworksheet.PageSetup.Orientation = XlPageOrientation.xlLandscape;
                excelworksheet.PageSetup.CenterHorizontally = true;
                excelworksheet.PageSetup.CenterVertically = true;
                //получаем ссылку на первые 3 ячейки
                Microsoft.Office.Interop.Excel.Range excelcells = excelworksheet.get_Range("A1", "C1");
                //объединяем их
                excelcells.Merge(Type.Missing);
                //задаем текст, настройки шрифта и ячейки
                excelcells.Font.Bold = true;
                excelcells.Value2 = "Список туров";
                excelcells.RowHeight = 25;
                excelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                excelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                excelcells.Font.Name = "Times New Roman";
                excelcells.Font.Size = 14;

                excelcells = excelworksheet.get_Range("A2", "C2");
                excelcells.Merge(Type.Missing);
                excelcells.Value2 = "на " + DateTime.Now.ToShortDateString();
                excelcells.RowHeight = 20;
                excelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                excelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                excelcells.Font.Name = "Times New Roman";
                excelcells.Font.Size = 12;

                var dict = context.Services.ToList();
                for (int i = 0; i < dict.Count; i++)
                {
                    excelcells = excelworksheet.get_Range("C1", "C1");
                    excelcells = excelcells.get_Offset(i + 2, -2);
                    excelcells.ColumnWidth = 15;
                    excelcells.Value2 = dict[i].ServiceName;
                    excelcells = excelcells.get_Offset(0, 1);
                    excelcells.ColumnWidth = 15;
                    excelcells.Value2 = dict[i].Price;
                    excelcells.Font.Bold = true;
                }
                //сохраняем
                excel.Workbooks[1].Save();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //закрываем
                excel.Quit();
            }
        }

        public List<ClientOrdersViewModel> GetClientOrders(ReportBindingModel model)
        {
            return context.Orders
                            .Include(rec => rec.Client)
                            .Include(rec => rec.ServiceOrders)
                            .Where(rec => rec.DateCreate >= model.DateFrom && rec.DateCreate <= model.DateTo)
                            .Select(rec => new ClientOrdersViewModel
                            {
                                ClientName = rec.Client.ClientFIO,
                                DateOfCreate = SqlFunctions.DateName("dd", rec.DateCreate) + " " +
                                            SqlFunctions.DateName("mm", rec.DateCreate) + " " +
                                            SqlFunctions.DateName("yyyy", rec.DateCreate),
                                ServiceName = rec.ServiceOrders.Select(r => r.Service.ServiceName).ToList(),
                                Summa = rec.ServiceOrders.Select(r => r.Price).ToList(),
                                Status = rec.OrderStatus.ToString()
                            })
                            .ToList();
        }

        public void SaveClientOrders(ReportBindingModel model)
        {
            //открываем файл для работы
            FileStream fs = new FileStream(@"D:\REPRNDJS.pdf", FileMode.OpenOrCreate, FileAccess.Write);
            //создаем документ, задаем границы, связываем документ и поток
            iTextSharp.text.Document doc = new iTextSharp.text.Document();
            doc.SetMargins(0.5f, 0.5f, 0.5f, 0.5f);
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            doc.Open();
            BaseFont baseFont = BaseFont.CreateFont("TIMCYR.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            //вставляем заголовок
            var phraseTitle = new Phrase("Заказы клиентов",
                new iTextSharp.text.Font(baseFont, 16, iTextSharp.text.Font.BOLD));
            iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph(phraseTitle)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 12
            };
            doc.Add(paragraph);

            var phrasePeriod = new Phrase("c " + model.DateFrom.ToShortDateString() +
                               " по " + model.DateTo.ToShortDateString(),
                               new iTextSharp.text.Font(baseFont, 14, iTextSharp.text.Font.BOLD));
            paragraph = new iTextSharp.text.Paragraph(phrasePeriod)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 12
            };
            doc.Add(paragraph);

            //вставляем таблицу, задаем количество столбцов, и ширину колонок
            PdfPTable table = new PdfPTable(6)
            {
                TotalWidth = 800F
            };
            table.SetTotalWidth(new float[] { 160, 140, 160, 100, 100, 140 });
            //вставляем шапку
            PdfPCell cell = new PdfPCell();
            var fontForCellBold = new iTextSharp.text.Font(baseFont, 10, iTextSharp.text.Font.BOLD);
            table.AddCell(new PdfPCell(new Phrase("ФИО клиента", fontForCellBold))
            {
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            table.AddCell(new PdfPCell(new Phrase("Дата создания", fontForCellBold))
            {
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            table.AddCell(new PdfPCell(new Phrase("Путешествие", fontForCellBold))
            {
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            table.AddCell(new PdfPCell(new Phrase("Дни", fontForCellBold))
            {
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            table.AddCell(new PdfPCell(new Phrase("Сумма", fontForCellBold))
            {
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            table.AddCell(new PdfPCell(new Phrase("Статус", fontForCellBold))
            {
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            //заполняем таблицу
            var list = GetClientOrders(model);
            var fontForCells = new iTextSharp.text.Font(baseFont, 10);
            for (int i = 0; i < list.Count; i++)
            {
                cell = new PdfPCell(new Phrase(list[i].ClientName, fontForCells));
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(list[i].DateOfCreate, fontForCells));
                table.AddCell(cell);
                for(int j=0; j< list[i].ServiceName.Count; j++) { 
                cell = new PdfPCell(new Phrase(list[i].ServiceName[j], fontForCells));
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(list[i].Summa[j].ToString(), fontForCells));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell);
                }
                cell = new PdfPCell(new Phrase(list[i].Status, fontForCells));
                table.AddCell(cell);
            }
            //вставляем итого
            cell = new PdfPCell(new Phrase("Итого:", fontForCellBold))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Colspan = 4,
                Border = 0
            };
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("", fontForCellBold))
            {
                Border = 0
            };
            table.AddCell(cell);
            //вставляем таблицу
            doc.Add(table);

            doc.Close();
        }
    }
}
