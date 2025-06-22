    public class DisplayName
    {
        public required string Text { get; set; }
        public required string LanguageCode { get; set; }
    }

    public class Place
    {
        public required string Id { get; set; }
        public required DisplayName DisplayName { get; set; }
    }

    public class SearchNearbyResponse
    {
        public required List<Place> Places { get; set; }
    }