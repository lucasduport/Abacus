namespace Ref.Token
{
    public class TokenEnd : Token
    {
        public TokenEnd(string c)
            : base(c)
        {
        }

        protected override string AllowedChars => ")";
        
    }
}
