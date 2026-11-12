using RetailEase.Domain.Entities;

namespace RetailEase.Application.Clients;

/// <summary>
/// Command to register a new client device with the Client Manager.
/// </summary>
public record RegisterClientRequest(
    ClientDeviceType DeviceType,
    string Identifier);
