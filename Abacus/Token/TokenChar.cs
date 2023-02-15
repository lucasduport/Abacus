namespace Ref.Token
{
    public class TokenChar : Token
    {
        public TokenChar(string c)
            : base(c)
        {
        }

        protected override string AllowedChars => ",;";
        
    }
}
