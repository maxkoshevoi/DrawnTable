using System;
using System.Drawing;

namespace DrawnTableControl.Demo
{
    public class Event
    {
        public int ID;
        public string Text = null;

        public DateTime Date { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }

        public int? RoomId { get; set; }

        public Color Color { get; set; }
        public bool Enabled { get; set; }
        
        public Event(int id, string text)
        {
            ID = id;
            Text = text;
        }
    }
}
