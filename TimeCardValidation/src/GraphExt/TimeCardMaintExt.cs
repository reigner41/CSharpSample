using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.CR;
using static PX.Objects.EP.TimeCardMaint;
using System.Linq;
using System.Globalization;
using PX.TM;
using PX.Objects.FS;
using TMEPEmployee = PX.Objects.CR.Standalone.EPEmployee;
namespace PC.Objects.AA.HOJTimeCardValidation {
    public class TimeCardMaintExt : PXGraphExtension<TimeCardMaint> {
        public PXSetup<EPSetup> setup;

        #region EPTimeCardDetail Field Events
        protected void _(Events.RowPersisting<EPTimecardDetail> e) {
            var row = e.Row;
            var linnbr = row.SummaryLineNbr;
            var doc = Base.Document.Current;
            if (row == null || doc == null) return;
            var rowExt = row?.GetExtension<EPTimecardDetailExt>();
            var DateFieldName = CommonUtil.GetMemberName((EPTimecardDetail c) => c.Date);
            var TimeSpentFieldName = CommonUtil.GetMemberName((EPTimecardDetail c) => c.TimeSpent);
            var currentrefid = CommonUtil.GetMemberName((EPTimecardDetail c) => c.RefNoteID);
            if(TimeCardValidationUtils.ConflictChecking<EPTimecardDetail>(e.Row, doc.EmployeeID, Base, DateFieldName, TimeSpentFieldName, Base.Activities.Select(), e.Cache, currentrefid, CommonUtil.ConvertTo<DateTime>(rowExt.UsrDateTimeEnd))) return;
            if (row.Date != null) {
                DateTime currentRowDate = CommonUtil.ConvertTo<DateTime>(row.Date);
                TimeSpan currentRowtimespent = TimeSpan.FromMinutes(CommonUtil.ConvertTo<int>(row.TimeSpent));
                e.Cache.SetValueExt<EPTimecardDetailExt.usrDateTimeEnd>(row, currentRowDate + currentRowtimespent);
            }
            checkAppointmentLogForConflict(row, DateFieldName, e.Cache);

        }

        public delegate void RecalculateTotalsDelegate(EPTimeCard timecard, List<EPTimecardDetail> details);
        [PXOverride]
        public void RecalculateTotals(EPTimeCard timecard, List<EPTimecardDetail> details, RecalculateTotalsDelegate baseMethod) {
            baseMethod(timecard, details);
            if (details == null) return;
            var timecardExt = timecard?.GetExtension<EPTimeCardExt>();
            EPSetupExt setupExt = setup.Current.GetExtension<EPSetupExt>();
            PXResult<CSAnswers> epEmployee = PXSelectJoin<CSAnswers,
                InnerJoin<EPEmployee, On<CSAnswers.refNoteID, Equal<Current<EPEmployee.noteID>>>>, Where<EPEmployee.bAccountID, Equal<Required<EPEmployee.bAccountID>>,
                And<CSAnswers.value, Greater<string0>>>>
                .Select(Base, timecard.EmployeeID);
            if (epEmployee != null) {
                
                CSAnswers CSAnserRes = (CSAnswers)epEmployee[0];
                var convertDouble = CommonUtil.ConvertTo<double>(CSAnserRes.Value);
                TimeSpan AllotedPersonalTimeValue = TimeSpan.FromHours(Math.Abs(convertDouble));
                var TotalBalance = CommonUtil.ConvertTo<Int32>(AllotedPersonalTimeValue.TotalMinutes);
                timecardExt.UsrPTOBalance = TotalBalance;
                if (TotalBalance > 0) {
                    var ePTimeCardPersonalTime = TimeCardValidationUtils.TimeCardCollection(Base, timecard.EmployeeID);
                    var totalPersonalTime = CommonUtil.ConvertTo<Int32>(ePTimeCardPersonalTime);
                    //calculate Available PTO
                    timecardExt.UsrPTOAvailable = TotalBalance - totalPersonalTime;
                }
            }
            timecardExt.UsrNonworkingTime = details.Where(res => res.EarningTypeID == setupExt.UsrNonworkingtype).Sum(x => x.TimeSpent);
            timecard.TimeSpentCalc = details.Where(res => res.EarningTypeID != setupExt.UsrNonworkingtype).Sum(x => x.TimeSpent);
            Base.Document.View.RequestRefresh();
        }

        protected void _(Events.FieldDefaulting<EPTimeCard, EPTimeCardExt.usrPersonalAlottedTime> e) {
            var row = e.Row;
            if (row == null) return;
            var setupExt = setup.Current?.GetExtension<EPSetupExt>();
            var rowExt = row.GetExtension<EPTimeCardExt>();
            var PersonalAlottedTime = TimeSpan.FromHours(Math.Abs(TimeCardValidationUtils.GetAllotedPersonalTime(Base, row)));
            var TotalAllowedTime = PersonalAlottedTime.TotalMinutes + setupExt.UsrAllowPesonalTime;
            e.NewValue = Convert.ToInt32(PersonalAlottedTime.TotalMinutes);
        }

        protected void _(Events.FieldUpdated<EPTimecardDetail, EPTimecardDetail.timeSpent> e) {
            var row = e.Row;
            var doc = Base.Document.Current;
            if (row == null || doc == null) return;
            if (row.Date != null) {
                DateTime currentRowDate = CommonUtil.ConvertTo<DateTime>(row.Date);
                TimeSpan currentRowtimespent = TimeSpan.FromMinutes(CommonUtil.ConvertTo<int>(row.TimeSpent));
                e.Cache.SetValueExt<EPTimecardDetailExt.usrDateTimeEnd>(row, currentRowDate + currentRowtimespent);
            }
            var fieldName = CommonUtil.GetMemberName((EPTimecardDetail c) => c.Date);
            TimeCardValidationUtils.EarningTypeValidation<EPTimecardDetail>(e.Row, e.Cache, row.EarningTypeID, doc.EmployeeID, Base, fieldName);
            if (e.ExternalCall == true) {
                PersonalTime(row, doc, Base.Document.Cache);
            }
        }

        protected void _(Events.FieldVerifying<EPTimecardDetail, EPTimecardDetailExt.usrDateTimeEnd> e) {
            var row = e.Row;
            var linnbr = row.SummaryLineNbr;
            var doc = Base.Document.Current;
            if (row == null || doc == null) return;
            var DateFieldName = CommonUtil.GetMemberName((EPTimecardDetail c) => c.Date);
            var TimeSpentFieldName = CommonUtil.GetMemberName((EPTimecardDetail c) => c.TimeSpent);
            var currentrefid = CommonUtil.GetMemberName((EPTimecardDetail c) => c.RefNoteID);
            if(TimeCardValidationUtils.ConflictChecking<EPTimecardDetail>(e.Row, doc.EmployeeID, Base, DateFieldName, TimeSpentFieldName, Base.Activities.Select(), e.Cache, currentrefid, CommonUtil.ConvertTo<DateTime>(e.NewValue))) return;
            checkAppointmentLogForConflict(row, DateFieldName, e.Cache);
        }

        protected void _(Events.FieldUpdated<EPTimecardDetail, EPTimecardDetail.date> e) {
            var row = e.Row;
            var doc = Base.Document.Current;
            if (row == null || doc == null) return;
            if (row.Date != null) {
                DateTime currentRowDate = CommonUtil.ConvertTo<DateTime>(row.Date);
                TimeSpan currentRowtimespent = TimeSpan.FromMinutes(CommonUtil.ConvertTo<int>(row.TimeSpent));
                e.Cache.SetValueExt<EPTimecardDetailExt.usrDateTimeEnd>(row, currentRowDate + currentRowtimespent);
            }
            var fieldName = CommonUtil.GetMemberName((EPTimecardDetail c) => c.Date);
            TimeCardValidationUtils.EarningTypeValidation<EPTimecardDetail>(e.Row, e.Cache, row.EarningTypeID, doc.EmployeeID, Base, fieldName);
            if (e.ExternalCall == true) {
                PersonalTime(row, doc, Base.Document.Cache);
            }
        }

        protected void _(Events.FieldUpdated<EPTimecardDetail, EPTimecardDetail.earningTypeID> e) {
            var row = e.Row;
            var doc = Base.Document.Current;
            if (row == null || doc == null) return;
            var fieldName = CommonUtil.GetMemberName((EPTimecardDetail c) => c.Date);
            TimeCardValidationUtils.EarningTypeValidation<EPTimecardDetail>(e.Row, e.Cache, e.NewValue.ToString(), doc.EmployeeID, Base, fieldName);
            PersonalTime(row, doc, Base.Document.Cache);
        }

        protected void _(Events.RowSelected<EPTimeCard> e) {
            var row = e.Row;
            if (row == null) return;
            var errorMsg = string.Empty;
            var rowExt = row.GetExtension<EPTimeCardExt>();
            var setupExt = setup.Current?.GetExtension<EPSetupExt>();
            var PersonalAlottedTime = TimeSpan.FromHours(Math.Abs(TimeCardValidationUtils.GetAllotedPersonalTime(Base, row)));
            rowExt.UsrPersonalAlottedTime = Convert.ToInt32(PersonalAlottedTime.TotalMinutes);
            var TotalAllowedTime = rowExt.UsrPersonalAlottedTime + setupExt.UsrAllowPesonalTime;
            if (rowExt.UsrPersonalTotalSpent > TotalAllowedTime) {
                errorMsg = PXMessages.LocalizeFormatNoPrefixNLA(Messages.PersonalTimeOverExceedNotAllowed, Math.Floor(PersonalAlottedTime.TotalHours), PersonalAlottedTime.Minutes);
                e.Cache.RaiseExceptionHandling<EPTimeCardExt.usrPersonalTotalSpent>(row, rowExt.UsrPersonalTotalSpent, new PXSetPropertyException(errorMsg, PXErrorLevel.Warning));
            }
        }

        //protected void _(Events.RowUpdated<EPTimecardDetail> e) {
        //    var row = e.Row;
        //    if (row == null) return;
        //    //Calculate EndTime
        //    if (row.Date != null) {
        //        DateTime currentRowDate = CommonUtil.ConvertTo<DateTime>(row.Date);
        //        TimeSpan currentRowtimespent = TimeSpan.FromMinutes(CommonUtil.ConvertTo<int>(row.TimeSpent));
        //        e.Cache.SetValueExt<EPTimecardDetailExt.usrDateTimeEnd>(row, currentRowDate + currentRowtimespent);
        //    }
        //}

        protected void _(Events.RowDeleted<EPTimecardDetail> e) {
            var row = e.Row;
            var doc = Base.Document.Current;
            if (row == null || doc == null) return;
            if (row.EarningTypeID == setup.Current.VacationsType) {
                var personalTime = row.TimeSpent;
                var docext = doc.GetExtension<EPTimeCardExt>();
                var newPersonalTime = docext.UsrPersonalTotalSpent - personalTime;
                Base.Document.Cache.SetValueExt<EPTimeCardExt.usrPersonalTotalSpent>(doc, newPersonalTime);
            }
        }

        public void checkAppointmentLogForConflict(EPTimecardDetail row, string DateFieldName, PXCache cache) {
            BAccount getBAccountID = PXSelect<BAccount, Where<BAccount.defContactID, Equal<Required<BAccount.defContactID>>>>.Select(Base, row.OwnerID);
            //get year from row
            var rowExt = row?.GetExtension<EPTimecardDetailExt>();
            var rowYear = CommonUtil.ConvertTo<DateTime>(row.Date);
            var getDateWeekID = WeekHelper.GetIso8601WeekOfYear(rowYear);
            var getFisrtWeekOfWeekID = WeekHelper.FirstDateOfWeek(rowYear.Year, getDateWeekID);
            var getLastWeekOfWeekID = WeekHelper.LastDateOfWeek(rowYear.Year, getDateWeekID);
            DateTime currentRowDate = CommonUtil.ConvertTo<DateTime>(row.Date);
            DateTime currentRowEndDate = CommonUtil.ConvertTo<DateTime>(rowExt.UsrDateTimeEnd);
            PXResultset<FSAppointmentLog> appointmengLog = PXSelect<FSAppointmentLog, Where<FSAppointmentLog.bAccountID, Equal<Required<FSAppointmentLog.bAccountID>>,
                And<FSAppointmentLog.dateTimeBegin, Between<FSAppointmentLog.dateTimeBegin, FSAppointmentLog.dateTimeBegin>>>,
                OrderBy<Desc<FSAppointmentLog.dateTimeBegin>>>
                .Select(Base, getBAccountID.BAccountID, getFisrtWeekOfWeekID, getLastWeekOfWeekID);
            //PXResultset<FSAppointmentLog> appointmengLog = PXSelect<FSAppointmentLog, Where<FSAppointmentLog.bAccountID, Equal<Required<FSAppointmentLog.bAccountID>>,
            //    And<FSAppointmentLog.dateTimeBegin, GreaterEqual<Required<FSAppointmentLog.dateTimeBegin>>, 
            //    And<FSAppointmentLog.dateTimeEnd, LessEqual<Required<FSAppointmentLog.dateTimeEnd>>>>>>
            //    .Select(Base, getBAccountID.BAccountID, getFisrtWeekOfWeekID, getLastWeekOfWeekID);
            var cardTimeSpentFieldName = CommonUtil.GetMemberName((EPTimecardDetail c) => c.TimeSpent);
            var cardcurrentrefid = CommonUtil.GetMemberName((EPTimecardDetail c) => c.RefNoteID);
            if (appointmengLog == null || appointmengLog.Count <= 0) return;
            //Iterate to list to check if date is between the date
            foreach (FSAppointmentLog AppointmentLogrows in appointmengLog) {
                if (AppointmentLogrows != null) {
                    TMEPEmployee epEmployeeRow = PXSelect<TMEPEmployee, Where<TMEPEmployee.bAccountID, Equal<Required<TMEPEmployee.bAccountID>>>>.Select(Base, getBAccountID.BAccountID);
                    EPActivityApprove epActivityApproveRow = TimeCardValidationUtils.FindEPActivityApprove(Base, AppointmentLogrows, epEmployeeRow);
                    if (epActivityApproveRow != null) return;
                    TimeSpan currentRowtimespent = TimeSpan.FromMinutes(CommonUtil.ConvertTo<int>(row.TimeSpent));
                    //Get Date of iterated row
                    DateTime IteratedRowDate = CommonUtil.ConvertTo<DateTime>(AppointmentLogrows.DateTimeBegin);
                    DateTime IteratedRowEndDate = CommonUtil.ConvertTo<DateTime>(AppointmentLogrows.DateTimeEnd);
                    //Get how many hours/minutes are in the row
                    TimeSpan timespent = TimeSpan.FromMinutes(CommonUtil.ConvertTo<int>(AppointmentLogrows.TimeDuration));

                    //Check if Conflict
                    bool isConflict = TimeCardValidationUtils.IsBewteenTwoTime(currentRowDate, currentRowEndDate, IteratedRowDate, IteratedRowEndDate);
                    if (isConflict == true) {
                            cache.RaiseExceptionHandling(DateFieldName, row, currentRowDate, new PXSetPropertyException(Messages.ExistingSession, PXErrorLevel.RowError));
                            return;
    
                    }
                }
            }
        }
        #endregion

        #region EPTimeCardSummary Field Events
        //this is to recalculate the personal time if earningtype is changed on summary tab
        protected void _(Events.FieldUpdated<EPTimeCardSummaryWithInfo, EPTimeCardSummaryWithInfo.earningType> e) {
            var row = e.Row;
            var doc = Base.Document.Current;
            if (row == null || doc == null) return;
            if (e.ExternalCall == true) {
                var setupExt = setup.Current?.GetExtension<EPSetupExt>();
                var fromDate = CommonUtil.ConvertTo<DateTime>(doc.WeekStartDate);
                var toDate = CommonUtil.ConvertTo<DateTime>(doc.WeekEndDate);
                var dateofTheDay = TimeCardValidationUtils.GetWeekdayInRange(fromDate, toDate, DayOfWeek.Monday);
                if (row.EarningType == setup.Current.VacationsType) {
                    var personalTime = row.TimeSpent;
                    var totalPersonal = TimeCardValidationUtils.TotalPersonalTime(Base, personalTime, Base.Document.Cache, Base.Document.Current, setupExt.UsrAllowPesonalTime, CommonUtil.ConvertTo<DateTime>(dateofTheDay));
                    if (totalPersonal == true) {
                        var docPersonalTimeFieldName = CommonUtil.GetMemberName((EPTimeCardExt c) => c.UsrPersonalTotalSpent);
                        Base.Document.Cache.RaiseExceptionHandling(docPersonalTimeFieldName, row, null, new PXSetPropertyException(Messages.PersonalTimeExceed, PXErrorLevel.Warning));
                    }
                } 
            }
        }
        #endregion


        #region Personal Time Method
        protected void PersonalTime(EPTimecardDetail row, EPTimeCard doc, PXCache cache) {
            var setupExt = setup.Current?.GetExtension<EPSetupExt>();
            List<EPTimecardDetail> getTotal = TimeCardValidationUtils.RecalculateTotalsUtils(doc, Base.Activities.Select());
            var personalTime = getTotal.Where(r => r.EarningTypeID == setup.Current.VacationsType).Sum(Item => Item.TimeSpent);
            var totalPersonal = TimeCardValidationUtils.TotalPersonalTime(Base, personalTime, Base.Document.Cache, Base.Document.Current, setupExt.UsrAllowPesonalTime, CommonUtil.ConvertTo<DateTime>(row.Date));
            if (totalPersonal == true) {
                var docPersonalTimeFieldName = CommonUtil.GetMemberName((EPTimeCardExt c) => c.UsrPersonalTotalSpent);
                cache.RaiseExceptionHandling(docPersonalTimeFieldName, doc, null, new PXSetPropertyException(Messages.PersonalTimeExceed, PXErrorLevel.Warning));
            }
        }

        #endregion

    }
}
