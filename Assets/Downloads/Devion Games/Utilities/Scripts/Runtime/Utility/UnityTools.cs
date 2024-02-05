using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace DevionGames
{
    public static class UnityTools
    {
        private static CoroutineHandler m_CoroutineHandler;
        private static CoroutineHandler Handler
        {
            get{
                if(m_CoroutineHandler == null){
                    GameObject handlerObject = new GameObject("Coroutine Handler");
                    m_CoroutineHandler = handlerObject.AddComponent<CoroutineHandler>();
                }

                return m_CoroutineHandler;
            }
        }

        private static AudioSource audioSource;

        /// <summary>
        /// Play an AudioClip.
        /// </summary>
        /// <param name="clip">Clip.</param>
        /// <param name="volume">Volume.</param>
        public static void PlaySound(AudioClip clip, float volumeScale, AudioMixerGroup audioMixerGroup = null)
        {
            if(clip == null){
                return;
            }

            if(audioSource == null){
                AudioListener listener = Object.FindObjectOfType<AudioListener>();

                if(listener != null){
                    audioSource = listener.GetComponent<AudioSource>();

                    if(audioSource == null){
                        audioSource = listener.gameObject.AddComponent<AudioSource>();
                    }
                }
            }

            if(audioSource != null){
                audioSource.outputAudioMixerGroup = audioMixerGroup;
                audioSource.PlayOneShot(clip, volumeScale);
            }
        }

        public static bool IsPointerOverUI()
        {
            if(EventSystem.current == null || EventSystem.current.currentInputModule == null){
                return false;
            }

            Type type = EventSystem.current.currentInputModule.GetType();
            MethodInfo methodInfo;
            methodInfo = type.GetMethod("GetLastPointerEventData",
                                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if(methodInfo == null){
                return false;
            }

            PointerEventData eventData =
                (PointerEventData)methodInfo.Invoke(EventSystem.current.currentInputModule,
                                                    new object[]{ PointerInputModule.kMouseLeftId });

            if(eventData != null && eventData.pointerEnter){
                return eventData.pointerEnter.layer == 5;
            }

            return false;
        }

        /// <summary>
        /// Converts a color to hex.
        /// </summary>
        /// <returns>Hex string</returns>
        /// <param name="color">Color.</param>
        public static string ColorToHex(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

        /// <summary>
        /// Converts a hex string to color.
        /// </summary>
        /// <returns>Color</returns>
        /// <param name="hex">Hex.</param>
        public static Color HexToColor(string hex)
        {
            hex = hex.Replace("0x", "");
            hex = hex.Replace("#", "");
            byte a = 255;
            byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

            if(hex.Length == 8){
                a = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            }

            return new Color32(r, g, b, a);
        }

        /// <summary>
        /// Colors the string.
        /// </summary>
        /// <returns>The colored string.</returns>
        /// <param name="value">Value.</param>
        /// <param name="color">Color.</param>
        public static string ColorString(string value, Color color)
        {
            return "<color=#" + ColorToHex(color) + ">" + value + "</color>";
        }

        /// <summary>
        /// Replaces a string ignoring case.
        /// </summary>
        /// <param name="source">Source.</param>
        /// <param name="oldString">Old string.</param>
        /// <param name="newString">New string.</param>
        public static string Replace(string source, string oldString, string newString)
        {
            int index = source.IndexOf(oldString, StringComparison.CurrentCultureIgnoreCase);

            bool MatchFound = index >= 0;

            if(MatchFound){
                source = source.Remove(index, oldString.Length);

                source = source.Insert(index, newString);
            }

            return source;
        }

        /// <summary>
        /// Determines if the object is numeric.
        /// </summary>
        /// <returns><c>true</c> if is numeric the specified expression; otherwise, <c>false</c>.</returns>
        /// <param name="expression">Expression.</param>
        public static bool IsNumeric(object expression)
        {
            if(expression == null)
                return false;

            double number;
            return Double.TryParse(Convert.ToString(expression, CultureInfo.InvariantCulture),
                                   NumberStyles.Any, NumberFormatInfo.InvariantInfo, out number);
        }

        public static bool IsInteger(Type value)
        {
            return (value == typeof(SByte) || value == typeof(Int16) || value == typeof(Int32)
                    || value == typeof(Int64) || value == typeof(Byte) || value == typeof(UInt16)
                    || value == typeof(UInt32) || value == typeof(UInt64));
        }

        public static bool IsFloat(Type value)
        {
            return (value == typeof(float) | value == typeof(double) | value == typeof(Decimal));
        }

        /// <summary>
        /// Finds the child by name.
        /// </summary>
        /// <returns>The child.</returns>
        /// <param name="target">Target.</param>
        /// <param name="name">Name.</param>
        /// <param name="includeInactive">If set to <c>true</c> include inactive.</param>
        public static GameObject FindChild(this GameObject target, string name, bool includeInactive)
        {
            if(target != null){
                if(target.name == name && includeInactive ||
                   target.name == name && !includeInactive && target.activeInHierarchy){
                    return target;
                }

                for(int i = 0; i < target.transform.childCount; ++i){
                    GameObject result = target.transform.GetChild(i).gameObject.FindChild(name, includeInactive);

                    if(result != null)
                        return result;
                }
            }

            return null;
        }

        public static void Stretch(this RectTransform rectTransform, RectOffset offset)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = new Vector2(-(offset.right + offset.left), -(offset.bottom + offset.top));
            rectTransform.anchoredPosition =
                new Vector2(offset.left + rectTransform.sizeDelta.x * rectTransform.pivot.x,
                            -offset.top - rectTransform.sizeDelta.y * (1f - rectTransform.pivot.y));
        }

        public static void Stretch(this RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
        }

        public static void SetActiveObjectsOfType<T>(bool state) where T : Component
        {
            T[] objects = Object.FindObjectsOfType<T>();

            for(int i = 0; i < objects.Length; i++){
                objects[i].gameObject.SetActive(state);
            }
        }

        public static void IgnoreCollision(GameObject gameObject1, GameObject gameObject2)
        {
            Collider collider = gameObject2.GetComponent<Collider>();
            if(collider == null)
                return;
            Collider[] colliders = gameObject1.GetComponentsInChildren<Collider>(true);

            for(int i = 0; i < colliders.Length; i++){
                Physics.IgnoreCollision(colliders[i], collider);
            }
        }

        public static Bounds GetBounds(GameObject obj)
        {
            Bounds bounds = new Bounds();
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

            if(renderers.Length > 0){
                foreach(Renderer renderer in renderers){
                    if(renderer.enabled){
                        bounds = renderer.bounds;
                        break;
                    }
                }

                foreach(Renderer renderer in renderers){
                    if(renderer.enabled)
                        bounds.Encapsulate(renderer.bounds);
                }
            }

            return bounds;
        }

        public static string KeyToCaption(InputAction action)
        {
            return null;
        }

        public static string KeyToCaption(KeyCode key)
        {
            return key switch{
                KeyCode.None => "None",
                KeyCode.Backspace => "Backspace",
                KeyCode.Tab => "Tab",
                KeyCode.Clear => "Clear",
                KeyCode.Return => "Return",
                KeyCode.Pause => "Pause",
                KeyCode.Escape => "Esc",
                KeyCode.Space => "Space",
                KeyCode.Exclaim => "!",
                KeyCode.DoubleQuote => "\"",
                KeyCode.Hash => "#",
                KeyCode.Dollar => "$",
                KeyCode.Ampersand => "&",
                KeyCode.Quote => "'",
                KeyCode.LeftParen => "(",
                KeyCode.RightParen => ")",
                KeyCode.Asterisk => "*",
                KeyCode.Plus => "+",
                KeyCode.Comma => ",",
                KeyCode.Minus => "-",
                KeyCode.Period => ".",
                KeyCode.Slash => "/",
                KeyCode.Alpha0 => "0",
                KeyCode.Alpha1 => "1",
                KeyCode.Alpha2 => "2",
                KeyCode.Alpha3 => "3",
                KeyCode.Alpha4 => "4",
                KeyCode.Alpha5 => "5",
                KeyCode.Alpha6 => "6",
                KeyCode.Alpha7 => "7",
                KeyCode.Alpha8 => "8",
                KeyCode.Alpha9 => "9",
                KeyCode.Colon => ":",
                KeyCode.Semicolon => ";",
                KeyCode.Less => "<",
                KeyCode.Equals => "=",
                KeyCode.Greater => ">",
                KeyCode.Question => "?",
                KeyCode.At => "@",
                KeyCode.LeftBracket => "[",
                KeyCode.Backslash => "\\",
                KeyCode.RightBracket => "]",
                KeyCode.Caret => "^",
                KeyCode.Underscore => "_",
                KeyCode.BackQuote => "`",
                KeyCode.A => "A",
                KeyCode.B => "B",
                KeyCode.C => "C",
                KeyCode.D => "D",
                KeyCode.E => "E",
                KeyCode.F => "F",
                KeyCode.G => "G",
                KeyCode.H => "H",
                KeyCode.I => "I",
                KeyCode.J => "J",
                KeyCode.K => "K",
                KeyCode.L => "L",
                KeyCode.M => "M",
                KeyCode.N => "N",
                KeyCode.O => "O",
                KeyCode.P => "P",
                KeyCode.Q => "Q",
                KeyCode.R => "R",
                KeyCode.S => "S",
                KeyCode.T => "T",
                KeyCode.U => "U",
                KeyCode.V => "V",
                KeyCode.W => "W",
                KeyCode.X => "X",
                KeyCode.Y => "Y",
                KeyCode.Z => "Z",
                KeyCode.Delete => "Del",
                KeyCode.Keypad0 => "K0",
                KeyCode.Keypad1 => "K1",
                KeyCode.Keypad2 => "K2",
                KeyCode.Keypad3 => "K3",
                KeyCode.Keypad4 => "K4",
                KeyCode.Keypad5 => "K5",
                KeyCode.Keypad6 => "K6",
                KeyCode.Keypad7 => "K7",
                KeyCode.Keypad8 => "K8",
                KeyCode.Keypad9 => "K9",
                KeyCode.KeypadPeriod => ".",
                KeyCode.KeypadDivide => "/",
                KeyCode.KeypadMultiply => "*",
                KeyCode.KeypadMinus => "-",
                KeyCode.KeypadPlus => "+",
                KeyCode.KeypadEnter => "NT",
                KeyCode.KeypadEquals => "=",
                KeyCode.UpArrow => "UP",
                KeyCode.DownArrow => "DN",
                KeyCode.RightArrow => "LT",
                KeyCode.LeftArrow => "RT",
                KeyCode.Insert => "Ins",
                KeyCode.Home => "Home",
                KeyCode.End => "End",
                KeyCode.PageUp => "PU",
                KeyCode.PageDown => "PD",
                KeyCode.F1 => "F1",
                KeyCode.F2 => "F2",
                KeyCode.F3 => "F3",
                KeyCode.F4 => "F4",
                KeyCode.F5 => "F5",
                KeyCode.F6 => "F6",
                KeyCode.F7 => "F7",
                KeyCode.F8 => "F8",
                KeyCode.F9 => "F9",
                KeyCode.F10 => "F10",
                KeyCode.F11 => "F11",
                KeyCode.F12 => "F12",
                KeyCode.F13 => "F13",
                KeyCode.F14 => "F14",
                KeyCode.F15 => "F15",
                KeyCode.Numlock => "Num",
                KeyCode.CapsLock => "Caps Lock",
                KeyCode.ScrollLock => "Scr",
                KeyCode.RightShift => "Shift",
                KeyCode.LeftShift => "Shift",
                KeyCode.RightControl => "Control",
                KeyCode.LeftControl => "Control",
                KeyCode.RightAlt => "Alt",
                KeyCode.LeftAlt => "Alt",
                KeyCode.AltGr => "Alt",
                KeyCode.Menu => "Menu",
                KeyCode.Mouse0 => "Mouse 0",
                KeyCode.Mouse1 => "Mouse 1",
                KeyCode.Mouse2 => "M2",
                KeyCode.Mouse3 => "M3",
                KeyCode.Mouse4 => "M4",
                KeyCode.Mouse5 => "M5",
                KeyCode.Mouse6 => "M6",
                KeyCode.JoystickButton0 => "(A)",
                KeyCode.JoystickButton1 => "(B)",
                KeyCode.JoystickButton2 => "(X)",
                KeyCode.JoystickButton3 => "(Y)",
                KeyCode.JoystickButton4 => "(RB)",
                KeyCode.JoystickButton5 => "(LB)",
                KeyCode.JoystickButton6 => "(Back)",
                KeyCode.JoystickButton7 => "(Start)",
                KeyCode.JoystickButton8 => "(LS)",
                KeyCode.JoystickButton9 => "(RS)",
                KeyCode.JoystickButton10 => "J10",
                KeyCode.JoystickButton11 => "J11",
                KeyCode.JoystickButton12 => "J12",
                KeyCode.JoystickButton13 => "J13",
                KeyCode.JoystickButton14 => "J14",
                KeyCode.JoystickButton15 => "J15",
                KeyCode.JoystickButton16 => "J16",
                KeyCode.JoystickButton17 => "J17",
                KeyCode.JoystickButton18 => "J18",
                KeyCode.JoystickButton19 => "J19",
                _ => null
            };
        }

        private static void CheckIsEnum<T>(bool withFlags)
        {
            if(!typeof(T).IsEnum)
                throw new ArgumentException(string.Format("Type '{0}' is not an enum", typeof(T).FullName));
            if(withFlags && !Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
                throw new ArgumentException(string.Format("Type '{0}' doesn't have the 'Flags' attribute",
                                                          typeof(T).FullName));
        }

        public static bool HasFlag<T>(this T value, T flag) where T : struct
        {
            CheckIsEnum<T>(true);
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flag);
            return (lValue & lFlag) != 0;
        }

        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            return Handler.StartCoroutine(routine);
        }

        public static Coroutine StartCoroutine(string methodName, object value)
        {
            return Handler.StartCoroutine(methodName, value);
        }

        public static Coroutine StartCoroutine(string methodName)
        {
            return Handler.StartCoroutine(methodName);
        }

        public static void StopCoroutine(IEnumerator routine)
        {
            Handler.StopCoroutine(routine);
        }

        public static void StopCoroutine(string methodName)
        {
            Handler.StopCoroutine(methodName);
        }

        public static void StopCoroutine(Coroutine routine)
        {
            Handler.StopCoroutine(routine);
        }

        public static void StopAllCoroutines()
        {
            Handler.StopAllCoroutines();
        }

        public static bool ContainBindings(InputAction action, params string[] binds)
        {
            return binds.Any(bind => action.bindings.Any(inputBinding => inputBinding.effectivePath.Contains(bind)));
        }
    }
}