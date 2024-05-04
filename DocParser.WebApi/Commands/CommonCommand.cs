namespace DocParser.WebApi.Commands;

public abstract class CommonCommand<TArgument>
{
    protected readonly IServiceScopeFactory scopeFactory;

    public CommonCommand(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        
    }
    public abstract Task<CommandResult> Execute(TArgument argument);
}