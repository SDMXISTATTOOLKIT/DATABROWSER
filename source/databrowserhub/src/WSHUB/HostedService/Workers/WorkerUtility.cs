using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DataBrowser.AC.Utility;
using DataBrowser.Domain.Serialization;
using DataBrowser.Interfaces.Dto.Users;
using Newtonsoft.Json;
using WSHUB.Models.Request;

namespace WSHUB.HostedService.Workers
{
    public static class WorkerUtility
    {
        public static TimeSpan CalcolateStart(string startTime)
        {
            var dateNow = DateTime.Now;
            var startTimeParse = DateTime.ParseExact(startTime, "HH:mm:ss", CultureInfo.InvariantCulture);
            var startDate = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, startTimeParse.Hour,
                startTimeParse.Minute, startTimeParse.Second);
            var startWork = startDate.Subtract(dateNow);
            if (startWork.TotalMinutes <= 0) startWork = startDate.AddDays(1).Subtract(dateNow);

            return startWork;
        }

        public static TimeSpan CalcolatePeriod(string repeatTime)
        {
            if (string.IsNullOrWhiteSpace(repeatTime)) return TimeSpan.FromDays(1);

            switch (repeatTime.ToUpperInvariant())
            {
                case "H":
                    return TimeSpan.FromHours(1);
                case "D":
                    return TimeSpan.FromDays(1);
                case "W":
                    return TimeSpan.FromDays(7);
            }

            return TimeSpan.FromDays(1);
        }

        public static async Task<string> GetTokenForServiceUserAsync(string baseUri, string passwordEncrypted)
        {
            var fixedUrl = baseUri.TrimEnd('\\').TrimEnd('/');
            using (var client = new HttpClient())
            {
                var tokenRequest = new TokenRequest();
                tokenRequest.Email = "UserService";
                var password =
                    CryptUtility.SimpleDecryptWithPassword(passwordEncrypted, CryptUtility.PasswordUserService);
                tokenRequest.Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
                using (var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(DataBrowserJsonSerializer.SerializeObject(tokenRequest), Encoding.UTF8,
                        "application/json"),
                    RequestUri = new Uri($"{fixedUrl}/Auth/Token")
                })
                {
                    using (var response = await client.SendAsync(httpRequestMessage))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var userToken =
                                JsonConvert.DeserializeObject<UserAuthenticatedResult>(
                                    await response.Content.ReadAsStringAsync());
                            return userToken.Token;
                        }

                        return null;
                    }
                }
            }
        }
    }
}