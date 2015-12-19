using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace PinboardLinkBlogger.Model
{
    public class Template
    {
        private Dictionary<string, List<string>> _sectionTagMap = new Dictionary<string, List<string>>();

        public string UserName { get; set; }

        public string Password { get; set; }

        public string PostHeader { get; set; }
        
        public string SectionHeader { get; set; }
        
        public string PostItem { get; set; }
        
        public string SectionFooter { get; set; }
        
        public string PostFooter { get; set; }

        public Dictionary<string, List<string>> SectionTagMap
        {
            get { return _sectionTagMap; }
            set { _sectionTagMap = value; }
        }

      
      private string FileName { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DefaultTemplate.json");

      public void LoadTemplate()
      {
      if (File.Exists(FileName))
      {
        // load the file
        var templateJson = System.IO.File.ReadAllText(FileName);

        var loadedTemplate = (Template)new JavaScriptSerializer().Deserialize(templateJson, typeof(Template));
        this.UserName = loadedTemplate.UserName;
        this.Password = loadedTemplate.Password;
        this.PostHeader = loadedTemplate.PostHeader;
        this.PostFooter = loadedTemplate.PostFooter;
        this.SectionHeader = loadedTemplate.SectionHeader;
        this.SectionFooter = loadedTemplate.SectionFooter;
        this.SectionTagMap = loadedTemplate.SectionTagMap;
        this.PostItem = loadedTemplate.PostItem;
      }
      else
      {
        SetDefaultValues();
      }
    }

      private void SetDefaultValues()
      {
        this.UserName = "";
        this.Password = "";
        this.SectionHeader = @"<h2>%%SECTION%%</h2><ul>";
        this.SectionFooter = @"</ul>";
        this.PostHeader = "";
        this.PostFooter = "";
        this.PostItem = @"  <li><a href='%% URL %%' target='_blank'>%%TITLE%%</a>%%DESCRIPTION%%</li>";
        this.SectionTagMap = new Dictionary<string, List<string>>();
        

      }

      public void SaveTemplate()
      {
      var json = new JavaScriptSerializer().Serialize(this);
      Console.WriteLine(json);

      System.IO.File.WriteAllText(FileName, json);


    }

  }

}
