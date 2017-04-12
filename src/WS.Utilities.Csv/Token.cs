namespace WS.Utilities.Csv
{
    public class Token<TType, TRepresentation>
    {
        public TType TokenType { get; }
        public TRepresentation Representation { get; }

        public Token(TType tokenType, TRepresentation representation)
        {
            TokenType = tokenType;
            Representation = representation;
        }
    }
}
