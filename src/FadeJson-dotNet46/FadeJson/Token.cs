using System;

namespace FadeJson
{
    public enum TokenType
    {
        IntegerType,
        StringType,
        SyntaxType,
        BoolType,
        DoubleType
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
                    case TokenType.IntegerType:
                        return Convert.ToInt32(Value);

                    case TokenType.StringType:
                    case TokenType.SyntaxType:
                        return Value;

                    case TokenType.BoolType:
                        return Value == "true";

                    case TokenType.DoubleType:
                        return double.Parse(Value);

                    default:
                        throw new InvalidOperationException(
                            $"UnkownTypeToken.LineNumber:{LineNumber}.LinePosition{LinePosition}");
                }
            }
        }
    }
}