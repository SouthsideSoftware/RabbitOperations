using System;
using System.Configuration;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Utility.DesignByContract;

namespace RabbitOperations.Collector.Configuration
{
    public class Settings : ISettings
    {
        public string AuditQueue
        {
            get { return GetString("AuditQueue", throwExceptionIfNotFound:true); }
        }

        public string ErrorQueue
        {
            get { return GetString("ErrorQueue", throwExceptionIfNotFound: true); }
        }

        public static bool GetBoolean(string key, bool defaultValue = false, bool throwExceptionIfNotFound = false)
        {
            Verify.RequireStringNotNullOrWhitespace(key, "key");

            bool val = defaultValue;
            string rawValue = ConfigurationManager.AppSettings.Get(key);
            if (rawValue != null)
            {
                bool.TryParse(rawValue, out val);
            }
            else if (throwExceptionIfNotFound)
            {
                throw new ConfigurationErrorsException(string.Format("appsetting value required in application configuration file for key {0}", key));
            }

            return val;
        }

        protected static decimal? GetDecimal(string key, decimal? defaultValue = null, bool throwExceptionIfNotFound = false)
        {
            Verify.RequireStringNotNullOrWhitespace(key, "key");

            decimal val = decimal.MinValue;
            string rawValue = ConfigurationManager.AppSettings.Get(key);
            if (rawValue != null)
            {
                decimal.TryParse(rawValue, out val);
            }
            else if (throwExceptionIfNotFound)
            {
                throw new ConfigurationErrorsException(string.Format("appsetting value required in application configuration file for key {0}", key));
            }

            return val != decimal.MinValue ? val : defaultValue;
        }

        protected static int? GetInt(string key, int? defaultValue, bool throwExceptionIfNotFound = false)
        {
            Verify.RequireStringNotNullOrWhitespace(key, "key");

            int val = int.MinValue;
            string rawValue = ConfigurationManager.AppSettings.Get(key);
            if (rawValue != null)
            {
                int.TryParse(rawValue, out val);
            }
            else if (throwExceptionIfNotFound)
            {
                throw new ConfigurationErrorsException(string.Format("appsetting value required in application configuration file for key {0}", key));
            }

            return val != int.MinValue ? val : defaultValue;
        }

        protected static string GetString(string key, string defaultValue = null, bool throwExceptionIfNotFound = false)
        {
            string val = ConfigurationManager.AppSettings[key];

            if (val == null && throwExceptionIfNotFound)
            {
                throw new ConfigurationErrorsException(string.Format("appsetting value required in application configuration file for key {0}", key));
            }

            return !string.IsNullOrWhiteSpace(val) ? val : defaultValue;
        }


        protected static double? GetDouble(string key, double? defaultValue, bool throwExceptionIfNotFound = false)
        {
            Verify.RequireStringNotNullOrWhitespace(key, "key");

            double val = 0d;
            string rawValue = ConfigurationManager.AppSettings.Get(key);
            if (rawValue != null)
            {
                double.TryParse(rawValue, out val);
            }
            else if (throwExceptionIfNotFound)
            {
                throw new ConfigurationErrorsException(string.Format("appsetting value required in application configuration file for key {0}", key));
            }

            return Math.Abs(val - double.MinValue) > .000001 ? val : defaultValue;
        }
    }
}