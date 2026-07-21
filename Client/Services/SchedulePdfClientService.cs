using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Oqtane.Services;
using Oqtane.Shared;

namespace GIBS.Module.Resource.Services
{
    public interface ISchedulePdfClientService
    {
        Task<byte[]> GenerateSchedulePdfAsync(Models.SchedulePdfRequest request, int moduleId);
    }

    public class SchedulePdfClientService : ServiceBase, ISchedulePdfClientService
    {
        private readonly HttpClient _httpClient;

        public SchedulePdfClientService(HttpClient http, SiteState siteState) : base(http, siteState)
        {
            _httpClient = http;
        }

        private string Apiurl => CreateApiUrl("SchedulePdf");

        public async Task<byte[]> GenerateSchedulePdfAsync(Models.SchedulePdfRequest request, int moduleId)
        {
            var url = CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, moduleId);

            var response = await _httpClient.PostAsJsonAsync(url, request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }

            return null;
        }
    }
}
