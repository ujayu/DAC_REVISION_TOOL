namespace apidotnet.Helper
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string AcccessToken { get; set; }
        public object Data { get; set; }

    }

}
