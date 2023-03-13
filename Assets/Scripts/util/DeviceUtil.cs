using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.util {
    class DeviceUtil {

        public static long getMillis() {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(DateTime.UtcNow - epochStart).TotalMilliseconds;
        }
    }
}
