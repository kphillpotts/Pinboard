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
using System.Windows.Forms.VisualStyles;
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
                PopulateTemplate();

            }
        }

        private void PopulateTemplate()
        {
            txtTemplateHeading.Text = SettingsTemplate.PostHeader;
            txtTemplateFooter.Text = SettingsTemplate.PostFooter;
            txtTemplateSectionHeader.Text = SettingsTemplate.SectionHeader;
            txtTemplateSectionFooter.Text = SettingsTemplate.SectionFooter;
            txtTemplatePostItem.Text = SettingsTemplate.PostItem;

            StringBuilder sb = new StringBuilder();
            foreach (var key in SettingsTemplate.SectionTagMap.Keys)
            {
                var tagValues = string.Join(" ", SettingsTemplate.SectionTagMap[key]);
                sb.AppendLine(string.Format("{0}={1}", key, tagValues));
            }

            txtTemplateSectionDefinitons.Text = sb.ToString();

            //t.SectionTagMap = new Dictionary<string, List<string>>();
            //t.SectionTagMap.Add("Business", new List<string>() { "business", "xamarin" });
            //t.SectionTagMap.Add("Forms", new List<string>() { "xamarin", "forms", "xamarin.forms" });

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

                PostItem ni = new PostItem();
                ni.Url = daypost.Href;
                ni.Title = daypost.Description;
                ni.Description = daypost.Extended;
                ni.Source = "Pinboard";
                ni.TagsString = daypost.Tag;
                
                ni.PostDateTime = DateTime.MinValue;
                DateTime postDateTime = ni.PostDateTime;
                if (DateTime.TryParse(daypost.Time, out postDateTime))
                    ni.PostDateTime = postDateTime;

                _postItems.Add(ni);

                //System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", daypost.Description, daypost.Href.ToString()));
            }
            PopulateList();


        }

        private void PopulateList()
        {
            foreach (var postItem in _postItems)
            {
                ListViewItem item = new ListViewItem();                                 
                item.Text = postItem.Title;
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, postItem.PostDateTime.ToShortDateString()));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, postItem.Url));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, postItem.Description));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, postItem.TagsString));
                //item.Checked = postItem.IncludeInOutput;
                item.Tag = postItem;
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

            var post = _postItems.FirstOrDefault(i => i.Title == itemTitle);

            if (post == null)
                return;

            txtTitle.Text = post.Title;
            txtDescription.Text = post.Description;
            txtUrl.Text = post.Url;
            txtTags.Text = post.TagsString;

            webBrowser1.Navigate(post.Url);

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

            var sectionText = txtTemplateSectionDefinitons.Text;
            string[] lines = sectionText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                var pos1 = line.IndexOf("=", System.StringComparison.Ordinal);
                if (pos1 < 0) continue;
                var sectionName = line.Substring(0, pos1);

                var tagNames = line.Substring(pos1 + 1);

                var tags = tagNames.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);

                if (!t.SectionTagMap.ContainsKey(sectionName))
                {
                    t.SectionTagMap.Add(sectionName, new List<string>());
                }

                t.SectionTagMap[sectionName].AddRange(tags);
            }

            var json = new JavaScriptSerializer().Serialize(t);
            Console.WriteLine(json);

            System.IO.File.WriteAllText(FullTemplatePath, json);

            //Template x = (Template) new JavaScriptSerializer().Deserialize(json, typeof (Template));

            //Console.WriteLine(x.SectionTagMap.ToString());

            //System.IO.File.WriteAllText();
            //System.IO.File.Read
        }

        private void btnPublish_Click(object sender, EventArgs e)
        {
            List<PostItem> selectedItems = new List<PostItem>();

            foreach (var selectedItem in lvPosts.CheckedItems)
            {
                ListViewItem lv = selectedItem as ListViewItem;

                if (lv == null) continue;

                if (lv.Tag is PostItem)
                {
                    selectedItems.Add((PostItem)lv.Tag);
                }
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(TokenReplacement(SettingsTemplate.PostHeader));

            Dictionary<string, List<string>> groupListings = new Dictionary<string, List<string>>();

            foreach (var selectedItem in selectedItems)
            {
                var group = FindGroupForTagList(selectedItem.TagsList);
                var item = PostTokenReplacement(SettingsTemplate.PostItem, selectedItem);
                if (!groupListings.ContainsKey(group))
                    groupListings.Add(group, new List<string>());
                groupListings[group].Add(item);
            }

            foreach (var item in groupListings)
            {
                string head = GetHeader(item.Key);
                sb.AppendLine(head);
                foreach (var postEntry in item.Value)
                {
                    sb.AppendLine(postEntry);
                }
                sb.AppendLine(SettingsTemplate.SectionFooter);
            }

            sb.AppendLine(TokenReplacement(SettingsTemplate.PostFooter));

            txtPreview.Text = sb.ToString();


        }

        private string GetHeader(string heading)
        {
            return SettingsTemplate.SectionHeader.Replace("%%SECTION%%", heading);
        }

        private string FindGroupForTagList(List<string> list)
        {
            foreach (var item in SettingsTemplate.SectionTagMap)
            {
                foreach (var itemTag in list)
                {
                    foreach (var sectionTag in item.Value)
                    {
                        if (itemTag.ToLower() == sectionTag.ToLower())
                            return item.Key;
                    }
                }


                //foreach (var sectionTag in item.Value)
                //{
                //    if (list.Contains(sectionTag.ToLower()))
                //        return item.Key;
                //}
            }
            return "Other";
        }

        private string TokenReplacement(string inputString)
        {
            inputString = inputString.Replace("%%DATE%%", DateTime.Now.ToShortDateString());
            return inputString;

        }


        private string PostTokenReplacement(string inputString, PostItem itemValues)
        {
            inputString = inputString.Replace("%%DATE%%", DateTime.Now.ToShortDateString());

            inputString = inputString.Replace("%%URL%%", itemValues.Url);
            inputString = inputString.Replace("%%TITLE%%", itemValues.Title);
            inputString = inputString.Replace("%%AUTHOR%%", itemValues.Author);
            inputString = inputString.Replace("%%DESCRIPTION%%", itemValues.Description);

            inputString = inputString.Replace("%%SECTION%%", itemValues.Section);
            return inputString;




        }


    }
}
