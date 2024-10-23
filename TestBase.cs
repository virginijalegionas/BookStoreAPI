using System.Net.Http.Headers;

namespace BookStoreAPI
{
    [TestClass]
    public class TestBase
    {
        protected static string userId;
        protected static Uri pageUrl = new Uri("https://demoqa.com/");

        public static HttpClient authorizedClient;
        public static HttpClient anonymousClient;
        public static HttpClient unauthorizedClient;

        [AssemblyInitialize]
        public static void Initialize(TestContext testContext)
        {
            userId = testContext.Properties["userId"] as string;
            authorizedClient = new HttpClient();
            authorizedClient.BaseAddress = pageUrl;
            AddClientAuthorization(authorizedClient, testContext.Properties["userName"] as string, testContext.Properties["userPassword"] as string);

            unauthorizedClient = new HttpClient();
            unauthorizedClient.BaseAddress = pageUrl;
            AddClientAuthorization(unauthorizedClient, "noUser", "noPassword");

            anonymousClient = new HttpClient();
            anonymousClient.BaseAddress = pageUrl;
        }

        public static void AddClientAuthorization(HttpClient client, string userName, string userPassword)
        {
            string authenticationString = $"{userName}:{userPassword}";
            string base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        }
    }
}