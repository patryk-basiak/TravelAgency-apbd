using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class ClientsService : IClientsService
{
    private readonly string _connectionString =
        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;";

    private async Task<List<CountryDTO>> GetCountries(int idT)
    {

        var countries = new List<CountryDTO>();
        string command =
            "SELECT Country.IdCountry, Country.Name FROM Country_Trip INNER JOIN Country ON Country_Trip.IdCountry = Country.IdCountry WHERE Country_Trip.IdTrip =" +
            idT;

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdCountry");
                    countries.Add(new CountryDTO()
                    {
                        Name = reader.GetString(1),
                    });
                }
            }
        }

        return countries;
    }

    public async Task<List<TripDTO>> GetClientTrips(int id)
    {
        var trips = new List<TripDTO>();
        string command =
            "SELECT Client.IdClient, Trip.IdTrip, Trip.Name, Trip.Description FROM Client INNER JOIN Client_Trip ON Client.IdClient = Client_Trip.IdClient Inner Join Trip ON Client_Trip.IdTrip = Trip.IdTrip WHERE Client.IdClient =" +
            id;

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdTrip");
                    trips.Add(new TripDTO()
                    {
                        Id = reader.GetInt32(idOrdinal),
                        Name = reader.GetString(2),
                        Countries = await GetCountries(reader.GetInt32(idOrdinal))
                    });
                }
            }
        }

        return trips;
    }

    public async Task<List<ClientsTrips>> GetTrips()
    {
        var clients = new List<ClientsTrips>();

        string command = "SELECT IdClient,FirstName, LastName FROM Client";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdClient");
                    clients.Add(new ClientsTrips()
                    {
                        Id = reader.GetInt32(idOrdinal),
                        Name = reader.GetString(1),
                        Surname = reader.GetString(2),
                    });
                }
            }
        }

        return clients;
    }
    public Task<bool> CreateClient(ClientsTripDTO client)
    {
        string command = "INSERT INTO Client (FirstName, LastName, Email, PhoneNumber, Pesel) VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @Pesel)";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@FirstName", client.Name);
            cmd.Parameters.AddWithValue("@LastName", client.Surname);
            cmd.Parameters.AddWithValue("@Email", client.Email);
            cmd.Parameters.AddWithValue("@PhoneNumber", client.PhoneNumber);
            cmd.Parameters.AddWithValue("@Pesel", client.Pesel);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        
        return Task.FromResult(true);
    }
}