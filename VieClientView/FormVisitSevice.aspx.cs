using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VetService.VetBindingModels;
using VetService.VetViewModels;

namespace VetClientView
{
    public partial class FormVisitSevice : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                List<ServiceViewModel> listP = Task.Run(() => APIСlient.GetRequestData<List<ServiceViewModel>>("api/Service/GetList")).Result;
                if (listP != null)
                {
                    DropDownListVisit.DataSource = listP;
                    DropDownListVisit.DataBind();
                    DropDownListVisit.DataTextField = "ServiceName";
                    DropDownListVisit.DataValueField = "Id";
                }

                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Что-то пошло не так');</script>");
                }

                Page.DataBind();

            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('" + ex.Message + "');</script>");
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (DropDownListVisit.SelectedValue == null)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Выберите услугу');</script>");
                return;
            }
            int serviceid = Convert.ToInt32(DropDownListVisit.SelectedValue);
            string servicename = DropDownListVisit.SelectedItem.Text;
            Task task = Task.Run(() => APIСlient.PostRequestData("api/Visit/AddElement", new VisitServiceBindingModel
            {
                ServiceId= serviceid,
                ServiceName= servicename

            }));

            task.ContinueWith((prevTask) => Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Сохранение прошло успешно');</script>"),
                TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith((prevTask) =>
            {
                var ex = (Exception)prevTask.Exception;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('" + ex.Message + "');</script>");
            }, TaskContinuationOptions.OnlyOnFaulted);
            Server.Transfer("FormVisit.aspx");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Server.Transfer("FormVisit.aspx");
        }
    }
}