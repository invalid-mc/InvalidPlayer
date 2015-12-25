namespace InvalidPlayer.Parser.Iqiyi
{
    public class ApiParam
    {
        public string ManyId { get; set; } = "433818900";

        public string Block { get; set; }

        public int X { get; set; }

        public string DeviceId
        {
            get { return "060074bab261efd81a872452872eb1c1"; }
        }

        public string ApiName
        {
            get { return "nebula"; }
        }

        public string ApiVersion
        {
            get { return "2.4.0"; }
        }

        public string IqiyiVersion
        {
            get { return "3.2.2"; }
        }

        public string Ua
        {
            get { return "Hasee%20QTC6"; }
        }

        public string QueryString
        {
            get
            {
                const string str = "many_id={0}{1}&user_res=8&ts=4,8,16,512&compat=1&other=1&x={2}" +
                                   "&api={3}&key=1008610914816218a216784c43a08aaa&version={4}&network=1" +
                                   "&platform=winPad&device_id={5}&os=8.1&ua={6}";
                return string.Format(str, ManyId, string.IsNullOrEmpty(Block) ? "" : "&block=" + Block, X,
                    ApiVersion, IqiyiVersion, DeviceId, Ua);
            }
        }
    }
}
