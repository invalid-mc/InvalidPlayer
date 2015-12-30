using System;

namespace InvalidPlayerCore.Model
{
    public class VideoItem
    {
        public string Url { get; set; }

        public double Seconds { get; set; }

        public int Size { get; set; }

        public override string ToString()
        {
            return String.Format("Url: {0}, Seconds: {1}, Size: {2}", Url, Seconds, Size);
        }
    }
}
