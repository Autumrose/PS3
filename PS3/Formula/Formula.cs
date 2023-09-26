// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens
// Author: Autumrose Stubbs



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        //Global variables: String to hold the formula and hashset to hold the tokens.
        private String formulaString;
        private HashSet<String> variables;
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {

        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            //Check if the given formula string is empty or null and throws and exception
            if (formula == "" || Equals(formula, null))
            {
                throw new FormulaFormatException("Your formula can not be null or empty!");
            }
            // Initializing variables 
            formulaString = "";
            variables = new HashSet<string>();
            double value = 0; ;
            //Array to keep track of the formula items 
            String[] array = GetTokens(formula).ToArray();
            int rightPar = 0;
            int leftPar = 0;
            String token = normalize(array[0]);
            //Throws exception if the first element is not a parenthesis, a number, or a variable
            if (!token.Equals("(") && !Double.TryParse(token, out value) && !isAVariable(token))
            {
                throw new FormulaFormatException("The first item in the equation is invalid!");
            }
            token = normalize(array[array.Length - 1]);
            //Throws exception if the last element is not a closing parenthesis, number, or variable
            if (!token.Equals(")") && !Double.TryParse(token, out value) && !isAVariable(token))
            {
                throw new FormulaFormatException("The last item in the equation is invalid!");
            }
            //Throws exception if the array has nothing in it
            if (!(array.Length > 0))
            {
                throw new FormulaFormatException("You must have at least one item in the formula!");
            }
            
            //Iterate through the entire array
            for (int i = 0; i < array.Length; i++)
            {
                //Store the item at the current position of the array in a string and normalize it
                token = normalize(array[i]);
                //Skip the loop if it's blank space
                if (String.IsNullOrEmpty(token))
                {
                    continue;
                }
                //Check if the token is a number and stores the number in a new variable
                if (Double.TryParse(token, out value))
                {
                    //Throws an exception if the number is negative
                    if (value < 0)
                    {
                        throw new FormulaFormatException("Negative numbers are not allowed!");
                    }
                    //Throws and exception if the next token isn't valid such as an opening parenthesis
                    if (i < array.Length - 1)
                    {
                        if (!array[i + 1].Equals("+") && !array[i + 1].Equals("-") && !array[i + 1].Equals("*") && !array[i + 1].Equals("/") && !array[i + 1].Equals(")"))
                        {
                            throw new FormulaFormatException("Item following a number should be an operation!");
                        }
                    }
                    token = value.ToString();
                }
                //Checks it the token is an opening parenthesis
                else if (token.Equals("("))
                {
                    //Increase the counter
                    leftPar++;
                    if (i < array.Length - 1)
                    {
                        //Throws and exception if the next token isn't valid such as an operator
                        if (!normalize(array[i + 1]).Equals("(") && !Double.TryParse(normalize(array[i + 1]), out value) && !isAVariable(normalize(array[i + 1])))
                        {
                            throw new FormulaFormatException("Item following an opening parenthesis should be a number, variable, or another opening parenthesis!");
                        }
                    }
                }
                //Checks it the token is a closing parenthesis
                else if (token.Equals(")"))
                {
                    //Increase the counter and check there's a next item
                    rightPar++;
                    if (i < array.Length - 1)
                    {
                        //Throws and exception if the next token isn't valid such as an operator
                        if (!array[i + 1].Equals("+") && !array[i + 1].Equals("-") && !array[i + 1].Equals("*") && !array[i + 1].Equals("/") && !array[i + 1].Equals(")"))
                        {
                            throw new FormulaFormatException("Item following an opening parenthesis should be a number, variable, or another opening parenthesis!");
                        }
                    }
                }
                //Checks it the token is an operator
                else if (token.Equals("+") || token.Equals("-") || token.Equals("*") || token.Equals("/"))
                {
                    //Check there's another item in the array
                    if (i < array.Length - 1)
                    {
                        //Throws and exception if the next token isn't valid such as another operator
                        if (!normalize(array[i + 1]).Equals("(") && !Double.TryParse(normalize(array[i + 1]), out value) && !isAVariable(normalize(array[i + 1])))
                        {
                            throw new FormulaFormatException("Item following an opening parenthesis should be a number, variable, or another opening parenthesis!");
                        }
                    }
                }
                else
                {
                    if (!isAVariable(token))
                    {
                        throw new FormulaFormatException("The formula contains a symbol that is not a number, operator, or variable consisting of letters or underscores: " + token);
                    }
                    if (!isValid(token))
                    {
                        throw new FormulaFormatException("Your variable has to be valid after normalizing!");
                    }
                }
                //Compile the normalized formula string
                formulaString += token;
            }
            //If at the end the left parenthesis and right aren't equal throw an exception
            if (rightPar != leftPar)
            {
                throw new FormulaFormatException("There should not be more closing parenthesis than open!");
            }

        }
        private Boolean isAVariable(String token)
        {
            Char[] charArray = new Char[10];
            //Turn the current token into a char array to check for proper variable structure
            charArray = token.ToCharArray();
            String letter = charArray[0].ToString();
            //If the first character isn't a letter throws an exception
            if (!Regex.IsMatch(letter, @"^[a-zA-Z]+$") && !letter.Equals("_"))
            {
                return false;
            }
            //Iterate through the rest of the variable
            for (int j = 0; j < charArray.Length; j++)
            {
                letter = charArray[j].ToString();
                //Throws an exception if the following characters aren't an underscore, number or another letter
                if (!letter.Equals("_") && !Double.TryParse(letter, out Double num) && !Regex.IsMatch(letter, @"^[a-zA-Z]+$"))
                {
                    return false;
                }
                else
                {
                    //Keep a variable list to use in other methods
                    variables.Add(token);
                }
            }
            return true;
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an FormulaFormatException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            //Two stacks: one to keep track of operators and parenthesis and one for numbers and variables
            Stack<string> operatorStack = new Stack<string>();
            Stack<Double> valueStack = new Stack<Double>();

            //Create the a string from the formula, take out white space
            formulaString = formulaString.Trim();
            string[] substrings = Regex.Split(formulaString, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            //iterate through the whole string
            foreach (String token in substrings)
            {
                //Skip the loop if it's blank space
                if (String.IsNullOrEmpty(token))
                {
                    continue;
                }

                Double doubleValue;
                Double computed;
                if (Double.TryParse(token, out doubleValue)) // if token is int
                {
                    if (operatorStack.Count > 0)
                    {
                        if (Equals(operatorStack.Peek(), "*") || Equals(operatorStack.Peek(), "/")) // if operator is * or /
                        {
                            if (valueStack.Count == 0) // value stack empty error
                            {
                                return new FormulaError("Value stack is empty!");
                            }

                            string op = operatorStack.Pop();
                            Double val = valueStack.Pop();

                            if (doubleValue == 0 && Equals(op, "/")) // divide by 0 error
                            {
                                return new FormulaError("You can't divide by 0!");
                            }
                            else
                            {
                                if (Equals(op, "*"))
                                {
                                    computed = val * doubleValue;
                                    valueStack.Push(computed); // apply *
                                }
                                else if (Equals(op, "/"))
                                {
                                    computed = val / doubleValue;
                                    valueStack.Push(computed); // apply /
                                }
                            }
                        }
                        else
                        {
                            valueStack.Push(doubleValue); // else push to value stack
                        }
                    }
                    else
                    {
                        valueStack.Push(doubleValue); // else push to value stack
                    }
                }
                else if (isAVariable(token)) // if token is a variable
                {
                    Double varVal = 0;
                    try
                    {
                        varVal = lookup(token);
                    }catch(Exception e)
                    {
                        return new FormulaError("A value for the variable " + token + "doesn't exist");
                    }
                    if (operatorStack.Count > 0)
                    {
                        if (Equals(operatorStack.Peek(), "*") || Equals(operatorStack.Peek(), "/")) // if operator is * or /
                        {
                            if (valueStack.Count == 0) // value stack empty error
                            {
                                return new FormulaError("Value stack is empty!");
                            }

                            string op = operatorStack.Pop();
                            Double val = valueStack.Pop();

                            if (varVal == 0 && Equals(op, "/")) // divide by 0 error
                            {
                                return new FormulaError("You can not divide by 0!!");
                            }
                            else
                            {
                                if (Equals(op, "*"))
                                {
                                    computed = val * varVal;
                                    valueStack.Push(computed); // apply *
                                }
                                else if (Equals(op, "/"))
                                {
                                    computed = val / varVal;
                                    valueStack.Push(computed); // apply /
                                }
                            }
                        }
                        else
                        {
                            valueStack.Push(varVal); // else push to value stack
                        }
                    }
                    else
                    {
                        valueStack.Push(varVal); // else push to value stack
                    }
                }
                else if (Equals(token, "+") || Equals(token, "-")) // if token is + or -
                {
                    if (operatorStack.Count > 0)
                    {
                        if (Equals(operatorStack.Peek(), "+") || Equals(operatorStack.Peek(), "-"))
                        {
                            if (valueStack.Count < 2) // value stack less than 2
                            {
                                return new FormulaError("You can't have more operators than values!");
                            }

                            string op = operatorStack.Pop();
                            Double firstVal = valueStack.Pop();
                            Double secondVal = valueStack.Pop();

                            if (Equals(op, "-"))
                            {
                                computed = secondVal - firstVal;
                                valueStack.Push(computed); // apply /
                            }
                            else if (Equals(op, "+"))
                            {
                                computed = firstVal + secondVal;
                                valueStack.Push(computed); // apply /
                            }
                        }
                    }
                    else if (valueStack.Count < 1)
                    {
                        return new FormulaError("You don't have enough values to compute!");
                    }
                    operatorStack.Push(token);
                }
                else if (Equals(token, "*") || Equals(token, "/")) // if token is * or /
                {
                    if (valueStack.Count < 1)
                    {
                        return new FormulaError("You don't have enough values to compute!");
                    }
                    operatorStack.Push(token);
                }
                else if (Equals(token, "(")) // if token is (
                {
                    operatorStack.Push(token);
                }
                else if (Equals(token, ")")) // if token is ) 
                {
                    if (operatorStack.Count > 0)
                    {
                        if (Equals(operatorStack.Peek(), "+") || Equals(operatorStack.Peek(), "-"))
                        {
                            if (valueStack.Count < 2) // value stack less than 2
                            {
                                return new FormulaError("You don't have enough values to compute!");
                            }

                            string op = operatorStack.Pop();
                            Double firstVal = valueStack.Pop();
                            Double secondVal = valueStack.Pop();

                            if (Equals(op, "-"))
                            {
                                computed = secondVal - firstVal;
                                valueStack.Push(computed); // apply /
                            }
                            else if (Equals(op, "+"))
                            {
                                computed = firstVal + secondVal;
                                valueStack.Push(computed); // apply /
                            }

                            if (operatorStack.Count < 1)
                            {
                                return new FormulaError("You don't have enough values to compute!");
                            }
                            else if (!operatorStack.Pop().Equals("("))
                            {
                                return new FormulaError("You're missing an opening bracket!");
                            }
                            else if (operatorStack.Count > 0)
                            {
                                if (Equals(operatorStack.Peek(), "*") || Equals(operatorStack.Peek(), "/"))
                                {
                                    if (valueStack.Count < 2) // value stack less than 2
                                    {
                                        return new FormulaError("You don't have enough values to compute!");
                                    }

                                    string op2 = operatorStack.Pop();
                                    Double firstVal2 = valueStack.Pop();
                                    Double secondVal2 = valueStack.Pop();

                                    if (Equals(op2, "*"))
                                    {
                                        computed = firstVal2 * secondVal2;
                                        valueStack.Push(computed); // apply *
                                    }
                                    else if (Equals(op2, "/"))
                                    {
                                        computed = secondVal2 / firstVal2;
                                        if (firstVal2 == 0)
                                        {
                                            return new FormulaError("No division by 0!");
                                        }
                                        valueStack.Push(computed); // apply /
                                    }

                                }
                            }
                        }
                        else if (!operatorStack.Pop().Equals("("))
                        {
                            return new FormulaError("You can't have a closing parenthesis without and open one first!");
                        }
                    }
                }
                else // token did not match any acceptable conditions
                {
                    return new FormulaError("Error: You can only have variables made up of letters and underscores, numbers, operators, and parenthesis!");
                }
            }

            // After all tokens are processed
            if (operatorStack.Count == 0)
            {
                if (valueStack.Count != 1)
                {
                    return new FormulaError("You had more variables than oeprators!");
                }
                else
                {
                    return valueStack.Pop();
                }
            }
            else
            {
                //Throws exception if there's not enough variables or operators
                if ((operatorStack.Count == 1 && valueStack.Count != 2) || (operatorStack.Count != 1 && valueStack.Count == 2))
                {
                    return new FormulaFormatException("You're left with an unmatching number of operators and values");
                }
                else
                {
                    //Perform the last operation
                    string op1 = operatorStack.Pop();
                    Double val1 = valueStack.Pop();
                    Double val2 = valueStack.Pop();

                    if (Equals(op1, "-"))
                    {
                        return val2 - val1;
                    }
                    else if (Equals(op1, "+"))
                    {
                        return val2 + val1;
                    }
                }
            }

            return 0;
        }


        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return variables;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return formulaString.Trim();
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            //Returns false if the input object is not an instant of Formula or is null
            if (!(obj is Formula) || obj == null)
            {
                return false;
            }
            //Two arrays to keep track of our two formulas
            String[] Formula1 = GetTokens(this.ToString().Trim()).ToArray();
            String[] Formula2 = GetTokens(((Formula)obj).ToString().Trim()).ToArray();
            //Doubles to hold any doubles we parse from the array
            double double1 = 0;
            double double2 = 0;
            //Iterate through the entire array
            for (int i = 0; i < Formula1.Length; i++)
            {
                //Checks if the elements are doubles, this accounts for floats
                if (Double.TryParse(Formula1[i], out double1) && Double.TryParse(Formula2[i], out double2))
                {
                    //Returns false if they don't equal each other
                    if (!double1.Equals(double2))
                    {
                        return false;
                    }
                    //Else compares the strings and returns false if they don't equal each other
                }
                else if (!Formula1[i].Equals(Formula2[i]))
                {
                    return false;
                }
            }
            //Finished the loop so everything matches, return true
            return true;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {

            //Returns true if both items equal null
            if (ReferenceEquals(f1, null) && ReferenceEquals(f2, null))
            {
                return true;
            }
            //Returns false if one item equals null
            else if (ReferenceEquals(f1, null) || ReferenceEquals(f2, null))
            {
                return false;
            }
            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            //Returns true if both items equal null
            if (ReferenceEquals(f1, null) && ReferenceEquals(f2, null))
            {
                return false;
            }
            //Returns false if one item equals null
            else if (ReferenceEquals(f1, null) || ReferenceEquals(f2, null))
            {
                return true;
            }
            return !f1.Equals(f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return formulaString.GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        public static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }

    }
}
