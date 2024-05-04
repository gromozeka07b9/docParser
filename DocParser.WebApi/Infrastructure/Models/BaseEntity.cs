using System.Text.Json;

namespace DocParser.WebApi.Infrastructure.Models;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public string UserIdentity { get; set; } 
    public string Identity { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}