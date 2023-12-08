using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace JackHenryReddit.Common
{
    public static class Extensions
    {
        #region Private Members
        static string APP_NAME = "JackHenryReddit";

        /// <summary>
        /// prop1_name == prop1Name where "_" is toReplace
        /// </summary>
        /// <param name="prop1"></param>
        /// <param name="prop2"></param>
        /// <param name="toReplace"></param>
        /// <returns></returns>
        static bool AreEqualWithReplace(this string prop1, string prop2, string toReplace = "_")
            => (prop1 ?? "").Replace(toReplace, "").TrimAndCompare((prop2 ?? "").Replace(toReplace, ""));

        #endregion

        #region Public Members
        public static void LogInfoJackHenryReddit(this ILogger logger, string? message, [CallerMemberName] string callerMethod = "", [CallerFilePath] string callerFile = "")
            => logger.Log(LogLevel.Information, message.FormatLogMessage(callerMethod, callerFile));

        public static void LogExceptionJackHenryReddit(this ILogger logger, Exception ex, [CallerMemberName] string callerMethod = "", [CallerFilePath] string callerFile = "")
         => logger.Log(LogLevel.Information, $"{ex.Message} - {ex.StackTrace}".FormatLogMessage(callerMethod, callerFile));

        public static string FormatLogMessage(this string? message, [CallerMemberName] string callerMethod = "", [CallerFilePath] string callerFile = "")
        {
            message ??= "";
            string callerFileNameAndNoExtension = Path.GetFileNameWithoutExtension(callerFile);
            string callingMethod = callerMethod == "" ? "UnknownMethod" : callerMethod;
            string callingClass = callerFileNameAndNoExtension == "" ? "UnknownClass" : callerFileNameAndNoExtension;

            return $"<{APP_NAME}-{callingClass}-{callingMethod}()> {message}";
        }

        public static int ToInt(this string? value, int defaultValue = -1)
        {
            value = value.TrimAndCompare("") ? defaultValue.ToString() : value;
            int.TryParse(value, out defaultValue);
            return defaultValue;
        }

        public static double ToDouble(this string? value, double defaultValue = -1)
        {
            value = value.TrimAndCompare("") ? defaultValue.ToString() : value;
            double.TryParse(value, out defaultValue);
            return defaultValue;
        }

        public static int FirstValueToInt(this IEnumerable<string> values)
            => (values ?? new List<string>()).FirstOrDefault().ToInt();

        public static double FirstValueToDouble(this IEnumerable<string> values)
         => (values ?? new List<string>()).FirstOrDefault().ToDouble();

        /// <summary>
        /// Trims and compares two strings. This function considers null to be equivalent to an empty string. null === ""
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static bool TrimAndCompare(this string? a, string b, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
            => string.Equals((a ?? "").Trim(), (b ?? "").Trim(), comparison);

        public static string ToFriendlyCase(this string pascalCase, char separator = ' ')
        {
            StringBuilder sb = new StringBuilder();
            (pascalCase ?? "").ToList().ForEach(c => sb.Append($"{(char.IsUpper(c) ? separator : "")}{c}"));
            return sb.ToString().Trim();
        }

        public static T? FromJson<T>(this string json) where T : class, new()
        {
            if (json.TrimAndCompare(""))
            {
                return new T();
            }

            T? to = JsonConvert.DeserializeObject<T>(json);
            return to;
        }

        public static List<string> FromSeparatedString(this string separatedString, char sep = ',')
        {
            string[] temp = (separatedString ?? "").Split(sep);
            return temp.ToList();
        }

        public static string Append(this string prefix, string name)
            => prefix.TrimAndCompare("") ? name : $"{prefix}.{name}";

        public static Dictionary<string, object> FlattenJson(this string json)
            => new Dictionary<string, object>().FillDictionary(JToken.Parse(json));

        private static Dictionary<string, object> FillDictionary(this Dictionary<string, object> flatListing, JToken jToken, string prefix = "")
        {
            flatListing ??= new Dictionary<string, object>();

            var tokenChildren = (JToken jToken) => jToken.Children<JProperty>().ToList();

            if (jToken.Type == JTokenType.Object)
            {
                tokenChildren(jToken).ForEach(child => flatListing.FillDictionary(
                                                                                    child.Value,
                                                                                    prefix.Append(child.Name)
                                                                                 )
                                             );
            }
            else if (jToken.Type == JTokenType.Array)
            {
                int index = 0;
                tokenChildren(jToken).ForEach(child => flatListing.FillDictionary(
                                                                                    child,
                                                                                    prefix.Append((index++).ToString())
                                                                                 )
                                            );
            }
            else
            {
                var tokenValue = (jToken as JValue ?? new JValue("unknown")).Value;
                flatListing.Add(prefix, tokenValue ??= "");
            }
            return flatListing;
        }

        public static IEnumerable<T> AutoMap<T>(this IEnumerable<object> from) where T : class, new()
            => (from ?? new List<object>()).Select(f => f.AutoMap<T>());

        public static T AutoMap<T>(this object? from) where T : class, new()
        {
            T to = new T();
            if (from != null)
            {
                var propsFrom = from.GetType().GetProperties();
                to.GetType().GetProperties().Where(p => p.CanWrite).ToList().ForEach(p =>
                {
                    var propExists = propsFrom.FirstOrDefault(f => f.Name.AreEqualWithReplace(p.Name));
                    if (propExists != null)
                    {
                        var val = propExists.GetValue(from);
                        p.SetValue(to, val);
                    }
                });
            }
            return to;
        }

        public static T Clone<T>(this T from) where T : class, new()
            => from.AutoMap<T>();

        #endregion
    }
}
