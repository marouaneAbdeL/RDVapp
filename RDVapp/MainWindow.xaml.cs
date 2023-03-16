using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {/*
        //variable de connexion
        private MySqlConnection conDB;

        public MainWindow()
        {
            InitializeComponent();

            //désactiver les dates futures pour la date de naissance
            //dateDeNaissance.DisplayDateEnd = DateTime.Today;

            // l'objet qui vas contenir la route de connexion
            conDB = new MySqlConnection("Server=localhost;Uid=root;Pwd=;database=application_stagiares");

            //appel a la fonction pour ajouter les nom de s utilisateurs disponible  dans la combox utilisateurs disponible dans la page rendez-vous
            Ajouter_UtlisateurDisponible();

            //appel a la fonction pour afficher les UTILISATEURS ajouter dans le datagrid de page Utilisateurs
            Ajouter_Utilisateur();

            //appel a la fonction ajoute la date des rendez-vous disponible 
            Ajouter_RendezvousDisponible();

            //appel a la fonction afficher les rdvs ajouter dans le datagrid de page rdvs 
            Ajouter_Rendezvous();

        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //afficher la liste des utilisateur dans le datagrid de page utilisateur
        private void Ajouter_Utilisateur()
        {
            try
            {
                conDB.Open();

                string sql = "SELECT * FROM utilisateur";
                MySqlCommand cmd = new MySqlCommand(sql, conDB);
                MySqlDataReader dr = cmd.ExecuteReader();

                DataTable dataTable = new DataTable();
                dataTable.Load(dr);

                dr.Close();
                conDB.Close();

                dataGrid_users.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //afficher la liste des utilisateur dans le datagrid de page utilisateur
        private void Ajouter_RendezvousDisponible()
        {
            try
            {
                conDB.Open();
                string sql = "INSERT INTO rendezvousdisponible values (@date_dispo, @heure_debut, @heure_fin,@duree)";
                MySqlCommand cmd = new MySqlCommand(sql, conDB);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@date_dispo", date_dispo.Text);
                cmd.Parameters.AddWithValue("@heure_debut", heure_debut.Text);
                cmd.Parameters.AddWithValue("@heure_fin", heure_fin.Text);
                cmd.Parameters.AddWithValue("@duree", duree.Text);



                cmd.ExecuteNonQuery();

                
                conDB.Close();

                dataGrid_users.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        //affichage rendez-vous dans datagrid rendez-vous
        private void Ajouter_Rendezvous()
        {
            try
            {
                conDB.Open();

                string sql = "SELECT * FROM rendezvousdisponible ";

                MySqlCommand cmd = new MySqlCommand(sql, conDB);


                MySqlDataReader dr = cmd.ExecuteReader();

                DataTable dataTable = new DataTable();
                dataTable.Load(dr);


                dr.Close();

                conDB.Close();


                dataGrid.ItemsSource = dataTable.DefaultView;
                dataGrid_appointements.ItemsSource = dataTable.DefaultView;

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());

            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // slectioner les nom des programmes dans la table programme et les afficher dans la combox programe d'étude dans la page
            // stagiare et nom de programme dans la page consulter
            private void Ajouter_UtlisateurDisponible()
            {
                try
                {
                    conDB.Open();

                    string sql = "SELECT *FROM utilisateurs JOIN rendezvousdisponible rd ON u.date_dispo = rd.date_dispo WHERE u.heure_debut >= rd.heure_debut AND u.heure_fin <= rd.heure_fin;";
                    MySqlCommand cmd = new MySqlCommand(sql, conDB);
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        UserComboBox.Items.Add(dr[0]);
                        
                    }
                    dr.Close();

                    conDB.Close();

                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.ToString());
                }
            }



            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //   l'ajout des Utilisateur 
            private void btn_ajouterUtilisateur_Click(object sender, RoutedEventArgs e)
            {

                //// tous les champs doivent etre remplis
                if (string.IsNullOrEmpty(nom.Text) || string.IsNullOrEmpty(prenom.Text) || string.IsNullOrEmpty(courriel.Text) ||
                    string.IsNullOrEmpty(date_dispo.Text) || string.IsNullOrEmpty(heure_debut.Text) || string.IsNullOrEmpty(heure_fin.Text))
                {
                    MessageBox.Show("Veiller entrer tous les champs Exeption modifer/supprimer", "Gestion des stagiaires", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                
                else
                {
                    // validation au niveau du courriel lorsque on essaye de rentrer un courriel deja existant
                    conDB.Open();
                    string sql_checkNum = "SELECT courriel FROM utilisateurs where courriel = " + courriel.Text;

                    MySqlCommand cmd_checkNum = new MySqlCommand(sql_checkNum, conDB);

                    string dr_checkNum = (string)cmd_checkNum.ExecuteScalar();


                    conDB.Close();
                    if (dr_checkNum == courriel.Text)
                    {
                        MessageBox.Show("courriel déja existant", "Gestion des utilisateurs", MessageBoxButton.OK, MessageBoxImage.Information);
                        nom.Clear();
                        prenom.Clear();
                        courriel.Text = "";
                        date_dispo.Text = "";
                        heure_debut.Text = "";
                        heure_fin.Text = "";

                    }
                    // insertion des stagaires a la base de données
                    else
                    {

                        try
                        {
                            conDB.Open();

                            string sql = "INSERT INTO utilisateurs values (@nom, @prenom, @courriel, @date_dispo, @heure_debut, @heure_fin)";
                            MySqlCommand cmd = new MySqlCommand(sql, conDB);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@nom", nom.Text);
                            cmd.Parameters.AddWithValue("@prenom", prenom.Text);
                            cmd.Parameters.AddWithValue("@courriel", courriel.Text);
                            cmd.Parameters.AddWithValue("@date_dispo", date_dispo.Text);
                            cmd.Parameters.AddWithValue("@heure_debut", heure_debut.Text);
                            cmd.Parameters.AddWithValue("@heure_fin", heure_fin.Text);

                            cmd.ExecuteNonQuery();

                            conDB.Close();

                            Ajouter_Utilisateur();
                            

                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine(ex.ToString());
                        }




                        // afficher un pop-up que l'utilisatuer a bien eté ajouter
                        MessageBox.Show("Le nouveau Utilisateur a été ajouter avec succés", "Gestion des utilisateur", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Efaccer tous les champs une fois ajouter le stagiare
                        nom.Clear();
                        prenom.Clear();
                        courriel.Text = "";
                        date_dispo.Text = "";
                        heure_debut.Text = "";
                        heure_fin.Text="";
                    }
                }
            }


            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // supprimer un utilisatuer
            private void btn_supprimerUtilisateur_Click(object sender, RoutedEventArgs e)
            {
                if (string.IsNullOrEmpty(modSupp.Text))
                {
                    MessageBox.Show("Veiller selectionner l'utilisateur a supprimer dans le datagrid ou bien écrire id utilisateur désirer a supprimer dans le champ modifier/supprimer", "Gestion des stagiaires", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    try
                    {
                        conDB.Open();

                        string sql = "DELETE FROM utilisateur where id = @id";

                        MySqlCommand cmd = new MySqlCommand(sql, conDB);

                        cmd.CommandType = CommandType.Text;

                        //Inserer nos valeurs
                        cmd.Parameters.AddWithValue("@id", modSupp.Text);

                        cmd.ExecuteNonQuery();
                        conDB.Close();
                        Ajouter_Utilisateur();

                        nom.Clear();
                        prenom.Clear();
                        courriel.Text = "";
                        date_dispo.Text = "";
                        heure_debut.Text = "";
                        heure_fin.Text = "";

                        modSupp.Clear();


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////
            
            //   l'ajout des Disponibilite des rendez-cous 
            private void btn_ajouterrendezvous_Click(object sender, RoutedEventArgs e)
            {

                Ajouter_RendezvousDisponible();
                //l'utilisateur selection d'apres combobox
                Ajouter_UtlisateurDisponible();
                
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            
            // Annuler un rendez vous
            private void btn_supprimerRendezvous_Click(object sender, RoutedEventArgs e)
            {
                if (string.IsNullOrEmpty(modSupp.Text))
                {
                    MessageBox.Show("Veiller selectionner le rendez-vous a supprimer dans le datagrid ou bien écrire id rendez-vous désirer a supprimer dans le champ supprimer", "Gestion des rendez-vous", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    try
                    {
                        conDB.Open();

                        string sql = "DELETE FROM RendezVous where id = @id";

                        MySqlCommand cmd = new MySqlCommand(sql, conDB);

                        cmd.CommandType = CommandType.Text;

                        //Inserer nos valeurs
                        cmd.Parameters.AddWithValue("@id", modSupp.Text);

                        cmd.ExecuteNonQuery();
                        conDB.Close();

                        Ajouter_RendezVous();

                        id_utilisateur.Clear();
                        id_responsable.Clear();
                        nom_utilisateur.Text = "";
                        date_rdv.Text = "";
                        heure_debut.Text = "";
                        heure_fin.Text = "";

                        modSupp.Clear();


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }






        }



*/
    }
            
    }




