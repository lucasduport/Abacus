using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

namespace Ref.Token
{
    public abstract class Token
    {
        public string Value { get; set; }

        protected abstract string AllowedChars { get; }
        public int Precedence { get; set; }
        public bool AsociatifG { get; set; }
        
        
        public bool isVar { get; set; }
        
        public string Name { get; set; }
        public bool moinsUnaire { get; set; }
        public bool initVar { get; set; }

        protected Token(string s)
        {
            bool allAllowed = true;
            foreach (var c in s)
            {
                if (!AllowedChars.Contains(c)) allAllowed = false;
            }
            
            Value = allAllowed? s : throw new FormatException("Lexing: invalid string for token type" + GetType());
        }
        
        
    }
}
