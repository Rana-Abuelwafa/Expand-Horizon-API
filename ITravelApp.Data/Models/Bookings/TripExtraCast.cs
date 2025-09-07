using ITravelApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.Bookings
{
    public class TripExtraCast : trip_extra_translation
    {
        public decimal? extra_price { get; set; }

        public string? currency_code { get; set; }
    }
}
