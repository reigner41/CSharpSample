using System;
using System.Linq;
using System.Linq.Expressions;

namespace PC.Objects.AA.HOJTimeCardValidation {
    public class CommonUtil {

        //Convert To Any type with returning default if null
        public static T ConvertTo<T>(object x) where T : struct {
            return (T)(x == null ? default(T) : (T?)Convert.ChangeType(x, typeof(T)));
        }

        //Get Member FieldName
        public static string GetMemberName<T, TValue>(Expression<Func<T, TValue>> memberAccess) {
            return ((MemberExpression)memberAccess.Body).Member.Name;
        }

    }
}
