using System.Data;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class ClientsService : IClientsService
{
    private readonly string _connectionString =
        "Data Source=localhost, 1433; User=SA; Password=Patryk123; Initial Catalog=master; Integrated Security=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False";

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

    public async Task<int> AddClient(ClientDTO client)
    {
        string command = "INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel) VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @Pesel)";
        string user = "SELECT MAX(IdClient) AS IdClient FROM Client";
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        using (SqlCommand us = new SqlCommand(user, conn))
        {
            cmd.Parameters.AddWithValue("@FirstName", client.Name);
            cmd.Parameters.AddWithValue("@LastName", client.Surname);
            cmd.Parameters.AddWithValue("@Email", client.Email);
            cmd.Parameters.AddWithValue("@PhoneNumber", client.Telephone);
            cmd.Parameters.AddWithValue("@Pesel", client.Pesel);
            
            await conn.OpenAsync();
            cmd.ExecuteNonQuery();
            using (var reader = await us.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return reader.GetInt32(0);
                }
            }
        }
        return 0;
        
    }

    public Task<bool> AssignClientToTrip(int id, int trip)
    {
        Console.WriteLine(trip);
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();

            var checkClient = new SqlCommand("SELECT COUNT(*) FROM Client WHERE IdClient = @id", conn);
            checkClient.Parameters.AddWithValue("@id", id);
            if ((int)checkClient.ExecuteScalar() == 0)
                return Task.FromResult(false);
            
            var checkTrip = new SqlCommand("SELECT MaxPeople FROM Trip WHERE IdTrip = @tripId", conn);
            checkTrip.Parameters.AddWithValue("@tripId", trip);
            var maxPeople = checkTrip.ExecuteScalar();
            if (maxPeople == null)
                return Task.FromResult(false);

            var insert = new SqlCommand(@"
            INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt)
            VALUES (@id, @tripId, @date)", conn);
            insert.Parameters.AddWithValue("@id", id);
            insert.Parameters.AddWithValue("@tripId", trip);
            insert.Parameters.AddWithValue("@date", int.Parse(DateTime.Now.ToString("yyyyMMdd")));
            insert.ExecuteNonQuery();
        }
        return Task.FromResult(true);
    }

    public Task<bool>  DeleteClientFromTrip(int id, int tripId)
    {
        using (var con = new SqlConnection(_connectionString))
            
        using (var delete = new SqlCommand(@"DELETE FROM Client_Trip WHERE IdClient = @id AND IdTrip = @tripId", con))
        {
            con.Open();
            var tripComm = new SqlCommand("SELECT COUNT(*) FROM Client_Trip WHERE IdTrip = @tripId", con);
            tripComm.Parameters.AddWithValue("@tripId", tripId);
            int tripExist = (int)tripComm.ExecuteScalar();
            if (tripExist == 0)
                return Task.FromResult(false);
            var clientComm = new SqlCommand("SELECT COUNT(*) FROM Client_Trip WHERE IdTrip = @tripId", con);
            clientComm.Parameters.AddWithValue("@tripId", tripId);
            int clientExist = (int)clientComm.ExecuteScalar();
            if (clientExist == 0)
                return Task.FromResult(false);
            delete.Parameters.AddWithValue("@id", id);
            delete.Parameters.AddWithValue("@tripId", tripId);
            
            var rows = delete.ExecuteNonQuery();
            if (rows == 0)
                return Task.FromResult(false);
        }

        return Task.FromResult(false);;
    }
}