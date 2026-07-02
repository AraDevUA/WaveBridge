using API.Authorization;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Shared.Authorization;
using System.Security.Claims;
using Xunit;

namespace API.Tests.Authorization;

public class PermissionAuthorizationTests
{
    [Fact]
    public async Task PolicyProvider_PermissionPolicy_BuildsAuthenticatedPolicy()
    {
        // Arrange
        var provider = new PermissionAuthorizationPolicyProvider(
            Options.Create(new AuthorizationOptions()));

        // Act
        var policy = await provider.GetPolicyAsync(PermissionPolicies.Build(PermissionNames.Users.Read));

        // Assert
        policy.Should().NotBeNull();
        var requirement = policy!.Requirements
            .OfType<PermissionAuthorizationRequirement>()
            .Single();
        requirement.Permission.Should().Be(PermissionNames.Users.Read);
    }

    [Fact]
    public async Task Handler_UserHasPermissionClaim_Succeeds()
    {
        // Arrange
        var requirement = new PermissionAuthorizationRequirement(PermissionNames.Users.Delete);
        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(CustomClaimTypes.Permission, PermissionNames.Users.Delete)
        ], "Bearer"));
        var context = new AuthorizationHandlerContext([requirement], user, null);
        var handler = new PermissionAuthorizationHandler();

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task Handler_UserLacksPermissionClaim_Fails()
    {
        // Arrange
        var requirement = new PermissionAuthorizationRequirement(PermissionNames.Users.Delete);
        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(CustomClaimTypes.Permission, PermissionNames.Users.Read)
        ], "Bearer"));
        var context = new AuthorizationHandlerContext([requirement], user, null);
        var handler = new PermissionAuthorizationHandler();

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }
}
