using System;
using System.Linq;

namespace UniRedux
{
    public static class Assert
    {
        public static ReduxException CreateException()
        {
            return new ReduxException("ReduxAssert hit!");
        }

        public static ReduxException CreateException(string message)
        {
            return new ReduxException("ReduxAssert hit! " + message);
        }

        public static ReduxException CreateException(string message, params object[] parameters)
        {
            return new ReduxException(FormatString(message, parameters));
        }

        public static ReduxException CreateException(Exception innerException, string message,
            params object[] parameters)
        {
            return new ReduxException(FormatString(message, parameters), innerException);
        }

        private static string FormatString(string format, params object[] parameters)
        {
            if (parameters == null || parameters.Length <= 0) return format;
            var paramToUse = parameters;

            if (parameters.Any(cur => cur == null))
            {
                paramToUse = new object[parameters.Length];

                for (var i = 0; i < parameters.Length; ++i)
                {
                    paramToUse[i] = parameters[i] ?? "NULL";
                }
            }

            format = string.Format(format, paramToUse);

            return format;
        }
    }
}