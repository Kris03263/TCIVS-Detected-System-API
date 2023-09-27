using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Data;
using DetectedAPI.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DetectedAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        SqlConnection cn = new SqlConnection();
        private DataTable sql(string command)
        {
            cn.ConnectionString = "Data Source=.;Initial Catalog=DetectedDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            SqlDataAdapter da = new SqlDataAdapter(command, cn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        // GET: api/<EquipmentController>
        [HttpGet("getAllEquipmentInfo")]

        public IEnumerable<EquipmentDTO> Get()
        {
            DataTable dt = sql("select * from EquipmentData");
            IEnumerable<EquipmentDTO> items = dt.AsEnumerable().Select(x => 
                 new EquipmentDTO
                 {
                     ID = x["ID"].ToString(),
                     userName = x["UserName"].ToString(),
                     Description = x["Description"].ToString(),
                     Status = Convert.ToBoolean(x["Status"])
                 }
            );
            return items;

        }

        // GET api/<EquipmentController>/5
        [HttpGet("getEquipmentInfo/{id}")]
        public EquipmentDTO Get(string id)
        {
            string EquipmentID = id;
            DataTable dt = sql($"select * from EquipmentData where ID = '{id}'");
            if(dt.Rows.Count == 0) 
            {
                var data = new EquipmentDTO
                {
                    ID = "noData!",
                    userName = "noData!",
                    Description = "noData!",
                    Status = false
                };
                return data;
            }
            EquipmentDTO item =
                new EquipmentDTO
                {
                    ID = dt.Rows[0]["ID"].ToString(),
                    userName = dt.Rows[0]["UserName"].ToString(),
                    Description = dt.Rows[0]["Description"].ToString(),
                    Status = Convert.ToBoolean(dt.Rows[0]["Status"])
                };
            return item;
        }
        [HttpGet("resetEquipment/{id}")]
        public string ResetEquipment(string id)
        {
        newID:
            var rd = new Random();
            int tmp = rd.Next(1000000, 99999999);
            string newUID = tmp.ToString();
            DataTable dt = sql($"select * from EquipmentData where ID = '{newUID}'");
            if (dt.Rows.Count >= 1)
            {
                goto newID;
            }
            sql($"update EquipmentData set Status = 0 where ID = '{id}'");
            sql($"INSERT INTO EquipmentData (ID,UserName,Description,Status) VALUES ({newUID},'DefaultUser','None',1);");
            return "Your NEW ID: " + newUID;
        }
        [HttpGet("registerNewEquipment")]

        public string RegisterNewEquipment()
        {   
            newID:
            var rd = new Random();
            int tmp = rd.Next(1000000, 99999999);
            string newUID = tmp.ToString();
            DataTable dt = sql($"select * from EquipmentData where ID = '{newUID}'");
            if(dt.Rows.Count >= 1)
            {
                goto newID;
            }
            sql($"INSERT INTO EquipmentData (ID,UserName,Description,Status) VALUES ({newUID},'DefaultUser','None',1);");
            return "Your NEW ID: " + newUID;
        }

        // POST api/<EquipmentController>
        [HttpPost("updateEquipmentData")]
        public string Post([FromBody] EquipmentDTO value)
        {
            DataTable dt = sql($"select * from EquipmentData where ID = '{value.ID}'");
            if(dt.Rows.Count == 0 )
            {
                return "No such as UID!";
            }
            try
            {
                sql($"update EquipmentData set UserName = '{value.userName}', Description = '{value.Description}' where ID = '{value.ID}'");
                return "Loaded successfulluy!";
            }catch(Exception ex)
            {
                return $"Error caused by {ex.Message}";
            }


        }
    }
}
