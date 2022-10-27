using System;
using PX.Data;
using PX.Objects.EP;
using PX.Objects.CR;
using static PX.Objects.EP.EPTimeCardItem;
using static PX.Objects.EP.TimeCardMaint;

namespace PC.Objects.AA.HOJTimeCardValidation {

    public class EPTimeCardSummaryWithInfoExt : PXCacheExtension<EPTimeCardSummary> {

        // [PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        //// [PXDefault(typeof(Search<EPSetup.regularHoursType>))]
        // //[PXDefault(PersistingCheck = PXPersistingCheck.Null)]
        // [PXSelector(typeof(Search5<EPEarningType.typeCD,
        //     CrossJoin<EPSetup,
        //     LeftJoin<EPEmployeeClassLaborMatrix, On<EPEmployeeClassLaborMatrix.earningType, Equal<EPEarningType.typeCD>>>>,
        // Where<EPSetupExt.usrUseEmployeeEarningType, Equal<False>, Or<EPEmployeeClassLaborMatrix.employeeID, Equal<Current<EPTimeCardSummaryWithInfo.employeeID>>>>, Aggregate<GroupBy<EPEarningType.typeCD>>>))]
        // [PXUIField(DisplayName = "Earning Type")]
        // public string EarningType { get; set; }

        #region EarningType  
        [PXMergeAttributes(Method = MergeMethod.Replace)]
        [PXDBString(EPEarningType.typeCD.Length, IsUnicode = true, InputMask = EPEarningType.typeCD.InputMask)]
        [PXDefault(typeof(Search<EPSetup.regularHoursType>))]
        [PXRestrictor(typeof(Where<EPEarningType.isActive, Equal<True>>), PX.Objects.EP.Messages.EarningTypeInactive, typeof(EPEarningType.typeCD))]
        //[PXSelector(typeof(EPEarningType.typeCD))]
        [PXSelector(typeof(Search5<EPEarningType.typeCD,
             CrossJoin<EPSetup,
             LeftJoin<EPEmployeeClassLaborMatrix, On<EPEmployeeClassLaborMatrix.earningType, Equal<EPEarningType.typeCD>>>>,
         Where<EPSetupExt.usrUseEmployeeEarningType, Equal<False>, Or<EPEmployeeClassLaborMatrix.employeeID, Equal<Current<EPTimeCardSummary.employeeID>>>>, Aggregate<GroupBy<EPEarningType.typeCD>>>))]
        [PXUIField(DisplayName = "Earning Type")]

        public string EarningType { get; set; }
        #endregion

        [PXDBInt]
        [PXTimeList(15, 96)]
        [TimeRound(typeof(Current<mon>), false)]
        [TimeCardSummary(DayOfWeek.Monday)]
        [PXUIField(DisplayName = "Mon")]
        public int? Mon {
            get;
            set;
        }

        #region Tue  
        [PXTimeList(15, 96)]
        [TimeRound(typeof(Current<tue>), false)]
        [TimeCardSummary(DayOfWeek.Tuesday)]
        [PXDBInt]
        [PXUIField(DisplayName = "Tue")]
        public int? Tue { 
            get; 
            set; 
        }
        #endregion

        #region Wed  
        [PXTimeList(15, 96)]
        [TimeRound(typeof(Current<wed>), false)]
        [TimeCardSummary(DayOfWeek.Wednesday)]
        [PXDBInt]
        [PXUIField(DisplayName = "Wed")]
        public int? Wed { 
            get; 
            set; 
        }
        #endregion

        #region Thu  
        [PXTimeList(15, 96)]
        [TimeRound(typeof(Current<thu>), false)]
        [TimeCardSummary(DayOfWeek.Thursday)]
        [PXDBInt]
        [PXUIField(DisplayName = "Thu")]
        public int? Thu { 
            get; 
            set; 
        }
        #endregion

        #region Fri  
        [PXTimeList(15, 96)]
        [TimeRound(typeof(Current<fri>), false)]
        [TimeCardSummary(DayOfWeek.Friday)]
        [PXDBInt]
        [PXUIField(DisplayName = "Fri")]
        public int? Fri { 
            get; 
            set; 
        }
        #endregion

        #region Sat  
        [PXTimeList(15, 96)]
        [TimeRound(typeof(Current<sat>), false)]
        [TimeCardSummary(DayOfWeek.Saturday)]
        [PXDBInt]
        [PXUIField(DisplayName = "Sat")]
        public int? Sat { 
            get; 
            set; 
        }
        #endregion

        #region Sun  
        [PXTimeList(15, 96)]
        [TimeRound(typeof(Current<sun>), false)]
        [TimeCardSummary(DayOfWeek.Sunday)]
        [PXDBInt]
        [PXUIField(DisplayName = "Sun")]
        public int? Sun { 
            get; 
            set; 
        }
        #endregion

    }
}
