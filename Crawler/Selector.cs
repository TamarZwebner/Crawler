using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crawler
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public Selector Parent { get; set; }
        public Selector Child { get; set; }

        public static Selector FromQueryString(string queryString)
        {
            var parts = queryString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            Selector root = null;
            Selector currentSelector = null;

            foreach (var part in parts)
            {
                var tagName = "";
                var id = "";
                var classes = new List<string>();

                if (part.Contains("#"))
                {
                    var subparts = part.Split('#');
                    tagName = subparts[0] != "" ? subparts[0] : null;
                    var idClassPart = subparts[1].Split('.');
                    id = idClassPart[0];
                    classes = idClassPart.Skip(1).ToList();
                }
                else if (part.Contains("."))
                {
                    var subparts = part.Split('.');
                    tagName = subparts[0] != "" ? subparts[0] : null;
                    classes = subparts.Skip(1).ToList();
                }
                else
                {
                    tagName = part;
                }

                var newSelector = new Selector
                {
                    TagName = tagName,
                    Id = id,
                    Classes = classes
                };

                if (root == null)
                {
                    root = newSelector;
                    currentSelector = root;
                }
                else
                {
                    currentSelector.Child = newSelector;
                    newSelector.Parent = currentSelector;
                    currentSelector = newSelector;
                }
            }

            return root;
        }

        public bool Matches(HtmlElement element)
        {
            // Compare tag name
            if (!string.IsNullOrEmpty(TagName) && TagName != element.Name)
            {
                return false;
            }

            // Compare ID
            if (!string.IsNullOrEmpty(Id) && Id != element.Id)
            {
                return false;
            }

            // Compare classes
            if (Classes != null && Classes.Count > 0)
            {
                foreach (var className in Classes)
                {
                    if (!element.Classes.Contains(className))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
