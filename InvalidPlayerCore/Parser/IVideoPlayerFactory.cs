namespace InvalidPlayerCore.Parser
{
    public interface IVideoPlayerFactory
    {
        IVideoPlayer GetPlayer(string url);
    }
}