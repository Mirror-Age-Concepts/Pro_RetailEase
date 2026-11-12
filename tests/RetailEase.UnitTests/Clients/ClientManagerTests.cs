using RetailEase.Application.Clients;
using RetailEase.Domain.Entities;
using RetailEase.Infrastructure.Clients;

namespace RetailEase.UnitTests.Clients;

public class ClientManagerTests
{
    [Fact]
    public async Task RegisterAsync_Then_GetAsync_ReturnsRegisteredClient()
    {
        var store = new InMemoryClientStore();
        var sut = new ClientManager(store);

        var registered = await sut.RegisterAsync(new RegisterClientRequest(DeviceType: ClientDeviceType.Pos, Identifier: "POS-001"));

        Assert.NotEqual(Guid.Empty, registered.Id);
        Assert.Equal(ClientDeviceType.Pos, registered.DeviceType);
        Assert.Equal("POS-001", registered.Identifier);
        Assert.Equal(ClientDeviceStatus.Offline, registered.Status);

        var fetched = await sut.GetAsync("POS-001");

        Assert.NotNull(fetched);
        Assert.Equal(registered.Id, fetched!.Id);
    }

    [Fact]
    public async Task RegisterAsync_SameIdentifierReturnsExistingClient()
    {
        var store = new InMemoryClientStore();
        var sut = new ClientManager(store);

        var first = await sut.RegisterAsync(new RegisterClientRequest(DeviceType: ClientDeviceType.Kiosk, Identifier: "Kiosk-001"));
        var second = await sut.RegisterAsync(new RegisterClientRequest(DeviceType: ClientDeviceType.Kiosk, Identifier: "Kiosk-001"));

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task HeartbeatAsync_UnknownIdentifier_Throws()
    {
        var sut = new ClientManager(new InMemoryClientStore());

        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.HeartbeatAsync("unknown-device"));
    }

    [Fact]
    public async Task HeartbeatAsync_SetsClientActiveAndRecordsHeartbeat()
    {
        var store = new InMemoryClientStore();
        var sut = new ClientManager(store);
        var registered = await sut.RegisterAsync(new RegisterClientRequest(DeviceType: ClientDeviceType.Dmd, Identifier: "DMD-001"));

        var heartbeat = await sut.HeartbeatAsync("DMD-001");

        Assert.Equal(ClientDeviceStatus.Active, heartbeat.Status);
        Assert.NotNull(heartbeat.LastHeartbeatUtc);
        Assert.Equal(registered.Id, heartbeat.Id);
    }
}
