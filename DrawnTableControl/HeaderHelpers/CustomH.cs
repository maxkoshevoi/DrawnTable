using DrawnTableControl.Models;
using System.Collections.Generic;
using System.Linq;

namespace DrawnTableControl.HeaderHelpers
{
    public class CustomH
    {
        public int Count => Headers.Count;

        public List<DrawnTableHeader> Headers { get; private set; } = new();

        internal CustomH()
        {
        }

        public void Add(string text) =>
            Headers.Add(new DrawnTableHeader(text));

        public void Add(string text, object? tag) =>
            Headers.Add(new DrawnTableHeader(text, tag: tag));

        public void Add(DrawnTableHeader header) =>
            Headers.Add(header);

        public void AddRange(IEnumerable<string> dataCollection) =>
            Headers.AddRange(dataCollection.Select(d => new DrawnTableHeader(d)));

        public void AddRange(IEnumerable<(string text, object? tag)> dataTagCollection) =>
            Headers.AddRange(dataTagCollection.Select(dt => new DrawnTableHeader(dt.text, tag: dt.tag)));

        public void AddRange(IEnumerable<DrawnTableHeader> headerCollection) =>
            Headers.AddRange(headerCollection);

        public void Clear() =>
            Headers.Clear();

        public void SetHeaders(IEnumerable<string> h) =>
            Headers = HeaderCreator.ParseList(h);

        public int GetIndexByHeader(DrawnTableHeader header) =>
            Headers.IndexOf(header);

        public int GetIndexByHeaderText(string headerText)
        {
            var match = Headers.Where(x => x.Text == headerText).ToList();
            if (match.Count == 0) return -1;

            return GetIndexByHeader(match[0]);
        }

        public int GetIndexByHeaderTag(object? tag)
        {
            var match = Headers.Where(x => x.Tag?.Equals(tag) == true).ToList();
            if (match.Count == 0) return -1;

            return GetIndexByHeader(match[0]);
        }

        public DrawnTableHeader? this[int index]
        {
            get
            {
                if (index < 0 || index >= Headers.Count)
                {
                    return null;
                }
                return Headers[index];
            }
        }
    }
}
