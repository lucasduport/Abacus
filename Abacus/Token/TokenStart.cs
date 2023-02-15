namespace Ref.Token
{
    public class TokenStart : Token
    {
        public TokenStart(string c)
            : base(c)
        {
        }

        protected override string AllowedChars => "(";
        
    }
}
