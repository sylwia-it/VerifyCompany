namespace DocumentGenerator.Lib.Helpers
{
    internal class PostAddress
    {
        public int NumOfStoredFields = 7;

        public string PostalCode { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }
        public string Numbers { get; set; }
        public string Gmina { get; set; }
        public string Powiat { get; set; }
        public string Wojewodztwo { get; set; }
        

    }
}