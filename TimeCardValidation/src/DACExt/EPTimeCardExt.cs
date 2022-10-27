using PX.Data;
using PX.Objects.CS;
using PX.Objects.EP;
using System;

namespace PC.Objects.AA.HOJTimeCardValidation {
    public class EPTimeCardExt : PXCacheExtension<EPTimeCard> {

        #region UsrNonworkingTime
        [PXDBInt(MinValue = 0)]
        [PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Non-Worked")]
        [PXTimeList(30, 335, ExclusiveValues = false)]
        public virtual int? UsrNonworkingTime { get; set; }
        public abstract class usrNonworkingTime : PX.Data.BQL.BqlInt.Field<usrNonworkingTime> { }
        #endregion

        #region UsrPersonalTotalSpent
        [PXDBInt(MinValue = 0)]
        [PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXTimeList(30, 335, ExclusiveValues = false)]
        [PXUIField(DisplayName = "Total Spent", IsReadOnly = true)]
        public virtual int? UsrPersonalTotalSpent { get; set; }
        public abstract class usrPersonalTotalSpent : PX.Data.BQL.BqlInt.Field<usrPersonalTotalSpent> { }
        #endregion

        #region UsrPersonalAlottedTime
        [PXInt(MinValue = 0)]
        [PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXTimeList(30, 335, ExclusiveValues = false)]
        [PXUIField(DisplayName = "Personal Time Alloted", IsReadOnly = true)]
        public virtual int? UsrPersonalAlottedTime { get; set; }
        public abstract class usrPersonalAlottedTime : PX.Data.BQL.BqlInt.Field<usrPersonalAlottedTime> { }
        #endregion

        #region UsrPTOBalance
        [PXInt(MinValue = 0)]
        [PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXTimeList(30, 335, ExclusiveValues = false)]
        [PXUIField(DisplayName = "Balance", IsReadOnly = true)]

        public virtual int? UsrPTOBalance { get; set; }
        public abstract class usrPTOBalance : PX.Data.BQL.BqlInt.Field<usrPTOBalance> { }
        #endregion

        #region UsrPTOAvailable
        [PXInt]
        [PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXTimeList(30, 335)]
        [PXUIField(DisplayName = "Available", IsReadOnly = true)]

        public virtual int? UsrPTOAvailable { get; set; }
        public abstract class usrPTOAvailable : PX.Data.BQL.BqlInt.Field<usrPTOAvailable> { }
        #endregion

        #region UsrPTORemaining
        [PXInt(MinValue = 0)]
        [PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXTimeList(30, 335, ExclusiveValues = false)]
        [PXUIField(DisplayName = "Remaining", IsReadOnly = true)]

        public virtual int? UsrPTORemaining { get; set; }
        public abstract class usrPTORemaining : PX.Data.BQL.BqlInt.Field<usrPTORemaining> { }
        #endregion

    }
}
