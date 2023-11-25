using System.Text.Json;

namespace Server.Models
{
    public class ResponseModel
    {
        public string msg { get; set; }
        public int statusCode { get; set; }

        public ResponseModel() { }

        public ResponseModel(string msg, int statusCode)
        {
            this.msg = msg;
            this.statusCode = statusCode;
        }

        public string toJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
