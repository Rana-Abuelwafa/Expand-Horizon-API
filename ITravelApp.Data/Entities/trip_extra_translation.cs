using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class trip_extra_translation
{
    public int id { get; set; }

    public int? extra_id { get; set; }

    public string? lang_code { get; set; }

    public string? extra_name { get; set; }

    public string? extra_code { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public string? created_by { get; set; }
}
