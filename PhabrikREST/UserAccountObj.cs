
/**
 * Created by davidvronay on 9/5/16.
 */

using System;

namespace Phabrik.Core
{
    public class UserAccountObj {
        public long Id;
        public string U;    // username
        public string D;    // digest
        public string S;    // salt
        public DateTime c;      // account creation date
    }
}