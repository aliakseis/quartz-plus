using System.CodeDom;
using System.Web.Compilation;
using System.Web.UI;

namespace ILCWebApplication
{
    [ExpressionPrefix("Code")]
    public class CodeExpressionBuilder : ExpressionBuilder
    {
        /// <summary>
        /// Class that allows us to use raw code to assign values to control properties.
        /// To use it, or any custom ExpressionBuilder for that matter, we must register it in the web.config expressionBuilders section.
        /// See http://weblogs.asp.net/infinitiesloop/archive/2006/08/09/The-CodeExpressionBuilder.aspx
        /// </summary>
        public override CodeExpression GetCodeExpression(BoundPropertyEntry entry, object parsedData,
                                                         ExpressionBuilderContext context)
        {
            return new CodeSnippetExpression(entry.Expression);
        }
    }
}