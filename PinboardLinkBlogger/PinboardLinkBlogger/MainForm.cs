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
        List<PostItem> _postItems = new List<PostItem>(); 
        List<Section> _sections = new List<Section>(); 

        

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

            _postItems.Clear();

            foreach (var daypost in posts)
            {

                PostItem newitem = new PostItem();
                newitem.PinboardPost = daypost;
                newitem.IncludeInOutput = false;

                _postItems.Add(newitem);

                //System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", daypost.Description, daypost.Href.ToString()));
            }
            PopulateList();


        }

        private void PopulateList()
        {
            foreach (var postItem in _postItems)
            {
                ListViewItem item = new ListViewItem();                                 
                item.Text = postItem.PinboardPost.Description;
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, postItem.PinboardPost.Href));
                item.Checked = postItem.IncludeInOutput;
                lvPosts.Items.Add(item);
            }



        }

        private void lvPosts_DoubleClick(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void lvPosts_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get the selected item
            if (lvPosts.SelectedItems.Count == 0)
                return;

            var item = lvPosts.SelectedItems[0];

            // get the title of the item
            var itemTitle = item.Text;

            var post = _postItems.FirstOrDefault(i => i.PinboardPost.Description == itemTitle);

            if (post == null)
                return;

            txtTitle.Text = post.PinboardPost.Description;
            txtDescription.Text = post.PinboardPost.Extended;
            txtUrl.Text = post.PinboardPost.Href;
            txtTags.Text = post.PinboardPost.Tag;
        }

        private void btnSectionAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSectionTitle.Text))
                return;

            var newSection = _sections.FirstOrDefault(s => s.SectionName == txtSectionTitle.Text);

            if (newSection == null)
            {
                newSection = new Section();
                
            }
            Section section = new Section();
            section.SectionName = txtSectionTitle.Text;
            section.TagList = txtSectionTags.Text;



        }
    }
}
