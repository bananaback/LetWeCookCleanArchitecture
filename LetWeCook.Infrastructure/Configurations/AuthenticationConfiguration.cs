namespace LetWeCook.Infrastructure.Configurations;

public class AuthenticationConfiguration
{
    public GoogleAuthentication Google { get; set; } = new();
    public FacebookAuthentication Facebook { get; set; } = new();
}

public class GoogleAuthentication
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

public class FacebookAuthentication
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
