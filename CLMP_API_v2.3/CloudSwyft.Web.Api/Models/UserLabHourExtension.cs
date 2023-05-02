using System;

namespace CloudSwyft.Web.Api.Models
{
    public class UserLabHourExtension
    {
        public int UserId { get; set; }
        public int VEProfileId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? ExtensionTypeId { get; set; }
        public int? LabHourExtensionId { get; set; }
        public bool? IsDeleted { get; set; }

        private DateTime? _startDate;
        public DateTime? StartDate {
            get 
            {
                return _startDate?.ToLocalTime();
            }
            set 
            {
                _startDate = value;
            } 
        }

        private DateTime? _endDate;
        public DateTime? EndDate 
        {
            get 
            {
                return _endDate?.ToLocalTime();
            }
            set 
            {
                _endDate = value;
            } 
        }

        private decimal? _totalHours;

        public decimal? TotalHours 
        {
            get 
            {
                return _totalHours;
            }
            set 
            {
                _totalHours = value;
            }
        }

        public bool? IsFixedLabHourExtension { get; set; }

        public string CreatedBy { get; set; }

        public string Duration 
        {
            get 
            {
                string duration = string.Empty;

                if (_endDate.HasValue && _startDate.HasValue)
                {
                    TimeSpan span = (_endDate.Value.AddSeconds(-_endDate.Value.Second).AddMilliseconds(-_endDate.Value.Millisecond) - _startDate.Value.AddSeconds(-_startDate.Value.Second).AddMilliseconds(-_startDate.Value.Millisecond));
                    duration = string.Format("{0:00}:{1:00}:{2:00}", span.Days, span.Hours, span.Minutes);
                }

                return duration;
            }
        }

        public string TotalHoursDisplay 
        {
            get 
            {
                if (_totalHours.HasValue)
                {
                    var timeSpan = TimeSpan.FromHours((double)_totalHours.Value);
                    int hh = timeSpan.Hours;
                    int mm = timeSpan.Minutes;

                    return string.Format("{0:00}:{1:00}", hh, mm);
                }

                return string.Empty;
            }
        }

        public string StartDateDisplay 
        {
            get 
            {
                return string.Format("{0:g}", StartDate);
            }
        }

        public string EndDateDisplay
        {
            get
            {
                return string.Format("{0:g}", EndDate);
            }
        }

        public string DateValidDisplay
        {
            get 
            {
                return string.Format("{0:g} - {1:g}", StartDate, EndDate);
            }
        }

        public int TotalHourValue 
        {
            get 
            {
                if (_totalHours.HasValue)
                {
                    var timeSpan = TimeSpan.FromHours((double)_totalHours.Value);
                    int hh = timeSpan.Hours;
                    return hh;
                }

                return 0;
            }
        }

        public int TotalMinuteValue
        {
            get
            {
                if (_totalHours.HasValue)
                {
                    var timeSpan = TimeSpan.FromHours((double)_totalHours.Value);
                    int mm = timeSpan.Minutes;
                    return mm;
                }

                return 0;
            }
        }
    }
}