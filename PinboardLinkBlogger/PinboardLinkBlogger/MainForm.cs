using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PinboardLinkBlogger
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtPassword.Text)
                || (String.IsNullOrWhiteSpace(txtUserName.Text)))
            {
                MessageBox.Show("Please enter your credentials");
                return;
            }

            Pinboard.Connection.Username = txtUserName.Text;
            Pinboard.Connection.Password = txtPassword.Text;

            // List<Delicious.Post> posts = Delicious.Post.Get();


            List<Pinboard.Post> posts = Pinboard.Post.Get();

            foreach (var daypost in posts)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", daypost.Description, daypost.Href.ToString()));


            }
        }
    }
}
