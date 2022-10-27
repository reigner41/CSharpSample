using System;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using static PX.Objects.CR.CRPMTimeActivity;
using static PX.Objects.EP.TimeCardMaint;

namespace PC.Objects.AA.HOJTimeCardValidation {
    public class EPTimecardDetailExt : PXCacheExtension<PMTimeActivity> {

        //#region EarningTypeID
        //[PXDBString(EPEarningType.typeCD.Length, IsUnicode = true, InputMask = EPEarningType.typeCD.InputMask)]
        ////[PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), "Required", true)]
        //[PXRestrictor(typeof(Where<EPEarningType.isActive, Equal<True>>), PX.Objects.EP.Messages.EarningTypeInactive, typeof(EPEarningType.typeCD))]
        //[PXSelector(typeof(Search5<EPEarningType.typeCD,
        //    CrossJoin<EPSetup,
        //    LeftJoin<EPEmployeeClassLaborMatrix, On<EPEmployeeClassLaborMatrix.earningType, Equal<EPEarningType.typeCD>>>>,
        //Where<EPSetupExt.usrUseEmployeeEarningType, Equal<False>, Or<EPEmployeeClassLaborMatrix.employeeID, Equal<Current<EPTimeCard.employeeID>>>>, Aggregate<GroupBy<EPEarningType.typeCD>>>))]
        //[PXUIField(DisplayName = "Earning Type")]
        //public virtual string EarningTypeID { get; set; }
        //#endregion

        #region EarningTypeID  
        [PXDBString(EPEarningType.typeCD.Length, IsUnicode = true, InputMask = EPEarningType.typeCD.InputMask)]
        [PXDefault("RG", typeof(Search<EPSetup.regularHoursType>), PersistingCheck = PXPersistingCheck.Null)]
        [PXRestrictor(typeof(Where<EPEarningType.isActive, Equal<True>>), PX.Objects.EP.Messages.EarningTypeInactive, typeof(EPEarningType.typeCD))]
        //[PXSelector(typeof(EPEarningType.typeCD), DescriptionField = typeof(EPEarningType.description))]
        [PXSelector(typeof(Search5<EPEarningType.typeCD,
            CrossJoin<EPSetup,
            LeftJoin<EPEmployeeClassLaborMatrix, On<EPEmployeeClassLaborMatrix.earningType, Equal<EPEarningType.typeCD>>>>,
        Where<EPSetupExt.usrUseEmployeeEarningType, Equal<False>, Or<EPEmployeeClassLaborMatrix.employeeID, Equal<Current<EPTimeCard.employeeID>>>>, Aggregate<GroupBy<EPEarningType.typeCD>>>))]
        [PXUIField(DisplayName = "Earning Type")]
        public string EarningTypeID { get; set; }
        #endregion

        #region TimeSpent  
        [PXTimeList(15, 96)]
        [TimeRound(typeof(Current<timeSpent>), true)]
        [PXDefault(0)]
        //[PXFormula((typeof
        //    (Switch<
        //        Case<Where<EPTimecardDetail.timeSpent.IfNullThen<int0>, Equal<int0>>, int0,
        //        Case<Where<EPTimecardDetail.earningTypeID, Equal<Current<EPSetupExt.usrNonworkingtype>>>, EPTimecardDetail.timeSpent>>>)), typeof(SumCalc<EPTimeCardExt.usrNonworkingTime>))]
        //[PXFormula(null, typeof(SumCalc<EPTimeCardExt.usrNonworkingTime>))]
        [PXDBInt()]
        [PXUIField(DisplayName = "Time Spent")]
        public int? TimeSpent { get; set; }
        #endregion

        #region UsrDateTimeEnd
        //[PXDBDate]
        [PXDBDateAndTime(DisplayNameDate = "End Date", DisplayNameTime = "End Time", UseTimeZone = true)]
        [PXUIField(DisplayName = "DateTimeEnd", IsReadOnly = true)]
        public virtual DateTime? UsrDateTimeEnd { get; set; }
        public abstract class usrDateTimeEnd : PX.Data.BQL.BqlDateTime.Field<usrDateTimeEnd> { }
        #endregion

        //#region UsrTimeCardTimeSpent
        //[PXInt]
        //[PXTimeList(15, 96)]
        //[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXFormula((typeof
        //    (Switch<
        //        Case<Where<EPTimecardDetail.timeSpent.IfNullThen<int0>, Equal<int0>>, int0,
        //        Case<Where<EPTimecardDetail.earningTypeID, Equal<usrIsInventoryServiceType>>, EPTimecardDetail.timeSpent>>>)), typeof(SumCalc<EPTimeCardExt.usrPersonalSpentCalc>))]
        //[PXUIField(DisplayName = "usrTimeCardTimeSpent")]
        //public int? UsrTimeCardTimeSpent { get; set; }
        //public abstract class usrTimeCardTimeSpent : PX.Data.BQL.BqlInt.Field<usrTimeCardTimeSpent> { }
        //protected void _(Events.CacheAttached<usrTimeCardTimeSpent> e) {
        //}
        //#endregion

        #region Date 
        [PXDBDateAndTime(DisplayNameDate = "Date", DisplayNameTime = "Time", UseTimeZone = true)]
        [TimeRound(typeof(Current<date>), true)]
        [PXFormula(typeof(IsNull<Current<CRActivity.startDate>, Current<CRSMEmail.startDate>>))]
        [PXUIField(DisplayName = "Date")]
        public DateTime? Date {
            get;
            set;
        }
        #endregion

    }
}
