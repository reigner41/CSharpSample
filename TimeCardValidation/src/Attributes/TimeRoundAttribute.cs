using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.EP;
using static PX.Objects.EP.TimeCardMaint;

namespace PC.Objects.AA.HOJTimeCardValidation {
    public class TimeRoundAttribute : PXEventSubscriberAttribute, IPXFieldUpdatingSubscriber, IPXFieldDefaultingSubscriber {
        protected Type _DateTime;

        protected bool? _IsRequired;

        private PXGraph _Graph = null;

        public TimeRoundAttribute(Type DateTime, bool IsRequired) {
            this._DateTime = DateTime;
            this._IsRequired = IsRequired;
        }

        public override void CacheAttached(PXCache sender) {
            this._Graph = sender.Graph;
            base.CacheAttached(sender);
        }

        public void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e) {
            if (e.NewValue != null && e.NewValue is DateTime) {
                var RoundDateTime = RoundDateTimeNearestMinute(sender, e.Row, e.NewValue);
                e.NewValue = RoundDateTime;
            } else if (e.NewValue != null) {
                var RoundTime = RoundMinNearestMinute(sender, e.Row, e.NewValue);
                e.NewValue = RoundTime;
            }
        }

        public void FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e) {
            if (e.NewValue != null && e.NewValue is DateTime) {
                var RoundDateTime = RoundDateTimeNearestMinute(sender, e.Row, e.NewValue);
                e.NewValue = RoundDateTime;
            } else if (e.NewValue != null) {
                var RoundTime = RoundMinNearestMinute(sender, e.Row, e.NewValue);
                e.NewValue = RoundTime;
            }
        }

        //public void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e) {
        //    if (e.NewValue != null && this._IsRequired != null) {
        //        if (this._IsRequired == true) {
        //            if (e.NewValue != null && e.NewValue is DateTime) {
        //                return;
        //            } else {
        //                //var currentHeader = _Graph.Caches[typeof(EPTimeCard)];
        //                //if (currentHeader == null) return;
        //                //IEnumerable<EPTimeCard> currentHeaderRow = (IEnumerable<EPTimeCard>)currentHeader.Cached;
        //                //IEnumerable<EPTimeCard> employeeID = currentHeaderRow;
        //                //var employeeIDValue = employeeID.Where(x => x.EmployeeID != null);
        //                //int? epid = employeeIDValue.First().EmployeeID;
        //                //EPEmployee CurrEmployee = PXSelect<EPEmployee, Where<EPEmployee.bAccountID, Equal<Required<EPEmployee.bAccountID>>>>.Select(this._Graph, epid);
        //                //if (CurrEmployee != null) {
        //                //    if (CurrEmployee.TimeCardRequired == null || CurrEmployee.TimeCardRequired == false) return;
        //                //    DateTime rowDate = (DateTime)((PMTimeActivity)e.Row).Date;
        //                //    var CurrentObject = _Graph.Caches[typeof(EPTimecardDetail)];
        //                //    foreach (EPTimecardDetail TimeDetailRows in CurrentObject.Cached) {
        //                //        if (TimeDetailRows != null) {
        //                //            if (((PMTimeActivity)e.Row).RefNoteID != TimeDetailRows.RefNoteID) {
        //                //                TimeSpan timespent = TimeSpan.FromMinutes((int)TimeDetailRows.TimeSpent);
        //                //                bool? isConflict = TimeCardValidationUtils.IsBewteenTwoTime(rowDate, (DateTime)TimeDetailRows.Date, (DateTime)TimeDetailRows.Date + timespent);
        //                //                if (isConflict == true) {
        //                //                    sender.RaiseExceptionHandling(this._FieldName, e.Row, null, new PXSetPropertyException(Messages.ExistingSession));
        //                //                }
        //                //            }
        //                //        }
        //                //    }
        //                //}
        //            }
        //        }
        //    }
        //}

        private DateTime? RoundDateTimeNearestMinute(PXCache sender, object row, object DateTime) {
            DateTime dtConverted = CommonUtil.ConvertTo<DateTime>(DateTime);
            if ((this._DateTime != null)) {
                if (DateTime != null && this._FieldName != null) {
                    try {
                        dtConverted = TimeCardValidationUtils.RoundToNearestMinuteProper(dtConverted, 15);
                        return dtConverted;
                    } catch (Exception ex) {
                        sender.RaiseExceptionHandling(_FieldName, row, dtConverted, ex);
                    }
                }
            }
            return dtConverted;
        }

        private int RoundMinNearestMinute(PXCache sender, object row, object Min) {
            int intts = CommonUtil.ConvertTo<int>(Min);
            if ((this._DateTime != null)) {
                if (Min != null && this._FieldName != null) {
                    try {
                        TimeSpan tsresult = TimeSpan.FromMinutes(intts);
                        TimeSpan roundmin = TimeCardValidationUtils.RoundToNearestMinutes(tsresult, 15);
                        int totalmin = Convert.ToInt32(roundmin.TotalMinutes);
                        return totalmin;
                    } catch (Exception ex) {
                        sender.RaiseExceptionHandling(_FieldName, row, intts, ex);
                    }
                }
            }
            return intts;
        }
    }
}
