using System;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace FreeDnsCore
{
    public class FreeDns
    {
        private XmlSerializer _responseSerializer;
        private string _apiKey;

        public FreeDns(string apiKey)
        {
            _responseSerializer = new XmlSerializer(typeof(xml));
            _apiKey = apiKey;
        }

        /// <summary>
        /// Uses the current Api Key to get all the host addresses.
        /// This will throw all sorts of nifty exceptions because it makes WebRequests.  You'd do well to catch them and consider retrying.
        /// </summary>
        /// <param name="timeout">The amount of time to wait for the web request to be fulfilled. (-1millis to wait forever.)</param>
        /// <exception cref="TimeoutException">If timeout expires.</exception>
        /// <returns></returns>
        public xml GetRecords(TimeSpan timeout)
        {
            // https://freedns.afraid.org/api/?action=getdyndns&v=2&sha=[]&style=xml

            string uri = string.Format(@"https://freedns.afraid.org/api/?action=getdyndns&v=2&sha={0}&style=xml", _apiKey);
            var response = GetResponse(uri, timeout);
            using(var responseStream = response.GetResponseStream())
            {
                return (xml)_responseSerializer.Deserialize(responseStream);
            }
        }

        /// <summary>
        /// Perform a super magic server-side reverse lookup update using only the url you dig out of the GetRecords call.
        /// </summary>
        /// <param name="updateUri">The url you get out of an item in an xml from GetRecords.</param>
        /// <param name="timeout">The amount of time to wait for the web request to be fulfilled.  (-1millis to wait forever.)</param>
        /// <exception cref="TimeoutException">If timeout expires.</exception>
        public void UpdateRecord(string updateUri, TimeSpan timeout)
        {
            GetResponse(updateUri, timeout)
                .Close();
        }

        /// <summary>
        /// Simple web request wrapper which throws TimeoutException or returns you the result.
        /// </summary>
        /// <exception cref="TimeoutException">If timeout expires.</exception>
        private static WebResponse GetResponse(string uri, TimeSpan timeout)
        {
            var req = WebRequest.Create(uri);

            using (var response = req.GetResponseAsync())
            {
                response.Wait(timeout);
                if (response.IsCompleted)
                {
                    return response.Result;
                }
                else
                {
                    throw new TimeoutException("Waited longer than " + timeout.ToString() + " and received no reply from freedns.");
                }
            }
        }

        /// <summary>
        /// The SHA-1 string is your SHA hashed "username|password" (without quotes).
        /// http://freedns.afraid.org/api/
        /// </summary>
        /// <param name="user">FreeDns user name.</param>
        /// <param name="password">FreeDns password.</param>
        /// <returns>The hashed API key for this credential pair.</returns>
        public static string GetApiKey(string user, string password)
        {
            var hasher = System.Security.Cryptography.SHA1.Create();
            var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}|{1}", user, password)));
            var sb = new StringBuilder(hash.Length * 2);

            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
