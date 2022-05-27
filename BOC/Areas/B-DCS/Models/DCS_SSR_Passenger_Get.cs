namespace BOC.Areas.B_DCS.Models
{
    public class DCS_SSR_Passenger_Get
    {
        public int TableID { get; set; }
        public int FlightID { get; set; }
        public int ifly_Pax_ID { get; set; }
        public string SSRID { get; set; }
        public int Count { get; set; }
        public string SSR { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string LastUpdate { get; set; }
        public string UserUpdate { get; set; }
        public int RecUserID { get; set; }
        public string Type { get; set; }
    }
}
