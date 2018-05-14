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
    public partial class FormPenalty : Form
    {
        public int Id { set { id = value; } }

        private int? id;

        public FormPenalty()
        {
            InitializeComponent();
        }

        private void FormPenalty_Load(object sender, EventArgs e)
        {
            if (id.HasValue)
            {
                try
                {
                    var client = Task.Run(() => ApiClient.GetRequestData<ClientViewModel>("api/client/Get/" + id.Value)).Result;
                    textBoxFIO.Text = client.ClientFIO;
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
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxSum.Text))
            {
                MessageBox.Show("Заполните поле Сумма штрафа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int sum = 0;
            try
            {
                sum = Convert.ToInt32(textBoxSum.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Task task = Task.Run(() => ApiClient.PutRequestData("api/client/DecreaseBonuses", new RetributionBindingModel
            {
                ClientId = id.Value,
                Penalty = sum
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
