using System.Collections.Generic;

namespace Ref.Token
{
    public class TokenOperand : Token
    {
        public static List<TokenOperand> variableList = new ();

        public TokenOperand(string c, string name = "")
            : base(c)
        {
            isVar = false;
            moinsUnaire = false;
            initVar = false;
            if (name != "")
            {
                Name = name;
                isVar = true;
                foreach (var v in variableList)
                {
                    if (v.Name == name)
                    {
                        Value = v.Value;
                        return;
                    }
                    
                }
                variableList.Add(this);
            }
            
        }

        protected override string AllowedChars => "-0123456789.";
        
    }
}
