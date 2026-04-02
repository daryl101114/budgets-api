using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using budget_api.Models.DTOs;
using budget_api.Tests.Helpers;
using Xunit;

namespace budget_api.Tests.Wallet
{
    public class WalletEndpointTests : IClassFixture<TestWebAppFactory>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private string _token = string.Empty;

        public WalletEndpointTests(TestWebAppFactory factory)
        {
            _client = factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            _token = await AuthHelper.GetTokenAsync(_client);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private void Authorize() =>
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

        // Creates a wallet and returns its ID — used as setup in multiple tests
        private async Task<Guid> CreateWalletAsync(string name)
        {
            Authorize();
            var post = await _client.PostAsJsonAsync("/api/wallets", new
            {
                walletName = name,
                walletType = "Checking",
                institution = "Test Bank",
                balance = 500.00m,
                currency = "USD"
            });
            post.EnsureSuccessStatusCode();

            var result = await _client.GetFromJsonAsync<UserWalletsDto>(
                "/api/wallets", TestJsonOptions.Default);
            return result!.Wallets.First(w => w.WalletName == name).Id;
        }

        // ── 1. GET without auth ─────────────────────────────────────────────
        [Fact]
        public async Task GetWallets_WithoutToken_Returns401()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync("/api/wallets");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── 2. POST without auth ────────────────────────────────────────────
        [Fact]
        public async Task CreateWallet_WithoutToken_Returns401()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.PostAsJsonAsync("/api/wallets", new { walletName = "Test" });
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── 3. POST valid payload ───────────────────────────────────────────
        [Fact]
        public async Task CreateWallet_ValidPayload_Returns201()
        {
            Authorize();
            var response = await _client.PostAsJsonAsync("/api/wallets", new
            {
                walletName = "Chase Savings",
                walletType = "Savings",
                institution = "Chase",
                balance = 2000.00m,
                currency = "USD"
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        // ── 4. POST missing required fields ────────────────────────────────
        [Fact]
        public async Task CreateWallet_MissingRequiredFields_Returns400()
        {
            Authorize();
            var response = await _client.PostAsJsonAsync("/api/wallets", new
            {
                institution = "Chase"
                // walletName and walletType intentionally omitted
            });
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // ── 5. POST invalid enum value ──────────────────────────────────────
        [Fact]
        public async Task CreateWallet_InvalidWalletType_Returns400()
        {
            Authorize();
            var response = await _client.PostAsJsonAsync("/api/wallets", new
            {
                walletName = "Bad Wallet",
                walletType = "NotARealType",
                currency = "USD"
            });
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // ── 6. GET returns correct fields ───────────────────────────────────
        [Fact]
        public async Task GetWallets_ReturnsCreatedWallet_WithCorrectFields()
        {
            Authorize();
            await _client.PostAsJsonAsync("/api/wallets", new
            {
                walletName = "Field Check Wallet",
                walletType = "CreditCard",
                institution = "Amex",
                balance = 0.00m,
                currency = "USD"
            });

            var result = await _client.GetFromJsonAsync<UserWalletsDto>(
                "/api/wallets", TestJsonOptions.Default);

            var wallet = result!.Wallets.First(w => w.WalletName == "Field Check Wallet");
            Assert.Equal("CreditCard", wallet.WalletType.ToString());
            Assert.Equal("Amex", wallet.Institution);
            Assert.Equal(0.00m, wallet.Balance);
            Assert.Equal("USD", wallet.Currency.ToString());
            Assert.NotEqual(default, wallet.CreatedAt);
            Assert.NotEqual(default, wallet.UpdatedAt);
        }

        // ── 7. PUT updates fields correctly ────────────────────────────────
        [Fact]
        public async Task UpdateWallet_ValidPayload_Returns204AndUpdatesFields()
        {
            var walletId = await CreateWalletAsync("Update Me Wallet");

            Authorize();
            var put = await _client.PutAsJsonAsync($"/api/wallets/{walletId}", new
            {
                walletName = "Updated Name",
                institution = "New Bank"
            });
            Assert.Equal(HttpStatusCode.NoContent, put.StatusCode);

            var result = await _client.GetFromJsonAsync<UserWalletsDto>(
                "/api/wallets", TestJsonOptions.Default);
            var updated = result!.Wallets.First(w => w.Id == walletId);
            Assert.Equal("Updated Name", updated.WalletName);
            Assert.Equal("New Bank", updated.Institution);
        }

        // ── 8. PUT non-existent wallet ──────────────────────────────────────
        [Fact]
        public async Task UpdateWallet_NonExistentWallet_Returns404()
        {
            Authorize();
            var response = await _client.PutAsJsonAsync($"/api/wallets/{Guid.NewGuid()}", new
            {
                walletName = "Ghost"
            });
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // ── 9. DELETE returns 204 ───────────────────────────────────────────
        [Fact]
        public async Task DeleteWallet_ValidId_Returns204()
        {
            var walletId = await CreateWalletAsync("Delete Me Wallet");

            Authorize();
            var response = await _client.DeleteAsync($"/api/wallets/{walletId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        // ── 10. GET after delete — wallet not in list ───────────────────────
        [Fact]
        public async Task GetWallets_AfterDelete_WalletNotReturned()
        {
            var walletId = await CreateWalletAsync("Soft Delete Check Wallet");

            Authorize();
            await _client.DeleteAsync($"/api/wallets/{walletId}");

            var result = await _client.GetFromJsonAsync<UserWalletsDto>(
                "/api/wallets", TestJsonOptions.Default);
            Assert.DoesNotContain(result!.Wallets, w => w.Id == walletId);
        }

        // ── 11. DELETE already-deleted wallet ──────────────────────────────
        [Fact]
        public async Task DeleteWallet_AlreadyDeleted_Returns404()
        {
            var walletId = await CreateWalletAsync("Double Delete Wallet");

            Authorize();
            await _client.DeleteAsync($"/api/wallets/{walletId}");

            var second = await _client.DeleteAsync($"/api/wallets/{walletId}");
            Assert.Equal(HttpStatusCode.NotFound, second.StatusCode);
        }
    }
}
