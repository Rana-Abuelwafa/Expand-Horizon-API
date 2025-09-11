using ITravelApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.Bookings.Client
{
    public class BookingExtraCast :booking_extra
    {
        public decimal? extra_price { get; set; }
        public string? extra_name { get; set; }
    }
}
