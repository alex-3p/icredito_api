namespace iCredito.Api.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }

    private User() { }

    public User(string username, string password)
    {
        Id = Guid.NewGuid();
        Username = username;
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password)
        => BCrypt.Net.BCrypt.Verify(password, PasswordHash);
}
