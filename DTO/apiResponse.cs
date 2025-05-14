namespace Service2.DTO
{
    public class apiResponse<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
