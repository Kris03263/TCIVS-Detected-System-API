using DetectedAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DetectedAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
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

        [HttpGet("tamperature/1DayAverage/{id}")]
        public IEnumerable<TamperatureAverageDTO> GetOneDayTamperatureAverage(int id)
        {
            HashSet<string> TotalDateTimeSet = new HashSet<string>();
            IEnumerable<TamperatureAverageDTO>? resultData = Enumerable.Empty<TamperatureAverageDTO>();
            DataTable tmp = sql($"select * from TemparatureData where TemparatureData.EquipmentID = '{id}' order by HappenedTime");
            if (tmp.Rows.Count == 0)
            {
                IEnumerable<TamperatureAverageDTO>? falseData = Enumerable.Empty<TamperatureAverageDTO>();
                var a = new TamperatureAverageDTO
                {
                    averagePeriod = 0,
                    LastestDay = "0000",
                    Value = 0.0
                };
                falseData = falseData.Append<TamperatureAverageDTO>(a);
                return falseData;
            }
            if (tmp.Rows.Count > 1000)
            {
                int less = tmp.Rows.Count - 1000;
                sql($"Delete TemparatureData where TemparatureData.HappenedTime \r\n" +
                    $"in (select TOP {less} TemparatureData.HappenedTime from TemparatureData where TemparatureData.EquipmentID = '{id}' " +
                    $"order by HappenedTime) \r\nand TemparatureData.EquipmentID = '{id}'");
                tmp = sql($"select * from TemparatureData where TemparatureData.EquipmentID = '{id}' order by HappenedTime");
            }
            DataTable Time = sql($"select TemparatureData.HappenedTime from TemparatureData where TemparatureData.EquipmentID = '{id}' order by HappenedTime");
            for (int i = 0; i < Time.Rows.Count; i++)
            {
                string tt = Time.Rows[i]["HappenedTime"].ToString();
                DateTime dateTime = DateTime.Parse(tt);
                tt = dateTime.ToShortDateString();
                TotalDateTimeSet.Add(tt);
            }
            List<string> TotalDateTime = TotalDateTimeSet.ToList();
            int dataCount = 0;
            string dataDate = "";
            double dataPlus = 0;
            for (int i = 0; i < TotalDateTime.Count; i++)
            {
                var TemparatureData = new TamperatureAverageDTO();
                for (int j = 0; j < tmp.Rows.Count; j++)
                {
                    string a = tmp.Rows[j]["HappenedTime"].ToString();
                    DateTime dateTime = DateTime.Parse(a);
                    a = dateTime.ToShortDateString();
                    if (a == TotalDateTime[i])
                    {
                        dataDate = a;
                        dataCount++;
                        dataPlus = dataPlus + Convert.ToDouble(tmp.Rows[j]["Value"]); ;
                    }
                }
                TemparatureData.averagePeriod = 1;
                TemparatureData.LastestDay = dataDate;
                TemparatureData.Value = Math.Round(dataPlus / dataCount,1);
                resultData = resultData.Append(TemparatureData);
                dataCount = 0;
                dataDate = "";
                dataPlus = 0;
            }
            return resultData;
        }
        [HttpGet("tamperatureData/all/{id}")]

        public IEnumerable<TemparatureDTO> GetAllTemparature(int id)
        {
            DataTable tmp = sql($"select * from TemparatureData where TemparatureData.EquipmentID = '{id}' order by HappenedTime");
            if (tmp.Rows.Count == 0)
            {
                IEnumerable<TemparatureDTO>? falseData = Enumerable.Empty<TemparatureDTO>();
                string dateInput = "Jan 1, 1900";
                DateTime dateTime = DateTime.Parse(dateInput);
                var a = new TemparatureDTO
                {
                    EquipmentID = 0,
                    Value = 0.0,
                    HappendTime = dateTime
                };
                falseData = falseData.Append<TemparatureDTO>(a);
                return falseData;
            }
            if (tmp.Rows.Count > 1000)
            {
                int less = tmp.Rows.Count - 1000;
                sql($"Delete TemparatureData where TemparatureData.HappenedTime \r\nin (select TOP {less} TemparatureData.HappenedTime from TemparatureData where TemparatureData.EquipmentID = '{id}' order by HappenedTime) \r\nand TemparatureData.EquipmentID = '{id}'");
                tmp = sql($"select * from TemparatureData where TemparatureData.EquipmentID = '{id}' order by HappenedTime");
            }
            IEnumerable<TemparatureDTO> items = tmp.AsEnumerable().Select(rw =>
                new TemparatureDTO
                {
                    EquipmentID = Convert.ToInt32(rw["EquipmentID"].ToString()),
                    Value = Convert.ToDouble(rw["Value"].ToString()),
                    HappendTime = DateTime.Parse(rw["HappenedTime"].ToString())
                }

            );
            return items;
        }
        [HttpGet("bloodOxygen/1DayAverage/{id}")]

        public IEnumerable<DataAverageDTO> GetOneDayBloodOxygenAverage(int id)
        {
            HashSet<string> TotalDateTimeSet = new HashSet<string>();
            IEnumerable<DataAverageDTO>? resultData = Enumerable.Empty<DataAverageDTO>();
            DataTable tmp = sql($"select * from BloodOxygenData where BloodOxygenData.EquipmentID = '{id}' order by HappenedTime");
            if (tmp.Rows.Count == 0)
            {
                IEnumerable<DataAverageDTO>? falseData = Enumerable.Empty<DataAverageDTO>();
                var a = new DataAverageDTO
                {
                    averagePeriod = 0,
                    LastestDay = "0000",
                    Value = 0
                };
                falseData = falseData.Append<DataAverageDTO>(a);
                return falseData;
            }
            if (tmp.Rows.Count > 1000)
            {
                int less = tmp.Rows.Count - 1000;
                sql($"Delete BloodOxygenData where BloodOxygenData.HappenedTime \r\n" +
                    $"in (select TOP {less} BloodOxygenData.HappenedTime from BloodOxygentData where BloodOxygenData.EquipmentID = '{id}' " +
                    $"order by HappenedTime) \r\nand BloodOxygenData.EquipmentID = '{id}'");
                tmp = sql($"select * from BloodOxygenData where BloodOxygenData.EquipmentID = '{id}' order by HappenedTime");
            }
            DataTable Time = sql($"select BloodOxygenData.HappenedTime from BloodOxygenData where BloodOxygenData.EquipmentID = '{id}' order by HappenedTime");
            for (int i = 0; i < Time.Rows.Count; i++)
            {
                string tt = Time.Rows[i]["HappenedTime"].ToString();
                DateTime dateTime = DateTime.Parse(tt);
                tt = dateTime.ToShortDateString();
                TotalDateTimeSet.Add(tt);
            }
            List<string> TotalDateTime = TotalDateTimeSet.ToList();
            int dataCount = 0;
            string dataDate = "";
            int dataPlus = 0;
            for (int i = 0; i < TotalDateTime.Count; i++)
            {
                var BloodOxygenData = new DataAverageDTO();
                for (int j = 0; j < tmp.Rows.Count; j++)
                {
                    string a = tmp.Rows[j]["HappenedTime"].ToString();
                    DateTime dateTime = DateTime.Parse(a);
                    a = dateTime.ToShortDateString();
                    if (a == TotalDateTime[i])
                    {
                        dataDate = a;
                        dataCount++;
                        dataPlus = dataPlus + Convert.ToInt32(tmp.Rows[j]["Value"]); ;
                    }
                }
                BloodOxygenData.averagePeriod = 1;
                BloodOxygenData.LastestDay = dataDate;
                BloodOxygenData.Value = dataPlus / dataCount;
                resultData = resultData.Append(BloodOxygenData);
                dataCount = 0;
                dataDate = "";
                dataPlus = 0;
            }
            return resultData;
        }
        [HttpGet("bloodOxygen/all/{id}")]
        public IEnumerable<BloodOxygenDTO> GetAllBloodOxygen(int id)
        {
            DataTable tmp = sql($"select * from BloodOxygenData where BloodOxygenData.EquipmentID = '{id}' order by HappenedTime");
            if (tmp.Rows.Count == 0)
            {
                IEnumerable<BloodOxygenDTO>? falseData = Enumerable.Empty<BloodOxygenDTO>();
                string dateInput = "Jan 1, 1900";
                DateTime dateTime = DateTime.Parse(dateInput);
                var a = new BloodOxygenDTO
                {
                    EquipmentID = 0,
                    Value = 0,
                    HappendTime = dateTime
                };
                falseData = falseData.Append<BloodOxygenDTO>(a);
                return falseData;
            }
            if (tmp.Rows.Count > 1000)
            {
                int less = tmp.Rows.Count - 1000;
                sql($"Delete BloodOxygenData where BloodOxygenData.HappenedTime \r\nin (select TOP {less} BloodOxygenData.HappenedTime from BloodOxygenData where BloodOxygenData.EquipmentID = '{id}' order by HappenedTime) \r\nand BloodOxygenData.EquipmentID = '{id}'");
                tmp = sql($"select * from BloodOxygenData where BloodOxygenData.EquipmentID = '{id}' order by HappenedTime");
            }
            IEnumerable<BloodOxygenDTO> items = tmp.AsEnumerable().Select(rw =>
                new BloodOxygenDTO
                {
                    EquipmentID = Convert.ToInt32(rw["EquipmentID"].ToString()),
                    Value = Convert.ToInt32(rw["Value"].ToString()),
                    HappendTime = DateTime.Parse(rw["HappenedTime"].ToString())
                }

            );
            return items;
        }
        [HttpGet("heartData/1DayAverage/{id}")]
        public IEnumerable<DataAverageDTO> GetOneDayHeartDataAverage(int id)
        {
            HashSet<string> TotalDateTimeSet = new HashSet<string>();
            IEnumerable<DataAverageDTO>? resultData = Enumerable.Empty<DataAverageDTO>();
            DataTable tmp = sql($"select * from HeartData where HeartData.EquipmentID = '{id}' order by HappendTime");
            if (tmp.Rows.Count == 0)
            {
                IEnumerable<DataAverageDTO>? falseData = Enumerable.Empty<DataAverageDTO>();
                var a = new DataAverageDTO
                {
                    averagePeriod = 0,
                    LastestDay = "0000",
                    Value = 0
                };
                falseData = falseData.Append<DataAverageDTO>(a);
                return falseData;
            }
            if (tmp.Rows.Count > 1000)
            {
                int less = tmp.Rows.Count - 1000;
                sql($"Delete HeartData where HeartData.HappendTime \r\n" +
                    $"in (select TOP {less} HeartData.HappendTime from HeartData where HeartData.EquipmentID = '{id}' " +
                    $"order by HappendTime) \r\nand HeartData.EquipmentID = '{id}'");
                tmp = sql($"select * from HeartData where HeartData.EquipmentID = '{id}' order by HappendTime");
            }
            DataTable Time = sql($"select HeartData.HappendTime from HeartData where HeartData.EquipmentID = '{id}' order by HappendTime");
            for(int i=0; i<Time.Rows.Count; i++)
            {
                string tt = Time.Rows[i]["HappendTime"].ToString();
                DateTime dateTime = DateTime.Parse(tt);
                tt = dateTime.ToShortDateString();
                TotalDateTimeSet.Add(tt);
            }
            List<string> TotalDateTime = TotalDateTimeSet.ToList();
            int dataCount = 0;
            string dataDate = "";
            int dataPlus = 0;
            for(int i=0;i<TotalDateTime.Count;i++)
            {
                var HeartData = new DataAverageDTO();
                for (int j = 0; j < tmp.Rows.Count; j++)
                {   
                    string a = tmp.Rows[j]["HappendTime"].ToString();
                    DateTime dateTime = DateTime.Parse(a);
                    a = dateTime.ToShortDateString();
                    if(a == TotalDateTime[i])
                    {
                        dataDate = a;
                        dataCount++;
                        dataPlus = dataPlus + Convert.ToInt32(tmp.Rows[j]["Value"]); ;
                    }
                }
                HeartData.averagePeriod = 1;
                HeartData.LastestDay = dataDate;
                HeartData.Value = dataPlus / dataCount;
                resultData = resultData.Append(HeartData);
                dataCount = 0;
                dataDate = "";
                dataPlus = 0;
            }
            return resultData;
        }
        // GET api/<DataController>/5
        [HttpGet("heartData/all/{id}")]
        public IEnumerable<HeartDataDTO> GetAllHeartData(int id)
        {   
            DataTable tmp = sql($"select * from HeartData where HeartData.EquipmentID = '{id}' order by HappendTime");
            if(tmp.Rows.Count == 0)
            {
                IEnumerable<HeartDataDTO>? falseData = Enumerable.Empty<HeartDataDTO>();
                string dateInput = "Jan 1, 1900";
                DateTime dateTime = DateTime.Parse(dateInput);
                var a = new HeartDataDTO
                {
                    EquipmentID = 0,
                    Value = 0,
                    HappendTime = dateTime
                };
                falseData = falseData.Append<HeartDataDTO>(a);
                return falseData;
            }
            if(tmp.Rows.Count > 1000)
            {
                int less = tmp.Rows.Count - 1000;
                sql($"Delete HeartData where HeartData.HappendTime \r\nin (select TOP {less} HeartData.HappendTime from HeartData where HeartData.EquipmentID = '{id}' order by HappendTime) \r\nand HeartData.EquipmentID = '{id}'");
                tmp = sql($"select * from HeartData where HeartData.EquipmentID = '{id}' order by HappendTime");
            }
            IEnumerable<HeartDataDTO> items = tmp.AsEnumerable().Select(rw =>
                new HeartDataDTO
                {
                    EquipmentID = Convert.ToInt32(rw["EquipmentID"].ToString()),
                    Value = Convert.ToInt32(rw["Value"].ToString()),
                    HappendTime = DateTime.Parse(rw["HappendTime"].ToString())
                }

            );
            return items;
        }

        [HttpGet("EmergencySituationData/{id}")]

        public IEnumerable<EmergencySituationDTO> GetEmergencyData(int id)
        {   
            
            DataTable dt = sql($"select EquipmentID, EmergencyType.TypeName, HappenedTime from EmergencySituationData\r\n" +
                $"inner join EmergencyType\r\n" +
                $"on EmergencyType.ID = EmergencySituationData.TypeID\r\n" +
                $"where EmergencySituationData.EquipmentID = '{id}'");

            DataTable test = sql($"select * from EquipmentData where EquipmentData.ID = {id}");
            if(test.Rows.Count == 0)
            {
                IEnumerable<EmergencySituationDTO>? falseData = Enumerable.Empty<EmergencySituationDTO>();
                DateTime date = DateTime.Parse("1900/01/01 00:00:00");
                var a = new EmergencySituationDTO { EquipmentID = 0000, Type = "No Type", HappendTime = date };
                falseData = falseData.Append(a);
                return falseData;
            }
            if(dt.Rows.Count == 0)
            {
                IEnumerable<EmergencySituationDTO>? falseData = Enumerable.Empty<EmergencySituationDTO>();
                DateTime date = DateTime.Parse("1900/01/01 00:00:00");
                var a = new EmergencySituationDTO { EquipmentID = id, Type = "No Type", HappendTime = date };
                falseData = falseData.Append(a);
                return falseData;
            }
            IEnumerable<EmergencySituationDTO> items = dt.AsEnumerable().Select(x =>
            new EmergencySituationDTO
            {
                EquipmentID = Convert.ToInt32(x["EquipmentID"]),
                Type = x["TypeName"].ToString(),
                HappendTime = DateTime.Parse(x["HappenedTime"].ToString())
            }
            );
            return items;
        }
        // POST api/<DataController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
    }
}
