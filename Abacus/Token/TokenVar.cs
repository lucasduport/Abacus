using System;
using System.Data;

namespace Ref.Token
{
    public class TokenVar : TokenOperand
    {
        private static readonly string allnumbers = "0123456789";

        public TokenVar(string c):base(c)
        { 
            if (allnumbers.Contains(c[0])) throw new FormatException("Lexer: variable name must not begin by a number");
        }

        protected override string AllowedChars => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_";
        
    }
}
