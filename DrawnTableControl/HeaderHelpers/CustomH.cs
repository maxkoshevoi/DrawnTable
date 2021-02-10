using DrawnTableControl.Models;
using System.Collections.Generic;
using System.Linq;

namespace DrawnTableControl.HeaderHelpers
{
    public class CustomH
    {
        List<DrawnTableHeader> list = new();
        
        public int Count => list.Count;

        public List<DrawnTableHeader> Headers => list;

        public void Add(string data)
        {
            list.Add(new DrawnTableHeader(data));
        }

        public void Add(string data, object tag)
        {
            list.Add(new DrawnTableHeader(data, tag: tag));
        }

        public void Add(DrawnTableHeader header)
        {
            list.Add(header);
        }

        public void Clear()
        {
            list.Clear();
        }

        public void SetHeaders(IEnumerable<string> h)
        {
            list = HeaderCreator.ParseList(h.ToList());
        }

        public int GetIndexByHeader(DrawnTableHeader header) => list.IndexOf(header);

        public int GetIndexByHeaderText(string headerText)
        {
            var match = list.Where(x => x.Text == headerText).ToList();
            if (match.Count == 0) return -1;

            return GetIndexByHeader(match[0]);
        }

        public int GetIndexByHeaderTag(object tag)
        {
            var match = list.Where(x => x.Tag.Equals(tag)).ToList();
            if (match.Count == 0) return -1;

            return GetIndexByHeader(match[0]);
        }

        public DrawnTableHeader GetHeaderByIndex(int index)
        {
            if (index < 0 || index >= list.Count) return null;

            return list[index];
        }
    }
}
