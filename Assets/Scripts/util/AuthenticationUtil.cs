using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.util {
    class AuthenticationUtil {

        public static long getMillis() {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(DateTime.UtcNow - epochStart).TotalMilliseconds;
        }

        public static string getCalcuatedSign(string reqJson)
        {
            //String key = "6667F8F45AB4187AD420D1ACF64B1B8B";
            String key = UnityEngine.PlayerPrefs.GetString("LOGIN_KEY");
            //Debug.Log("--------- MACing [" + reqJson + "]");
            //Debug.Log("--------- Calculation of hmac : " + );
            return HmacSha256Digest(reqJson, key);
        }

        public static string HmacSha256Digest(string message, string secret)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] keyBytes = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            System.Security.Cryptography.HMACSHA256 cryptographer = new System.Security.Cryptography.HMACSHA256(keyBytes);

            byte[] bytes = cryptographer.ComputeHash(messageBytes);

            return BitConverter.ToString(bytes).Replace("-", "").ToUpper();
        }
    }
}
