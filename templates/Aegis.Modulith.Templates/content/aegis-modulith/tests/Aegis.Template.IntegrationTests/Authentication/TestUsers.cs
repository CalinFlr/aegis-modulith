using Aegis.Template.BuildingBlocks.Authorization;

namespace Aegis.Template.IntegrationTests.Authentication;

public static class TestUsers
{
    public static TestUser Reader { get; } = new(
        "reader-user",
        "Reader User",
        ["reader"],
        [AegisPermissions.WorkItemsRead],
        [AegisPermissions.WorkItemsRead]);

    public static TestUser Admin { get; } = new(
        "admin-user",
        "Admin User",
        ["admin", "reader"],
        [AegisPermissions.WorkItemsRead, AegisPermissions.WorkItemsWrite, AegisPermissions.TasksRead, AegisPermissions.TasksWrite, AegisPermissions.OperationsRead, AegisPermissions.AdvancedRead],
        [AegisPermissions.WorkItemsRead, AegisPermissions.WorkItemsWrite, AegisPermissions.TasksRead, AegisPermissions.TasksWrite, AegisPermissions.OperationsRead, AegisPermissions.AdvancedRead]);

    public static TestUser OperationsReader { get; } = new(
        "operations-reader",
        "Operations Reader",
        ["operations-reader"],
        [AegisPermissions.OperationsRead],
        [AegisPermissions.OperationsRead]);

    public static TestUser WithoutPermissions { get; } = new(
        "limited-user",
        "Limited User",
        ["reader"],
        [],
        []);
}
