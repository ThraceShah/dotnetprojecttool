using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace dnf;

public static class ExMethod
{
    public static void SetNodeValue(this XmlDocument doc,string y,string z, XmlNode node)
    {
        var t = node[y];
        if (t != null)
        {
            t.InnerXml = z;
        }
        else
        {
            var xmlChild = doc.CreateElement(y, doc.DocumentElement?.NamespaceURI);
            xmlChild.InnerXml = z;
            node.AppendChild(xmlChild);
        }
    }

    public static void UpadateCsProj(string xmlPath,Action<XmlDocument,XmlNode> action)
    {
        if (!File.Exists(xmlPath))
        {
            return;
        }
        var doc = new XmlDocument();
        doc.Load(xmlPath);
        foreach (XmlNode root in doc.ChildNodes)
        {
            foreach (XmlNode node in root)
            {
                action(doc, node);
            }
        }
        doc.Save(xmlPath);
        var str = File.ReadAllText(xmlPath);
        str = str.Replace("Microsoft.Net.Compilers.1.0.0", "Microsoft.Net.Compilers.1.1.1");
        File.WriteAllText(xmlPath, str);
    }
    public static string GetGuidByStr(this string str)
    {
        string result = string.Empty;
        var matchResult = Regex.Match(str, @"[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12}", RegexOptions.IgnoreCase);
        if (matchResult.Success)
        {
            result = matchResult.Value;
        }
        return result;
    }

    public static void UpadateCsProj(string xmlPath)
    {
        if(!File.Exists(xmlPath))
        {
            return;
        }
        var doc=new XmlDocument();
        doc.Load(xmlPath);
        foreach(XmlNode root in doc.ChildNodes)
        {
            foreach (XmlNode node in root)
            {
                switch (node.Name)
                {
                    case "PropertyGroup":
                        UpadatePropertyGroup(node,doc);
                        break;
                    case "ItemGroup":
                        UpadateItemGroup(node);
                        break;
                    default:
                        break;
                }
            }
        }
        doc.Save(xmlPath);
        var str=File.ReadAllText(xmlPath);
        str=str.Replace("Microsoft.Net.Compilers.1.0.0", "Microsoft.Net.Compilers.1.1.1");
        File.WriteAllText(xmlPath,str);
    }

    private static void UpadatePropertyGroup(XmlNode node,XmlDocument doc)
    {
        void setNodeValue (string y,string z)
        {
            var t = node[y];
            if (t != null)
            {
                t.InnerXml = z;
            }
            else
            {
                var xmlChild = doc.CreateElement(y,doc.DocumentElement?.NamespaceURI);
                xmlChild.InnerXml=z;
                node.AppendChild(xmlChild);
            }
        }

        var config=node["Configuration"];
        if(config!=null)
        {
            setNodeValue("RuntimeIdentifiers","win-x64;win7-x64");
        }
        else
        {
            var attr=node.Attributes?["Condition"];
            if(attr!=null&&attr.Value.Contains("Debug"))
            {
                setNodeValue("PlatformTarget","x64");
                setNodeValue("DebugSymbols", "true");
                setNodeValue("DebugType", "portable");
                setNodeValue("Optimize", "false");
                setNodeValue("DefineConstants", "DEBUG;TRACE");
            }
        }
    }
    private static void UpadateItemGroup(XmlNode node)
    {
        // foreach(XmlNode childNode in node)
        // {
        //     if(childNode.Name=="Reference")
        //     {
        //         var attr=childNode.Attributes?["Include"];
        //         if(attr!=null&&attr.Value.Contains("Microsoft.Net.Compilers"))
        //         {
        //             attr.Value="Microsoft.Net.Compilers";
        //             // var pathNode= childNode["HintPath"];
        //             // if(pathNode!=null)
        //             // {
        //             //     pathNode.InnerXml=pathNode.InnerXml.Replace("Microsoft.Net.Compilers.1.0.0","Microsoft.Net.Compilers");
        //             // }
        //         }
        //     }
        // }
    }

}
