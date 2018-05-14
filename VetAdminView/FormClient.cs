using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using VetService.VetBindingModels;
using VetService.VetViewModels;

namespace VetAdminView
{
    public partial class FormClient : Form
    {
        public int Id { set { id = value; } }

        private int? id;


        public FormClient()
        {
            InitializeComponent();
        }

        private void FormClient_Load(object sender, EventArgs e)
        {
            if (id.HasValue)
            {
                try
                {
                    var client = Task.Run(() => ApiClient.GetRequestData<ClientViewModel>("api/Client/Get/" + id.Value)).Result;
                    textBoxFIO.Text = client.ClientFIO;
                    textBoxLogin.Text = client.UserName;
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
            if (!string.IsNullOrEmpty(login))
            {
                if (!Regex.IsMatch(login, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$"))
                {
                    MessageBox.Show("Неверный формат для электронной почты", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            Task task;
            if (id.HasValue)
            {
                task = Task.Run(() => ApiClient.PutRequestData("api/Client/UpdElement", new ClientBindingModel
                {
                    Id = id.Value,
                    ClientFIO = fio,
                    UserName = login
                }));
            }
            else
            {
                if (string.IsNullOrEmpty(textBoxPassword.Text))
                {
                    MessageBox.Show("Заполните Пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                task = Task.Run(() => ApiClient.PostRequestData("api/Client/AddElement", new ClientCreateModel
                {
                    ClientFIO = fio,
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

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
