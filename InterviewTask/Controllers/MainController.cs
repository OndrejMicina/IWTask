using InterviewTask.Models;
using InterviewTask.Tools;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//       0x0A5E0027473D9D

namespace InterviewTask.Controllers
{
    [ApiController]
    public class MainController : ControllerBase
    {

        Parser parser = new Parser();

        [HttpPost("service/data")]
        public IActionResult Post(string value)
        {
            if (value != null)
            {
                Packet recievedPacket;
                try
                {
                    recievedPacket = parser.ParsePacket(value);
                }
                catch (Exception ex)
                {
                    //return BadRequest(ex.Message);
                    return BadRequest();
                }
                //Print in console
                Debug.WriteLine(recievedPacket.ToString());
                if (value.Substring(0, 4) == "0x0A")
                {
                    DataContext.Instance.packets.Add((Format0x0A)recievedPacket);
                }
                return Ok();
            }

            return BadRequest("message null");
        }

        [HttpGet("api/data")]
        public IActionResult Get()
        {
            return Ok(DataContext.Instance.packets.Select(item => item.AsResponse()));
        }

    }



    public sealed class DataContext
    {
        private static DataContext instance = null;
        private static readonly object padlock = new object();
        public List<Format0x0A> packets = new List<Format0x0A>();

        DataContext()
        {
        }

        public static DataContext Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DataContext();
                    }
                    return instance;
                }
            }
        }
    }
}
