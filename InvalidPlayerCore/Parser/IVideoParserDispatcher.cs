namespace InvalidPlayerCore.Parser
{
    /// <summary>
    ///     提供IVideoParser调度
    /// </summary>
    public interface IVideoParserDispatcher
    {
        /// <summary>
        ///     根据指定的URL返回一个合适的IVideoParser
        /// </summary>
        /// <param name="url">需要解析的URL</param>
        IVideoParser GetParser(string url);
    }
}