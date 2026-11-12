namespace RetailEase.Domain.Entities;

/// <summary>
/// Device category managed by the Client Manager.
/// </summary>
public enum ClientDeviceType
{
    Pos = 0,

    Kiosk = 1,

    Dmd = 2,

    Oms = 3
}

/// <summary>
/// Operational status of a registered client device.
/// </summary>
public enum ClientDeviceStatus
{
    Offline = 0,

    Active = 1
}

/// <summary>
/// Represents a registered client device (POS terminal, kiosk, DMD or OMS) known to the Client Manager.
/// </summary>
public class ClientDevice
{
    public Guid Id { get; set; }

    public ClientDeviceType DeviceType { get; set; }

    public string Identifier { get; set; } = string.Empty;

    public ClientDeviceStatus Status { get; set; } = ClientDeviceStatus.Offline;

    public DateTime? LastHeartbeatUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    /// <summary>
    /// Creates a new client device in the Offline state. The device identifier must be
    /// supplied by the registration process to maintain device identity.
    /// </summary>
    public static ClientDevice Create(ClientDeviceType deviceType, string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            throw new ArgumentException("A client device identifier is required.", nameof(identifier));
        }

        return new ClientDevice
        {
            Id = Guid.NewGuid(),
            DeviceType = deviceType,
            Identifier = identifier,
            Status = ClientDeviceStatus.Offline,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
    }

    /// <summary>Records a heartbeat from the device, transitioning it to the Active state.</summary>
    public void Heartbeat()
    {
        Status = ClientDeviceStatus.Active;
        LastHeartbeatUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
