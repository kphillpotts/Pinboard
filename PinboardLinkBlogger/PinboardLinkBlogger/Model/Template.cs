using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinboardLinkBlogger.Model
{
    public class Template
    {
        public string PostHeader { get; set; }
        public string SectionHeader { get; set; }
        public string PostItem { get; set; }
        public string SectionFooter { get; set; }
        public string PostFooter { get; set; }
        public Dictionary<string, List<string>> SectionTagMap { get; set; }
    }

}
