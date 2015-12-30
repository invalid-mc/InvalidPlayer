using System.Collections.Generic;

namespace InvalidPlayerCore.Model
{
    public abstract class VideoInfo
    {
        public string Title { get; set; }
        public List<VideoItem> Items { get; set; }
        public string WebUrl { get; set; }
    }
}