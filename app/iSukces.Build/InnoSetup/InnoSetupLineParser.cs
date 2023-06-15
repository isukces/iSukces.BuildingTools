using System;
using System.Collections.Generic;
using System.Text;

namespace iSukces.Build.InnoSetup;

public static class InnoSetupLineParser
{
    public static List<string> GetTokens(string s)
    {
        List<string> tokens    = new();
        var          tokenType = TokenParsingState.Begin;
        var          sb        = new StringBuilder();

        void Flush()
        {
            if (sb.Length > 0)
            {
                var trim = sb.ToString().Trim();
                if (trim.Length > 0)
                    tokens.Add(trim);
                sb.Clear();
            }

            tokenType = TokenParsingState.Begin;
        }

        foreach (var i in s)
        {
            if (tokenType == TokenParsingState.Begin)
            {
                if (i is ' ' or '\t')
                    continue;
                if (i == '\"')
                {
                    tokenType = TokenParsingState.QuotedString;
                    continue;
                }

                tokenType = TokenParsingState.String;

                sb.Append(i);
                continue;
            }

            if (tokenType == TokenParsingState.String)
            {
                switch (i)
                {
                    case ':' or ';':
                        Flush();
                        tokens.Add(i.ToString());
                        break;
                    case ' ' or '\t':
                        Flush();
                        break;
                    default:
                        sb.Append(i);
                        break;
                }

                continue;
            }

            if (tokenType == TokenParsingState.QuotedString)
            {
                if (i == '\"')
                    Flush();
                else
                    sb.Append(i);

                continue;
            }

            throw new NotImplementedException();
        }

        switch (tokenType)
        {
            case TokenParsingState.String:
                Flush();
                break;
            case TokenParsingState.Begin: break;
            case TokenParsingState.QuotedString:
                throw new NotImplementedException();
            default: throw new ArgumentOutOfRangeException();
        }

        return tokens;
    }

    private enum TokenParsingState
    {
        Begin,
        String,
        QuotedString
    }
}
