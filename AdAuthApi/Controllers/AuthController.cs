using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.DirectoryServices.Protocols;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly string _ldapServer = "192.168.10.82";
        private readonly string _ldapBaseDn = "CN=Users,DC=abc,DC=xyz";

        [HttpGet("windows-auth")]
        [Authorize]
        public IActionResult Get()
        {
            var userName = User.Identity.Name;
            var userDetails = GetAdUserDetails(userName);
            var token = GenerateJwtToken(userName);
            return Ok(new { Message = "is Authenticated!", Token = token, name = userName, User = userDetails });
        }

        public static string GetAttributeValue(SearchResultEntry entry, string attributeName)
        {
            var attributeValues = entry.Attributes[attributeName];
            if (attributeValues.Count > 0)
            {
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
            var ldapConnection = new LdapConnection(new LdapDirectoryIdentifier(_ldapServer));
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
                    DisplayName = GetAttributeValue(entry, "displayname"),
                    Company = GetAttributeValue(entry, "company"),
                    Department = GetAttributeValue(entry, "department"),
                };

                return userDetails;
            }

            return null;
        }

        private string GenerateJwtToken(string userName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("S8D0tPR2kYE0IM6Qup3INtCVYdfuZbQC");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userName)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = "http://localhost:5000",
                Audience = "http://192.168.37.1:18080",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
