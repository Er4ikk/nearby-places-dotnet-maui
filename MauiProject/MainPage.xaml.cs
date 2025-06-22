using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Newtonsoft.Json;

namespace MauiProject;


[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(PlacesBodyRequest))]
[JsonSerializable(typeof(Circle))]
[JsonSerializable(typeof(LocationRestriction))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(double))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}

public partial class MainPage : ContentPage
{
	private CancellationTokenSource? _cancelTokenSource;
	
	Location? location;
	Application? app = Application.Current;
	private static readonly HttpClient httpClient = new HttpClient();
	const string SearchNearbyApi = "https://places.googleapis.com/v1/places:searchNearby";
	const string PlaceDetailApi = "https://places.googleapis.com/v1/places/";
	const string API_KEY = "INSERT-YOUR-KEY-HERE";

	public MainPage()
	{
		InitializeComponent();
	}




	private void StartLocation(object? sender, EventArgs e)
	{
		Console.WriteLine("Start Location");




		Task.Run(GetCurrentLocation);
	}



	public async Task GetCurrentLocation()
	{
		try
		{
			// _isCheckingLocation = true;

			GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

			_cancelTokenSource = new CancellationTokenSource();

			location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

			if (location != null)
			{


				SetLocationOnMap(location);

				string? placeType = SearchItems.searchResults.GetValueOrDefault(searchBar.Text);

				if (placeType == null)
				{
					searchBar.Text = "ristoranti";
					placeType = "restaurant";
				}

				PlacesBodyRequest placesBodyRequest = InitiateSearch(placeType, location, 5);

				GoogleHeaders googleHeaders = new GoogleHeaders()
				{
					XGoogleApiKey = API_KEY,
					XGoogleFieldMask = [
						FieldMasksForSearchNearby.DISPLAYNAME,
						FieldMasksForSearchNearby.ID,
					]
				};


				//UNCOMMENT AFTER 
				//SETTING THE REQUIRED HEADERS
				SetGoogleHeaders(googleHeaders);

				//GETTING PLACES AROUND THE DEVICE
				SearchNearbyResponse? searchNearbyResponse = null;

				var content = new StringContent(JsonConvert.SerializeObject(placesBodyRequest), Encoding.UTF8, "application/json");
				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

				HttpResponseMessage response = await httpClient.PostAsync(SearchNearbyApi, content);

				if (response.Content != null)
				{

					string searchNearbyString = await response.Content.ReadAsStringAsync();
					searchNearbyResponse = JsonConvert.DeserializeObject<SearchNearbyResponse>(searchNearbyString);

				}

				List<PlaceDetail?> placeInfos = new List<PlaceDetail?>();

				//GET COORDINATES OF PLACES
				googleHeaders.XGoogleFieldMask = [
					FieldMasksForPlaceDetail.ID,
					FieldMasksForPlaceDetail.LOCATION
				];
				SetGoogleHeaders(googleHeaders);


				await SetNearbyPointsOfInterest(searchNearbyResponse, response, placeInfos);
			}
			else
			{
				app?.Dispatcher.Dispatch(async() =>
			{
				await DisplayAlert("Alert", "Impossibile accedere alla posizion. Concedere i permessi all'applicazione di accedere alla posizione", "OK");
				
			});
				
				
			}
		}
		// Catch one of the following exceptions:
		//   FeatureNotSupportedException
		//   FeatureNotEnabledException
		//   PermissionException
		catch (FeatureNotSupportedException featureNotSupportedException)
		{

			app?.Dispatcher.Dispatch(async() =>
			{
			await DisplayAlert("Alert", "Impossibile accedere alla posizione funzionalita non supportata: " + featureNotSupportedException.Message, "OK");
				
			});
			
			// Unable to get location
			// Console.WriteLine("Cannot use this feature. Message:" + featureNotSupportedException.Message);
		}
		catch (Exception ex)
		{
			app?.Dispatcher.Dispatch(async() =>
			{
				await DisplayAlert("Alert", "Impossibile accedere alla posizione eccezione: " + ex.Message, "OK");
			});
			
			// Console.WriteLine("An unknown error occurred: " + ex.Message);
		}

		finally
		{
			// _isCheckingLocation = false;
		}
	}

	void SetLocationOnMap(Location location)
	{
		Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");

		if (app != null)
		{
			app.Dispatcher.Dispatch(() =>
			{
				// LocationData.Text = $"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}";
				AddPin(location, "You're here!");
				Map.MoveToRegion(new MapSpan(location, 0.01, 0.01));
			}


			);


		}


	}

	PlacesBodyRequest InitiateSearch(string pointOfInterest,Location location, int maxNumberOfResults)
	{
		return new PlacesBodyRequest()
				{
					maxResultCount = maxNumberOfResults,
					includedTypes = [pointOfInterest],
					locationRestriction = new()
					{
						circle = new()
						{
							center = new()
							{
								latitude = location.Latitude,
								longitude = location.Longitude
							},
							radius = 1000
						}
					}

				};
		
	}
	async Task SetNearbyPointsOfInterest(SearchNearbyResponse? searchNearbyResponse, HttpResponseMessage response, List<PlaceDetail?> placeInfos)
	{
		if (searchNearbyResponse != null && searchNearbyResponse.Places.Count > 0)
		{
			foreach (Place place in searchNearbyResponse.Places)
			{

				response = await httpClient.GetAsync(PlaceDetailApi + place.Id);
				PlaceDetail? placeDetailResponse = null;
				if (response.Content != null)
				{

					string searchNearbyString = await response.Content.ReadAsStringAsync();
					placeDetailResponse = JsonConvert.DeserializeObject<PlaceDetail>(searchNearbyString);
					if (placeDetailResponse != null)
					{
						placeInfos.Add(placeDetailResponse);
						app?.Dispatcher.Dispatch(() =>
						AddPin(new Location()
						{
							Longitude = placeDetailResponse.Location.longitude,
							Latitude = placeDetailResponse.Location.latitude
						}, place.DisplayName.Text)
						);
					}

				}



			}

		}
		else
		{
					await DisplayAlert("Alert", "Nessun risultato trovato", "OK");

		}
	}


	void OnTextChanged(object sender, EventArgs e)
	{
		SearchBar searchBar = (SearchBar)sender;

		searchResults.ItemsSource = SearchItems.searchResults
											   .Keys
											   .Where(key => key.Contains(searchBar.Text.ToLower()))
											   .Distinct()
											   .ToList();
	}


	void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
	{
		ListView listView = (ListView)sender;
		searchBar.Text = args.SelectedItem.ToString();
		listView.ItemsSource = null;
		Map.Pins.Clear();
		Task.Run(GetCurrentLocation);
	}

	public void AddPin(Location location, string label)
	{
		// LocationData.Text = $"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}";
		var pin = new Pin
		{
			Location = location,
			Label = label
		};
		Map.Pins.Add(pin);

	}


	public void SetGoogleHeaders(GoogleHeaders googleHeaders)
	{
		httpClient.DefaultRequestHeaders.Clear();
		httpClient.DefaultRequestHeaders.Add("User-Agent", "Fake Agent");
		httpClient.DefaultRequestHeaders.Add("X-Goog-Api-Key", googleHeaders.XGoogleApiKey);
		httpClient.DefaultRequestHeaders.Add("X-Goog-FieldMask", String.Join(",", googleHeaders.XGoogleFieldMask));
	}




}
