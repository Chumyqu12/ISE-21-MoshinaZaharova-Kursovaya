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
    public partial class FormCreateZakaz : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                List<VisitViewModel> listP = Task.Run(() => APIСlient.GetRequestData<List<VisitViewModel>>("api/Visit/GetList")).Result;
                if (listP != null)
                {
                    DropDownListVisit.DataSource = listP;
                    DropDownListVisit.DataBind();
                    DropDownListVisit.DataTextField = "VisitName";
                    DropDownListVisit.DataValueField = "Id";
                }

                List<ClientViewModel> listC = Task.Run(() => APIСlient.GetRequestData<List<ClientViewModel>>("api/Client/GetList")).Result; ;
                if (listC != null)
                {
                    DropDownListClient.DataSource = listC;
                    DropDownListClient.DataBind();
                    DropDownListClient.DataTextField = "ClientFIO";
                    DropDownListClient.DataValueField = "Id";
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

        protected void ButtonSave_Click(object sender, EventArgs e)
        {
            if (DropDownListClient.SelectedValue == null)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Выберите клиента');</script>");
                return;
            }
            int clientId = Convert.ToInt32(DropDownListClient.SelectedValue);
            int productid = Convert.ToInt32(DropDownListVisit.SelectedValue);
            int summ = Convert.ToInt32(TextBoxPrice.Text);
            Task task = Task.Run(() => APIСlient.PostRequestData("api/Order/CreateOrder", new OrderBindingModel
            {
                ClientId = clientId,
                VisitId = productid,
                Summa=summ
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
            Server.Transfer("FormMain.aspx");
        }

        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            Server.Transfer("FormMain.aspx");

        }

        protected void ButtonVisit_Click(object sender, EventArgs e)
        {
            Server.Transfer("FormVisits.aspx");
        }

        private void CalcSum()
        {

            if (DropDownListVisit.SelectedValue != null)
            {

                try
                {
                    int id = Convert.ToInt32(DropDownListVisit.SelectedValue);

                    VisitViewModel product = Task.Run(() => APIСlient.GetRequestData<VisitViewModel>("api/Visit/Get/" + id)).Result;
                    int count = Convert.ToInt32(product.Price);
                    TextBoxPrice.Text = count.ToString();
                    Page.DataBind();

                }
                catch (Exception ex)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('" + ex.Message + "');</script>");
                }
            }
        }
    }
}