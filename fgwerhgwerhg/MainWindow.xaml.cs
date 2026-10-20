using fgwerhgwerhg;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq;

namespace GymReservationApp
{
    public partial class MainWindow : Window
    {
        private Gym gym = new Gym();
        private const string FilePath = "foglalasok.txt";

        public class TimeSlotVM
        {
            public string TimeLabel { get; set; }
            public string StatusText { get; set; }
            public Brush BackgroundColor { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            PopulateTimeComboBox();
            gym.LoadFromFile(FilePath);
            RefreshGrid();
        }

        private void PopulateTimeComboBox()
        {
            for (int hour = 6; hour <= 21; hour++)
            {
                CmbTime.Items.Add($"{hour}:00");
            }
            CmbTime.SelectedIndex = 0;
        }

        private void Type_Changed(object sender, RoutedEventArgs e)
        {
            if (PanelSingle == null || PanelTeam == null) return;

            if (RbSingle.IsChecked == true)
            {
                PanelSingle.Visibility = Visibility.Visible;
                PanelTeam.Visibility = Visibility.Collapsed;
            }
            else
            {
                PanelSingle.Visibility = Visibility.Collapsed;
                PanelTeam.Visibility = Visibility.Visible;
            }
        }

        private void RefreshGrid()
        {
            List<TimeSlotVM> items = new List<TimeSlotVM>();

            for (int hour = 6; hour <= 21; hour++)
            {
                var reservationsAtHour = gym.Reservations.Where(r => r.GetReservationTime() == hour).ToList();
                var teamRes = reservationsAtHour.OfType<TeamReservation>().FirstOrDefault();

                string status;
                Brush bg;

                if (teamRes != null)
                {
                    int memberCount = teamRes.TeamMembers.Count + 1;
                    status = $"BLOKKOLVA - Csoportos foglalás ({teamRes.ContactName}, {memberCount} fő) | Fizetendő: {teamRes.CalculatePriceToPay()} Ft/fő";
                    bg = new SolidColorBrush(Color.FromRgb(255, 205, 210)); 
                }
                else if (reservationsAtHour.Any())
                {
                    int count = reservationsAtHour.Count;
                    status = $"{count} db Egyéni foglalás.";
                    bg = new SolidColorBrush(Color.FromRgb(225, 245, 254));
                }
                else
                {
                    status = "Szabad idősáv";
                    bg = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                }

                items.Add(new TimeSlotVM
                {
                    TimeLabel = $"{hour}:00 - {hour + 1}:00",
                    StatusText = status,
                    BackgroundColor = bg
                });
            }

            TimeSlotsItemsControl.ItemsSource = items;
        }

        private void BtnReserve_Click(object sender, RoutedEventArgs e)
        {
            string name = TxtName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Kérjük, adja meg a nevet!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int selectedHour = CmbTime.SelectedIndex + 6;
            GymReservation newReservation;

            if (RbSingle.IsChecked == true)
            {
                bool hasTicket = ChkSeasonTicket.IsChecked == true;
                newReservation = new SingleReservation(name, selectedHour, hasTicket);
            }
            else
            {
                List<string> members = TxtTeamMembers.Text
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(m => m.Trim())
                    .ToList();

                newReservation = new TeamReservation(name, members, selectedHour);
            }

           
            bool success = gym.AddReservation(newReservation);

            if (!success)
            {
                MessageBox.Show("Erre az időpontra már van csoportos foglalás! Egyik típusú foglalás sem engedélyezett erre az idősávra.",
                                "Foglalási Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show($"Sikeres foglalás! Fizetendő: {newReservation.CalculatePriceToPay()} Ft",
                                "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                TxtName.Clear();
                TxtTeamMembers.Clear();
                ChkSeasonTicket.IsChecked = false;
                RefreshGrid();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                gym.SaveToFile(FilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a mentés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}