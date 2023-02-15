using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.VisualBasic.CompilerServices;

namespace Ref.Token
{
    public class Lexer
    {
        private static readonly Tuple<Type, string>[] AllowedChars =
        {
            new (typeof(TokenEmpty), " "),
            new (typeof(TokenEnd), ")"),
            new (typeof(TokenStart), "("),
            new (typeof(TokenOperator), "+-*/%^="),
            new (typeof(TokenFun), "sqrtminmaxfactisprimefibogcd"),
            new (typeof(TokenChar), ",;"),
            new (typeof(TokenVar), "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_"),
            new (typeof(TokenOperand), "0123456789.")
        };

        private static readonly string[] allowedFun =
        {
            "sqrt",
            "min",
            "max",
            "facto",
            "isprime",
            "fibo",
            "gcd"
        };

        private static readonly string var = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_";
        private static readonly string allnumbers = "0123456789";


        public static List<Token> Lex(string s)
        {
            List<Token> res = new List<Token>();
            Token instance;
            Type type = typeof(TokenEmpty);
            string letters = "";
            string numbers = "";
            int i = 0;
            while (i < s.Length)
            {
                bool here = false;
                foreach (var tuple in AllowedChars)
                {
                    if (tuple.Item2.Contains(s[i]))
                    {
                        type = tuple.Item1;
                        here = true;
                    }
                }

                if (!here) throw new FormatException();

                if (var.Contains(s[i]))
                {
                    while (i < s.Length && allnumbers.Contains(s[i]))
                    {
                        numbers += s[i];
                        i++;
                    }

                    while (i < s.Length && var.Contains(s[i]))
                    {
                        letters += s[i];
                        i++;
                    }
                }

                if (letters == "")
                {
                    if (numbers != "")
                    {
                        res.Add(new TokenOperand(numbers));
                        numbers = "";
                        i--;
                    }
                    else
                    {
                        instance = (Token) Activator.CreateInstance(type, s[i].ToString());
                        res.Add(instance);
                    }
                }
                else
                {
                    bool find = false;
                    foreach (var f in allowedFun)
                    {
                        if (numbers+letters == f) find = true;
                    }

                    if (find)
                    {
                        res.Add(new TokenFun(letters));
                    }
                    else
                    {
                        res.Add(new TokenVar(letters));
                    }

                    letters = "";
                    numbers = "";
                    i--;
                }

                i++;
            }

            return res;
        }

        public static List<Token> ShuttingYard(List<Token> lexing)
        {
            Token a;
            var operatorStack = new Stack<Token>();
            var outputQueue = new Queue<Token>();
            int index = 0;
            while (index < lexing.Count)
            {
                Token token = lexing[index];
                switch (token)
                {
                    case (TokenOperand):
                        outputQueue.Enqueue(token);
                        break;
                    case (TokenFun):
                        operatorStack.Push(token);
                        break;
                    case (TokenChar):
                        if (token.Value == ",")
                        {
                            if (operatorStack.Count == 0) throw new SyntaxErrorException("Unbalanced parentheses");
                            while (operatorStack.Count > 0 && operatorStack.First() is not TokenStart)
                            {
                                if (operatorStack.Count == 0) throw new SyntaxErrorException("Unbalanced parentheses");
                                outputQueue.Enqueue(operatorStack.Pop());
                            }
                        }
                        else
                        {
                            while (operatorStack.Count > 0)
                            {
                                outputQueue.Enqueue(operatorStack.Pop());
                            }
                        }

                        break;
                    case (TokenOperator):
                        if (token.Value == "-" && (index==0 || (lexing[index-1] is not TokenOperand) &&  (lexing[index-1] is not TokenEnd)))
                        {
                            int j = index;
                            j++;
                            while (j<lexing.Count && lexing[j] is not TokenOperand && lexing[j] is not TokenFun && lexing[j] is not TokenVar)
                            {
                                j++;
                            }

                            if (j < lexing.Count) lexing[j].moinsUnaire = !lexing[j].moinsUnaire;
                            else
                            {
                                throw new SyntaxErrorException("Syntax Error");
                            }
                        }
                        else
                        {

                            if (operatorStack.Count > 0)
                            {
                                Token op2 = operatorStack.First();
                                while (operatorStack.Count > 0 && op2 is TokenOperator &&
                                       (token.AsociatifG && token.Precedence <= op2.Precedence ||
                                        !token.AsociatifG && token.Precedence < op2.Precedence))
                                {
                                    outputQueue.Enqueue(operatorStack.Pop());
                                    if (operatorStack.Count > 0) op2 = operatorStack.First();
                                }
                            }

                            operatorStack.Push(token);
                        }
                        break;
                    case (TokenStart):
                        operatorStack.Push(token);
                        if (index + 1 < lexing.Count && lexing[index + 1] is TokenEnd)
                        {
                            throw new SyntaxErrorException("Paranthèses vides");
                        }
                        break;
                    case (TokenEnd):
                        do
                        {
                            if (operatorStack.Count == 0) throw new SyntaxErrorException("Unbalanced parentheses");
                            a = operatorStack.Pop();
                            if (a is not TokenStart) outputQueue.Enqueue(a);
                        } while (a is not TokenStart);

                        if (operatorStack.Count > 0)
                        {
                            if (operatorStack.Count > 0 && operatorStack.First() is TokenFun)
                                do
                                {
                                    a = operatorStack.Pop();
                                    outputQueue.Enqueue(a);
                                } while (operatorStack.Count > 0 && a is not TokenFun);
                        }

                        break;
                }

                index++;
            }

            while (operatorStack.Count > 0)
            {
                a = operatorStack.Pop();
                outputQueue.Enqueue(a);
                if (a is TokenStart) throw new SyntaxErrorException("Unbalanced parentheses");
            }
            
            return outputQueue.ToArray().ToList();
        }
    }
}