using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Ref.Token;

namespace Abacus
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            //il faut split les list token avec les ; et faire en sorte que quand il ya une égalité tu renvoies la variable
            try
            {
                //--------------------LEXING--------------------------
                List<Token> lexing = Lexer.Lex(Console.ReadLine());
                //args = new[] {"--rpn"};
                //------------------CHECK-EMPTY-----------------------
                bool tousvide = true;
                foreach (var token in lexing)
                {
                    tousvide = tousvide && (token is TokenEmpty || token is TokenChar);
                }

                if (lexing.Count == 0 || tousvide)
                {
                    Console.WriteLine(0);
                    return 0;
                }
                int i = 0;
                while ( i < lexing.Count-1)
                {
                    var token = lexing[i];
                    switch (token)
                    {
                        case TokenOperand:
                            if (token.isVar)
                            {
                                if ((lexing[i + 1] is TokenOperand) || (lexing[i+1] is TokenStart))
                                {
                                    lexing.InsertRange(i+1, new [] {new TokenOperator("*")});
                                }
                            }
                            else
                            {
                                if ((lexing[i + 1] is TokenOperand && lexing[i + 1].isVar) ||
                                    (lexing[i + 1] is TokenStart))
                                {
                                    lexing.InsertRange(i+1, new [] {new TokenOperator("*")});
                                }
                            }
                            break;
                        case TokenEnd:
                            if (lexing[i + 1] is TokenStart || (lexing[i+1] is TokenOperand && lexing[i+1].isVar))
                            {
                                lexing.InsertRange(i+1, new [] {new TokenOperator("*")});
                            }
                            break;
                        case TokenOperator:
                            if (token.Value == "-")
                            {
                                if (lexing[i + 1] is TokenStart || lexing[i + 1] is TokenFun)
                                {
                                    lexing.InsertRange(i + 1, new Token[] {new TokenOperand("1"), new TokenOperator("*")});
                                }
                            }
                            break;

                    }

                    i++;
                }
                List<List<Token>> list_lexing = new List<List<Token>>();
                int ilist = 0;
                list_lexing.Add(new List<Token>());
                foreach (var token in lexing)
                {
                    if (token is TokenChar && token.Value == ";")
                    {
                        list_lexing.Add(new List<Token>());
                        ilist++;
                    }
                    else
                    {
                        list_lexing[ilist].Add(token);
                    }
                }

                //------------------REMOVE-SPACES---------------------
                List<List<Token>> lrsp = new List<List<Token>>();
                lrsp.Add(new List<Token>());
                ilist = 0;
                foreach (var l in list_lexing)
                {
                    foreach (var t in l)
                    {
                        if (t is not TokenEmpty)
                        {
                            if (t is TokenOperand && t is not TokenVar && !Int32.TryParse(t.Value, out int a))
                            {
                                throw new DivideByZeroException();
                            }
                            lrsp[ilist].Add(t);
                        }                       
                    }
                    lrsp.Add(new List<Token>());
                    ilist++;
                }
                lrsp.RemoveRange(lrsp.Count -1,1);

                bool same = true;
                foreach (var l in lrsp)
                {
                    same = same && l == Lexer.ShuttingYard(l);
                }
                if (same) throw new SyntaxErrorException();
                //--------------SWITCH-TO-RPN-IF-NEEDED---------------
                List<List<Token>> lrpn = new List<List<Token>>();
                if (args.Length == 0 || args[0] != "--rpn")
                {
                    foreach (var l in lrsp)
                    {
                        foreach (var t in l)
                        {
                            if (t is TokenOperand) t.moinsUnaire = false;
                        }
                    }
                    
                    
                    foreach (var l in lrsp)
                    {
                        lrpn.Add(Lexer.ShuttingYard(l));
                    }
                }
                else
                {
                    lrpn = lrsp;
                }

                //--------TOKENVAR-TO-TOKENOPERAND-CONVERSION---------
                foreach (var l in lrpn)
                {

                    for (int j = 0; j < l.Count; j++)
                    {
                        if (l[j] is TokenVar)
                        {
                            if (l[j].moinsUnaire)
                            {
                                l[j] = new TokenOperand("0", l[j].Value);
                                l[j].moinsUnaire = true;
                            }
                            else
                            {
                                l[j] = new TokenOperand("0", l[j].Value);
                            }
                        }
                    }
                }

                //-------------------CALCULATION----------------------
                List<int> res = new List<int>();
                foreach (var l in lrpn)
                {
                    res.Add(Calculator.Result(l));
                }
                Console.WriteLine(res.Last());
            }
            //-------------------ERROR-CATCHING-------------------
            catch (ArgumentException)
            {
                return 1;
            }
            catch (DivideByZeroException)
            {
                Console.Error.WriteLine("Invalid operation.");
                return 3;
            }
            catch (SyntaxErrorException)
            {
                Console.Error.WriteLine("Syntax error.");
                return 2;
            }
            catch (InvalidOperationException)
            {
                Console.Error.WriteLine("Syntax error.");
                return 2;
            }
            catch (FormatException)
            {
                Console.Error.WriteLine("Unexpected token.");
                return 2;
            }

            return 0;
        }
    }
}