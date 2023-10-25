using DetectedAPI.DTO;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using Nancy.Json;
using System.Net;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DetectedAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmergencyController : ControllerBase
    {

        // POST api/<EmergencyController>
        [HttpPost]
        public string Post([FromBody] EmergencySituationDTO ed)
        {   
            SendPushNotification(ed,"test");
            return "successfully!";
        }
        public async void SendPushNotification(EmergencySituationDTO ed, string deviceID)
        {
            try
            {
                //伺服器
                string applicationID = "BOk4AZyhyBfYgD6eUrsUwrqfRue98wQcQJhu7ebBDdO343c";

                string senderId = "443425339631";

                string deviceId = deviceID;


                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    notification = new
                    {
                        body = "test",
                        title = "Emergency just happened!",
                        sound = "Enabled"

                    }
                };
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                string str = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
        }
    }
}
