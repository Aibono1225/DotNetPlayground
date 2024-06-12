using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.DirectoryServices.Protocols;
using System.Text;

namespace AdAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly string _ldapServer = "192.168.10.82";
        private readonly int _ldapPort = 389;
        private readonly string _ldapBaseDn = "CN=Users,DC=abc,DC=xyz";

        [HttpGet("windows-auth")]
        [Authorize]
        public IActionResult Get()
        {
            var userName = User.Identity.Name;
            var userDetails = GetAdUserDetails(userName);
            return Ok(new { Message = "Authenticated", name = userName, User = userDetails });
        }

        public static string GetAttributeValue(SearchResultEntry entry, string attributeName)
        {
            var attributeValues = entry.Attributes[attributeName];
            if (attributeValues.Count > 0)
            {
                //var value = System.Convert.FromBase64String(attributeVal[0].ToString());
                //return System.Text.Encoding.UTF8.GetString(value);
                var attributeValue = attributeValues[0] as byte[];
                if (attributeValue != null)
                {
                    return Encoding.UTF8.GetString(attributeValue);
                }
                return attributeValues[0].ToString();
            }
            return "nope";
        }

        private object GetAdUserDetails(string userName)
        {
            //var ldapConnection = new LdapConnection(new LdapDirectoryIdentifier(_ldapServer, _ldapPort));
            var ldapConnection = new LdapConnection(new LdapDirectoryIdentifier(_ldapServer));
            ldapConnection.AuthType = AuthType.Negotiate;
            ldapConnection.Bind();

            var searchFilter = $"(sAMAccountName={userName.Split('\\')[1]})";
            //var searchFilter = $"(sAMAccountName={userName})";
            var searchRequest = new SearchRequest(_ldapBaseDn, searchFilter, SearchScope.Subtree, null);
            var searchResponse = (SearchResponse)ldapConnection.SendRequest(searchRequest);

            if (searchResponse.Entries.Count == 1)
            {
                var entry = searchResponse.Entries[0];
                var userDetails = new
                {
                    DisplayName = GetAttributeValue(entry, "displayname"),
                    Company = GetAttributeValue(entry, "company"),
                    Department = GetAttributeValue(entry, "department"),
                };

                return userDetails;
            }

            return null;

        }

    }
}
