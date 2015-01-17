using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinboardLinkBlogger
{
    public class Section
    {
        public string SectionName { get; set; }
        public int Order { get; set; }
        public List<string> Tags { get; set; }

        public string TagList
        {
            get
            {
                return string.Join(" ", Tags.ToArray());
            }   
            set
            {
                Tags = new List<string>();
                Tags.AddRange(value.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries));

            }
        }
 
    }
}
