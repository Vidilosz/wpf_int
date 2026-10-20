using fgwerhgwerhg;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace GymReservationApp
{
    public class Gym
    {
        public List<GymReservation> Reservations { get; private set; } = new List<GymReservation>();

        public bool AddReservation(GymReservation reservation)
        {
            
            if (reservation is TeamReservation)
            {
                bool isSlotBlockedByTeam = Reservations
                    .OfType<TeamReservation>()
                    .Any(r => r.GetReservationTime() == reservation.GetReservationTime());

                if (isSlotBlockedByTeam)
                {
                    return false; 
                }
            }

            Reservations.Add(reservation);
            return true;
        }

        public void SaveToFile(string filePath)
        {
            List<string> lines = new List<string>();

            foreach (var res in Reservations)
            {
                if (res is SingleReservation single)
                {
                    lines.Add($"SINGLE;{single.ContactName};{single.ReservationTime};{single.HasSeasonTicket}");
                }
                else if (res is TeamReservation team)
                {
                    string members = string.Join(",", team.TeamMembers);
                    lines.Add($"TEAM;{team.ContactName};{team.ReservationTime};{members}");
                }
            }

            File.WriteAllLines(filePath, lines);
        }

        public void LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            Reservations.Clear();
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split(';');
                if (parts[0] == "SINGLE")
                {
                    string name = parts[1];
                    int time = int.Parse(parts[2]);
                    bool hasTicket = bool.Parse(parts[3]);
                    AddReservation(new SingleReservation(name, time, hasTicket));
                }
                else if (parts[0] == "TEAM")
                {
                    string name = parts[1];
                    int time = int.Parse(parts[2]);
                    List<string> members = parts[3].Length > 0
                        ? parts[3].Split(',').ToList()
                        : new List<string>();

                    AddReservation(new TeamReservation(name, members, time));
                }
            }
        }
    }
}