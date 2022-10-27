using PX.Data;
using PX.Data.BQL;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.FS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static PX.Objects.EP.TimeCardMaint;
using TMEPEmployee = PX.Objects.CR.Standalone.EPEmployee;
namespace PC.Objects.AA.HOJTimeCardValidation {
    public static class TimeCardValidationUtils {
        //TODO Remove Unused Codes from changes on concept
        public static DateTime RoundToNearestMinuteProper(DateTime dt, int roundmin) {
            if (roundmin == 0) //can be > 60 mins
                return dt;

            TimeSpan d = TimeSpan.FromMinutes(roundmin); //this can be passed as a parameter, or use any timespan unit FromDays, FromHours, etc.  

            long delta = 0;
            long modTicks = dt.Ticks % d.Ticks;

            bool roundUp = modTicks > (d.Ticks / 2);
            var offset = roundUp ? d.Ticks : 0;
            delta = offset - modTicks;
            return new DateTime(dt.Ticks + delta, dt.Kind);
        }
        public static TimeSpan RoundToNearestMinutes(this TimeSpan input, int minutes) {
            var totalMinutes = (int)(input + new TimeSpan(0, minutes / 2, 0)).TotalMinutes;
            return new TimeSpan(0, totalMinutes - totalMinutes % minutes, 0);
        }
        public static bool IsBewteenTwoTime(this DateTime rowDateTimeStart, DateTime rowDateTimeEnd, DateTime iteratedStartTime, DateTime iteratedEndTime) {
            bool isBetween = false;
            // dt=8:00 dtend=9:30 start=9:00 end=10:00
            // 1st line 8:00 to 9:30
            // 2nd line 8:00 to 10:00
            //9:00
            //if (rowdatetime >= iteratedstarttime || rowdatetime < iteratedendtime) {
            //    isBetween = true;
            //    inRow = true;
            //} else if(rowdatetime > iteratedstarttime && rowdatetimeend < iteratedendtime) {
            //    isBetween = true;
            //}
            isBetween = HasOverlap(rowDateTimeStart, rowDateTimeEnd, iteratedStartTime, iteratedEndTime);
            return isBetween;
        }

        public static bool HasOverlap(DateTime start1, DateTime end1, DateTime start2, DateTime end2) {
            return Min(start1, end1) < Max(start2, end2) && Max(start1, end1) > Min(start2, end2);
        }

        public static DateTime Max(DateTime d1, DateTime d2) {
            return d1 > d2 ? d1 : d2;
        }

        public static DateTime Min(DateTime d1, DateTime d2) {
            return d2 > d1 ? d1 : d2;
        }

        public static CSCalendarExceptions IsHoliday(PXGraph graph, string defaultworkcalendar, object dt) {
            DateTime dtConverted = CommonUtil.ConvertTo<DateTime>(dt);
            return PXSelect<CSCalendarExceptions,
                Where<CSCalendarExceptions.calendarID, Equal<Required<CSCalendarExceptions.calendarID>>,
                And<CSCalendarExceptions.date, Equal<Required<CSCalendarExceptions.date>>>>>.Select(graph, defaultworkcalendar, dtConverted.Date);
        }

        public static EPTimeCard WeekCollection(PXGraph graph, int? employeeid, string weekid) {
            return PXSelect<EPTimeCard,
                Where<EPTimeCard.employeeID, Equal<Required<EPTimeCard.employeeID>>,
                And<EPTimeCard.weekId, Equal<Required<EPTimeCard.weekId>>,
                And<EPTimeCard.status, Equal<Required<EPTimeCard.status>>>>>>.Select(graph, employeeid, weekid, EPTimeCardStatusAttribute.ReleasedStatus);
        }

        public static double TimeCardCollection(PXGraph graph, int? employeeid) {
            double releaseTimeSpan = 0;
            PXResultset<EPTimeCard> PersonalTimeCollection = PXSelect<EPTimeCard,
                Where<EPTimeCard.employeeID, Equal<Required<EPTimeCard.employeeID>>,
                And<EPTimeCard.status, NotEqual<Required<EPTimeCard.status>>,
                And<EPTimeCardExt.usrPersonalTotalSpent, Greater<int0>,
                And<EPTimeCard.timeCardCD, IsNotNull>>>>>.Select(graph, employeeid, EPTimeCardStatusAttribute.ReleasedStatus);
            foreach (EPTimeCard weeks in PersonalTimeCollection) {
                var ePTimeCardExt = weeks.GetExtension<EPTimeCardExt>();
                releaseTimeSpan += CommonUtil.ConvertTo<double>(ePTimeCardExt?.UsrPersonalTotalSpent);
            }
            return releaseTimeSpan;
        }

        public static void EarningTypeValidation<T>(T currow, PXCache cache, string field, int? employee, PXGraph Base, string _FieldName) where T : IBqlTable {
            EPEmployee CurrEmployee = PXSelect<EPEmployee, Where<EPEmployee.bAccountID, Equal<Required<EPEmployee.bAccountID>>>>.Select(Base, employee);
            EPSetup setup = PXSelect<EPSetup>.Select(Base);
            var holiday = setup.HolidaysType;
            if (field == holiday) {
                var isCheckearningtype = IsHoliday(Base, CurrEmployee?.CalendarID, currow.GetType().GetProperty(_FieldName).GetValue(currow, null));
                if (isCheckearningtype == null) {
                    cache.RaiseExceptionHandling(currow.GetType().GetProperty(_FieldName).Name, currow, currow.GetType().GetProperty(_FieldName).GetValue(currow, null), new PXSetPropertyException(Messages.ErrorHoliday, PXErrorLevel.Error)); //TODO put e.Newvalue instead of null
                }
            } else {
                var isCheckearningtype = IsHoliday(Base, CurrEmployee?.CalendarID, currow.GetType().GetProperty(_FieldName).GetValue(currow, null));
                if (isCheckearningtype != null) {
                    cache.RaiseExceptionHandling(currow.GetType().GetProperty(_FieldName).Name, currow, currow.GetType().GetProperty(_FieldName).GetValue(currow, null), new PXSetPropertyException(Messages.ErrorRegular, PXErrorLevel.Warning)); //TODO put e.Newvalue instead of null
                }
            }
        }

        public static void EarningTypeValidationSummary<T>(T currow, PXCache cache, string field, int? employee, PXGraph Base, object specificDate, string fieldname) where T : IBqlTable {
            EPEmployee CurrEmployee = PXSelect<EPEmployee, Where<EPEmployee.bAccountID, Equal<Required<EPEmployee.bAccountID>>>>.Select(Base, employee);
            EPSetup setup = PXSelect<EPSetup>.Select(Base);
            var holiday = setup.HolidaysType;
            if (field == holiday) {
                var isCheckearningtype = IsHoliday(Base, CurrEmployee?.CalendarID, specificDate);
                if (isCheckearningtype == null) {
                    cache.RaiseExceptionHandling(currow.GetType().GetProperty(fieldname).Name, currow, currow.GetType().GetProperty(fieldname).GetValue(currow, null), new PXSetPropertyException(Messages.ErrorHoliday, PXErrorLevel.Error)); //TODO put e.Newvalue instead of null
                }
            } else {
                var isCheckearningtype = IsHoliday(Base, CurrEmployee?.CalendarID, specificDate);
                if (isCheckearningtype != null) {
                    cache.RaiseExceptionHandling(currow.GetType().GetProperty(fieldname).Name, currow, currow.GetType().GetProperty(fieldname).GetValue(currow, null), new PXSetPropertyException(Messages.ErrorRegular, PXErrorLevel.Warning)); //TODO put e.Newvalue instead of null
                }
            }
        }

        //Check Conflict on TimeCardDetails Lists
        public static bool ConflictChecking<T>(T currow, int? employee, PXGraph Base, string _DateFieldName, string _TimeSpendFieldName, IEnumerable lists, PXCache cache, string _id, DateTime endDate) where T : IBqlTable {
            //Check if Employee is Required to check conflict
            EPEmployee CurrEmployee = PXSelect<EPEmployee, Where<EPEmployee.bAccountID, Equal<Required<EPEmployee.bAccountID>>>>.Select(Base, employee);
            if (CurrEmployee != null) {
                if (CurrEmployee.TimeCardRequired == null || CurrEmployee.TimeCardRequired == false) return false;
                //CurrentRow DateTime and Reference ID
                var rowDate = currow.GetType().GetProperty(_DateFieldName).GetValue(currow, null);
                DateTime currentRowDate = CommonUtil.ConvertTo<DateTime>(rowDate);
                TimeSpan currentRowtimespent = TimeSpan.FromMinutes(CommonUtil.ConvertTo<int>(currow.GetType().GetProperty(_TimeSpendFieldName).GetValue(currow, null)));
                var currentrowid = currow.GetType().GetProperty(_id).GetValue(currow, null);

                //Get List of Rows In the Field with the same Date and not the same as current row
                List<T> list = new List<T>();
                foreach (PXResult result in lists) {
                    T detail = (T)result[typeof(T)];
                    var detailsDate = detail.GetType().GetProperty(_DateFieldName).GetValue(detail, null);
                    DateTime detailsDateConverted = CommonUtil.ConvertTo<DateTime>(detailsDate);
                    var resultrowID = detail.GetType().GetProperty(_id).GetValue(detail, null);
                    if (resultrowID.ToString() != currentrowid.ToString()) {
                        list.Add(detail);
                    }
                }

                //Iterate to list to check if date is between the date
                foreach (T TimeDetailRows in list) {
                    if (TimeDetailRows != null) {
                        //Get Date of iterated row
                        var TimeDetailDate = TimeDetailRows.GetType().GetProperty(_DateFieldName).GetValue(TimeDetailRows, null);
                        DateTime IteratedRowDate = CommonUtil.ConvertTo<DateTime>(TimeDetailDate);
                        //Get how many hours/minutes are in the row
                        TimeSpan timespent = TimeSpan.FromMinutes(CommonUtil.ConvertTo<int>(TimeDetailRows.GetType().GetProperty(_TimeSpendFieldName).GetValue(TimeDetailRows, null)));

                        //Check if Conflict
                        bool isConflict = TimeCardValidationUtils.IsBewteenTwoTime(currentRowDate, endDate, IteratedRowDate, IteratedRowDate + timespent);
                        if (isConflict == true) {
                                cache.RaiseExceptionHandling(_DateFieldName, currow, currentRowDate, new PXSetPropertyException(Messages.ExistingSession, PXErrorLevel.RowError));
                                return true;
                        } 
                    }
                }
                
            }
            return false;
        }

        public static bool TotalPersonalTime(PXGraph graph, int? total, PXCache cache, EPTimeCard timeCard, int? allowedpersonaltime, DateTime rowdate) {
            bool isError = false;
            var timeCardExt = timeCard.GetExtension<EPTimeCardExt>();
            TimeSpan allowedpersonaltimeValue = TimeSpan.FromHours(Math.Abs(CommonUtil.ConvertTo<double>(allowedpersonaltime)));
            var allowedpersonaltimeint = CommonUtil.ConvertTo<Int32>(allowedpersonaltimeValue.TotalMinutes);
            var TotalAllowedTime = timeCardExt.UsrPTOAvailable + allowedpersonaltimeint;
            if (total > TotalAllowedTime) {
                isError = true;
            } else {
                isError = false;
            }
            cache.SetValueExt<EPTimeCardExt.usrPersonalAlottedTime>(timeCard, CommonUtil.ConvertTo<Int32>(allowedpersonaltimeint));
            cache.SetValueExt<EPTimeCardExt.usrPersonalTotalSpent>(timeCard, total);
            return isError;
        }

        public static List<EPTimecardDetail> RecalculateTotalsUtils(EPTimeCard timecard, IEnumerable activities) {
            if (timecard == null)
                throw new ArgumentNullException();

            List<EPTimecardDetail> list = new List<EPTimecardDetail>();

            if (timecard.IsHold == true) {
                foreach (PXResult res in activities) {
                    EPTimecardDetail detail = (EPTimecardDetail)res[typeof(EPTimecardDetail)];
                    list.Add(detail);
                }
            }

            return list;
        }

        public static DateTime SetDateValue(object value, object Date) {
            TimeSpan timespent = TimeSpan.FromMinutes(CommonUtil.ConvertTo<double>(value));
            TimeSpan roundeddate = TimeCardValidationUtils.RoundToNearestMinutes(timespent, 15);
            double currentval = CommonUtil.ConvertTo<double>(roundeddate.TotalMinutes);
            DateTime currentDate = CommonUtil.ConvertTo<DateTime>(Date);
            DateTime currentDateOnly = currentDate.Date;
            DateTime comBineDate = currentDateOnly.AddMinutes(currentval);
            return comBineDate;
        }
        public static double GetAllotedPersonalTime(PXGraph graph, EPTimeCard timeCard) {
            double TotalPersonalTime = 0;
            PXResult<CSAnswers> epEmployee = PXSelectJoin<CSAnswers,
                    InnerJoin<EPEmployee, On<CSAnswers.refNoteID, Equal<Current<EPEmployee.noteID>>>>, Where<EPEmployee.bAccountID, Equal<Required<EPEmployee.bAccountID>>,
                    And<CSAnswers.value, Greater<string0>>>>
                    .Select(graph, timeCard.EmployeeID);
            if (epEmployee != null) {
                CSAnswers CSAnserRes = (CSAnswers)epEmployee[0];
                TotalPersonalTime = CommonUtil.ConvertTo<double>(CSAnserRes.Value);
            }

            return TotalPersonalTime;
        }

        public static DateTime GetWeekdayInRange(this DateTime? from, DateTime? to, DayOfWeek day) {
            const int daysInWeek = 7;
            var daysToAdd = ((int)day - (int)DayOfWeek.Sunday + daysInWeek) % daysInWeek;
            DateTime addDaytoDate = CommonUtil.ConvertTo<DateTime>(from);
            DateTime returnDate = addDaytoDate.AddDays(daysToAdd);

            return returnDate;
        }

            public static void GetCalendarWeek() {
                CultureInfo myCI = new CultureInfo("en-US");
                Calendar myCal = myCI.Calendar;

                CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
                DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

                Console.WriteLine("Week: {0}", myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW));
            }

        public static EPActivityApprove FindEPActivityApprove(PXGraph graph,
                                         FSAppointmentLog fsAppointmentLogRow,
                                         TMEPEmployee epEmployeeRow) {
            if (fsAppointmentLogRow == null || epEmployeeRow == null)
                return null;

            return PXSelect<EPActivityApprove,
                                   Where<
                                       EPActivityApprove.ownerID, Equal<Required<EPActivityApprove.ownerID>>,
                                       And<FSxPMTimeActivity.appointmentID, Equal<Required<FSxPMTimeActivity.appointmentID>>,
                                       And<FSxPMTimeActivity.logLineNbr, Equal<Required<FSxPMTimeActivity.logLineNbr>>>>>>
                                   .Select(graph, epEmployeeRow.DefContactID, fsAppointmentLogRow.DocID, fsAppointmentLogRow.LineNbr);
        }

        public static TMEPEmployee FindTMEmployee(PXGraph graph, int? employeeID) {
            TMEPEmployee epEmployeeRow = PXSelect<TMEPEmployee,
                            Where<
                                TMEPEmployee.bAccountID, Equal<Required<TMEPEmployee.bAccountID>>>>
                            .Select(graph, employeeID);

            if (epEmployeeRow == null) {
                throw new Exception(TX.Error.MISSING_LINK_ENTITY_STAFF_MEMBER);
            }

            return epEmployeeRow;
        }

    }
}
