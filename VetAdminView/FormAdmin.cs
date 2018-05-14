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
    public partial class FormAdmin : Form
    {
        public int Id { set { id = value; } }

        private int? id;

        public FormAdmin()
        {
            InitializeComponent();
        }

        private void FormAdmin_Load(object sender, EventArgs e)
        {
            if (id.HasValue)
            {
                try
                {
                    var employee = Task.Run(() => ApiClient.GetRequestData<AdminViewModel>("api/admin/Get/" + id.Value)).Result;
                    textBoxFIO.Text = employee.AdminFIO;
                    textBoxLogin.Text = employee.UserName;
                    textBoxPassword.ReadOnly = true;
                    textBoxConfimPassword.ReadOnly = true;
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
            else
            {
                textBoxPassword.ReadOnly = false;
                textBoxConfimPassword.ReadOnly = false;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(textBoxFIO.Text))
            {
                MessageBox.Show("Заполните ФИО", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(textBoxLogin.Text))
            {
                MessageBox.Show("Заполните логин", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string fio = textBoxFIO.Text;
            string login = textBoxLogin.Text;
            Task task;
            if (id.HasValue)
            {
                task = Task.Run(() => ApiClient.PutRequestData("api/Admin/UpdElement", new AdminBindingModel
                {
                    Id = id.Value,
                    AdminFIO = fio,
                    UserName = login
                }));
            }
            else
            {
                if (string.IsNullOrEmpty(textBoxPassword.Text))
                {
                    MessageBox.Show("Заполните пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.IsNullOrEmpty(textBoxConfimPassword.Text))
                {
                    MessageBox.Show("Заполните Подтверждение пароля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!textBoxConfimPassword.Text.Equals(textBoxPassword.Text))
                {
                    MessageBox.Show("Пароли должны совпадать", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string password = textBoxPassword.Text;
                string confirmPassword = textBoxConfimPassword.Text;
                task = Task.Run(() => ApiClient.PostRequestData("api/Admin/AddElement", new AdminCreateModel
                {
                    AdminFIO = fio,
                    UserName = login,
                    PasswordHash = password,
                    ConfirmPassword = confirmPassword
                }));
            }
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
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

