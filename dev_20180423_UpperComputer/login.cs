using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dev_20180423_UpperComputer
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
            this.textBox2.PasswordChar = '*';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == "" || this.textBox2.Text == "")
            {
                MessageBox.Show("用户名和登录密码不能为空！");
                return;
            }
            else {
                if (this.textBox1.Text == "admin" && this.textBox2.Text == "123456")
                {
                    new UpperComputer().Show();
                    this.Hide();
                }
                else {
                    MessageBox.Show("用户名和登录密码错误！");
                    return;
                }
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
