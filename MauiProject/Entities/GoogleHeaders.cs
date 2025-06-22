public class GoogleHeaders
{
    public required string XGoogleApiKey { get; set; }
    public required string[] XGoogleFieldMask { get; set; }
}

public class FieldMasksForSearchNearby
{
    public const string DISPLAYNAME = "places.displayName";
    public const string  ID = "places.id";


}

public class FieldMasksForPlaceDetail
{
    public const string ID = "id";
    public const string  LOCATION = "location";

}