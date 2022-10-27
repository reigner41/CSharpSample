using System;
using PX.Data;
using PX.Objects.CR;

namespace PC.Objects.AA.HOJTimeCardValidation {
    public class BAccountExt : PXCacheExtension<BAccount> {
        #region UsrLaborItemOverride
        [PXDBBool]
        [PXUIField(DisplayName = "Labor Item Override")]

        public virtual bool? UsrLaborItemOverride { get; set; }
        public abstract class usrLaborItemOverride : PX.Data.BQL.BqlBool.Field<usrLaborItemOverride> { }
        #endregion
    }
}
