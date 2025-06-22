    public class DisplayName
    {
        public required string text { get; set; }
        public required string languageCode { get; set; }
    }

    public class Place
    {
        public required string id { get; set; }
        public required DisplayName displayName { get; set; }
    }

    public class SearchNearbyResponse
    {
        
        public required List<Place> places { get; set; }
    }