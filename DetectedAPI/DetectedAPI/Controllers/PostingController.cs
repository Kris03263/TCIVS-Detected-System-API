using DetectedAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DetectedAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostingController : ControllerBase
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
        // POST api/<PostingController>
        [HttpPost("insertBloodOxgenData")]
        public string InsertBloodOxgenData([FromBody] DataInsertDTO bloodOxygenDataDTO)
        {
            string result = "";
            DateTime dateTime;
            bool dateTimeParseResult;
            int value;
            int equipmentID = bloodOxygenDataDTO.EquipmentID;
            DataTable findPerson = sql($"select * from EquipmentData where EquipmentData.ID = '{equipmentID}'");
            if(findPerson.Rows.Count == 0)
            {
                result = "Error EquipmentID";
                return result;
            }
            dateTimeParseResult = DateTime.TryParseExact(bloodOxygenDataDTO.HappendTime, "yyyy/MM/dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dateTime);
            if(dateTimeParseResult == false)
            {
                return "Error Datetime Format!";
            }
            value = bloodOxygenDataDTO.Value;
            sql($"insert into BloodOxygenData(EquipmentID,Value,HappenedTime)\r\nValues('{equipmentID}',{value},'{dateTime.ToString("yyyy-MM-dd hh:mm:ss")}')\r\n");
            return "Data Insert Successfully!";
        }

        [HttpPost("insertTamperatureData")]

        public string InsertTamperatureData([FromBody] TamperatureDataInsertDTO TamperatureDataDTO)
        {
            string result = "";
            DateTime dateTime;
            bool dateTimeParseResult;
            double value;
            int equipmentID = TamperatureDataDTO.EquipmentID;
            DataTable findPerson = sql($"select * from EquipmentData where EquipmentData.ID = '{equipmentID}'");
            if (findPerson.Rows.Count == 0)
            {
                result = "Error EquipmentID";
                return result;
            }
            dateTimeParseResult = DateTime.TryParseExact(TamperatureDataDTO.HappendTime, "yyyy/MM/dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dateTime);
            if (dateTimeParseResult == false)
            {
                return "Error Datetime Format!";
            }
            value = TamperatureDataDTO.Value;
            sql($"insert into TemparatureData(EquipmentID,Value,HappenedTime)\r\nValues('{equipmentID}',{value},'{dateTime.ToString("yyyy-MM-dd hh:mm:ss")}')\r\n");
            return "Data Insert Successfully!";
        }

        [HttpPost("insertHeartData")]

        public string InsertHeartData([FromBody] DataInsertDTO HeartDataDTO)
        {
            string result = "";
            DateTime dateTime;
            bool dateTimeParseResult;
            int value;
            int equipmentID = HeartDataDTO.EquipmentID;
            DataTable findPerson = sql($"select * from EquipmentData where EquipmentData.ID = '{equipmentID}'");
            if (findPerson.Rows.Count == 0)
            {
                result = "Error EquipmentID";
                return result;
            }
            dateTimeParseResult = DateTime.TryParseExact(HeartDataDTO.HappendTime, "yyyy/MM/dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dateTime);
            if (dateTimeParseResult == false)
            {
                return "Error Datetime Format!";
            }
            value = HeartDataDTO.Value;
            sql($"insert into HeartData(EquipmentID,Value,HappendTime)\r\nValues('{equipmentID}',{value},'{dateTime.ToString("yyyy-MM-dd hh:mm:ss")}')\r\n");
            return "Data Insert Successfully!";
        }

        [HttpPost("insertEmergencyRecord")]

        public string InsertEmergencyRecord([FromBody] EmergencySituationInsertDTO emergencySituationDTO)
        {
            string result = "";
            DateTime dateTime;
            bool dateTimeParseResult;
            int equipmentID = emergencySituationDTO.EquipmentID;
            int typeID;
            DataTable findPerson = sql($"select * from EquipmentData where EquipmentData.ID = '{equipmentID}'");
            if (findPerson.Rows.Count == 0)
            {
                result = "Error EquipmentID";
                return result;
            }
            dateTimeParseResult = DateTime.TryParseExact(emergencySituationDTO.HappendTime, "yyyy/MM/dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dateTime);
            if (dateTimeParseResult == false)
            {
                return "Error Datetime Format!";
            }
            DataTable findEmergencyType = sql($"select * from EmergencyType where EmergencyType.TypeName = '{emergencySituationDTO.Type}'");
            if(findEmergencyType.Rows.Count == 0)
            {
                return "Error EmergencyType Name!";
            }
            typeID = Convert.ToInt32(findEmergencyType.Rows[0]["ID"]);
            sql($"insert into EmergencySituationData(EquipmentID,TypeID,HappenedTime)\r\nValues('{equipmentID}',{typeID},'{dateTime.ToString("yyyy-MM-dd hh:mm:ss")}')\r\n");
            return "Data Insert Successfully!";
        }
    }
}
