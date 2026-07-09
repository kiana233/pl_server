namespace PlServer.Application;

public interface ILoginRequestCandidateParser
{
    string ParserName { get; }

    bool CanParse(ActionHandlerContext context);

    LoginRequestParseResult Parse(ActionHandlerContext context);
}
