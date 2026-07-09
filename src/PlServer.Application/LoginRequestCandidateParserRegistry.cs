namespace PlServer.Application;

public sealed class LoginRequestCandidateParserRegistry
{
    private readonly List<ILoginRequestCandidateParser> parsers = new();

    public LoginRequestCandidateParserRegistry(IEnumerable<ILoginRequestCandidateParser>? parsers = null)
    {
        if (parsers is not null)
        {
            foreach (var parser in parsers)
            {
                Register(parser);
            }
        }
    }

    public IReadOnlyList<ILoginRequestCandidateParser> Parsers => parsers.ToArray();

    public static LoginRequestCandidateParserRegistry CreateDefault()
    {
        return new LoginRequestCandidateParserRegistry(new[]
        {
            new OpaqueLoginRequestCandidateParser()
        });
    }

    public void Register(ILoginRequestCandidateParser parser)
    {
        ArgumentNullException.ThrowIfNull(parser);

        if (parsers.Any(existing => existing.ParserName == parser.ParserName))
        {
            throw new InvalidOperationException($"Duplicate login request parser: {parser.ParserName}");
        }

        parsers.Add(parser);
    }

    public LoginRequestParseResult Parse(ActionHandlerContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var parser = parsers.FirstOrDefault(candidate => candidate.CanParse(context))
            ?? new OpaqueLoginRequestCandidateParser();

        return parser.Parse(context);
    }
}
