namespace DetectedAPI.DTO
{
    public class TamperatureDataInsertDTO
    {
        public int EquipmentID { get; set; }
        public double Value { get; set; }
        public string HappendTime { get; set; }
    }
}
