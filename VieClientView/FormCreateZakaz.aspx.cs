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
                List<ServiceViewModel> listP = Task.Run(() => APIСlient.GetRequestData<List<ServiceViewModel>>("api/Visit/GetList")).Result;
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
                Load_data();
                Page.DataBind();

            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('" + ex.Message + "');</script>");
            }
        }
        protected void Load_data() {
            List<ServiceOrderBindingModel> list = APIСlient.listS;
            if (list != null)
            {
                DataGridView.DataSource = list;
                DataGridView.AutoGenerateSelectButton = true;
                DataGridView.DataBind();
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
            int summ = Convert.ToInt32(TextBoxPrice.Text);
            Task task = Task.Run(() => APIСlient.PostRequestData("api/Order/CreateOrder", new OrderBindingModel
            {
                ClientId = clientId,
                ServiceOrders=APIСlient.listS,
                Summa= summ
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
            APIСlient.listS.Clear();
            Server.Transfer("FormMain.aspx");
        }

        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            Server.Transfer("FormMain.aspx");

        }

        protected void ButtonVisit_Click(object sender, EventArgs e)
        {
            if (DropDownListVisit.SelectedValue == null) {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Выберите услугу');</script>");
                return;
            }
            int productid = Convert.ToInt32(DropDownListVisit.SelectedValue);
            ServiceViewModel product = Task.Run(() => APIСlient.GetRequestData<ServiceViewModel>("api/Service/Get/" + productid)).Result;
            APIСlient.listS.Add(new ServiceOrderBindingModel
            {
                ServiceId = productid,
                Count = 1,
                Price=Convert.ToInt32(product.Price)
            });
            Load_data();
            DataBind();



        }

        private void CalcSum()
        {

            if (DropDownListVisit.SelectedValue != null)
            {

                try
                {
                    int price = 0;
                    for (int i = 0; i < APIСlient.listS.Count; i++) {
                        price += APIСlient.listS[i].Price;
                    }
                   
                    
                    TextBoxPrice.Text = price.ToString();
                    Page.DataBind();

                }
                catch (Exception ex)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('" + ex.Message + "');</script>");
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (DataGridView.SelectedIndex >= 0) {
                int id= Convert.ToInt32(DataGridView.Rows[DataGridView.SelectedIndex].Cells[1].Text);
                APIСlient.listS.RemoveAt(id);
                Load_data();
                DataBind();
            }
        }
    }
}