namespace Microled.Pix.Domain.Response
{
    public class ServiceResult<T> : IServiceResult<T>
    {
        private bool _status = true;

        public bool Status
        {
            get
            {
                return string.IsNullOrEmpty(Error) && Result != null && _status;
            }
            set { _status = value; }
        }

        public IList<string>? Mensagens { get; set; } = new List<string>();
        public string? Error { get; set; }
        public T? Result { get; set; }
    }
}
