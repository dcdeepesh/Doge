using Microsoft.Win32;

using System;
using System.ComponentModel;
using System.Configuration;
using System.Reflection;

namespace Doge {
    internal class Preferences : INotifyPropertyChanged {
        #region The preferences

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _overlayEnabled = true;
        public bool OverlayEnabled {
            get => _overlayEnabled;
            set {
                _overlayEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OverlayEnabled)));
            }
        }

        private bool _runAtStartup = false;
        public bool RunAtStartup {
            get => _runAtStartup;
            set {
                if (_runAtStartup != value) {
                    _runAtStartup = value;

                    using var subkey = Registry.CurrentUser.OpenSubKey(ConfigurationManager.AppSettings["startupSubkey"], true);
                    if (value)
                        subkey.SetValue("Doge", Assembly.GetEntryAssembly().Location);
                    else
                        subkey.DeleteValue("Doge", false);
                }
            }
        }

        private bool _displayUsersAlways = true;
        public bool DisplayUsersAlways {
            get => _displayUsersAlways;
            set {
                _displayUsersAlways = value;
                PropertyChanged?.Invoke(this, new(nameof(DisplayUsersAlways)));
            }
        }

        public bool DisplayUsersSpeaking { get; set; }
        //public bool DisplayUsersNever { get; set; }

        //public bool DisplayNamesAlways { get; set; } = true;
        //public bool DisplayNamesSpeaking { get; set; }

        private double _idleOpacity = 0.4;
        public double IdleOpacity {
            get => _idleOpacity;
            set {
                _idleOpacity = value;
                PropertyChanged?.Invoke(this, new(nameof(IdleOpacity)));
            }
        }

        private double _speakingOpacity = 1;
        public double SpeakingOpacity {
            get => _speakingOpacity;
            set {
                _speakingOpacity = value;
                PropertyChanged?.Invoke(this, new(nameof(SpeakingOpacity)));
            }
        }

        private int _windowTop;
        public int WindowTop {
            get => _windowTop;
            set {
                _windowTop = value;
                PropertyChanged?.Invoke(this, new(nameof(WindowTop)));
            }
        }

        private int _windowLeft;
        public int WindowLeft {
            get => _windowLeft;
            set {
                _windowLeft = value;
                PropertyChanged?.Invoke(this, new(nameof(WindowLeft)));
            }
        }

        #endregion

        #region Auth-related values

        // TODO notifypropertychanged
        private bool authPending = true;
        public bool AuthPending {
            get => authPending;
            set {
                if (authPending != value) {
                    authPending = value;
                    PropertyChanged.Invoke(this, new(nameof(AuthPending)));
                }
            }
        }

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public long AccessTokenValidUntil { get; set; }

        #endregion

        #region Static helpers

        public static Preferences Current { get; set; }
        public static void Load() => Current = Loader.Load<Preferences>(ConfigurationManager.AppSettings["preferencesKey"]);
        public static void Save() => Loader.Save(ConfigurationManager.AppSettings["preferencesKey"], Current);

        #endregion

        #region Utility loader class

        private static class Loader {
            public static T Load<T>(string baseKey) {
                Type type = typeof(T);
                T result = Activator.CreateInstance<T>();

                foreach (var property in type.GetProperties()) {
                    if (property.Name == "Current")
                        continue;
                    var value = Registry.GetValue(baseKey, property.Name, property.GetValue(result)) ?? property.GetValue(result);
                    var castedValue = Convert.ChangeType(value, property.PropertyType);
                    property.SetValue(result, castedValue);
                }

                return result;
            }

            public static void Save<T>(string baseKey, T obj) {
                foreach (var property in obj.GetType().GetProperties())
                    if (property.Name != "Current")
                        Registry.SetValue(baseKey, property.Name, property.GetValue(obj).ToString());
            }
        }

        #endregion
    }
}
