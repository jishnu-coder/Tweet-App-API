using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Threading.Tasks;
using Tweet_App_API.CustomAuthorization;

namespace Tweet_App_APT_TestFixture
{
    class AuthorizationHandlerFisture
    {
        [Test]
        public async Task CanOnlyEditAndDeleteItsResource_With_DiffrentToken() //user accessesing resoure with invalid claim
        {
            //Arrange    
            var requirements = new[] { new ManageUserResourceEdit() };
            var author = "user";
            var user = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new Claim[] {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, author),
                            },
                            "Basic")
                        );
            var resource = new DefaultHttpContext();
            resource.Request.RouteValues["userName"] = "author";
            var context = new AuthorizationHandlerContext(requirements, user, resource);
            var subject = new CanOnlyEditAndDeleteItsResource();

            //Act
            await subject.HandleAsync(context);

            //Assert
            context.HasSucceeded.Should().BeFalse(); //FluentAssertions
        }

        [Test]
        public async Task CanOnlyEditAndDeleteItsResource_Should_Succeeded() //user accessesing resource with valid claim
        {
            //Arrange    
            var requirements = new[] { new ManageUserResourceEdit() };
            var author = "author";
            var user = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new Claim[] {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, author),
                            },
                            "Basic")
                        );
            var resource = new DefaultHttpContext();
            resource.Request.RouteValues["userName"] = "author";
            var context = new AuthorizationHandlerContext(requirements, user, resource);
            var subject = new CanOnlyEditAndDeleteItsResource();

            //Act
            await subject.HandleAsync(context);

            //Assert
            context.HasSucceeded.Should().BeTrue(); //FluentAssertions
        }

        [Test]
        public async Task CanOnlyEditAndDeleteItsResource_Should_Fail() // UNauthenticated user accessesing resource 
        {
            //Arrange    
            var requirements = new[] { new ManageUserResourceEdit() };
            var author = "author";
            var user = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new Claim[] {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, author),
                            },
                            "Basic")
                        );
            var resource = new Document() { };

            var context = new AuthorizationHandlerContext(requirements, user, resource);
            var subject = new CanOnlyEditAndDeleteItsResource();

            //Act
            await subject.HandleAsync(context);

            //Assert
            context.HasSucceeded.Should().BeFalse(); //FluentAssertions
        }

    }
}
