using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.EP;

namespace PC.Objects.AA.HOJTimeCardValidation {
    public class EPSetupExt : PXCacheExtension<EPSetup>{

        #region UsrUseEmployeeEarningType
        [PXDBBool()]
        [PXUIField(DisplayName = "Use Employee Earning Type")]
        public virtual bool? UsrUseEmployeeEarningType { get; set; }
        public abstract class usrUseEmployeeEarningType : PX.Data.BQL.BqlBool.Field<usrUseEmployeeEarningType> { }
        #endregion

        #region UsrAllowPesonalTime
        [PXDBInt]
        [PXUIField(DisplayName = "Allow Buffer Personal Time")]
        public virtual int? UsrAllowPesonalTime { get; set; }
        public abstract class usrAllowPesonalTime : PX.Data.BQL.BqlInt.Field<usrAllowPesonalTime> { }
        #endregion

        #region UsrNonworkingtype
        [PXDBString]
        [PXUIField(DisplayName = "Non-working Earning Type")]
        [PXSelector(typeof(Search<EPEarningType.typeCD>))]
        public virtual string UsrNonworkingtype { get; set; }
        public abstract class usrNonworkingtype : PX.Data.BQL.BqlString.Field<usrNonworkingtype> { }
        #endregion
    }
}
