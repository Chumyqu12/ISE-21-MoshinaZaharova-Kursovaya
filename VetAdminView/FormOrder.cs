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
    public partial class FormOrder : Form
    {

        private List<ServiceOrderViewModel> serviceOrders;

        public FormOrder()
        {
            InitializeComponent();
        }

        private void FormOrder_Load(object sender, EventArgs e)
        {
            try
            {
                List<ClientViewModel> list = Task.Run(() => ApiClient.GetRequestData<List<ClientViewModel>>("api/Client/GetList")).Result;
                if (list != null)
                {
                    comboBoxClient.DisplayMember = "ClientFIO";
                    comboBoxClient.ValueMember = "Id";
                    comboBoxClient.DataSource = list;
                    comboBoxClient.SelectedItem = null;
                }
                dateTimePickerCredit.Value = DateTime.Now.AddMonths(3);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            serviceOrders = new List<ServiceOrderViewModel>();

        }

        private void LoadData()
        {
            try
            {
                if (serviceOrders != null)
                {
                    dataGridView.DataSource = null;
                    dataGridView.DataSource = serviceOrders;
                    dataGridView.Columns[0].Visible = false;
                    dataGridView.Columns[1].Visible = false;
                    dataGridView.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var form = new FormAddService();
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (form.Model != null)
                {
                    serviceOrders.Add(form.Model);
                }
                textBoxSum.Text = serviceOrders.Select(rec => rec.Total).DefaultIfEmpty(0).Sum().ToString();
                LoadData();
            }
        }

        private void buttonUpd_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 1)
            {
                var form = new FormAddService();
                form.Model = serviceOrders[dataGridView.SelectedRows[0].Cells[0].RowIndex];
                if (form.ShowDialog() == DialogResult.OK)
                {
                    serviceOrders[dataGridView.SelectedRows[0].Cells[0].RowIndex] = form.Model;
                    LoadData();
                }
            }
        }

        private void buttonDel_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 1)
            {
                if (MessageBox.Show("Удалить запись", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        serviceOrders.RemoveAt(dataGridView.SelectedRows[0].Cells[0].RowIndex);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    LoadData();
                }
            }
        }

        private void buttonRef_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (comboBoxClient.SelectedValue == null)
            {
                MessageBox.Show("Выберите клиента", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (serviceOrders == null || serviceOrders.Count == 0)
            {
                MessageBox.Show("Заполните компоненты", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (dateTimePickerCredit.Value < DateTime.Now)
            {
                MessageBox.Show("Ошибка в дате окончания кредита", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            List<ServiceOrderBindingModel> serviceOrderBM = new List<ServiceOrderBindingModel>();
            foreach (var serviceOrder in serviceOrders)
            {
                serviceOrderBM.Add(new ServiceOrderBindingModel
                {
                    ServiceId = serviceOrder.ServiceId,
                    Count = serviceOrder.Count
                });
            }

            int clientId = Convert.ToInt32(comboBoxClient.SelectedValue);
            Task task = Task.Run(() => ApiClient.PostRequestData("api/Order/AddElement", new OrderBindingModel
            {
                ClientId = clientId,
                CreditEnd = dateTimePickerCredit.Value,
                ServiceOrders = serviceOrderBM
            }));


            task.ContinueWith((prevTask) => MessageBox.Show("Сохранение прошло успешно. Обновите список", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information),
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

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
