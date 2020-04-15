using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace PrivacyConsentDB.Commons
{
    public class CommonUtil
    {
        private const String phonepattern = @"^[0-9]{2,3}[0-9]{3,4}[0-9]{4}$";
        private const String phonepattern2 = @"^[0-9]{2,3}-[0-9]{3,4}-[0-9]{4}$";
        private const String emailpattern = @"^([0-9a-zA-Z]" + //Start with a digit or alphabetical
                                        @"([\+\-_\.][0-9a-zA-Z]+)*" + // No continuous or ending +-_. chars in email
                                        @")+" +
                                        @"@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,17})$";

        public static DateTime toKoreaDT(DateTime utc) {
            return TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time"));
        }
        public static DateTime toUtcDT(DateTime korea) {
            TimeZoneInfo kr = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
            return TimeZoneInfo.ConvertTimeToUtc(korea, kr);
        }

        public static bool checkMobile(string mobile) {
            if (string.IsNullOrEmpty(mobile))
            {
                return false;
            }
            else
            {
                if (Regex.IsMatch(mobile, phonepattern) || Regex.IsMatch(mobile, phonepattern2)) {
                    return true;
                }                
            }
            return false;
        }

        public static bool checkEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            else
            {
                email = email.Trim();
                if (Regex.IsMatch(email, emailpattern))
                {
                    return true;
                }
            }
            return false;
        }
    }
}