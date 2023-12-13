using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CCBnb_Admin_Web.Controllers
{
    //[Authorize] // Ensure this controller requires authentication
    public class ApiController : Controller
    {
        private readonly HttpClient client;
        private const string ApiKey = "AAztAHKI9cUnlVHzxDqtY1g43aLr4FyrJJkGAbhQTBmDfm94";
        private const string ApiBaseUrl = "https://34.128.145.217.nip.io/bnb_auth_v1";
        //private const string ApiBaseUrl = "http://localhost:8306";

        private const string ApiHost = "https://34.128.145.217.nip.io";

        public HttpClient Client => client;

        public ApiController()
        {
            //https://34.128.145.217.nip.io/bnb_auth_v1/api/Booking

            client = new HttpClient(new ApiKeyHttpClientHandler())
            {
                //BaseAddress = new Uri(ApiBaseUrl)
                BaseAddress = new Uri(ApiHost),
                Timeout = TimeSpan.FromSeconds(45)

            };
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        private class ApiKeyHttpClientHandler : HttpClientHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
               


                
                //拦截API请求的 Path
                var apiPath = request.RequestUri.AbsolutePath;

                //拦截API请求的 Method
                var apiMethod = request.Method.Method;

                //Url上加入 前缀 /bnb_auth_v1
                var  targetUri  = new Uri($"{ApiBaseUrl}{apiPath}");




               


                // Append the API Key to each request
                var uriBuilder = new UriBuilder(targetUri);
                var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);

                //check if path has /bnb_auth_v1 ,add apikey
                if (ApiBaseUrl.ToUpper().Contains("bnb_auth_v1".ToUpper()))
                {
                    query["apikey"] = ApiKey;

                }

                uriBuilder.Query = query.ToString();
                request.RequestUri = uriBuilder.Uri;

                // Log the request (optional)

                return await base.SendAsync(request, cancellationToken);
            }
        }

        // Rest of the controller methods...
    }
}
