namespace Ref.Token
{
    public class TokenEmpty : Token
    {
        public TokenEmpty(string c)
            : base(c)
        {
        }

        protected override string AllowedChars => " ";
        
    }
}
