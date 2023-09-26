using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;

namespace FormulaTests
{
    /// <summary>
    /// A collection of tests to push the bounds and verify all methods are working properly of the Formula class
    /// Author: Autumrose Stubbs
    /// </summary>
    [TestClass]
    public class FormulaTests
    {

        /// <summary>
        /// Tests for the GetVariables() Method
        /// </summary>
        [TestMethod()]
        public void GetVariables()
        {
            Formula formula = new Formula("1 / AZ + F");
            HashSet<String> answer = new HashSet<String>();
            answer.Add("AZ");
            answer.Add("F");
            Assert.IsTrue(formula.GetVariables().Equals(answer.Count));
        }
        [TestMethod()]
        public void UnderscoreLEading()
        {
            HashSet<String> answer = new HashSet<String>();
            Formula formula = new Formula("5*6/2+3 - F_ * _F");
            answer.Add("F_");
            answer.Add("_F");
            Assert.AreEqual(formula.GetVariables(), answer);
        }

        [TestMethod()]
        public void ReallyLongVariable()
        {
            Formula formula = new Formula("1 / AZ + _F");
            HashSet<String> answer = new HashSet<String>();
            answer.Add("AZ");
            answer.Add("A1573TKELW___3F");
            answer.Add("F_ * _F)");
            answer.Add("_F");
            Assert.AreEqual(formula.GetVariables(), answer);
        }

        [TestMethod()]
        public void TestGetVariablesRepeats()
        {
            Formula formula = new Formula("1 + _F / AZ + _F - _F");
            HashSet<String> answer = new HashSet<String>();
            answer.Add("AZ");
            answer.Add("_F");
            Assert.AreEqual(formula.GetVariables(), answer);
        }

        /// <summary>
        /// Tests for the ToString() Method
        /// </summary>
        [TestMethod()]
        public void ToStringWithSpaces()
        {
            Formula formula = new Formula("(5 +2) *1 - A");
            String answer = "(5+2)*1-A";
            Assert.AreEqual(formula.GetVariables(), answer);
        }

        /// <summary>
        /// Tests for Equals() method
        /// </summary>
        [TestMethod()]
        public void EqualsPassing()
        {
            Formula formula1 = new Formula("(5 +2) *1 - A");
            Formula formula2 = new Formula("(5 +2) *1 - A");
            Assert.IsTrue(formula1.Equals(formula2));
        }
        [TestMethod()]
        public void EqualsFailing()
        {
            Formula formula1 = new Formula("(5 +2) *1 - A");
            Formula formula2 = new Formula("(5 +2) *1 - b");
            Assert.IsFalse(formula1.Equals(formula2));
        }



        [TestMethod()]
        public void TestSingleNumber()
        {
            Formula formula = new Formula("5");
            Assert.AreEqual(5, formula.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestSingleVariable()
        {
            Formula formula = new Formula("X5");
            Assert.AreEqual(13, formula.Evaluate(s => 13));
        }

        [TestMethod(), Timeout(20000)]
        public void TestAddition()
        {
            Formula formula = new Formula("5+3");
            Assert.AreEqual(8, formula.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestSubtraction()
        {
            Formula formula = new Formula("18-10");
            Assert.AreEqual(8, formula.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestMultiplication()
        {
            Formula formula = new Formula("2*4");
            Assert.AreEqual(8, formula.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestDivision()
        {
            Formula formula = new Formula("16/2");
            Assert.AreEqual(8, formula.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestArithmeticWithVariable()
        {
            Formula formula = new Formula("2+X1");
            Assert.AreEqual(6, formula.Evaluate(s => 4));
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestUnknownVariable()
        {
            Formula formula = new Formula("2+X1");
            formula.Evaluate(s => { throw new FormulaFormatException("Unknown variable"); });
        }

        [TestMethod()]
        public void TestLeftToRight()
        {
            Formula formula = new Formula("2*6+3");
            Assert.AreEqual(15, formula.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestOrderOperations()
        {
            Formula formula = new Formula("2+6*3");
            Assert.AreEqual(20, formula.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestParenthesesTimes()
        {
            Formula formula = new Formula("(2+6)*3");
            Assert.AreEqual(24, formula.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestTimesParentheses()
        {
            Formula formula = new Formula("2*(3+5)");
            Assert.AreEqual(16, formula.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestPlusParentheses()
        {
            Formula formula = new Formula("2+(3+5)");
            Assert.AreEqual(10, formula.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestPlusComplex()
        {
            Formula formula = new Formula("2+(3+5*9)");
            Assert.AreEqual(50, formula.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestOperatorAfterParens()
        {
            Formula formula = new Formula("(1*1)-2/2");
            Assert.AreEqual(0, formula.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestComplexTimesParentheses()
        {
            Formula formula = new Formula("2+3*(3+5)");
            Assert.AreEqual(26, formula.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestComplexAndParentheses()
        {
            Formula formula = new Formula("2+3*5+(3+4*8)*5+2");
            Assert.AreEqual(194, formula.Evaluate(s => 0));
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestDivideByZero()
        {
            Formula formula = new Formula("5/0");
            formula.Evaluate(s => 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleOperator()
        {
            Formula formula = new Formula("+");
            formula.Evaluate(s => 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraOperator()
        {
            Formula formula = new Formula("2+5+");
            formula.Evaluate(s => 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraParentheses()
        {
            Formula formula = new Formula("2+5*7)");
            formula.Evaluate(s => 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidVariable()
        {
            Formula formula = new Formula("xx");
            formula.Evaluate(s => 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestParensNoOperator()
        {
            Formula formula = new Formula("5+7+(5)8");
            formula.Evaluate(s => 0);
        }


        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestEmpty()
        {
            Formula formula = new Formula("");
            formula.Evaluate(s => 0);
        }

        [TestMethod()]
        public void TestComplexMultiVar()
        {
            Formula formula = new Formula("y1*3-8/2+4*(8-9*2)/14*x7");
            Assert.AreEqual(6, formula.Evaluate(s => (s == "x7") ? 1 : 4));
        }

        [TestMethod()]
        public void TestComplexNestedParensRight()
        {
            Formula formula = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");
            Assert.AreEqual(6, formula.Evaluate(s => 1));
        }

        [TestMethod()]
        public void TestComplexNestedParensLeft()
        {
            Formula formula = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12, formula.Evaluate(s => 2));
        }

        [TestMethod()]
        public void TestRepeatedVar()
        {
            Formula formula = new Formula("a4-a4*a4/a4");
            Assert.AreEqual(0, formula.Evaluate(s => 3));
        }
    }
}
