using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Pinboard;
using PinboardLinkBlogger.Model;

namespace PinboardLinkBlogger
{
  public partial class MainForm : Form
  {
    private readonly List<PostItem> _postItems = new List<PostItem>();
    private readonly List<Section> _sections = new List<Section>();

    public MainForm()
    {
      InitializeComponent();

      SettingsTemplate.LoadTemplate();
      PopulateTemplate();
    }

    public Template SettingsTemplate { get; set; } = new Template();

    private void PopulateTemplate()
    {
      txtUserName.Text = SettingsTemplate.UserName;
      txtPassword.Text = SettingsTemplate.Password;
      txtTemplateHeading.Text = SettingsTemplate.PostHeader;
      txtTemplateFooter.Text = SettingsTemplate.PostFooter;
      txtTemplateSectionHeader.Text = SettingsTemplate.SectionHeader;
      txtTemplateSectionFooter.Text = SettingsTemplate.SectionFooter;
      txtTemplatePostItem.Text = SettingsTemplate.PostItem;

      var sb = new StringBuilder();
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
      if (string.IsNullOrWhiteSpace(txtPassword.Text)
          || (string.IsNullOrWhiteSpace(txtUserName.Text)))
      {
        MessageBox.Show("Please enter your credentials");
        return;
      }

      Connection.Username = txtUserName.Text;
      Connection.Password = txtPassword.Text;

      SettingsTemplate.UserName = txtUserName.Text;
      SettingsTemplate.Password = txtPassword.Text;


      // List<Delicious.Post> posts = Delicious.Post.Get();


      var posts = Post.GetRecentPosts(null, 50);

      _postItems.Clear();

      foreach (var daypost in posts)
      {
        var ni = new PostItem();
        ni.Url = daypost.Href;
        ni.Title = daypost.Description;
        ni.Description = daypost.Extended;
        ni.Source = "Pinboard";
        ni.TagsString = daypost.Tag;

        ni.PostDateTime = DateTime.MinValue;
        var postDateTime = ni.PostDateTime;
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
        var item = new ListViewItem();
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
      var section = new Section();
      section.SectionName = txtSectionTitle.Text;
      section.TagList = txtSectionTags.Text;
    }


    private void UpdateTemplate()
    {
      SettingsTemplate.UserName = txtUserName.Text;
      SettingsTemplate.Password = txtPassword.Text;
      SettingsTemplate.PostHeader = txtTemplateHeading.Text;
      SettingsTemplate.PostFooter = txtTemplateFooter.Text;
      SettingsTemplate.SectionHeader = txtTemplateSectionHeader.Text;
      SettingsTemplate.SectionFooter = txtTemplateSectionFooter.Text;
      SettingsTemplate.PostItem = txtTemplatePostItem.Text;

      var sectionText = txtTemplateSectionDefinitons.Text;
      var lines = sectionText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
      foreach (var line in lines)
      {
        var pos1 = line.IndexOf("=", StringComparison.Ordinal);
        if (pos1 < 0) continue;
        var sectionName = line.Substring(0, pos1);

        var tagNames = line.Substring(pos1 + 1);

        var tags = tagNames.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        if (!SettingsTemplate.SectionTagMap.ContainsKey(sectionName))
        {
          SettingsTemplate.SectionTagMap.Add(sectionName, new List<string>());
        }

        SettingsTemplate.SectionTagMap[sectionName].Clear();
        SettingsTemplate.SectionTagMap[sectionName].AddRange(tags);
      }


    }

    private void button1_Click(object sender, EventArgs e)
    {
      UpdateTemplate();
      SettingsTemplate.SaveTemplate();
    }

    private void btnPublish_Click(object sender, EventArgs e)
    {
      var selectedItems = new List<PostItem>();

      foreach (var selectedItem in lvPosts.CheckedItems)
      {
        var lv = selectedItem as ListViewItem;

        if (lv == null) continue;

        if (lv.Tag is PostItem)
        {
          selectedItems.Add((PostItem) lv.Tag);
        }
      }

      var sb = new StringBuilder();

      sb.AppendLine(TokenReplacement(SettingsTemplate.PostHeader));

      var groupListings = new Dictionary<string, List<string>>();

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
        var head = GetHeader(item.Key);
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