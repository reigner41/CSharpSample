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
    public class TimeCardSummaryAttribute : PXEventSubscriberAttribute, IPXFieldVerifyingSubscriber, IPXFieldUpdatedSubscriber {
        protected DayOfWeek _DayOfWeek;
        private PXGraph _Graph = null;

        public TimeCardSummaryAttribute(DayOfWeek _DayOfWeek) {
            this._DayOfWeek = _DayOfWeek;
        }

        public override void CacheAttached(PXCache sender) {
            this._Graph = sender.Graph;
            base.CacheAttached(sender);
        }

        public void FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e) {
            EPTimeCard timeCardrow = (EPTimeCard)this._Graph.Caches[typeof(EPTimeCard)].Current;
            PXCache timeCardCache = this._Graph.Caches[typeof(EPTimeCard)];
            EPTimeCardSummaryWithInfo timecardSummaryRow = (EPTimeCardSummaryWithInfo)this._Graph.Caches[typeof(EPTimeCardSummaryWithInfo)].Current;
            var fromDate = CommonUtil.ConvertTo<DateTime>(timeCardrow.WeekStartDate);
            var toDate = CommonUtil.ConvertTo<DateTime>(timeCardrow.WeekEndDate);
            var dateofTheDay = TimeCardValidationUtils.GetWeekdayInRange(fromDate, toDate, this._DayOfWeek);
            EPSetup setup = PXSelect<EPSetup>.Select(this._Graph);
            var setupExt = setup?.GetExtension<EPSetupExt>();
            if (timecardSummaryRow.EarningType == setup.VacationsType) {
                var personalTime = timecardSummaryRow.TimeSpent;
                var totalPersonal = TimeCardValidationUtils.TotalPersonalTime(this._Graph, personalTime, timeCardCache, timeCardrow, setupExt.UsrAllowPesonalTime, CommonUtil.ConvertTo<DateTime>(dateofTheDay));
                if (totalPersonal == true) {
                    var docPersonalTimeFieldName = CommonUtil.GetMemberName((EPTimeCardExt c) => c.UsrPersonalTotalSpent);
                    timeCardCache.RaiseExceptionHandling(docPersonalTimeFieldName, timeCardrow, null, new PXSetPropertyException(Messages.PersonalTimeExceed, PXErrorLevel.Warning));
                }
            }
        }

        public void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e) {
            EPTimeCard timeCardrow = (EPTimeCard)this._Graph.Caches[typeof(EPTimeCard)].Current;
            PXCache timeCardCache = this._Graph.Caches[typeof(EPTimeCard)];
            EPTimeCardSummaryWithInfo timecardSummaryRow = (EPTimeCardSummaryWithInfo)this._Graph.Caches[typeof(EPTimeCardSummaryWithInfo)].Current;
            var fromDate = CommonUtil.ConvertTo<DateTime>(timeCardrow.WeekStartDate);
            var toDate = CommonUtil.ConvertTo<DateTime>(timeCardrow.WeekEndDate);
            var dateofTheDay = TimeCardValidationUtils.GetWeekdayInRange(fromDate, toDate, this._DayOfWeek);
            TimeCardValidationUtils.EarningTypeValidationSummary<EPTimeCardSummaryWithInfo>(timecardSummaryRow, sender, timecardSummaryRow.EarningType, timeCardrow.EmployeeID, this._Graph, dateofTheDay, this._FieldName);
        }
    }
}
