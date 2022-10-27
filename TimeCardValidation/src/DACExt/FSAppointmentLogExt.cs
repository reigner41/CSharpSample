using System;
using PX.Data;
using PX.Objects.FS;
using static PX.Objects.FS.FSAppointmentLog;

namespace PC.Objects.AA.HOJTimeCardValidation {
    public class FSAppointmentLogExt : PXCacheExtension<FSAppointmentLog> {

        #region DateTimeBegin  
        [PXDefault]
        [TimeRound(typeof(Current<dateTimeBegin>), false)]
        [PXDBDateAndTime(UseTimeZone = true, PreserveTime = true, DisplayNameDate = "Start Date", DisplayNameTime = "Start Time")]
        [PXUIField(DisplayName = "Start Date")]
        public DateTime? DateTimeBegin { get; set; }
        #endregion

        #region DateTimeEnd  
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [TimeRound(typeof(Current<dateTimeEnd>), false)]
        [PXUIVerify(typeof(Where<dateTimeBegin, IsNull,
                             Or<dateTimeEnd, IsNull,
                             Or<dateTimeEnd, GreaterEqual<dateTimeBegin>>>>),
                      PXErrorLevel.Error, TX.Error.END_TIME_LESSER_THAN_START_TIME)]
        [PXUIRequired(typeof(Where<timeDuration, GreaterEqual<Zero>,
                               And<
                                   Where<
                                       status, Equal<status.Completed>,
                                       Or<status, Equal<status.Paused>>>>>))]
        [PXDBDateAndTime(UseTimeZone = true, PreserveTime = true, DisplayNameDate = "End Date", DisplayNameTime = "End Time")]
        [PXUIField(DisplayName = "End Date")]
        public DateTime? DateTimeEnd { get; set; }
        #endregion

    }
}
