using System.Net.Http.Headers;

namespace BookStoreAPI
{
    [TestClass]
    public class TestBase
    {
        private static Uri pageUrl = new Uri("https://demoqa.com/");

        public static HttpClient authorizedClient;
        public static HttpClient anonymusClient;
        public static HttpClient unauthorizedClient;

        [AssemblyInitialize]
        public static void Initialize(TestContext testContext)
        {
            authorizedClient = new HttpClient();
            authorizedClient.BaseAddress = pageUrl;
            AddClientAuthorization(authorizedClient, "userv", "Fdlgjkdlfg2232###");

            unauthorizedClient = new HttpClient();
            unauthorizedClient.BaseAddress = pageUrl;
            AddClientAuthorization(unauthorizedClient, "noUser", "noPassword");

            anonymusClient = new HttpClient();
            anonymusClient.BaseAddress = pageUrl;
        }

        public static void AddClientAuthorization(HttpClient client, string userName, string UserPassword)
        {
            string authenticationString = $"{userName}:{UserPassword}";
            string base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        }
    }
}