using System;

namespace FadeJson
{
    public enum TokenType
    {
        Integer,
        String,
        Symbol,
        Bool,
        Double
    }

    public struct Token
    {
        public string Value { get; }
        public TokenType TokenType { get; }
        public int LineNumber { get; }
        public int LinePosition { get; }

        public Token(string value, TokenType tokenType, int lineNumber, int linePosition) {
            TokenType = tokenType;
            Value = value;
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }

        public Token(char value, TokenType tokenType, int lineNumber, int linePosition) {
            TokenType = tokenType;
            Value = value.ToString();
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }

        public override string ToString() {
            return Value;
        }

        public JsonValue RealValue {
            get {
                switch (TokenType) {
                    case TokenType.Integer:
                        return Convert.ToInt32(Value);

                    case TokenType.String:
                    case TokenType.Symbol:
                        return Value;

                    case TokenType.Bool:
                        return Value == "true";

                    case TokenType.Double:
                        return double.Parse(Value);

                    default:
                        throw new InvalidOperationException(
                            $"UnkownTypeToken.LineNumber:{LineNumber}.LinePosition{LinePosition}");
                }
            }
        }
    }
}