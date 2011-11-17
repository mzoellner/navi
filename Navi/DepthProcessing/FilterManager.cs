using System;
using System.Collections.Generic;
using System.Linq;
using Navi.KinectEngine;

namespace Navi.DepthProcessing
{
    public class FilterManager
    {
        private readonly List<IDepthFilter> _filters = new List<IDepthFilter>();

        private readonly object _filterLocker = new object();

        public bool IsAnyFilterActive
        {
            get
            {
                lock (_filterLocker)
                {
                    return _filters.Count != 0;
                }
            }
        }

        public void AddDepthFilter(IDepthFilter filter)
        {
            lock (_filterLocker)
            {
                _filters.Add(filter);
            }
        }

        public void RemoveDepthFilter(IDepthFilter filter)
        {
            lock (_filterLocker)
            {
                _filters.Remove(filter);
            }
        }

        public void Filter(DepthImage image)
        {
            lock (_filterLocker)
            {
                var o = Observable.Start(() =>
                {

                    _filters.ForEach(filter => filter.Filter(image));

                    image.FilterApplied = true;

                });
                o.First();
            }
        }
    }
}
