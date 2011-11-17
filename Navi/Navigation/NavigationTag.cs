using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Navi.Navigation
{
    public class NavigationTag
    {
        public int Id { get; set; }
        public IEnumerable<NavigationHint> Hints { get; set; }

        private static NavigationTag FromXML(XElement xml)
        {
            NavigationTag tag = new NavigationTag();
            tag.Id = int.Parse(xml.Attribute("id").Value);

            var depthTupels = from depth in xml.Descendants("depth")
                              select new NavigationHint(int.Parse(depth.Attribute("value").Value), depth.Attribute("text").Value);

            tag.Hints = depthTupels.ToList();

            return tag;
        }

        public static IEnumerable<NavigationTag> FromXML(String xmlFile)
        {
            XDocument xdoc = XDocument.Load(xmlFile);

            var tags = xdoc.Root.Descendants("navigationTag").Select(tag => NavigationTag.FromXML(tag)).ToList();

            return tags;
        }
    }
}
