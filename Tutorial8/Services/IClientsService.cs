using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientsService
{
    Task<List<TripDTO>> GetClientTrips(int id);
    Task<List<ClientsTrips>> GetTrips();
    Task<bool> CreateClient(ClientsTripDTO client);
}