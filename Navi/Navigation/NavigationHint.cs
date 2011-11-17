using System;

namespace Navi.Navigation
{
    public class NavigationHint
    {
        public NavigationHint(int depth, string text)
        {
            Depth = depth;
            Text = text;
            LastTimeSeen = DateTime.MinValue;
        }

        public int Depth { get; set; }
        public String Text { get; set; }
        public DateTime LastTimeSeen { get; set; }
    }
}
