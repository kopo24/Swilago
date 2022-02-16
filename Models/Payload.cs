using System.Net;

namespace Borago.Models
{
    public class Payload
    {
        public Payload()
        {
            if (ErrorMessages == null)
                ErrorMessages = new List<string>();

            StatusCode = HttpStatusCode.InternalServerError;
        }
                
        public HttpStatusCode StatusCode { get; set; }

        public string Message { get; set; }

        public object Object { get; set; }

        public string RemainingSubscriptionReads  { get; set; }

        public List<string> ErrorMessages { get; set; }        
    }
}