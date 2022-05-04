using Microsoft.Win32;

using System;
using System.Configuration;

namespace Doge {
    internal class Preferences {
        #region The preferences object

        public bool OverlayEnabled { get; set; } = true;
        public bool RunAtStartup { get; set; }

        public bool DisplayUsersAlways { get; set; } = true;
        public bool DisplayUsersSpeaking { get; set; }
        public bool DisplayUsersNever { get; set; }

        public bool DisplayNamesAlways { get; set; } = true;
        public bool DisplayNamesSpeaking { get; set; }

        public double IdleOpacity { get; set; } = 20;
        public double SpeakingOpacity { get; set; } = 80;

        public int WindowTop { get; set; }
        public int WindowLeft { get; set; }

        #endregion

        #region Static helpers

        public static Preferences Current { get; set; }

        public static void Load() {
            Current = Loader.Load<Preferences>(ConfigurationManager.AppSettings["preferencesKey"]);
        }

        public static void Save() {
            Loader.Save(ConfigurationManager.AppSettings["preferencesKey"], Current);
        }

        #endregion

        #region Utility loader class

        private static class Loader {
            public static T Load<T>(string baseKey) {
                Type type = typeof(T);
                T result = Activator.CreateInstance<T>();

                foreach (var property in type.GetProperties()) {
                    if (property.Name == "Current")
                        continue;
                    var value = Registry.GetValue(baseKey, property.Name, property.GetValue(result));
                    var castedValue = Convert.ChangeType(value, property.PropertyType);
                    property.SetValue(result, castedValue);
                }

                return result;
            }

            public static void Save<T>(string baseKey, T obj) {
                foreach (var property in obj.GetType().GetProperties())
                    Registry.SetValue(baseKey, property.Name, property.GetValue(obj).ToString());
            }
        }

        #endregion
    }
}
