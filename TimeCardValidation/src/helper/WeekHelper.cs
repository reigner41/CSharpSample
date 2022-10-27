using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PC.Objects.AA.HOJTimeCardValidation {
    public class WeekHelper {
        public static List<string> GetMonthWeeks(int yearValue, int monthValue) {
            var weekList = new List<string>();
            var calendar = CultureInfo.CurrentCulture.Calendar;
            var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            var weekPeriods = Enumerable.Range(1, calendar.GetDaysInMonth(yearValue, monthValue))
                      .Select(d => {
                          var date = new DateTime(yearValue, monthValue, d);
                          var weekNumInYear = calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, firstDayOfWeek);
                          return new { date, weekNumInYear };
                      })
                      .GroupBy(x => x.weekNumInYear)
                      .Select(x => new { DateFrom = x.First().date, To = x.Last().date })
                      .ToList();
            foreach (var weeks in weekPeriods) {
                DateTime inputDate = weeks.To;
                var d = inputDate;
                CultureInfo cul = CultureInfo.CurrentCulture;
                var firstDayWeek = cul.Calendar.GetWeekOfYear(
                    d,
                    CalendarWeekRule.FirstDay,
                    DayOfWeek.Monday);
                int weekNum = cul.Calendar.GetWeekOfYear(
                    d,
                    CalendarWeekRule.FirstDay,
                    DayOfWeek.Monday);
                int year = weekNum == 52 && d.Month == 1 ? d.Year - 1 : d.Year;
                weekList.Add(year.ToString() + weekNum.ToString());
            }
            return weekList;
        }

        public static DateTime FirstDateOfWeek(int year, int weekOfYear) {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = Convert.ToInt32(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek) - Convert.ToInt32(jan1.DayOfWeek);
            DateTime firstWeekDay = jan1.AddDays(daysOffset);
            System.Globalization.CultureInfo curCulture = System.Globalization.CultureInfo.CurrentCulture;
            int firstWeek = curCulture.Calendar.GetWeekOfYear(jan1, curCulture.DateTimeFormat.CalendarWeekRule, curCulture.DateTimeFormat.FirstDayOfWeek);
            if (firstWeek <= 1) {
                weekOfYear -= 1;
            }
            return firstWeekDay.AddDays(weekOfYear * 7);
        }

        public static DateTime LastDateOfWeek(int year, int weeknumber) {
            var firstDate = FirstDateOfWeek(year, weeknumber);
            var allWeekDays = new List<DateTime>();
            allWeekDays.Add(firstDate);
            var currentDate = firstDate;
            currentDate = currentDate.AddDays(7);
            return currentDate;
        }

        public static List<DateTime> GetWeeksFromWeekID(int year, int weeknumber) {
            var firstDate = FirstDateOfWeek(year, weeknumber);
            var allWeekDays = new List<DateTime>();
            allWeekDays.Add(firstDate);
            var currentDate = firstDate;
            for (int d = 1; d < 7; d++) {
                currentDate = currentDate.AddDays(1);
                allWeekDays.Add(currentDate);
            }
            return allWeekDays;
        }

        public static int GetIso8601WeekOfYear(DateTime time) {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Sunday && day <= DayOfWeek.Wednesday) {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);
        }
    }
}
