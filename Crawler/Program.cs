
using Crawler;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

var html = await Load("  https://www.yourweddingcountdown.com/c048b");

var cleanHtml = new Regex("[\n\t\r]").Replace(html, " ");

var htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => s.Length > 0 && (!s.StartsWith(" ")));


HtmlElement dom = buildHtmlAttributesTree(htmlLines.ToList());

Selector s = new Selector();
s = Selector.FromQueryString("body#inner .wrapper .container .bickham");
var result = dom.FindElementsBySelector(s);
Console.ReadLine();
static List<string> getClasses(List<string> attributes)
{
    List<string> classes = new List<string>();
    foreach (var att in attributes)
    {
        if (att.Split(" ")[0] == "class")
        {
            classes = att.Split(' ').ToList();
        }
    }
    classes.Remove("class");
    return classes;
}

static string getId(List<string> attributes)
{
    string id = null;
    foreach (var att in attributes)
    {
        if (att.Split(" ")[0] == "id")
        {
            id = att.Split(' ')[1];
        }
    }
    return id;
}
static List<string> getAllAttributes(string s)
{
    List<string> allAttributes = new List<string>();
    MatchCollection matches = Regex.Matches(s, @"(\S+)=[""']([^""']+?)[""']");
    foreach (Match match in matches)
    {
        string attribute = match.Groups[1].Value;
        string value = match.Groups[2].Value;
        allAttributes.Add($"{attribute} {value}");
    }

    return allAttributes;
}


static HtmlElement buildHtmlAttributesTree(List<string> allStrings)
{

    HtmlElement root = new HtmlElement();
    HtmlElement currentElement = root;

    List<string> selfClosingTags = HtmlHelper.Helper().HtmlVoidTags.ToList();
    List<string> tags = HtmlHelper.Helper().HtmlTags.ToList();

    foreach (var remainingString in allStrings)
    {
        if (remainingString == "/html")
        {
            currentElement.Parent = null;
            break;
        }
        else if (remainingString.StartsWith("/"))
        {
            currentElement = currentElement.Parent;
        }
        else if (selfClosingTags.Contains(remainingString.Split(" ")[0]) || remainingString.EndsWith("/"))
        {
            HtmlElement newElem = new HtmlElement();
            string allAttributes = Regex.Replace(remainingString, @"^\w+\s*", "");
            newElem.Attributes = getAllAttributes(allAttributes);
            newElem.Classes = getClasses(newElem.Attributes);
            newElem.Id = getId(newElem.Attributes);
            newElem.Name = remainingString.Split(" ")[0];
            newElem.Parent = currentElement;
            newElem.Children = null;
        }
        else if (tags.Contains(remainingString.Split(" ")[0]))
        {
            HtmlElement newElem = new HtmlElement();
            string allAttributes = Regex.Replace(remainingString, @"^\w+\s*", "");
            newElem.Attributes = getAllAttributes(allAttributes);
            newElem.Classes = getClasses(newElem.Attributes);
            newElem.Id = getId(newElem.Attributes);
            newElem.Name = remainingString.Split(" ")[0];
            newElem.Parent = currentElement;
            currentElement.Children.Add(newElem);
            currentElement = newElem;
        }
        else
        {
            currentElement.InnerHtml = remainingString;
        }
    }
    return root;
}

static void Descendants(HtmlElement root)
{
    foreach (HtmlElement descendant in root.Descendants())
    {
        Console.WriteLine(descendant.Name); 
    }
}

static void Ancestors(HtmlElement parent)
{
    foreach (HtmlElement p in parent.Ancestors())
    {
        Console.WriteLine(p.Name); 
    }
}

static async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}
