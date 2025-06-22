// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class LocationSimple
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    public class PlaceDetail
    {
        public required string id { get; set; }
        public required LocationSimple location { get; set; }
    }

