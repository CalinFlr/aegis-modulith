namespace Aegis.Template.IntegrationTests.Authentication;

public static class TestUsers
{
    public static TestUser Reader { get; } = new(
        "reader-user",
        "Reader User",
        ["reader"],
        ["work-items:read"]);

    public static TestUser Admin { get; } = new(
        "admin-user",
        "Admin User",
        ["admin", "reader"],
        ["work-items:read", "work-items:write"]);
}
