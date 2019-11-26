using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chatapp
{
    /*
     * LoginForm - Энэ нь програм эхлэх үед гарч ирэх цонх юм. 
     */
    public partial class LoginForm : Form
    {
        public string UserName = null;

        public LoginForm()
        {
            InitializeComponent();
        }

        /* Энэ функц нь OK button дарагдах үед дуудагдах функц.
         *  1. userNameTextBox-д бичигдсэн текстийг UserName хувьсагчид хадгална.
         *  2. Хадгалж авсныхаа дараа цонхоо хаана.
         */
        private void okButton_Click(object sender, EventArgs e)
        {
            UserName = userNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(UserName))
            {
                MessageBox.Show("Please enter a user name");
                return;
            }
            this.Close();
        }
    }
}
