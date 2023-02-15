using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace Ref.Token
{

    public class TokenOperator : Token
    {
        public TokenOperator(string c)
            : base(c)
        {
            switch (c)
            {
                case "+":
                    Precedence = 1;
                    AsociatifG = true;
                    break;
                case "-":
                    Precedence = 1;
                    AsociatifG = true;
                    break;
                case "*":
                    Precedence = 2;
                    AsociatifG = true;
                    break;
                case "/":
                    Precedence = 2;
                    AsociatifG = true;
                    break;
                case "%":
                    Precedence = 2;
                    AsociatifG = true;
                    break;
                case "^":
                    Precedence = 3;
                    AsociatifG = false;
                    break;
                default:
                    Precedence = 0;
                    AsociatifG = false;
                    break;
            }
        }

        public static float Operation(TokenOperator op, TokenOperand op1, TokenOperand op2)
        {
            float operand1 = float.Parse(op1.Value);
            float operand2 = float.Parse(op2.Value);
            switch (op.Value)

            {
                case "+":
                    return operand1 + operand2;
                case "-":
                    return operand1 - operand2;
                case "*":
                    return operand1 * operand2;
                case "/":
                    if (operand2 == 0f) throw new DivideByZeroException();
                        return operand1 / operand2;
                case "%":
                    if (operand2 == 0f) throw new DivideByZeroException();
                    return operand1 % operand2;
                case "^":
                    return (float) Math.Pow(operand1, operand2);
                case "=":
                    if (op2.isVar && !op2.initVar) throw new SyntaxErrorException("Unbound variable");
                    foreach (var v in TokenOperand.variableList)
                    {
                        if (v.Name == op1.Name)
                        {
                            v.Value = op2.Value;
                            v.initVar = true;
                            return operand2;
                        }
                    }
                    throw new SyntaxErrorException("Variable non définie");

                default:
                    throw new SyntaxErrorException("Token Operator: default exception");
            }
        }

        protected override string AllowedChars => "+-*/%^=";
    }
}