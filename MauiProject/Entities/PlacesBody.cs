// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Center
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    public class Circle
    {
        public required Center center { get; set; }
        public double radius { get; set; }
    }

    public class LocationRestriction
    {
        public required Circle circle { get; set; }
    }

    public class PlacesBodyRequest
    {
        public required List<string> includedTypes { get; set; }
        public int maxResultCount { get; set; }
        public required LocationRestriction locationRestriction { get; set; }
    }
