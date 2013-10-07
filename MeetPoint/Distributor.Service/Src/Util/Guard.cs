using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Distributor.Service.Src.Util
{
    public static class Guard
    {
        public static void NotNullOrEmpty(Expression<Func<string>> parameterExpression)
        {
            string value = parameterExpression.Compile()();
            if (String.IsNullOrWhiteSpace(value))
            {
                string name = GetParameterName(parameterExpression);
                throw new ArgumentException("Cannot be null or empty", name);
            }
        }

        public static void NotNull<T>(Expression<Func<T>> parameterExpression)
            where T : class
        {
            if (null == parameterExpression.Compile()())
            {
                string name = GetParameterName(parameterExpression);
                throw new ArgumentNullException(name);
            }
        }

        public static void ArgumentNotNullOrEmpty(string argument, string parameterName)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName");
            }

            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static void ArgumentNotNull(object argument, string parameterName)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName");
            }

            if (argument == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        private static string GetParameterName<T>(Expression<Func<T>> parameterExpression)
        {
            dynamic body = parameterExpression.Body;
            return body.Member.Name;
        }
    }
}
