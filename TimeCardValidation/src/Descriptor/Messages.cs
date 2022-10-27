using PX.Common;

namespace PC.Objects.AA.HOJTimeCardValidation {
    [PXLocalizable]
    public static class Messages {
        public const string ExistingSession = "You already have a time card for this time.";
        public const string ErrorHoliday = "Holiday is not allowed for the regular day";
        public const string ErrorRegular = "Today is Holiday, are you sure you dont want to use Holiday?";
        public const string PersonalTimeExceed = "Personal Time Exceed input time";
        public const string ErrorEarningtypeOverride = " is not available for this employee Labor Override";
        public const string PersonalTimeOverExceedNotAllowed = "You have exceeded the available PTO. Manager approval will be required.";

    }
}
