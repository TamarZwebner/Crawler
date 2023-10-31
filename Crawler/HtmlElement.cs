using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crawler
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public  string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }

        public HtmlElement()
        {
            Attributes = new List<string>();
            Classes = new List<string>();
            Children = new List<HtmlElement>();
        }



        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                HtmlElement element = queue.Dequeue();
                yield return element;

                foreach (HtmlElement child in element.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }

        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement parent = this.Parent;

            while (parent != null)
            {
                yield return parent;
                parent = parent.Parent;
            }
        }

        public HashSet<HtmlElement> FindElementsBySelector(Selector selector)
        {
            HashSet<HtmlElement> result = new HashSet<HtmlElement>();
            FindElementsRecursive(this, selector, result);
            return result;
        }

        private static void FindElementsRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> result)
        {
            if (selector == null)
            {
                result.Add(element);
                return;
            }

            foreach (HtmlElement descendant in element.Descendants())
            {
                if (selector.Matches(descendant))
                {
                    if (selector.Child == null)
                    {
                        result.Add(descendant);
                    }
                    else
                    {
                        FindElementsRecursive(descendant, selector.Child, result);
                    }
                }
            }
        }

    }
}
