namespace Microled.Pix.Domain.Response
{
    public interface IServiceResult<T>
    {
        bool Status { get; set; }
        IList<string> Mensagens { get; set; }
    }
}
