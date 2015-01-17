using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

using PinboardLinkBlogger.Model;


namespace PinboardLinkBlogger
{
    public partial class MainForm : Form
    {
        List<PostItem> _postItems = new List<PostItem>(); 
        List<Section> _sections = new List<Section>();
        private string _templateFile = "DefaultTemplate.json";
        private string _settingsPath = AppDomain.CurrentDomain.BaseDirectory;
        private Template _settingsTemplate = new Template();

        public Template SettingsTemplate
        {
            get { return _settingsTemplate; }
            set { _settingsTemplate = value; }
        }


        public string TemplateFile
        {
            get { return _templateFile; }
            set { _templateFile = value; }
        }


        public string FullTemplatePath
        {
            get { return Path.Combine(SettingsPath, TemplateFile); }
        }

        public string SettingsPath
        {
            get { return _settingsPath; }
            set { _settingsPath = value; }
        }


        public MainForm()
        {
            InitializeComponent();

            if (File.Exists(FullTemplatePath))
            {
                // load the file
                var templateJson = System.IO.File.ReadAllText(FullTemplatePath);
                SettingsTemplate = (Template) new JavaScriptSerializer().Deserialize(templateJson, typeof (Template));

            }
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

        private void button1_Click(object sender, EventArgs e)
        {
            Template t = new Template();
            t.PostHeader = txtTemplateHeading.Text;
            t.PostFooter = txtTemplateFooter.Text;
            t.SectionHeader = txtTemplateSectionHeader.Text;
            t.SectionFooter = txtTemplateSectionFooter.Text;
            t.PostItem = txtTemplatePostItem.Text;
            t.SectionTagMap = new Dictionary<string, List<string>>();
            t.SectionTagMap.Add("Business", new List<string>() { "business", "xamarin" });
            t.SectionTagMap.Add("Forms", new List<string>() { "xamarin", "forms", "xamarin.forms" });

            var json = new JavaScriptSerializer().Serialize(t);
            Console.WriteLine(json);

            System.IO.File.WriteAllText(FullTemplatePath, json);

            //Template x = (Template) new JavaScriptSerializer().Deserialize(json, typeof (Template));

            //Console.WriteLine(x.SectionTagMap.ToString());

            //System.IO.File.WriteAllText();
            //System.IO.File.Read
        }
    }
}
