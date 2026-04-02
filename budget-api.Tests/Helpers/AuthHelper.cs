using System.Net.Http.Json;

namespace budget_api.Tests.Helpers
{
    public static class AuthHelper
    {
        public static async Task<string> GetTokenAsync(
            HttpClient client,
            string email = "wallet-test@test.com",
            string password = "Test1234")
        {
            // Register — ignore duplicate failures
            await client.PostAsJsonAsync("/api/authentication/register", new
            {
                firstName = "Test",
                lastName = "User",
                email,
                password,
                currency = "USD"
            });

            // Authenticate and extract the JWT from the Set-Cookie header
            var response = await client.PostAsJsonAsync("/api/authentication/authenticate", new
            {
                email,
                password
            });

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(
                    $"Authentication failed ({response.StatusCode}): {body}");
            }

            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                var tokenCookie = cookies.FirstOrDefault(c => c.StartsWith("app.at="));
                if (tokenCookie != null)
                    return tokenCookie.Split('=')[1].Split(';')[0];
            }

            throw new InvalidOperationException("JWT token not found in Set-Cookie response header.");
        }
    }
}
