using System.Data;
using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = "Data Source=localhost, 1433; User=SA; Password=Patryk123; Initial Catalog=master; Integrated Security=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;";
    
    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new List<TripDTO>();

        string command = "SELECT IdTrip, Name, MaxPeople FROM Trip";
        
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
                        Name = reader.GetString(1),
                        Countries = await GetCountries(reader.GetInt32(idOrdinal)),
                        MaxPeople = reader.GetInt32(2)
                    });
                }
            }
        }
        
        

        return trips;
    }
    

    private async Task<List<CountryDTO>> GetCountries(int idT)
    {
        
        var countries = new List<CountryDTO>();
        string command = "SELECT Country.IdCountry, Country.Name FROM Country_Trip INNER JOIN Country ON Country_Trip.IdCountry = Country.IdCountry WHERE Country_Trip.IdTrip =" + idT;
        
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
        string command = "SELECT Client.IdClient, Trip.IdTrip, Trip.Name, Trip.Description, ClientTrip.PaymentDate FROM Client INNER JOIN Client_Trip ON Client.IdClient = Client_Trip.IdClient Inner Join Trip ON Client_Trip.IdTrip = Trip.IdTrip WHERE Client.IdClient =" + id;
        
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
                        Name = reader.GetString(1),
                        Countries = await GetCountries(reader.GetInt32(idOrdinal))
                    });
                }
            }
        }

        return trips;
    }
}