using Microsoft.Win32;

using System;
using System.ComponentModel;
using System.Configuration;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Doge {
    internal class Preferences : INotifyPropertyChanged {
        #region The preferences

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            if (propertyName != null)
                PropertyChanged?.Invoke(this, new(propertyName));
        }

        // set by user
        private bool _overlayEnabled = true;
        public bool OverlayEnabled {
            get => _overlayEnabled;
            set {
                if (_overlayEnabled != value) {
                    _overlayEnabled = value;
                    OverlayVisible = OverlayAvailable && value;
                    RaisePropertyChanged();
                }
            }
        }

        // set/used by OverlayManager
        private bool _overlayAvailable = false;
        public bool OverlayAvailable {
            get => _overlayAvailable;
            set {
                if (_overlayAvailable != value) {
                    _overlayAvailable = value;
                    OverlayVisible = OverlayEnabled && value;
                    RaisePropertyChanged();
                }
            }
        }

        // OverlayAvailable && OverlayEnabled (precedence erased one-way binding, hence this)
        // setting either of the above updates this, and this is bound to OverlayWindow's visiblity
        private bool _overlayVisible = false;
        public bool OverlayVisible {
            get => _overlayVisible;
            set {
                if (_overlayVisible != value) {
                    _overlayVisible = value;
                    RaisePropertyChanged();
                }
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
                if (_displayUsersAlways != value) {
                    _displayUsersAlways = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _displayUsersSpeaking = false;
        public bool DisplayUsersSpeaking {
            get => _displayUsersSpeaking;
            set {
                if (_displayUsersSpeaking != value) {
                    _displayUsersSpeaking = value;
                    RaisePropertyChanged();
                }
            }
        }
        //public bool DisplayUsersNever { get; set; }

        //public bool DisplayNamesAlways { get; set; } = true;
        //public bool DisplayNamesSpeaking { get; set; }

        private double _idleOpacity = 0.4;
        public double IdleOpacity {
            get => _idleOpacity;
            set {
                if (_idleOpacity != value) {
                    _idleOpacity = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _speakingOpacity = 1;
        public double SpeakingOpacity {
            get => _speakingOpacity;
            set {
                if (_speakingOpacity != value) {
                    _speakingOpacity = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _windowTop;
        public int WindowTop {
            get => _windowTop;
            set {
                if (_windowTop != value) {
                    _windowTop = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _windowLeft;
        public int WindowLeft {
            get => _windowLeft;
            set {
                if (_windowLeft != value) {
                    _windowLeft = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _windowTopMax;
        public int WindowTopMax {
            get => _windowTopMax;
            set {
                if (_windowTopMax != value) {
                    _windowTopMax = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _windowLeftMax;
        public int WindowLeftMax {
            get => _windowLeftMax;
            set {
                if (_windowLeftMax != value) {
                    _windowLeftMax = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Auth-related values

        private bool _authPending = true;
        public bool AuthPending {
            get => _authPending;
            set {
                if (_authPending != value) {
                    _authPending = value;
                    RaisePropertyChanged();
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
                foreach (var property in obj.GetType().GetProperties()) {
                    if (property.Name != "Current") {
                        var value = property.GetValue(obj);
                        if (value != null)
                            Registry.SetValue(baseKey, property.Name, value.ToString());
                    }
                }
            }
        }

        #endregion
    }
}
