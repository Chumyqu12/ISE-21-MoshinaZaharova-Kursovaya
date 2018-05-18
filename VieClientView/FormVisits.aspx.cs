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
    public partial class FormVisits : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                List<VisitViewModel> list = Task.Run(() => APIСlient.GetRequestData<List<VisitViewModel>>("api/Visit/GetList")).Result;
                if (list != null)
                {
                    DataGridView.DataSource = list;
                    DataGridView.AutoGenerateSelectButton = true;
                    DataGridView.DataBind();
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('" + ex.Message + "');</script>");
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Server.Transfer("FormVisit.aspx");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Session["id"] = DataGridView.Rows[DataGridView.SelectedIndex].Cells[1].Text;
            Server.Transfer("FormVisit.aspx");
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            if (DataGridView.SelectedIndex >= 0)
            {

                int id = Convert.ToInt32(DataGridView.Rows[DataGridView.SelectedIndex].Cells[1].Text);
                Task task = Task.Run(() => APIСlient.PostRequestData("api/Visit/DelElement", new VisitBindingModel { Id = id }));

                task.ContinueWith((prevTask) => Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Запись удалена');</script>"),
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

                Server.Transfer("FormVisits.aspx");
            }
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            Server.Transfer("FormVisit.aspx");
        }
    }
}