using PX.Data;
using PX.Objects.FS;
using System;
using PX.Objects.FS.ParallelProcessing;
using PX.Objects.CR;
using System.Globalization;
using static PX.Objects.EP.TimeCardMaint;
using System.Collections.Generic;
using System.Collections;
using PX.Objects.EP;
using TMEPEmployee = PX.Objects.CR.Standalone.EPEmployee;
namespace PC.Objects.AA.HOJTimeCardValidation {
    public class AppointmentEntryExt : PXGraphExtension<AppointmentEntry> {
        protected void _(Events.RowPersisting<FSAppointmentLog> e) {
            var row = e.Row;
            if (row == null) return;
            var DateFieldName = CommonUtil.GetMemberName((FSAppointmentLog c) => c.DateTimeBegin);
            var TimeSpentFieldName = CommonUtil.GetMemberName((FSAppointmentLog c) => c.TimeDuration);
            var currentrefid = CommonUtil.GetMemberName((FSAppointmentLog c) => c.LineNbr);
            if(TimeCardValidationUtils.ConflictChecking<FSAppointmentLog>(e.Row, row.BAccountID, Base, DateFieldName, TimeSpentFieldName, Base.LogRecords.Select(), e.Cache, currentrefid, CommonUtil.ConvertTo<DateTime>(row.DateTimeEnd))) return;
            GetCurrentTimeCard(DateFieldName, row, e.Cache);

            
        }

        protected void _(Events.FieldUpdated<FSAppointment, FSAppointment.actualDateTimeBegin> e) {
            var row = e.Row;
            if (row == null) return;
            if (e.NewValue != null && e.NewValue is DateTime && e.NewValue != e.OldValue && e.OldValue != null) {
                DateTime? roundeddate = TimeCardValidationUtils.RoundToNearestMinuteProper(CommonUtil.ConvertTo<DateTime>(e.NewValue), 15);
                row.ActualDateTimeBegin = roundeddate;
                e.Cache.SetDefaultExt<FSAppointment.actualDuration>(e.Row);
            }
        }

        protected void _(Events.FieldUpdated<FSAppointment, FSAppointment.actualDateTimeEnd> e) {
            var row = e.Row;
            if (row == null) return;
            if (e.NewValue != null && e.NewValue is DateTime && e.NewValue != e.OldValue) {
                DateTime? roundeddate = TimeCardValidationUtils.RoundToNearestMinuteProper(CommonUtil.ConvertTo<DateTime>(e.NewValue), 15);
                row.ActualDateTimeEnd = roundeddate;
                e.Cache.SetDefaultExt<FSAppointment.actualDuration>(e.Row);
            }
        }

        protected void _(Events.FieldUpdated<FSAppointmentLog, FSAppointmentLog.dateTimeBegin> e) {
            var row = e.Row;
            if (row == null) return;
            var DateFieldName = CommonUtil.GetMemberName((FSAppointmentLog c) => c.DateTimeBegin);
            var fieldName = CommonUtil.GetMemberName((FSAppointmentLog c) => c.DateTimeBegin);
            TimeCardValidationUtils.EarningTypeValidation<FSAppointmentLog>(e.Row, e.Cache, row.EarningType, row.BAccountID, Base, fieldName);
            if (e.NewValue != null && e.NewValue is DateTime) {
                DateTime? roundeddate = TimeCardValidationUtils.RoundToNearestMinuteProper(CommonUtil.ConvertTo<DateTime>(e.NewValue), 15);
                row.DateTimeBegin = roundeddate;
                e.Cache.SetDefaultExt<FSAppointmentLog.timeDuration>(e.Row);
            }
            GetCurrentTimeCard(DateFieldName, row, e.Cache);
        }

        protected void _(Events.FieldUpdated<FSAppointmentLog, FSAppointmentLog.dateTimeEnd> e) {
            var row = e.Row;
            if (row == null) return;
            var DateFieldName = CommonUtil.GetMemberName((FSAppointmentLog c) => c.DateTimeBegin);
            var fieldName = CommonUtil.GetMemberName((FSAppointmentLog c) => c.DateTimeEnd);
            TimeCardValidationUtils.EarningTypeValidation<FSAppointmentLog>(e.Row, e.Cache, row.EarningType, row.BAccountID, Base, fieldName);
            if (e.NewValue != null && e.NewValue is DateTime) {
                DateTime? roundeddate = TimeCardValidationUtils.RoundToNearestMinuteProper(CommonUtil.ConvertTo<DateTime>(e.NewValue), 15);
                row.DateTimeEnd = roundeddate;
                e.Cache.SetDefaultExt<FSAppointmentLog.timeDuration>(e.Row);
            }
            GetCurrentTimeCard(DateFieldName, row, e.Cache);
        }

        protected void _(Events.FieldUpdated<FSAppointmentLog, FSAppointmentLog.earningType> e) {
            var row = e.Row;
            if (row == null) return;
            var fieldName = CommonUtil.GetMemberName((FSAppointmentLog c) => c.DateTimeBegin);
            if (e.NewValue == null || row.BAccountID == null) return;
            TimeCardValidationUtils.EarningTypeValidation<FSAppointmentLog>(e.Row, e.Cache, e.NewValue.ToString(), row.BAccountID, Base, fieldName);
        }

        protected void _(Events.FieldUpdated<FSAppointmentLog, FSAppointmentLog.timeDuration> e) {
            var row = e.Row;
            if (row == null) return;
            var DateFieldName = CommonUtil.GetMemberName((FSAppointmentLog c) => c.DateTimeBegin);
            var TimeSpentFieldName = CommonUtil.GetMemberName((FSAppointmentLog c) => c.TimeDuration);
            var currentrefid = CommonUtil.GetMemberName((FSAppointmentLog c) => c.LineNbr);
            if(TimeCardValidationUtils.ConflictChecking<FSAppointmentLog>(e.Row, row.BAccountID, Base, DateFieldName, TimeSpentFieldName, Base.LogRecords.Select(), e.Cache, currentrefid, CommonUtil.ConvertTo<DateTime>(row.DateTimeEnd))) return;
            GetCurrentTimeCard(DateFieldName, row, e.Cache);
        }

        public delegate void InsertUpdateEPActivityApproveDelegate(PXGraph graph,
                                                         EmployeeActivitiesEntry graphEmployeeActivitiesEntry,
                                                         FSAppointmentLog fsAppointmentLogRow,
                                                         FSAppointment fsAppointmentRow,
                                                         FSServiceOrder fsServiceOrderRow,
                                                         EPActivityApprove epActivityApproveRow,
                                                         TMEPEmployee epEmployeeRow);
        [PXOverride]
        public void InsertUpdateEPActivityApprove(PXGraph graph, EmployeeActivitiesEntry graphEmployeeActivitiesEntry, FSAppointmentLog fsAppointmentLogRow, FSAppointment fsAppointmentRow, FSServiceOrder fsServiceOrderRow, EPActivityApprove epActivityApproveRow, TMEPEmployee epEmployeeRow, InsertUpdateEPActivityApproveDelegate baseMethod) {
            if (epActivityApproveRow == null) {
                epActivityApproveRow = new EPActivityApprove();
                epActivityApproveRow.OwnerID = epEmployeeRow.DefContactID;
                epActivityApproveRow = graphEmployeeActivitiesEntry.Activity.Insert(epActivityApproveRow);
            }
            DateTime currentRowDate = CommonUtil.ConvertTo<DateTime>(fsAppointmentLogRow.DateTimeBegin);
            TimeSpan currentRowtimespent = TimeSpan.FromMinutes(CommonUtil.ConvertTo<int>(fsAppointmentLogRow.TimeDuration));
            var epActivityApproveRowExt = epActivityApproveRow.GetExtension<EPTimecardDetailExt>();
            epActivityApproveRowExt.UsrDateTimeEnd = currentRowDate + currentRowtimespent;
            baseMethod(graph, graphEmployeeActivitiesEntry, fsAppointmentLogRow, fsAppointmentRow, fsServiceOrderRow, epActivityApproveRow, epEmployeeRow);

        }

        protected void _(Events.RowPersisted<FSAppointmentLog> e) {
            var row = e.Row;
            if (row == null) return;


        }

        public void GetCurrentTimeCard(string DateFieldName, FSAppointmentLog row, PXCache cache) {
            //Find row in timecard if found return
            if (row.Status == "P" && row.DateTimeEnd == null) return;
            TMEPEmployee epEmployeeRow = TimeCardValidationUtils.FindTMEmployee(Base, row.BAccountID);
            EPActivityApprove epActivityApproveRow = TimeCardValidationUtils.FindEPActivityApprove(Base, row, epEmployeeRow);
            if (epActivityApproveRow != null) return;
            //check conflict between two screens
            DateTime currentRowDate = CommonUtil.ConvertTo<DateTime>(row.DateTimeBegin);
            BAccount baAccount = BAccount.PK.Find(Base, row.BAccountID);
            var convertCurrentDate = CommonUtil.ConvertTo<DateTime>(row.DateTimeBegin);
            var getDateWeekID = WeekHelper.GetIso8601WeekOfYear(currentRowDate);
            string getWeekID = currentRowDate.Year.ToString() + getDateWeekID.ToString();
            PXResultset<EPTimecardDetail> getTimeCardDetails = PXSelect<EPTimecardDetail,
                Where<EPTimecardDetail.ownerID, Equal<Required<EPTimecardDetail.ownerID>>,
                    And<EPTimecardDetail.weekID, Equal<Required<EPTimecardDetail.weekID>>,
                    And<EPTimecardDetail.trackTime, Equal<True>,
                    And<EPTimecardDetail.approvalStatus, NotEqual<ActivityStatusListAttribute.canceled>>>>>,
                OrderBy<Asc<EPTimecardDetail.date>>>.Select(Base, baAccount.DefContactID, getWeekID);
            var cardTimeSpentFieldName = CommonUtil.GetMemberName((EPTimecardDetail c) => c.TimeSpent);
            var cardcurrentrefid = CommonUtil.GetMemberName((EPTimecardDetail c) => c.RefNoteID);
            if (getTimeCardDetails == null || getTimeCardDetails.Count <= 0) return;
            //Iterate to list to check if date is between the date
            foreach (EPTimecardDetail TimeDetailRows in getTimeCardDetails) {
                if (TimeDetailRows != null) {
                    var timeDetailRowsExt = TimeDetailRows.GetExtension<EPTimecardDetailExt>();
                    if (TimeDetailRows.Date == row.DateTimeBegin && timeDetailRowsExt.UsrDateTimeEnd == row.DateTimeEnd) return;
                    TimeSpan currentRowtimespent = TimeSpan.FromMinutes(CommonUtil.ConvertTo<int>(row.TimeDuration));
                    //Get Date of iterated row
                    DateTime IteratedRowDate = CommonUtil.ConvertTo<DateTime>(TimeDetailRows.Date);
                    //Get how many hours/minutes are in the row
                    TimeSpan timespent = TimeSpan.FromMinutes(CommonUtil.ConvertTo<int>(TimeDetailRows.TimeSpent));

                    //Check if Conflict
                    bool isConflict = TimeCardValidationUtils.IsBewteenTwoTime(currentRowDate, CommonUtil.ConvertTo<DateTime>(row.DateTimeEnd), IteratedRowDate, IteratedRowDate + timespent);
                    if (isConflict == true) {
                            cache.RaiseExceptionHandling(DateFieldName, row, currentRowDate, new PXSetPropertyException(Messages.ExistingSession, PXErrorLevel.RowError));
                            return;
                    }
                }
            }
        }
    }
}
