using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pinboard;

namespace PinboardLinkBlogger
{
    public class PostItem
    {

        public PostItem()
        {
            TagsList = new List<string>();
        }

        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public List<string> TagsList { get; set; }



        public string TagsString
        {
            get
            {
                return string.Join(" ", TagsList.ToArray());
            }
            set
            {
                TagsList = new List<string>();
                TagsList.AddRange(value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        public DateTime PostDateTime { get; set; }
        public string Author { get; set; }
        public string Section { get; set; }

        //public Post PinboardPost { get; set; }
        //public bool IncludeInOutput { get; set; }




    }
}
