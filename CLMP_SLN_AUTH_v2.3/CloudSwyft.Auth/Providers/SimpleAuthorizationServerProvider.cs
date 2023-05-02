using CloudSwyft.Auth.Models;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace CloudSwyft.Auth.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public string AuthContext = System.Configuration.ConfigurationManager.ConnectionStrings["AuthContext"].ConnectionString;

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            context.TryGetFormCredentials(out clientId, out clientSecret);
            //string ltiClientKey = System.Configuration.ConfigurationManager.AppSettings["LtiClientKey"];

            //if (clientId == ltiClientKey)
            //{
            //    context.Validated(clientId);
            //}

            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            ApplicationUser user = new ApplicationUser();
            using (AuthRepository _repo = new AuthRepository())
            {
                //user = await _repo.FindUser(context.UserName, context.Password);
                user = _repo.FindUser(context.UserName, context.Password);
                //if (user == null || user.isDisabled == true || user.isDeleted == true)
                if (user == null)
                {
                    context.SetError("invalid_grant", "Invalid Username or Password.");
                    return;
                }
                else if (user.EmailConfirmed == false)
                {
                    context.SetError("invalid_grant", "Email Address is not verified.");
                    return;
                }
                else if (user.isDisabled == true)
                {
                    context.SetError("invalid_grant", "User is disabled. Please contact CS support.");
                    return;
                }
            }

            if (user.Thumbnail == null)
                user.Thumbnail = "";
            TextInfo info = CultureInfo.CurrentCulture.TextInfo;

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.GivenName, info.ToTitleCase(user.LastName) + "," + info.ToTitleCase(user.FirstName)));

            identity.AddClaim(new Claim(ClaimTypes.Name, user.FirstName));
            //identity.AddClaim(new Claim(ClaimTypes.Role, user.Roles.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim("Thumbnail", user.Thumbnail));
            identity.AddClaim(new Claim("UserGroup", user.UserGroup.ToString()));
            identity.AddClaim(new Claim("Id", user.Id.ToString()));

            //identity.AddClaim(new Claim("IsDeleted", user.isDeleted.ToString()));
            //identity.AddClaim(new Claim("IsDisabled", user.isDisabled.ToString()));

            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                        "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    },
                    {
                        "userName", user.UserName
                    }
                });
            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);
            context.Validated(identity);

        }

        public override async Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            var data = await context.Request.ReadFormAsync();
            var email = data.Where(x => x.Key == "email").Select(x => x.Value).FirstOrDefault();
            var userIdLTI = data.Where(x => x.Key == "userIdLTI").Select(x => x.Value).FirstOrDefault();
            var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            //var ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties());

            ApplicationUser user = new ApplicationUser();
            using (AuthRepository _repo = new AuthRepository())
            {
                if (email[0] != "")
                    user = _repo.FindUser(email[0]);
                else
                {
                    using (SqlConnection _db = new SqlConnection(AuthContext))
                    {
                        using (SqlCommand command = new SqlCommand("Select * FROM CloudLabUsers WHERE UserIdLTI = '" + userIdLTI[0] + "'", _db))
                        {
                            _db.Open();
                            //Int32 iterator = (Int32)command.ExecuteScalar();

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    user.Email = reader["Email"].ToString();
                                    user.EmailConfirmed = Boolean.Parse(reader["EmailConfirmed"].ToString());
                                    user.FirstName = reader["FirstName"].ToString();
                                    user.Id = reader["Id"].ToString();
                                    user.LastName = reader["LastName"].ToString();
                                    user.TenantId = Int32.Parse(reader["TenantId"].ToString());
                                    user.UserGroup = Int32.Parse(reader["UserGroup"].ToString());
                                    user.UserId = Int32.Parse(reader["UserId"].ToString());
                                    user.UserIdLTI = reader["UserIdLTI"].ToString();
                                    user.UserName = reader["UserName"].ToString();
                                }
                            }
                        }

                    }
                }
                //if (user == null || user.isDisabled == true || user.isDeleted == true)
                if (user == null)
                {
                    context.SetError("invalid_grant", "Invalid Username or Password.");
                    return;
                }
                else if (user.EmailConfirmed == false)
                {
                    context.SetError("invalid_grant", "Email Address is not verified.");
                    return;
                }

                //user = _repo.FindUser(email[0]);
                //if (user == null)
                //{
                //    context.SetError("invalid_grant", "Invalid Username or Password.");
                //    return;
                //}
                //else if (user.EmailConfirmed == false)
                //{
                //    context.SetError("invalid_grant", "Email Address is not verified.");
                //    return;
                //}
            }

            if (user.Thumbnail == null)
                user.Thumbnail = "";
            TextInfo info = CultureInfo.CurrentCulture.TextInfo;

            if (user.UserIdLTI == null)
                user.UserIdLTI = "";

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.GivenName, info.ToTitleCase(user.LastName) + "," + info.ToTitleCase(user.FirstName)));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.FirstName));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Roles.Select(x => x.RoleId).ToString()));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim("Thumbnail", user.Thumbnail));
            identity.AddClaim(new Claim("UserGroup", user.UserGroup.ToString()));
            identity.AddClaim(new Claim("Id", user.Id.ToString()));
            identity.AddClaim(new Claim("UserIdLTI", user.UserIdLTI));

            //identity.AddClaim(new Claim("IsDeleted", user.isDeleted.ToString()));
            //identity.AddClaim(new Claim("IsDisabled", user.isDisabled.ToString()));

            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                        "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    },
                    {
                        "userName", user.UserName
                    }
                });
            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);
            context.Validated(identity);
            context.Validated();
        }
    }
}