using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VetService.VetViewModels;

namespace VetAdminView
{
    public partial class FormAddService : Form
    {
        public ServiceOrderViewModel Model { get; set; }

        public FormAddService()
        {
            InitializeComponent();
        }

        private void FormAddService_Load(object sender, EventArgs e)
        {
            try
            {
                comboBoxService.DisplayMember = "ServiceName";
                comboBoxService.ValueMember = "Id";
                comboBoxService.DataSource = Task.Run(() => ApiClient.GetRequestData<List<ServiceViewModel>>("api/Service/GetList")).Result;
                comboBoxService.SelectedItem = null;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (Model != null)
            {
                comboBoxService.Enabled = false;
                comboBoxService.SelectedValue = Model.ServiceId;
                textBoxCount.Text = Model.Count.ToString();
            }
        }

        private async void buttonSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxCount.Text))
            {
                MessageBox.Show("Заполните поле Количество", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (comboBoxService.SelectedValue == null)
            {
                MessageBox.Show("Выберите услугу", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                if (Model == null)
                {
                    Model = new ServiceOrderViewModel
                    {
                        ServiceId = Convert.ToInt32(comboBoxService.SelectedValue),
                        ServiceName = comboBoxService.Text,
                        Count = Convert.ToInt32(textBoxCount.Text)
                    };
                }
                else
                {
                    Model.Count = Convert.ToInt32(textBoxCount.Text);
                }
                var model = await Task.Run(() => ApiClient.GetRequestData<ServiceViewModel>("api/Service/Get/" + Model.ServiceId));
                Model.Price = model.Price;
                Model.Total = Model.Price * Model.Count;
                MessageBox.Show("Сохранение прошло успешно", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
