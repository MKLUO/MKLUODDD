using System;

namespace MKLUODDD.Model.Domain {

    public class Time : Float {

        public Time(double value) : base(value) { }
        
        static DateTime Origin => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public Time(DateTime time) : this((time - Origin).TotalSeconds) { }
        public static implicit operator Time(DateTime time) => new Time(time);

        public static DateTime Now => DateTime.Now;

        public DateTime GetDateTime() => Origin.AddSeconds(Value);
    }
}