namespace TestIdentityServer
{
    public record ResetPasswordRequest(string userName, string token, string newPassword);
}
