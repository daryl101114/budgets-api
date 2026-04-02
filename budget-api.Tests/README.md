# budget-api.Tests

Integration test suite for the Budget API. Tests spin up the full HTTP pipeline in-process using `WebApplicationFactory` with an isolated in-memory database — no running API or SQL Server required.

## Running the tests

```bash
dotnet test budget-api.Tests/budget-api.Tests.csproj
```

Or from the solution root:

```bash
dotnet test
```

## How it works

| Component | Role |
|---|---|
| `WebApplicationFactory<Program>` | Boots the real API pipeline in-process |
| EF Core InMemory | Replaces SQL Server — each test class gets its own isolated DB |
| `AuthHelper` | Registers a test user and authenticates to get a real JWT token |
| `TestJsonOptions` | Configures `System.Text.Json` with string enum support to match the API's serializer |

## Test coverage — Wallet module

| Test | Validates |
|---|---|
| `GetWallets_WithoutToken_Returns401` | Auth guard on GET |
| `CreateWallet_WithoutToken_Returns401` | Auth guard on POST |
| `CreateWallet_ValidPayload_Returns201` | Happy path creation |
| `CreateWallet_MissingRequiredFields_Returns400` | Required field validation |
| `CreateWallet_InvalidWalletType_Returns400` | Enum validation |
| `GetWallets_ReturnsCreatedWallet_WithCorrectFields` | Field mapping and serialization |
| `UpdateWallet_ValidPayload_Returns204AndUpdatesFields` | Update + verify via GET |
| `UpdateWallet_NonExistentWallet_Returns404` | Ownership / not found guard |
| `DeleteWallet_ValidId_Returns204` | Soft delete |
| `GetWallets_AfterDelete_WalletNotReturned` | Soft delete filter on GET |
| `DeleteWallet_AlreadyDeleted_Returns404` | Double delete guard |

## Adding new tests

1. Add a new `[Fact]` method to `Wallet/WalletEndpointTests.cs`
2. Call `Authorize()` before any authenticated request
3. Use `CreateWalletAsync(name)` as a helper to set up wallet state
4. Use `_client.GetFromJsonAsync<T>("/api/wallets", TestJsonOptions.Default)` for deserialization

To test a different module, create a new test class in its own folder (e.g. `Transactions/TransactionEndpointTests.cs`) using `IClassFixture<TestWebAppFactory>`.
