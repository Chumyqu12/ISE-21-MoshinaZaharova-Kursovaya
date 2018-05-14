using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VetService.VetBindingModels;
using VetService.VetViewModels;

namespace VetAdminView
{
    public partial class FormClientOrder : Form
    {
        public int Id { set { id = value; } }

        private int? id;

        public FormClientOrder()
        {
            InitializeComponent();
        }

        private void FormClientOrder_Load(object sender, EventArgs e)
        {
            try
            {
                List<OrderViewModel> list =
                    Task.Run(() => ApiClient.GetRequestData<List<OrderViewModel>>("api/Order/GetClientList/" + id.Value)).Result;
                if (list != null)
                {
                    dataGridView.DataSource = list;
                    dataGridView.Columns[0].Visible = false;
                    dataGridView.Columns[1].Visible = false;
                    //dataGridView.Columns[7].Visible = false;
                    //dataGridView.Columns[8].Visible = false;
                    //dataGridView.Columns[9].Visible = false;
                    dataGridView.Columns[10].Visible = false;
                    // dataGridView.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridView.AutoResizeColumns();
                    dataGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (!checkBoxDoc.Checked && !checkBoxXls.Checked)
            {
                MessageBox.Show("Выберите формат документа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int id = Convert.ToInt32(dataGridView.SelectedRows[0].Cells[0].Value);
            Task task = Task.Run(() => ApiClient.PostRequestData("api/report/" + ((checkBoxDoc.Checked) ? "SendClientAccountDoc" : "SendClientAccountXls"), new ReportBindingModel
            {
                OrderId = id
            }));

            task.ContinueWith((prevTask) => MessageBox.Show("Сообщение отправлено", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information),
                TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith((prevTask) =>
            {
                var ex = (Exception)prevTask.Exception;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }, TaskContinuationOptions.OnlyOnFaulted);

            Close();
        }
    }
}
