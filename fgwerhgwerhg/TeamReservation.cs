using fgwerhgwerhg;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymReservationApp
{
    public class TeamReservation : GymReservation
    {
        public string ContactName { get; set; }
        public List<string> TeamMembers { get; set; }
        public int ReservationTime { get; set; }

        public TeamReservation(string contactName, List<string> teamMembers, int reservationTime)
        {
            ContactName = contactName;
            TeamMembers = teamMembers ?? new List<string>();
            ReservationTime = reservationTime;
        }

        public string GetContactName() => ContactName;

        public int GetReservationTime() => ReservationTime;

        public int CalculatePriceToPay()
        {
            int totalPeople = TeamMembers.Count + 1; 
            return 9000 / totalPeople;
        }
    }
}