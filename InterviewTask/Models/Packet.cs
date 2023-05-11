using Newtonsoft.Json;

namespace InterviewTask.Models
{
    public class Packet
    {
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }

    public class Format0x0E : Packet
    {
        public string? Type { get; set; }
        public int Battery { get; set; }
        public int Temperature { get; set; }
        public string? Chars { get; set; }
    }

    public class Format0x0A : Packet
    {
        public string? Type { get; set; }
        public int Id { get; set; }
        public string? State { get; set; }
        public string? IpAddr { get; set; }

        public ResponseObj AsResponse()
        {
            ResponseObj res = new ResponseObj();
            res.identifier = this.Id;
            res.state = this.State;
            res.ipAddress = this.IpAddr;
            return res;
        }

    }

    public class Format0x0F : Packet
    {
        public string? Type { get; set; }
        public float Battery { get; set; }
        public int Flags { get; set; }
        public string[]? FlagNames { get; set; }
        public char Char { get; set; }
        public int Axis1 { get; set; }
        public int Axis2 { get; set; }
    }

    public class ResponseObj
    {
        public int identifier { get; set; }
        public string? state { get; set; }
        public string? ipAddress { get; set; }
    }
}
