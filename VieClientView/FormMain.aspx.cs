using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using VetService.VetBindingModels;
using VetService.VetViewModels;

namespace VetClientView
{
    public partial class FormMain : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {

                List<OrderViewModel> list = Task.Run(() => APIСlient.GetRequestData<List<OrderViewModel>>("api/Order/GetList")).Result;
                if (list != null)
                {
                    dataGridView1.DataSource = list;
                    dataGridView1.AutoGenerateSelectButton = true;
                    dataGridView1.DataBind();
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

        protected void ButtonCreateIndent_Click(object sender, EventArgs e)
        {
            Server.Transfer("FormCreateZakaz.aspx");
        }
        protected void ButtonIndentReady_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedIndex >= 0)
            {
                int id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedIndex].Cells[1]);

                Task task = Task.Run(() => APIСlient.PostRequestData("api/Order/FinishOrder", new OrderBindingModel
                {
                    Id = id
                }));

                task.ContinueWith((prevTask) => Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Заказ готов');</script>"),
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
            }
        }

        protected void ButtonIndentPayed_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedIndex >= 0)
            {
                int id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedIndex].Cells[1]);
                Task task = Task.Run(() => APIСlient.PostRequestData("api/Order/PayOrder", new OrderBindingModel
                {
                    Id = id
                }));

                task.ContinueWith((prevTask) => Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Статус заказа изменён');</script>"),
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
            }
        }

        protected void ButtonUpd_Click(object sender, EventArgs e)
        {
            LoadData();
            Server.Transfer("FormMain.aspx");
        }

        protected void ButtonIndent1Payed_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedIndex >= 0)
            {
                int id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedIndex].Cells[1]);
                Task task = Task.Run(() => APIСlient.PostRequestData("api/Order/PolPayOrder", new OrderBindingModel
                {
                    Id = id
                }));

                task.ContinueWith((prevTask) => Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Статус заказа изменён');</script>"),
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
            }
        }


    }

    private void докToolStripMenuItem_Click(object sender, EventArgs e)
    {
        SaveFileDialog sfd = new SaveFileDialog
        {
            Filter = "doc|*.doc|docx|*.docx"
        };

        if (sfd.ShowDialog() == true)
        {

            try
            {

                reportService.SaveTourPriceW(new ReportBindingModel
                {
                    FileName = sfd.FileName
                });
                System.Windows.MessageBox.Show("Выполнено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ексельToolStripMenuItem_Click(object sender, EventArgs e)
    {
        SaveFileDialog sfd = new SaveFileDialog
        {
            Filter = "xls|*.xls|xlsx|*.xlsx"
        };
        if (sfd.ShowDialog() == true)
        {
            try
            {
                reportService.SaveTourPriceE(new ReportBindingModel
                {
                    FileName = sfd.FileName
                });
                System.Windows.MessageBox.Show("Выполнено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}