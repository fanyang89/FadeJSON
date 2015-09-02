using System;

namespace FadeJson2
{
    public enum TokenType
    {
        IntegerType,
        StringType,
        SyntaxType,
        BoolType
    }

    public struct Token
    {
        public string Value { get; }
        public TokenType TokenType { get; }

        public Token(string value, TokenType tokenType) {
            TokenType = tokenType;
            Value = value;
        }

        public Token(char value, TokenType tokenType) {
            TokenType = tokenType;
            Value = value.ToString();
        }

        public override string ToString() {
            return Value;
        }

        public dynamic RealValue {
            get {
                switch (TokenType) {
                    case TokenType.IntegerType:
                        return Convert.ToInt32(Value);
                    case TokenType.StringType:
                    case TokenType.SyntaxType:
                        return Value;
                    case TokenType.BoolType:
                        return Value == "true";
                    default:
                        throw new InvalidOperationException("UnkownTypeToken");
                }
            }
        }
    }
}