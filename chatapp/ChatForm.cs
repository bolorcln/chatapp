using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace chatapp
{
    public partial class chatForm : Form
    {
        private string myUserName = null;
        private Network network;
        private Thread checkMessage;
        private delegate void AddMessage(Message msg);
        private User otherUser;

        public chatForm()
        {
            InitializeComponent();
            network = new Network();
            InitializeChecker();
        }

        #region Functions
        // msg-г дэлгэцэнд хэвлэн харуулах функц
        private void putMessageToScreen(Message msg)
        {
            this.mainRichTextBox.Text += msg.senderName + " : ";
            this.mainRichTextBox.Text += msg.text + "\n";
        }

        // Socket-оор ирсэн message-нээс "<EOF>" гэсэн хэсгийг нь хасах функц
        private string cleanMessage(string message)
        {
            string msg = null;
            int idx = message.IndexOf("<EOF>");
            msg = message.Substring(0, idx);
            return msg;
        }

        // Checker нь socket-оор message ирсэн эсэхийг байн байн шалгадаг функц
        // хэрэв message ирсэн бол дэлгэц рүү хэвлэн харуулна.
        private void Checker()
        {
            AddMessage messageDelegate = putMessageToScreen;
            while (true)
            {
                if (network.message != null)
                {
                    string message = network.message;
                    network.message = null;
                    message = cleanMessage(message);
                    Message msg = new Message(otherUser.name, message);
                    Invoke(messageDelegate, msg);
                }
            }
        }

        // Үүсгэсэн Checker функцээ background-д нэг thread үүсгэн ажилуулна.
        private void InitializeChecker()
        {
            ThreadStart start = new ThreadStart(Checker);
            checkMessage = new Thread(start);
            checkMessage.IsBackground = true;
            checkMessage.Start();
        }
        #endregion

        #region Events
        /* ChatForm цонх анх ачааллах үед дуудагдах функц
         */
        private void chatForm_Load(object sender, EventArgs e)
        {
            // Програм эхлэх үед энэ цонхоо нууж LoginForm-оо харуулна.
            this.Hide();
            using (LoginForm loginForm = new LoginForm())
            {
                loginForm.ShowDialog();
                if (loginForm.UserName == null)
                {
                    this.Close();
                }
                else
                {
                    // LoginForm-оо дуудаж ажилуулсаны дараа UserName-г аваад, буцаад цонхоо харуулна.
                    myUserName = loginForm.UserName;
                    this.Show();
                }
            }
        }

        // Send Button дээр дарах үед бичсэн текстийг дэлгэцэнд хэвлэн харуулаад сүлжээгээр дамжуулна.
        private void sendButton_Click(object sender, EventArgs e)
        {
            string text = messageRichTextBox.Text;
            messageRichTextBox.Text = "";
            Message msg = new Message(myUserName, text);
            putMessageToScreen(msg);
            network.SendMessage(otherUser.ipString, text);
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            string ipString = ipAddrTextBox.Text;
            otherUser = new User("other", ipString);
        }
        #endregion


    }
}
