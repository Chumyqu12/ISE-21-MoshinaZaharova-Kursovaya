using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VetService.VetBindingModels;

namespace VetClientView
{
    public partial class FormDoc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (TextBox1.Text == null) {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Заполните мейл');</script>");
                return;
            }

            SmtpClient client = new SmtpClient("smtp.yandex.ru", 25);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("kyrsach1337yandex.ru", "89176294987");
            string mail = TextBox1.Text;
            if (!string.IsNullOrEmpty(mail))
            {
                if (!Regex.IsMatch(mail, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$"))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Неверный формат для электронной почты');</script>");
                    return;
                }
            }
            Task task = Task.Run(() => APIСlient.PostRequestData("api/Report/SaveTourPriceW", new ReportBindingModel
            {
                DateFrom =DateTime.Now,
                DateTo = DateTime.Now
            }));

            string subject = "PekaMarket";
            String text = "ОТчёты";
            string from = "kyrsach1337yandex.ru";
            MailMessage message = new MailMessage(from, mail, subject, text);
            try
            {
                Attachment sendfile = new Attachment(@"D:/Doc.docx");
                message.Attachments.Add(sendfile);

            }
            catch
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('ERRROR');</script>");
            }

            task.ContinueWith((prevTask) =>
            {
                var ex = (Exception)prevTask.Exception;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('" + ex.Message + "');</script>");
            }, TaskContinuationOptions.OnlyOnFaulted);

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            if (TextBox1.Text == null)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Заполните мейл');</script>");
                return;
            }

            SmtpClient client = new SmtpClient("smtp.yandex.ru", 25);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("kyrsach1337yandex.ru", "89176294987");
            string mail = TextBox1.Text;
            if (!string.IsNullOrEmpty(mail))
            {
                if (!Regex.IsMatch(mail, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$"))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Неверный формат для электронной почты');</script>");
                    return;
                }
            }
            Task task = Task.Run(() => APIСlient.PostRequestData("api/Report/SaveTourPriceW", new ReportBindingModel
            {
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now
            }));

            string subject = "PekaMarket";
            String text = "ОТчёты";
            string from = "kyrsach1337yandex.ru";
            MailMessage message = new MailMessage(from, mail, subject, text);
            try
            {
                Attachment sendfile = new Attachment(@"D:/Exel.xlsx");
                message.Attachments.Add(sendfile);

            }
            catch
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('ERRROR');</script>");
            }

            task.ContinueWith((prevTask) =>
            {
                var ex = (Exception)prevTask.Exception;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('" + ex.Message + "');</script>");
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}