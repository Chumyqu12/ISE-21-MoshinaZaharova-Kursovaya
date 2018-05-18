using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VetService.VetViewModels;

namespace VetClientView
{
    public partial class FormVisit : System.Web.UI.Page
    {
        private List<VisitServiceViewModel> VisitService;
        private int id;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Int32.TryParse((string)Session["id"], out id))
            {
                VisitViewModel v = Task.Run(() => APIСlient.GetRequestData<VisitViewModel>("api/Visit/GetElement" + id)).Result;
                TextBoxName.Text = v.VisitName;
                GridView1.DataSource = v.VisitService;
                GridView1.AutoGenerateSelectButton = true;
                GridView1.DataBind();

            }
            try
            {
                if (VisitService != null)
                {
                    List<VisitServiceViewModel> list = Task.Run(() => APIСlient.GetRequestData<List<VisitServiceViewModel>>("api/Client/GetList")).Result;
                    if (list != null)
                    {
                        GridView1.DataSource = list;
                        GridView1.AutoGenerateSelectButton = true;
                        GridView1.DataBind();
                    }
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
            Server.Transfer("FormVisitService.aspx");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Session["id"] = GridView1.Rows[GridView1.SelectedIndex].Cells[1].Text;
            Server.Transfer("FormVisitService.aspx");
            
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            if (GridView1.SelectedIndex >= 0)
            {

                int id = Convert.ToInt32(GridView1.Rows[GridView1.SelectedIndex].Cells[1].Text);
                VisitService.RemoveAt(id);

                Server.Transfer("FormVisit.aspx");
            }
    }
}