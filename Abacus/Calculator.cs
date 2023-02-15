using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using Ref.Token;

namespace Abacus
{
    public class Calculator
    {
        public static int Result(List<Token> rpn)
        {
            Stack<Token> stack = new Stack<Token>();
            TokenOperand f2;
            TokenOperand f1;
            TokenOperator t;
            TokenFun f;
            foreach (var token in rpn)
            {
                if (token is not TokenOperator && token is not TokenFun)
                {
                    if (token is TokenEmpty) continue;
                    if (token is TokenOperand && token.moinsUnaire)
                    {
                        if (token.isVar)
                        {
                            foreach (var v in TokenOperand.variableList)
                            {
                                if (v.Name == token.Name)
                                {
                                    token.Value = v.Value;
                                }
                            }
                        }
                        else
                        {
                            token.Value = (float.Parse(token.Value) * -1).ToString();
                        }
                        
                    }

                    stack.Push(token);
                }
                else
                {
                    switch (token)
                    {
                        case (TokenOperator):
                            t = new TokenOperator(token.Value);
                            f2 = stack.First().isVar
                                ? new TokenOperand(stack.First().Value, stack.First().Name)
                                : new TokenOperand(stack.First().Value);
                            stack.Pop();

                            f1 = stack.First().isVar
                                ? new TokenOperand(stack.First().Value, stack.First().Name)
                                : new TokenOperand(stack.First().Value);
                            stack.Pop();
                            if (token.Value == "=")
                            {
                                stack.Push(new TokenOperand(TokenOperator.Operation(t, f1, f2).ToString(), f2.Name));
                            }
                            else
                            {
                                stack.Push(new TokenOperand(TokenOperator.Operation(t, f1, f2).ToString()));
                            }

                            break;
                        case (TokenFun):
                            f = new TokenFun(token.Value);
                            if (!f.secondArg)
                            {
                                f1 = stack.First().isVar
                                    ? new TokenOperand(stack.First().Value, stack.First().Name)
                                    : new TokenOperand(stack.First().Value);
                                stack.Pop();
                                stack.Push(new TokenOperand(token.moinsUnaire
                                    ? (TokenFun.Function(f, f1) * -1).ToString()
                                    : TokenFun.Function(f, f1).ToString()));
                            }
                            else
                            {
                                f2 = stack.First().isVar
                                    ? new TokenOperand(stack.First().Value, stack.First().Name)
                                    : new TokenOperand(stack.First().Value);
                                stack.Pop();
                                f1 = stack.First().isVar
                                    ? new TokenOperand(stack.First().Value, stack.First().Name)
                                    : new TokenOperand(stack.First().Value);
                                stack.Pop();
                                stack.Push(new TokenOperand(token.moinsUnaire
                                    ? (TokenFun.Function(f, f1, f2) * -1).ToString()
                                    : TokenFun.Function(f, f1, f2).ToString()));
                            }

                            break;
                    }
                }
            }

            if (stack.Count !=1)
                throw new SyntaxErrorException("Calculator.cs : stack empty or unempty at the end of calculation");
            
            if (stack.First().isVar)
            {
                foreach (var vr in TokenOperand.variableList)
                {
                    if (vr.Name == stack.First().Name)
                    {
                        if (stack.First().moinsUnaire)
                        {
                            return (int) -float.Parse(vr.Value);
                        }

                        return (int) float.Parse(vr.Value);
                    }
                }
            }

            return (int) float.Parse(stack.Pop().Value);
        }
    }
}