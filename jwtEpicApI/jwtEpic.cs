using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using RestSharp;

namespace jwtEpicApI
{
    public static class jwtEpic
    {
        [FunctionName("jwtEpic")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string fhirEpic = @"https://fhir.epic.com/interconnect-fhir-oauth/oauth2/token";
            string clientID = @"0d9f82b2-0081-4518-a52c-c019d60a17f0";
            string privateKey =
            @"MIIEpAIBAAKCAQEAx8cjJPSTlFddGl2lrNoVMUtsoOiHT2wrgmcaniROnvKXAQvr
            1mAS/KIFLLuNCdslw10ssfqnVzIMgNbHQ8oGu04cHau+Hf6iYRFOoG9pkG+5u3tL
            2rsErbdlljgHnbPLIvil4U5R9RmvchZfiptIae+hXwV0oysq+oGcd0vHgBoSnKMC
            Zm8FEHrjrpcDxxIEuojqR4B8Rx3sCh8sJwPyILRJfQW650qcTp56AsnWnz33W1Wk
            cDe8Q2YCvPOf3n62hgaVIViyqYQgbimG14WJtbr7zP1iGGsiWPwteBNoTCztqsYs
            fpfsz/YanM/CQ5yiVzfhJcGZPf4QKBIJBcLTuwIDAQABAoIBAQCefpSZSxiShcUt
            l8JLIjHSC/7AofpHvo++Q0JQ2IkvfVLj16thRpOJrpjgZmN8wEpzdkZxZWTBvslA
            piUs2OkCTPPtbkXE0jHNRMfWbM2Zc6os3aLOg7UwhOGpjgYszWmJ8y7XUyWeiKQy
            pSFje9K1iRHDqzZgBW1xlManto0uSRofpNNFJh3MnzWKVwAkrQ6zySz6djSmNSoi
            vhPXbAL5MC1fKF/m77/BDoh4UjL+YVUba+j6/G8xEpmb4POELC8jJimhm484g0Tq
            nCHl/Gf0uYGb6rhKIgEtOvw7QKVd3SC8o4AkSvx3kpJ+pDyh1udBV2H3jdaf4nl4
            v74FTZSBAoGBAOkEmKRuj/HV6u0sdhzLg7Rs+wR9ZL//qYo57sdhEqV0MzIGYYBn
            EgC8EDoHqB6c4ulhXYUx/H7FIpwEYAazwCVUL1eVYcV02qUY1i2gi5/y4UphGuYp
            2aqqPsT5fOa4esTCNN0wLQBdawvIh3E4BpFfVTJccXO6Sr3K+ou8rcwhAoGBANt7
            RZqWmlYboIZ301o8poL0CH4b8X62OkqN7PJZpI7AyzqpMuGIs4w3cnaUevLN9Oa9
            unTSTFMKoKldd3ezSdS4iRQhUognB+4Qjom92mHke0QH3TmB+xA7EB5DsmF7a4Cm
            IvoWwVb2e17w2IsnW5t9KlhLAz+ISK3CmftdcsRbAoGAXajRSdZ+2S+om+gxGWXb
            7FuZkbZtFpdGqB6dEVq9Se+o3ESgUCIdpFnzE8AcHSHmOvQ9yNnMqY/HV3qStl7t
            rpNl7AhJIjrT32RaQkCznnjbgTACxdywdt6zGC1HvN0Ny6Rb50QD8o+aBUR23FUu
            nqP49KMfnuqUPDEn/565JMECgYBIe1ZKLhA7/ZWusqW2uC7ZMlQXqYzAJtfrRSGK
            C3afuiWjcrsd5jI9TRZc+L39r2yAQwnviH+yMOFL7VUFz8zDFkoWvsZttk4VHZyk
            +nDDQVw/5ET3t+g8vPSeugaP0N14t+T3KEqjOHUXrnwkwFOtbtFsJSGmepBd6dVG
            QN8k3wKBgQCbTT9khUa+uuUKwvpWFLUteOL3reogRjDl4Hga0A7YH+Z74GyBKx4r
            Vn97l5kgyujRNfqCfIWCCBdzfdab21j/QcAe1WSm5Fx+2k3J5GCZkFcWHbdBrKV8
            0pjT1kR0bat7qm55mvmxM+AcwKAEVQG067lremZhD/6MgQWdFrOdig==";
            string publicKey =
            @"-----BEGIN PUBLIC KEY-----
            MIIDGzCCAgOgAwIBAgIUPwcIWYaW7ChUditmCAFrxMgsmz4wDQYJKoZIhvcNAQEL
            BQAwHTEbMBkGA1UEAwwScHJhY3RpdGlvbmVyZmFjaW5nMB4XDTIxMDkwODIyMzQy
            NFoXDTIxMTAwODIyMzQyNFowHTEbMBkGA1UEAwwScHJhY3RpdGlvbmVyZmFjaW5n
            MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAx8cjJPSTlFddGl2lrNoV
            MUtsoOiHT2wrgmcaniROnvKXAQvr1mAS/KIFLLuNCdslw10ssfqnVzIMgNbHQ8oG
            u04cHau+Hf6iYRFOoG9pkG+5u3tL2rsErbdlljgHnbPLIvil4U5R9RmvchZfiptI
            ae+hXwV0oysq+oGcd0vHgBoSnKMCZm8FEHrjrpcDxxIEuojqR4B8Rx3sCh8sJwPy
            ILRJfQW650qcTp56AsnWnz33W1WkcDe8Q2YCvPOf3n62hgaVIViyqYQgbimG14WJ
            tbr7zP1iGGsiWPwteBNoTCztqsYsfpfsz/YanM/CQ5yiVzfhJcGZPf4QKBIJBcLT
            uwIDAQABo1MwUTAdBgNVHQ4EFgQU6SaiZaolcceS0ggX+SxbxM7eOYQwHwYDVR0j
            BBgwFoAU6SaiZaolcceS0ggX+SxbxM7eOYQwDwYDVR0TAQH/BAUwAwEB/zANBgkq
            hkiG9w0BAQsFAAOCAQEAuVByy8TJWdHarDjR9F1Nl2gPlDJqEJ5lASdWm769gfNT
            2sE+6MT1as4sA7+2AgrcbxE9+6WkjIvtKAhIREouUm7cocZCANWHXW0Q4SGK40ps
            VQV9yT94jr3fOhDCBNSb+qNTxhkp4rEj164mBepH39oIcdGCzJF3Reufn3v9EPpH
            brFxg5cVsTM07C4hpHGq8cWagP/Fr6BxFzTKzvQBvYUJgwN9MoclflT3MsKeFzd3
            RLtBqqWKIg5v9nlGoTgbHb3zuUcDENt9GEALu5lgWDxGwAdyMHLXZE35Afe2q/CG
            6l6pH+mYC7ZYTmY0dio2kF1DzzWYj/v8ElvcwsyFhw==
            -----END PUBLIC KEY-----";
            byte[] privateKeyRaw = Convert.FromBase64String(privateKey);
            // creating the RSA key 
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.ImportRSAPrivateKey(new ReadOnlySpan<byte>(privateKeyRaw), out _);
            RsaSecurityKey rsaSecurityKey = new RsaSecurityKey(provider);


            // Generating the token 
            var now = DateTime.UtcNow;

            var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Iss, clientID),
                    new Claim(JwtRegisteredClaimNames.Sub, clientID),
                    new Claim(JwtRegisteredClaimNames.Aud, fhirEpic),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
            var handler = new JwtSecurityTokenHandler();

            var token = new JwtSecurityToken
            (
                clientID,
                fhirEpic,
                claims,
                now.AddMilliseconds(-30),
                now.AddMinutes(4),
                new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha384)
            );

            var client = new RestClient(fhirEpic);
            var request = new RestRequest(Method.POST);
            //request.AddHeader("", "");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            //request.AddHeader("Cookie", "ASP.NET_SessionId=rpnbwymznnwxjmducdtkftwb; EpicPersistenceCookie=!CVjmNpMfs+7iqZXkywGuayYXr7azRZFIC45x2nB7A1I8FIYcjot1D78fYlfr0WEePSyLNA8Fb1YhXjE=");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
            request.AddParameter("client_assertion", handler.WriteToken(token));
            IRestResponse response = client.Execute(request);

            return new OkObjectResult(response.Content);
        }
    }
}
