using fgwerhgwerhg;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymReservationApp
{
    public class SingleReservation : GymReservation
    {
        public string ContactName { get; set; }
        public int ReservationTime { get; set; }
        public bool HasSeasonTicket { get; set; }

        public SingleReservation(string contactName, int reservationTime, bool hasSeasonTicket = false)
        {
            ContactName = contactName;
            ReservationTime = reservationTime;
            HasSeasonTicket = hasSeasonTicket;
        }

        public void BuySeasonTicket()
        {
            HasSeasonTicket = true;
        }

        public string GetContactName() => ContactName;

        public int GetReservationTime() => ReservationTime;

        public int CalculatePriceToPay()
        {
            return HasSeasonTicket ? 2250 : 4500;
        }
    }
}
