using System;
using System.Collections.Generic;
using System.Linq;

namespace iSukces.Build.InnoSetup;

public sealed partial class FileCommand
{
    internal sealed class Parser
    {
        internal static FileCommand ParseAll(string s)
        {
            var tokens = InnoSetupLineParser.GetTokens(s);
            return PatseTokens(tokens);
        }

        private static FileFlags ParseFileFlags(List<string> args)
        {
            var a = new Dictionary<string, FileFlags>(StringComparer.OrdinalIgnoreCase)
            {
                [FileFlags.IgnoreVersion.ToString()]      = FileFlags.IgnoreVersion,
                [FileFlags.ReplaceSameversion.ToString()] = FileFlags.ReplaceSameversion,
                [FileFlags.DontCopy.ToString()]           = FileFlags.DontCopy,
                [FileFlags.NoEncryption.ToString()]       = FileFlags.NoEncryption
            };

            var r = FileFlags.None;
            foreach (var i in args)
            {
                if (!a.TryGetValue(i, out var f))
                    throw new NotImplementedException(i);
                r |= f;
            }

            return r;
        }

        private static FileCommand PatseTokens(List<string> tokens)
        {
            if (tokens.Count == 0)
                return null;
            var          name   = "";
            var          state  = TokenParsingState.Begin;
            var          result = new FileCommand();
            List<string> args   = new();

            void FlushCommand()
            {
                switch (name)
                {
                    case "Source":
                        result.Source = args.Single();
                        break;
                    case "DestDir":
                        result.DestDir = args.Single();
                        break;
                    case "Flags":
                        result.Flags = ParseFileFlags(args);
                        break;
                    default:
                        throw new NotImplementedException(name);
                }

                args.Clear();
                state = TokenParsingState.Begin;
            }

            foreach (var item in tokens)
            {
                switch (state)
                {
                    case TokenParsingState.Begin:
                        if (item is ":" or ";")
                            return null;
                        state = TokenParsingState.Hasname;
                        name  = item;
                        continue;
                    case TokenParsingState.Hasname:
                        if (item == ":")
                        {
                            state = TokenParsingState.AfterColon;
                            args.Clear();
                            continue;
                        }

                        throw new NotImplementedException();
                    case TokenParsingState.AfterColon:
                        if (item == ";")
                        {
                            FlushCommand();
                            continue;
                        }

                        args.Add(item);
                        continue;
                    default: throw new ArgumentOutOfRangeException();
                }

                /*
                if (state == TokenParsingState.AfterColon)
                {
                    if (item == ";")
                    {
                        FlushCommand();
                        continue;
                    }

                    args.Add(item);
                    continue;
                }

                if (string.IsNullOrEmpty(name))
                {
                    name = item;
                }*/
            }

            switch (state)
            {
                case TokenParsingState.AfterColon:
                    FlushCommand();
                    break;
                case TokenParsingState.Hasname:
                    throw new NotImplementedException();
                case TokenParsingState.Begin:
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        private enum TokenParsingState
        {
            Begin,
            Hasname,
            AfterColon
        }
    }
}
