using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var expr=DynamicExpression.ParseLambda<double,double>("Math.Sin(@0)",1.0);
            //Assert.AreEqual(typeof(int), expr.Type);
            //Assert.IsTrue(expr.NodeType == ExpressionType.Constant);
        }

        [TestMethod]
        public void TestMethod2()
        {
            ParameterExpression x = Expression.Parameter(typeof(double), "x");
            //ParameterExpression y = Expression.Parameter(typeof(int), "y");
            LambdaExpression e = DynamicExpression.ParseLambda(
                 new ParameterExpression[] { x }, typeof(double), "Math.Sin(x)");

            var func = (Func<double, double>)e.Compile();
            Assert.AreEqual(Math.Sin(2), func(2));
        }
    }
}
