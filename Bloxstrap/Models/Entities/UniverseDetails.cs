using Bloxstrap.RobloxInterfaces;

namespace Bloxstrap.Models.Entities
{
    /// <summary>
    /// Explicit loading. Load from cache before and after a fetch.
    /// </summary>
    public class UniverseDetails
    {
        private static List<UniverseDetails> _cache { get; set; } = new();

        public GameDetailResponse Data { get; set; } = null!;

        /// <summary>
        /// Returns data for a 128x128 icon
        /// </summary>
        public ThumbnailResponse Thumbnail { get; set; } = null!;

        public static UniverseDetails? LoadFromCache(long id)
        {
            var cacheQuery = _cache.Where(x => x.Data?.Id == id);

            if (cacheQuery.Any())
                return cacheQuery.First();

            return null;
        }

        public static Task FetchSingle(long id) => FetchBulk(id.ToString());

        public static async Task FetchBulk(string ids)
        {
            Uri gameDetailsUrl = UrlBuilder.BuildApiUrl("games", $"v1/games?universeIds={ids}");
            Uri thumbnailsUrl = UrlBuilder.BuildApiUrl("thumbnails", $"v1/games/icons?universeIds={ids}&returnPolicy=PlaceHolder&size=128x128&format=Png&isCircular=false");
            ApiArrayResponse<GameDetailResponse> gameDetailResponse;

            
            if (App.Cookies.Loaded)
                gameDetailResponse = await Http.AuthGetJson<ApiArrayResponse<GameDetailResponse>>(gameDetailsUrl);
            else
                gameDetailResponse = await Http.GetJson<ApiArrayResponse<GameDetailResponse>>(gameDetailsUrl);

            if (!gameDetailResponse.Data.Any())
                throw new InvalidHTTPResponseException("Roblox API for Game Details returned invalid data");

            var universeThumbnailResponse = await Http.GetJson<ApiArrayResponse<ThumbnailResponse>>(thumbnailsUrl);

            if (!universeThumbnailResponse.Data.Any())
                throw new InvalidHTTPResponseException("Roblox API for Game Thumbnails returned invalid data");

            foreach (string strId in ids.Split(','))
            {
                if (!long.TryParse(strId, out long id))
                    continue;

                GameDetailResponse? details = gameDetailResponse.Data.FirstOrDefault(x => x.Id == id);

                if (details is null)
                {
                    App.Logger.WriteLine("UniverseDetails::FetchBulk", $"Universe {id} is not available from the games API");
                    continue;
                }

                ThumbnailResponse? thumbnail = universeThumbnailResponse.Data.FirstOrDefault(x => x.TargetId == id);

                thumbnail ??= new ThumbnailResponse
                {
                    RequestId = id.ToString(CultureInfo.InvariantCulture),
                    TargetId = id,
                    State = "Error"
                };

                _cache.Add(new UniverseDetails
                {
                    Data = details,
                    Thumbnail = thumbnail
                });
            }
        }
    }
}
