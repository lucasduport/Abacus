using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Abacus;

namespace Ref.Token
{
    public class TokenFun : Token
    {
        public bool secondArg { get;}

        protected override string AllowedChars => "sqrtminmaxfactoisprimefibogcd";
        public TokenFun(string c)
            : base(c)
        {
            moinsUnaire = false;
            switch (c)
            {
                case "sqrt":
                    secondArg = false;
                    break;
                case "min":
                    secondArg = true;
                    break;
                case "max":
                    secondArg = true;
                    break;
                case "facto":
                    secondArg = false;
                    break;
                case "isprime":
                    secondArg = false;
                    break;
                case "fibo":
                    secondArg = false;
                    break;
                case "gcd":
                    secondArg = true;
                    break;
                default:
                    throw new SyntaxErrorException("TokenFun : Invalid function name");
            }
        }


        public static float Function(TokenFun fun,TokenOperand op1 )
        {

            if (fun.secondArg) throw new ArgumentException("TokenFun : missing argument");
            float operand1 = float.Parse(op1.Value);
            if (op1.isVar)
            {
                foreach (var v in TokenOperand.variableList)
                {
                    if (v.Name == op1.Name)
                    { 
                        operand1 = float.Parse(v.Value);
                    }
                }
            }

            if (operand1 < 0) throw new DivideByZeroException();
            
            switch (fun.Value)
            {
                case "sqrt":
                    return (float) Math.Sqrt(operand1);
                case "facto":
                    return Facto((int) operand1);
                case "isprime":
                    return IsPrime((int) operand1) ? 1f : 0f;
                case "fibo":
                    return Fibo((int) operand1);
                default:
                    throw new ArgumentException("TokenFun : Invalid 1 argument function name");
            }
        }
        public static float Function(TokenFun fun,TokenOperand op1, TokenOperand op2)
        {

            if (!fun.secondArg) throw new SyntaxErrorException("TokenFun : too much argument");
            float operand1 = float.Parse(op1.Value);
            float operand2 = float.Parse(op2.Value);
            switch (fun.Value)
            {
                case "min":
                    return Math.Min(operand1,operand2);
                case "max":
                    return Math.Max(operand1,operand2);
                case "gcd":
                    return gcd((int) operand1, (int) operand2);
                default:
                    throw new SyntaxErrorException("TokenFun : Invalid 2 argument function name");
            }
        }
        private static bool IsPrime(int n)
        {
            if (n == 2)
            {
                return true;
            }

            if (n%2 == 0 || n==1)
            {
                return false;
            }

            var sqrt = (Int32) Math.Sqrt(n);
            for (Int64 t = 3; t <= sqrt; t = t + 2)
            {
                if (n%t == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static int Fibo(int n)
        {
            if(n == 1 || n==0) return n;
            return Fibo(n - 1) + Fibo(n - 2);
        }

        private static float Facto(int number)
        {
            int res = 1;

            for (int i = 1; i <= number; i++)
            {
                res *= i;
            }

            return (float) res;
        }
        private static float gcd(int a, int b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a | b;
        }
    }
}
