using System;
using PX.Data;
using PX.Objects.FS;
using static PX.Objects.FS.FSAppointment;

namespace PC.Objects.AA.HOJTimeCardValidation {
    public class FSAppointmentExt : PXCacheExtension<FSAppointment> {

        #region ScheduledDateTimeBegin  
        [PXDBDateAndTime(UseTimeZone = true, PreserveTime = true, DisplayNameDate = "Scheduled Start Date", DisplayNameTime = "Scheduled Start Time")]
        [PXDefault]
        [TimeRound(typeof(Current<scheduledDateTimeBegin>), false)]
        [PXUIField(DisplayName = "Scheduled Start Date", Visibility = PXUIVisibility.SelectorVisible)]
        public DateTime? ScheduledDateTimeBegin { get; set; }
        #endregion

        #region ScheduleTime  
        public abstract class scheduleTime : PX.Data.BQL.BqlDateTime.Field<scheduleTime> { }
        [PXTimeList(15, 96)]
        [TimeRound(typeof(Current<scheduleTime>), false)]
        [PXDateAndTime]
        [PXUIField(DisplayName = "Time")]
        public DateTime? ScheduleTime {
            get;
            set;
        }
        #endregion

        #region ScheduledDateTimeEnd  
        [PXDBDateAndTime(UseTimeZone = true, PreserveTime = true, DisplayNameDate = "Scheduled End Date", DisplayNameTime = "Scheduled End Time")]
        [PXDefault]
        [TimeRound(typeof(Current<scheduledDateTimeEnd>), false)]
        [PXUIEnabled(typeof(handleManuallyScheduleTime))]
        [PXUIField(DisplayName = "Scheduled End Date")]
        public DateTime? ScheduledDateTimeEnd { get; set; }
        #endregion

    }
}
