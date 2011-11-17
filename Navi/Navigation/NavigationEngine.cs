using System;
using System.Collections.Generic;
using System.Linq;

namespace Navi.Navigation
{
    public class NavigationEngine
    {
        private IEnumerable<NavigationTag> _tags;

        public NavigationEngine(String xmlFile)
        {
            _tags = NavigationTag.FromXML(xmlFile);
        }

        public NavigationTag GetTag(int id)
        {
            if (id == -1)
                return null;

            try
            {
                return _tags.Where(tag => tag.Id == id).Single();
            }
            catch { return null; }

        }

        public NavigationHint GetTuple(int id, int depth)
        {
            var t = _tags.Where(tag => tag.Id == id).Single();

            return t.Hints.Where(hint => depth <= hint.Depth).OrderByDescending(hint => hint.Depth).FirstOrDefault();
        }
    }
}
