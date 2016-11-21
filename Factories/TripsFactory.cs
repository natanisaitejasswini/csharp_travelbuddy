using System.Collections.Generic;
using System;
using System.Linq;
using Dapper;
using System.Data;
using MySql.Data.MySqlClient;
using travelbuddy.Models;
using CryptoHelper;

namespace TravelApp.Factory
{
    public class TripsRepository : IFactory<Trip>
    {
        private string connectionString;
        public TripsRepository()
        {
            connectionString = "server=localhost;userid=root;password=root;port=8889;database=newtrips;SslMode=None";
        }

        internal IDbConnection Connection
        {
            get {
                return new MySqlConnection(connectionString);
            }
        }
         public void AddTrip(Trip trip_item)
        {
             using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute("INSERT IGNORE INTO trips (destination, plan, travelstart, travelend, created_at, updated_at, user_id) VALUES (@destination, @plan, @travelstart, @travelend,  NOW(), NOW(), @user_id)", trip_item);
            }
        }
        public Trip Trip_Last_ID()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Trip>("SELECT * FROM trips ORDER BY id DESC LIMIT 1").FirstOrDefault();
            }
        }

        public void Add_Traveller(int num1, int num2)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute($"INSERT INTO travellers (trip_id, user_id) VALUES ('{num1}', '{num2}')");
            }
        }
        public IEnumerable<Trip> CurrentUserTrips(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Trip>($"SELECT destination, travelstart, travelend, trips.id, plan from trips  JOIN travellers ON trips.id = travellers.trip_id JOIN users ON users.id = travellers.user_id WHERE travellers.user_id ='{id}'");
            }
        }
        public IEnumerable<Trip> ExceptCurrentUserTrips(int id)
        {
            using (IDbConnection dbConnection = Connection) 
            {
                dbConnection.Open();
                return dbConnection.Query<Trip>($"SELECT users.first_name, trips.id, destination, travelstart, travelend, plan from trips JOIN travellers ON trips.id = travellers.trip_id  JOIN users ON users.id = travellers.user_id WHERE travellers.trip_id not in (SELECT trips.id from trips JOIN travellers ON trips.id = travellers.trip_id  JOIN users ON users.id = travellers.user_id WHERE travellers.user_id = '{id}');");
            }
        }
        public void Join_Trip(string trip_id, int user_id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute($"INSERT INTO travellers (trip_id, user_id) VALUES ('{trip_id}', '{user_id}')");
            }
        }
        public Trip destination_Info(string id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var test = dbConnection.Query<Trip>($"SELECT trips.destination, trips.plan, trips.travelstart,trips.travelend, users.first_name FROM trips LEFT JOIN users ON users.id = trips.user_id where trips.id ='{id}';").FirstOrDefault();
                return test;
            }
        }
        public IEnumerable<Trip>  others(string id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Trip>($"SELECT users.first_name from trips JOIN travellers ON trips.id = travellers.trip_id  JOIN users ON users.id = travellers.user_id WHERE trips.id = '{id}' AND users.id NOT IN (SELECT users.id FROM trips LEFT JOIN users ON users.id = trips.user_id where trips.id = '{id}');");
            }
        }
    }
}