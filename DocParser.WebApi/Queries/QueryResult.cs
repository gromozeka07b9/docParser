namespace DocParser.WebApi.Queries;

public class QueryResult<TData>
{
    public QueryResult(TData data)
    {
        this.Data = data;
        this.Error = string.Empty;
    }    
    
    public TData? Data { get; }

    public string Error { get; set; }

    public bool Succeeded => string.IsNullOrEmpty(Error);

}