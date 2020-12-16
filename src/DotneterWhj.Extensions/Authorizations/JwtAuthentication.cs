using DotneterWhj.DataTransferObject;
using DotneterWhj.ToolKits;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace DotneterWhj.Extensions
{
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JwtAuthentication : IAuthenticationFilter
    {
        internal class UserClaims
        {
            public string UserId { get; set; }

            public string Role { get; set; }
        }

        public string Realm { get; set; }
        public bool AllowMultiple { get { return false; } }

        /// <summary>
        /// 鉴权
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var controllerAttributes = context.ActionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true);

            if (controllerAttributes.Count > 0)
            {
                return;
            }

            var actionAttributes = context.ActionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true);

            if (actionAttributes.Count > 0)
            {
                return;
            }

            var request = context.Request;
            var authorization = request.Headers.Authorization;

            // Bearer认证
            if (authorization == null || authorization.Scheme != "Bearer")
            {
                var messageModel = new MessageModel<string>
                {
                    Data = null,
                    Msg = "Missing Bearer Authorization",
                    Status = HttpStatusCode.Unauthorized,
                };

                context.ErrorResult = new AuthenticationFailureResult(request, messageModel);

                return;
            }

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                var messageModel = new MessageModel<string>
                {
                    Data = null,
                    Msg = "Missing token",
                    Status = HttpStatusCode.Unauthorized,
                };

                context.ErrorResult = new AuthenticationFailureResult(request, messageModel);

                return;
            }

            var token = authorization.Parameter;
            var principal = await AuthenticateJwtToken(token);

            if (principal == null)
            {
                var messageModel = new MessageModel<string>
                {
                    Data = null,
                    Msg = "Invalid token",
                    Status = HttpStatusCode.Unauthorized,
                };
                context.ErrorResult = new AuthenticationFailureResult(request, messageModel);
            }
            else
            {
                context.Principal = principal;
                SetPrincipal(principal);
                //context.ActionContext.ControllerContext.RequestContext.Principal = principal;
            }
        }

        private void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }

        private static bool ValidateToken(string token, out UserClaims userClamis)
        {
            userClamis = new UserClaims();

            var simplePrinciple = JwtManager.GetPrincipal(token);
            var identity = simplePrinciple?.Identity as ClaimsIdentity;

            if (identity == null)
                return false;

            if (!identity.IsAuthenticated)
                return false;

            var userIdClaim = identity.FindFirst(ClaimTypes.Name);
            var userRoleClaim = identity.FindFirst(ClaimTypes.Role);

            userClamis.UserId = userIdClaim?.Value;
            userClamis.Role = userRoleClaim?.Value;

            if (string.IsNullOrEmpty(userClamis.UserId))
                return false;

            // More validate to check whether username exists in system

            return true;
        }

        protected Task<IPrincipal> AuthenticateJwtToken(string token)
        {
            if (ValidateToken(token, out UserClaims userClaims))
            {
                // based on username to get more information from database in order to build local identity
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userClaims.UserId),
                    new Claim(ClaimTypes.Role, userClaims.Role)
                    // Add more claims if needed: Roles, ...
                };

                var identity = new ClaimsIdentity(claims, "Jwt");
                IPrincipal user = new ClaimsPrincipal(identity);

                return Task.FromResult(user);
            }

            return Task.FromResult<IPrincipal>(null);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter = null;

            if (!string.IsNullOrEmpty(Realm))
                parameter = "realm=\"" + Realm + "\"";

            context.ChallengeWith("Bearer", parameter);
        }

    }
}