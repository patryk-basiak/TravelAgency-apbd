using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientsService
{
    Task<List<TripDTO>> GetClientTrips(int id);
    Task<List<ClientsTrips>> GetTrips();
    Task<Task<int>> AddClient(ClientDTO client);
    Task<bool> AssignClientToTrip(int id, int trip);
    Task<bool> DeleteClientFromTrip(int id, int tripId);
}