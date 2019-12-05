namespace Ctrl.Core.Core
{
    /// <summary>
    ///     脸部搜索信息
    /// </summary>
    public class FaceSearchInfo
    {
        public int error_code { get; set; }

        public string error_msg { get; set; }

        public string log_id { get; set; }

        public string timestamp { get; set; }

        public string cached { get; set; }

        public result result { get; set; }


    }
    public class result
    {
        public string face_token { get; set; }
        public string user_id { get; set; }

    }
    public class user_list
    {
        public string group_id { get; set; }
        public string user_id { get; set; }

        public string user_info { get; set; }

        public string score { get; set; }
    }
}
