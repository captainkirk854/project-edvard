namespace Utility.Mvvm
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Provides support for extracting property information based on a property expression
    /// </summary>
    public static class PropertyHelper
    {
        /// <summary>
        /// Extracts property name from a property expression.
        /// </summary>
        /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
        /// <param name="propertyExpression">The property expression (e.g. p => p.PropertyName)</param>
        /// <returns>The name of the property.</returns>
        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("Expression is not a member access expression.", "propertyExpression");
            }

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("Member access expression does not access a property.", "propertyExpression");
            }

            var getMethod = property.GetMethod;
            if (getMethod.IsStatic)
            {
                throw new ArgumentException("Referenced property is a static property.", "propertyExpression");
            }

            return memberExpression.Member.Name;
        }
    }
}
