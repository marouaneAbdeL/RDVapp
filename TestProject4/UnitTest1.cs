
using System.Data;
using System.Data.SqlClient;

namespace TestProject4
{
    public class Tests
    {
        private string _connectionString = "Data Source=LAPTOP-LJ9438PF\\SQLEXPRESS2;Initial Catalog=RDVAPP;Integrated Security=True";

        [SetUp]
        public void Setup()
        {
        }
        /// <summary>
        ///1- Ajoute de employe ou utilisateur
        /// </summary>
        [Test]
        public void TestAddUser()
        {
            // Test data
            string testFirstName = "Marouane";
            string testLastName = "Abderrahim";
            string testEmail = "ouane1@example.com";
            TimeSpan testStartTime = TimeSpan.FromHours(9);
            TimeSpan testEndTime = TimeSpan.FromHours(17);
            int testDuration = 480;

            // Add user to the database
            int newUserId =115;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("INSERT INTO Users (FirstName, LastName, Email, LundiStartTime, LundiEndTime, LundiDuration, MardiStartTime, MardiEndTime, MardiDuration, MercrediStartTime, MercrediEndTime, MercrediDuration, JeudiStartTime, JeudiEndTime, JeudiDuration, VendrediStartTime, VendrediEndTime, VendrediDuration) VALUES (@FirstName, @LastName, @Email, @LundiStartTime, @LundiEndTime, @LundiDuration, @MardiStartTime, @MardiEndTime, @MardiDuration, @MercrediStartTime, @MercrediEndTime, @MercrediDuration, @JeudiStartTime, @JeudiEndTime, @JeudiDuration, @VendrediStartTime, @VendrediEndTime, @VendrediDuration); SELECT SCOPE_IDENTITY();", connection))
                {
                    command.Parameters.AddWithValue("@FirstName", testFirstName);
                    command.Parameters.AddWithValue("@LastName", testLastName);
                    command.Parameters.AddWithValue("@Email", testEmail);

                    command.Parameters.AddWithValue("@LundiStartTime", testStartTime);
                    command.Parameters.AddWithValue("@LundiEndTime", testEndTime);
                    command.Parameters.AddWithValue("@LundiDuration", testDuration);

                    command.Parameters.AddWithValue("@MardiStartTime", testStartTime);
                    command.Parameters.AddWithValue("@MardiEndTime", testEndTime);
                    command.Parameters.AddWithValue("@MardiDuration", testDuration);

                    command.Parameters.AddWithValue("@MercrediStartTime", testStartTime);
                    command.Parameters.AddWithValue("@MercrediEndTime", testEndTime);
                    command.Parameters.AddWithValue("@MercrediDuration", testDuration);

                    command.Parameters.AddWithValue("@JeudiStartTime", testStartTime);
                    command.Parameters.AddWithValue("@JeudiEndTime", testEndTime);
                    command.Parameters.AddWithValue("@JeudiDuration", testDuration);

                    command.Parameters.AddWithValue("@VendrediStartTime", testStartTime);
                    command.Parameters.AddWithValue("@VendrediEndTime", testEndTime);
                    command.Parameters.AddWithValue("@VendrediDuration", testDuration);

                    newUserId = Convert.ToInt32(command.ExecuteScalar());
                }
            }

            // Check if the user was added
            Assert.Greater(newUserId, 0);
        }
        //4-test le select des utilisateurs 
        [Test]
        public void TestSelectUser()
        {
            int testUserId = 48;

            DataRow? selectedUser = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM Users WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", testUserId);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        selectedUser = new DataTable().NewRow();
                        reader.Read();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            selectedUser[reader.GetName(i)] = reader.GetValue(i);
                        }
                    }
                }
            }


            Assert.IsNotNull(selectedUser);
        }
        //2-supprimer utilisateurs par id 
        [Test]
        public void TestDeleteUser()
        {
            int testUserId = 115; // Replace with a valid user ID to be deleted for the test

            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("DELETE FROM Users WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", testUserId);
                    rowsAffected = command.ExecuteNonQuery();
                }
            }

            // Check if the user was deleted successfully
            Assert.That(rowsAffected, Is.EqualTo(1));
        }


        //3-tester le filtrage des utilisateurs en fonction de la durree ajouter
        [Test]
        public void TestFilterUser()



        {
            int testMinDuration = 60;

            DataTable filteredUsers = new DataTable();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT Id, FirstName, LastName, Email, 'Lundi' AS AvailableDay, LundiStartTime, LundiEndTime FROM Users WHERE LundiDuration >= @minDuration
                            UNION ALL
                            SELECT Id, FirstName, LastName, Email, 'Mardi' AS AvailableDay, MardiStartTime, MardiEndTime FROM Users WHERE MardiDuration >= @minDuration
                            UNION ALL
                            SELECT Id, FirstName, LastName, Email, 'Mercredi' AS AvailableDay, MercrediStartTime, MercrediEndTime FROM Users WHERE MercrediDuration >= @minDuration
                            UNION ALL
                            SELECT Id, FirstName, LastName, Email, 'Jeudi' AS AvailableDay, JeudiStartTime, JeudiEndTime FROM Users WHERE JeudiDuration >= @minDuration
                            UNION ALL
                            SELECT Id, FirstName, LastName, Email, 'Vendredi' AS AvailableDay, VendrediStartTime, VendrediEndTime FROM Users WHERE VendrediDuration >= @minDuration", connection))
                {
                    command.Parameters.AddWithValue("@minDuration", testMinDuration);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(filteredUsers);
                }
            }

            // Use the constraint model for the assertion
            Assert.That(filteredUsers.Rows.Count, Is.GreaterThan(0));
        }


        

    }
}

