using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.DirectoryServices.Protocols;

namespace AdAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly string _ldapServer = "ip?";
        private readonly int _ldapPort = 389;
        private readonly string _ldapBaseDn = "abc.xyz";

        [HttpGet("windows-auth")]
        [Authorize]
        public IActionResult Get()
        {
            var userName = User.Identity.Name;
            var userDetails = GetAdUserDetails(userName);
            return Ok(new { Message = "Authenticated", User = userDetails });
        }

        private object GetAdUserDetails(string userName)
        {
            var ldapConnection = new LdapConnection(new LdapDirectoryIdentifier(_ldapServer, _ldapPort));
            ldapConnection.AuthType = AuthType.Negotiate;
            ldapConnection.Bind();

            var searchFilter = $"(sAMAccountName={userName.Split('\\')[1]})";
            var searchRequest = new SearchRequest(_ldapBaseDn, searchFilter, SearchScope.Subtree, null);
            var searchResponse = (SearchResponse)ldapConnection.SendRequest(searchRequest);

            if (searchResponse.Entries.Count == 1)
            {
                var entry = searchResponse.Entries[0];
                var userDetails = new
                {
                    DisplayName = entry.Attributes["displayName"]?[0].ToString(),
                    // Other attributes...
                };

                return userDetails;
            }

            return null;
        }
    }
}
