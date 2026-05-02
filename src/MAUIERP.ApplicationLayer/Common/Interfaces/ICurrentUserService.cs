namespace MAUIERP.ApplicationLayer.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? UserName { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
        IList<string> Roles { get; }
        bool HasPermission(string permissionCode);
    }
}
