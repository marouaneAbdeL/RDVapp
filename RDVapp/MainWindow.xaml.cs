
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RDVapp
{
    public partial class MainWindow : Window
    {
        private DataTable selectedUsersTable;

        public MainWindow()
        {
            InitializeComponent();
            LoadUserData();
            // Initialize the selectedUsersTable
            selectedUsersTable = new DataTable();
            selectedUsersTable.Columns.Add("Id", typeof(int));
            selectedUsersTable.Columns.Add("FirstName", typeof(string));
            selectedUsersTable.Columns.Add("LastName", typeof(string));
            selectedUsersTable.Columns.Add("Email", typeof(string));
            selectedUsersTable.Columns.Add("AvailableDay", typeof(int));

            // Set the ItemsSource property of dataGrid_selectedUsers to the selectedUsersTable
            dataGrid_selectedUsers.ItemsSource = selectedUsersTable.DefaultView;
        }

        private void LoadUserData()
        {
            try
            {
                string connectionString = "Data Source=LAPTOP-LJ9438PF\\SQLEXPRESS2;Initial Catalog=RDVAPP;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT * FROM Users", connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // Clear existing columns before adding new ones
                        dataGrid_Users.Columns.Clear();

                        
                        dataGrid_Users.ItemsSource = dt.DefaultView;
                        dataGrid_Users.AutoGenerateColumns = false;

                        // Create the columns
                        DataGridTextColumn firstNameColumn = new DataGridTextColumn
                        {
                            Header = "First Name",
                            Binding = new Binding("FirstName")
                        };
                        dataGrid_Users.Columns.Add(firstNameColumn);

                        DataGridTextColumn lastNameColumn = new DataGridTextColumn
                        {
                            Header = "Last Name",
                            Binding = new Binding("LastName")
                        };
                        dataGrid_Users.Columns.Add(lastNameColumn);

                        DataGridTextColumn emailColumn = new DataGridTextColumn
                        {
                            Header = "Email",
                            Binding = new Binding("Email")
                        };
                        dataGrid_Users.Columns.Add(emailColumn);

                        // Add remaining columns for Lundi, Mardi, Mercredi, Jeudi, and Vendredi
                        AddTimeColumns("Lundi");
                        AddTimeColumns("Mardi");
                        AddTimeColumns("Mercredi");
                        AddTimeColumns("Jeudi");
                        AddTimeColumns("Vendredi");

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show("An error occurred while loading the user data.");
            }
        }

        private void AddTimeColumns(string day)
        {
            DataGridTextColumn startTimeColumn = new DataGridTextColumn
            {
                Header = $"{day} Start Time",
                Binding = new Binding($"{day}StartTime")
            };
            dataGrid_Users.Columns.Add(startTimeColumn);

            DataGridTextColumn endTimeColumn = new DataGridTextColumn
            {
                Header = $"{day} End Time",
                Binding = new Binding($"{day}EndTime")
            };
            dataGrid_Users.Columns.Add(endTimeColumn);

            DataGridTextColumn durationColumn = new DataGridTextColumn
            {
                Header = $"{day} Duration",
                Binding = new Binding($"{day}Duration")
            };
            dataGrid_Users.Columns.Add(durationColumn);
        }

        private bool ValidateUserInput()
        {
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Please fill in all the required fields (First Name, Last Name, and Email).");
                return false;
            }

            if (!EmailTextBox.Text.Contains("@") || !EmailTextBox.Text.Contains("."))
            {
                MessageBox.Show("Please enter a valid email address.");
                return false;
            }
            var days = new List<KeyValuePair<string, Xceed.Wpf.Toolkit.TimePicker>> {
        new KeyValuePair<string, Xceed.Wpf.Toolkit.TimePicker>("Lundi", AvailabilityStartTimePickerLundi),
        new KeyValuePair<string, Xceed.Wpf.Toolkit.TimePicker>("Mardi", AvailabilityStartTimePickerMardi),
        new KeyValuePair<string, Xceed.Wpf.Toolkit.TimePicker>("Mercredi", AvailabilityStartTimePickerMercredi),
        new KeyValuePair<string, Xceed.Wpf.Toolkit.TimePicker>("Jeudi", AvailabilityStartTimePickerJeudi),
        new KeyValuePair<string, Xceed.Wpf.Toolkit.TimePicker>("Vendredi", AvailabilityStartTimePickerVendredi)
    };

            foreach (var day in days)
            {
                if (day.Value.Value != null && (day.Value.Value.Value.TimeOfDay < TimeSpan.FromHours(9) || day.Value.Value.Value.TimeOfDay > TimeSpan.FromHours(17)))
                {
                    MessageBox.Show($"{day.Key}'s start time must be between 9 AM and 5 PM.");
                    return false;
                }
            }



            return true;
        }

        private void CreateUserButton_Click(object sender, RoutedEventArgs e)
        {


            if (!ValidateUserInput())
            {
                return;
            }

            try
            {

                string connectionString = "Data Source=LAPTOP-LJ9438PF\\SQLEXPRESS2;Initial Catalog=RDVAPP;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("INSERT INTO Users (FirstName, LastName, Email, LundiStartTime, LundiEndTime, LundiDuration, MardiStartTime, MardiEndTime, MardiDuration, MercrediStartTime, MercrediEndTime, MercrediDuration, JeudiStartTime, JeudiEndTime, JeudiDuration, VendrediStartTime, VendrediEndTime, VendrediDuration) VALUES (@FirstName, @LastName, @Email, @LundiStartTime, @LundiEndTime, @LundiDuration, @MardiStartTime, @MardiEndTime, @MardiDuration, @MercrediStartTime, @MercrediEndTime, @MercrediDuration, @JeudiStartTime, @JeudiEndTime, @JeudiDuration, @VendrediStartTime, @VendrediEndTime, @VendrediDuration)", connection))

                    {
                        command.Parameters.AddWithValue("@FirstName", FirstNameTextBox.Text);
                        command.Parameters.AddWithValue("@LastName", LastNameTextBox.Text);
                        command.Parameters.AddWithValue("@Email", EmailTextBox.Text);

                        command.Parameters.AddWithValue("@LundiStartTime", AvailabilityStartTimePickerLundi.Value.HasValue ? AvailabilityStartTimePickerLundi.Value.Value.TimeOfDay : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@LundiEndTime", AvailabilityEndTimePickerLundi.Value.HasValue ? AvailabilityEndTimePickerLundi.Value.Value.TimeOfDay : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@LundiDuration", AvailabilityStartTimePickerLundi.Value.HasValue && AvailabilityEndTimePickerLundi.Value.HasValue ? (int)AvailabilityEndTimePickerLundi.Value.Value.TimeOfDay.Subtract(AvailabilityStartTimePickerLundi.Value.Value.TimeOfDay).TotalMinutes : (object)DBNull.Value);


                        command.Parameters.AddWithValue("@MardiStartTime", AvailabilityStartTimePickerMardi.Value.HasValue ? AvailabilityStartTimePickerMardi.Value.Value.TimeOfDay : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@MardiEndTime", AvailabilityEndTimePickerMardi.Value.HasValue ? AvailabilityEndTimePickerMardi.Value.Value.TimeOfDay : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@MardiDuration", AvailabilityStartTimePickerMardi.Value.HasValue && AvailabilityEndTimePickerMardi.Value.HasValue ? (int)AvailabilityEndTimePickerMardi.Value.Value.TimeOfDay.Subtract(AvailabilityStartTimePickerMardi.Value.Value.TimeOfDay).TotalMinutes : (object)DBNull.Value);

                        command.Parameters.AddWithValue("@MercrediStartTime", AvailabilityStartTimePickerMercredi.Value.HasValue ? AvailabilityStartTimePickerMercredi.Value.Value.TimeOfDay : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@MercrediEndTime", AvailabilityEndTimePickerMercredi.Value.HasValue ? AvailabilityEndTimePickerMercredi.Value.Value.TimeOfDay : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@MercrediDuration", AvailabilityStartTimePickerMercredi.Value.HasValue && AvailabilityEndTimePickerMercredi.Value.HasValue ? (int)AvailabilityEndTimePickerMercredi.Value.Value.TimeOfDay.Subtract(AvailabilityStartTimePickerMercredi.Value.Value.TimeOfDay).TotalMinutes : (object)DBNull.Value);

                        command.Parameters.AddWithValue("@JeudiStartTime", AvailabilityStartTimePickerJeudi.Value.HasValue ? AvailabilityStartTimePickerJeudi.Value.Value.TimeOfDay : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@JeudiEndTime", AvailabilityEndTimePickerJeudi.Value.HasValue ? AvailabilityEndTimePickerJeudi.Value.Value.TimeOfDay : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@JeudiDuration", AvailabilityStartTimePickerJeudi.Value.HasValue && AvailabilityEndTimePickerJeudi.Value.HasValue ? (int)AvailabilityEndTimePickerJeudi.Value.Value.TimeOfDay.Subtract(AvailabilityStartTimePickerJeudi.Value.Value.TimeOfDay).TotalMinutes : (object)DBNull.Value);

                        command.Parameters.AddWithValue("@VendrediStartTime", AvailabilityStartTimePickerVendredi.Value.HasValue ? AvailabilityStartTimePickerVendredi.Value.Value.TimeOfDay : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@VendrediEndTime", AvailabilityEndTimePickerVendredi.Value.HasValue ? AvailabilityEndTimePickerVendredi.Value.Value.TimeOfDay : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@VendrediDuration", AvailabilityStartTimePickerVendredi.Value.HasValue && AvailabilityEndTimePickerVendredi.Value.HasValue ? (int)AvailabilityEndTimePickerVendredi.Value.Value.TimeOfDay.Subtract(AvailabilityStartTimePickerVendredi.Value.Value.TimeOfDay).TotalMinutes : (object)DBNull.Value);

                        command.ExecuteNonQuery();
                    }
                }



                LoadUserData();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show("An error occurred while creating the user.");
            }
        }

        private bool ValidateFilterInput()
        {
            if (string.IsNullOrWhiteSpace(DurationFilterTextBox.Text))
            {
                MessageBox.Show("Please enter a minimum duration.");
                return false;
            }

            if (!int.TryParse(DurationFilterTextBox.Text, out _))
            {
                MessageBox.Show("Please enter a valid minimum duration (integer).");
                return false;
            }

            return true;
        }

        private void FilterUsersButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFilterInput())
            {
                return;
            }

            try
            {
                int minDuration = int.Parse(DurationFilterTextBox.Text);

                string connectionString = "Data Source=LAPTOP-LJ9438PF\\SQLEXPRESS2;Initial Catalog=RDVAPP;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
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
                        command.Parameters.AddWithValue("@minDuration", minDuration);
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        selectedUsersTable = new DataTable();
                        adapter.Fill(selectedUsersTable);
                        dataGrid_filtredUsers.ItemsSource = selectedUsersTable.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show("An error occurred while filtering users.");
            }
        }

        private bool ValidateDeleteUserByIdInput()
        {
            if (string.IsNullOrWhiteSpace(DeleteUserByIdInput.Text))
            {
                MessageBox.Show("Please enter a user ID to delete.");
                return false;
            }

            if (!int.TryParse(DeleteUserByIdInput.Text, out _))
            {
                MessageBox.Show("Invalid user ID format. Please enter a valid number.");
                return false;
            }

            return true;
        }

        private void DeleteUserByIdButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateDeleteUserByIdInput())
            {
                return;
            }

            try
            {
                string connectionString = "Data Source=LAPTOP-LJ9438PF\\SQLEXPRESS2;Initial Catalog=RDVAPP;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("DELETE FROM Users WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", int.Parse(DeleteUserByIdInput.Text));

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("User successfully deleted.");
                            LoadUserData();
                            RefreshFilteredUsers();
                        }
                        else
                        {
                            MessageBox.Show("No user found with the provided ID.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show("An error occurred while deleting the user.");
            }
        }



        private void RefreshFilteredUsers()
        {
            if (!ValidateFilterInput())
            {
                return;
            }

            try
            {

                int minDuration = int.Parse(DurationFilterTextBox.Text);

                string connectionString = "Data Source=LAPTOP-LJ9438PF\\SQLEXPRESS2;Initial Catalog=RDVAPP;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
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
                        command.Parameters.AddWithValue("@minDuration", minDuration);
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        selectedUsersTable = new DataTable();
                        adapter.Fill(selectedUsersTable);
                        dataGrid_filtredUsers.ItemsSource = selectedUsersTable.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show("An error occurred while refreshing the filtered users.");
            }
        }
        private void AddSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected row in the dataGrid_filtredUsers
            DataRowView selectedRow = (DataRowView)dataGrid_filtredUsers.SelectedItem;

            if (selectedRow != null)
            {
                // Copy the selected row to dataGrid_selectedUsers
                DataRow newRow = ((DataView)dataGrid_selectedUsers.ItemsSource).Table.NewRow();
                newRow["Id"] = selectedRow["Id"];
                newRow["FirstName"] = selectedRow["FirstName"];
                newRow["LastName"] = selectedRow["LastName"];
                newRow["Email"] = selectedRow["Email"];
                


                ((DataView)dataGrid_selectedUsers.ItemsSource).Table.Rows.Add(newRow);
            }
        }


    }
}


