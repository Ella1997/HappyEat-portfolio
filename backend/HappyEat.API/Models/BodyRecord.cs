using System;
using System.Collections.Generic;

namespace HappyEat.API.Models;

public partial class BodyRecord
{
    public int BodyRecordId { get; set; }

    public int UserId { get; set; }

    public DateOnly RecordDate { get; set; }

    public decimal ActivityLevel { get; set; }

    public decimal Height { get; set; }

    public decimal Weight { get; set; }

    public decimal? WaistSize { get; set; }

    public decimal? NeckSize { get; set; }

    public decimal? HipSize { get; set; }

    public decimal? BodyFat { get; set; }

    public decimal? MuscleMass { get; set; }

    public decimal? VisceralFat { get; set; }

    public virtual User User { get; set; } = null!;
}
