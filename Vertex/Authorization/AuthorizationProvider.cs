﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using FirebirdSql.Data.FirebirdClient;

namespace Api.Authorization
{
    public sealed class AuthorizationProvider : OpenIdConnectServerProvider
    {
        public override Task ValidateTokenRequest(ValidateTokenRequestContext context)
        {
            // Reject token requests that don't use grant_type=password or grant_type=refresh_token.
            if (!context.Request.IsPasswordGrantType() && !context.Request.IsRefreshTokenGrantType())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    description: "Only grant_type=password and refresh_token " +
                                "requests are accepted by this server.");

                return Task.FromResult(0);
            }

            context.Skip();

            return Task.FromResult(0);
        }

        public override async Task HandleTokenRequest(HandleTokenRequestContext context)
        {
            if (context.Request.IsPasswordGrantType())
            {
                var Config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
                var conString = Config.GetSection("ConnectionString").GetValue<string>("dbCon");

                //if (user == null)
                //    {
                //        InvalidGrant(context);
                //        return;
                //    }

                using(FbConnection mycon = new FbConnection(conString))
                {
                    try
                    {
                        mycon.Open();
                    }
                    catch (Exception)
                    {

                        throw new ArgumentException("Wrong password or login");
                    }
                   
                }
                   

                    var identity = new ClaimsIdentity(
                        context.Options.AuthenticationScheme,
                        OpenIdConnectConstants.Claims.Name,
                        OpenIdConnectConstants.Claims.Role);

                    // Add the mandatory subject/user identifier claim.
                    identity.AddClaim(OpenIdConnectConstants.Claims.Subject, conString);
                    
                    var ticket = new AuthenticationTicket(
                        new ClaimsPrincipal(identity),
                        new AuthenticationProperties(),
                        context.Options.AuthenticationScheme);

                    // Call SetScopes with the list of scopes you want to grant
                    // (specify offline_access to issue a refresh token).
                    ticket.SetScopes(
                        OpenIdConnectConstants.Scopes.OfflineAccess,
                        OpenIdConnectConstants.Scopes.OpenId);

                    context.Validate(ticket);
                }
            }
        

        // public override async Task ValidateLogoutRequest(ValidateLogoutRequestContext context)
        // {
        //     // var database = context.HttpContext.RequestServices.GetRequiredService<ApplicationContext>();

        //     // // When provided, post_logout_redirect_uri must exactly
        //     // // match the address registered by the client application.
        //     // if (!string.IsNullOrEmpty(context.PostLogoutRedirectUri) &&
        //     //     !await database.Applications.AnyAsync(application => application.LogoutRedirectUri == context.PostLogoutRedirectUri))
        //     // {
        //     //     context.Reject(
        //     //         error: OpenIdConnectConstants.Errors.InvalidRequest,
        //     //         description: "The specified 'post_logout_redirect_uri' is invalid.");

        //     //     return;
        //     // }

        //     if (string.IsNullOrEmpty(context.PostLogoutRedirectUri))
        //     {
        //         context.Reject(
        //             error: OpenIdConnectConstants.Errors.InvalidRequest,
        //             description: "The specified 'post_logout_redirect_uri' is invalid.");

        //         return;
        //     }

        //     context.Validate();
        // }

        private void InvalidGrant(BaseValidatingTicketContext context)
        {
            context.Reject(error: OpenIdConnectConstants.Errors.InvalidGrant, description: "Invalid user credentials.");
        }
    }
}