﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace DevionGames
{
    public static class MiniJSON
    {
        /// <summary>
        /// Parses the string json into a value
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <returns>An List&lt;object&gt;, a Dictionary&lt;string, object&gt;, a double, an integer,a string, null, true, or false</returns>
        public static object Deserialize(string json)
        {
            if (json == null)
            {
                return null;
            }

            return Parser.Parse(json);
        }

        sealed class Parser : IDisposable
        {
            const string WORD_BREAK = "{}[],:\"";

            public static bool IsWordBreak(char c)
            {
                return Char.IsWhiteSpace(c) || WORD_BREAK.IndexOf(c) != -1;
            }

            enum TOKEN
            {
                NONE,
                CURLY_OPEN,
                CURLY_CLOSE,
                SQUARED_OPEN,
                SQUARED_CLOSE,
                COLON,
                COMMA,
                STRING,
                NUMBER,
                TRUE,
                FALSE,
                NULL,
            }

            StringReader json;

            Parser(string jsonString)
            {
                json = new StringReader(jsonString);
            }

            public static object Parse(string jsonString)
            {
                using (var instance = new Parser(jsonString))
                {
                    return instance.ParseValue();
                }
            }

            public void Dispose()
            {
                json.Dispose();
                json = null;
            }

            Dictionary<string, object> ParseObject()
            {
                Dictionary<string, object> table = new Dictionary<string, object>();

                json.Read();

                while (true)
                {
                    switch (NextToken)
                    {
                        case TOKEN.NONE:
                            return null;
                        case TOKEN.COMMA:
                            continue;
                        case TOKEN.CURLY_CLOSE:
                            return table;
                        default:
                            string name = ParseString();
                            if (name == null)
                            {
                                return null;
                            }

                            if (NextToken != TOKEN.COLON)
                            {
                                return null;
                            }

                            json.Read();

                            table[name] = ParseValue();
                            break;
                    }
                }
            }

            List<object> ParseArray()
            {
                List<object> array = new List<object>();

                json.Read();

                bool parsing = true;
                while (parsing)
                {
                    TOKEN nextToken = NextToken;

                    switch (nextToken)
                    {
                        case TOKEN.NONE:
                            return null;
                        case TOKEN.COMMA:
                            continue;
                        case TOKEN.SQUARED_CLOSE:
                            parsing = false;
                            break;
                        default:
                            object value = ParseByToken(nextToken);

                            array.Add(value);
                            break;
                    }
                }

                return array;
            }

            object ParseValue()
            {
                TOKEN nextToken = NextToken;
                return ParseByToken(nextToken);
            }

            object ParseByToken(TOKEN token)
            {
                switch (token)
                {
                    case TOKEN.STRING:
                        string value = ParseString();
                        if (value.Contains("v3("))
                        {
                            return ToVector3(value);
                        }

                        if (value.Contains("v2("))
                        {
                            return ToVector2(value);
                        }

                        if (value.Contains("c("))
                        {
                            return ToColor(value);
                        }

                        if (value.Contains("v4("))
                        {
                            return ToVector4(value);
                        }

                        if (value.Contains("q("))
                        {
                            return ToQuaternion(value);
                        }

                        return value;
                    case TOKEN.NUMBER:
                        return ParseNumber();
                    case TOKEN.CURLY_OPEN:
                        return ParseObject();
                    case TOKEN.SQUARED_OPEN:
                        return ParseArray();
                    case TOKEN.TRUE:
                        return true;
                    case TOKEN.FALSE:
                        return false;
                    case TOKEN.NULL:
                        return null;
                    default:
                        return null;
                }
            }

            string ParseString()
            {
                StringBuilder s = new StringBuilder();
                char c;

                json.Read();

                bool parsing = true;
                while (parsing)
                {

                    if (json.Peek() == -1)
                    {
                        parsing = false;
                        break;
                    }

                    c = NextChar;
                    switch (c)
                    {
                        case '"':
                            parsing = false;
                            break;
                        case '\\':
                            if (json.Peek() == -1)
                            {
                                parsing = false;
                                break;
                            }

                            c = NextChar;
                            switch (c)
                            {
                                case '"':
                                case '\\':
                                case '/':
                                    s.Append(c);
                                    break;
                                case 'b':
                                    s.Append('\b');
                                    break;
                                case 'f':
                                    s.Append('\f');
                                    break;
                                case 'n':
                                    s.Append('\n');
                                    break;
                                case 'r':
                                    s.Append('\r');
                                    break;
                                case 't':
                                    s.Append('\t');
                                    break;
                                case 'u':
                                    char[] hex = new char[4];

                                    for (int i = 0; i < 4; i++)
                                    {
                                        hex[i] = NextChar;
                                    }

                                    s.Append((char)Convert.ToInt32(new string(hex), 16));
                                    break;
                            }
                            break;
                        default:
                            s.Append(c);
                            break;
                    }
                }

                return s.ToString();
            }

            object ParseNumber()
            {
                string number = NextWord;
                if (number.IndexOf('.') == -1)
                {
                    Int32.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out int parsedInt);
                    return parsedInt;
                }

                Single.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out float parsedDouble);
                return parsedDouble;
            }


            Quaternion ToQuaternion(string quaternion)
            {
                string[] subs = quaternion.Split('(')[1].Split(')')[0].Split(',');
                return new Quaternion(
                    Convert.ToSingle(subs[0], CultureInfo.InvariantCulture),
                    Convert.ToSingle(subs[1], CultureInfo.InvariantCulture),
                    Convert.ToSingle(subs[2], CultureInfo.InvariantCulture),
                    Convert.ToSingle(subs[3], CultureInfo.InvariantCulture)
                );
            }


            Vector4 ToVector4(string vector)
            {
                string[] subs = vector.Split('(')[1].Split(')')[0].Split(',');
                return new Vector4(
                    Convert.ToSingle(subs[0], CultureInfo.InvariantCulture),
                    Convert.ToSingle(subs[1], CultureInfo.InvariantCulture),
                    Convert.ToSingle(subs[2], CultureInfo.InvariantCulture),
                    Convert.ToSingle(subs[3], CultureInfo.InvariantCulture)
                );
            }

            Vector3 ToVector3(string vector)
            {
                string[] subs = vector.Split('(')[1].Split(')')[0].Split(',');
                return new Vector3(
                    Convert.ToSingle(subs[0], CultureInfo.InvariantCulture),
                    Convert.ToSingle(subs[1], CultureInfo.InvariantCulture),
                    Convert.ToSingle(subs[2], CultureInfo.InvariantCulture)
                );
            }

            Vector2 ToVector2(string vector)
            {
                string[] subs = vector.Split('(')[1].Split(')')[0].Split(',');
                return new Vector2(
                    Convert.ToSingle(subs[0], CultureInfo.InvariantCulture),
                    Convert.ToSingle(subs[1], CultureInfo.InvariantCulture)
                );
            }

            Color ToColor(string color)
            {
                string[] subs = color.Split('(')[1].Split(')')[0].Split(',');
                return new Color(
                    Convert.ToSingle(subs[0], CultureInfo.InvariantCulture),
                    Convert.ToSingle(subs[1], CultureInfo.InvariantCulture),
                    Convert.ToSingle(subs[2], CultureInfo.InvariantCulture),
                    Convert.ToSingle(subs[3], CultureInfo.InvariantCulture)
                );
            }

            void EatWhitespace()
            {
                while (Char.IsWhiteSpace(PeekChar))
                {
                    json.Read();

                    if (json.Peek() == -1)
                    {
                        break;
                    }
                }
            }

            char PeekChar => Convert.ToChar(json.Peek());

            char NextChar => Convert.ToChar(json.Read());

            string NextWord
            {
                get
                {
                    StringBuilder word = new StringBuilder();

                    while (!IsWordBreak(PeekChar))
                    {
                        word.Append(NextChar);

                        if (json.Peek() == -1)
                        {
                            break;
                        }
                    }

                    return word.ToString();
                }
            }

            TOKEN NextToken
            {
                get
                {
                    EatWhitespace();

                    if (json.Peek() == -1)
                    {
                        return TOKEN.NONE;
                    }

                    switch (PeekChar)
                    {
                        case '{':
                            return TOKEN.CURLY_OPEN;
                        case '}':
                            json.Read();
                            return TOKEN.CURLY_CLOSE;
                        case '[':
                            return TOKEN.SQUARED_OPEN;
                        case ']':
                            json.Read();
                            return TOKEN.SQUARED_CLOSE;
                        case ',':
                            json.Read();
                            return TOKEN.COMMA;
                        case '"':
                            return TOKEN.STRING;
                        case ':':
                            return TOKEN.COLON;
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case '-':
                            return TOKEN.NUMBER;
                    }

                    return NextWord switch{
                        "false" => TOKEN.FALSE,
                        "true" => TOKEN.TRUE,
                        "null" => TOKEN.NULL,
                        _ => TOKEN.NONE
                    };
                }
            }
        }

        /// <summary>
        /// Converts a IDictionary / IList object or a simple type (string, int, etc.) into a JSON string
        /// </summary>
        /// <param name="json">A Dictionary&lt;string, object&gt; / List&lt;object&gt;</param>
        /// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
        public static string Serialize(object obj)
        {
            return Serializer.Serialize(obj);
        }

        sealed class Serializer
        {
            readonly StringBuilder builder;

            Serializer()
            {
                builder = new StringBuilder();
            }

            public static string Serialize(object obj)
            {
                var instance = new Serializer();

                instance.SerializeValue(obj, 1);

                return instance.builder.ToString();
            }

            void SerializeValue(object value, int indentationLevel)
            {
                IList asList;
                IDictionary asDict;
                string asStr;

                if (value == null)
                {
                    builder.Append("null");
                }
                else if ((asStr = value as string) != null)
                {
                    SerializeString(asStr);
                }
                else if (value is bool b)
                {
                    builder.Append(b ? "true" : "false");
                }
                else if ((asList = value as IList) != null)
                {
                    SerializeArray(asList, indentationLevel);
                }
                else if ((asDict = value as IDictionary) != null)
                {
                    SerializeObject(asDict, indentationLevel);
                }
                else if (value is char c)
                {
                    SerializeString(new string(c, 1));
                }
                else
                {
                    SerializeOther(value);
                }
            }

            void SerializeObject(IDictionary obj, int indentationLevel)
            {
                bool first = true;

                builder.Append('{');
                builder.Append('\n');
                for (int i = 0; i < indentationLevel; ++i)
                {
                    builder.Append('\t');
                }

                foreach (object e in obj.Keys)
                {
                    if (!first)
                    {
                        builder.Append(',');
                        builder.Append('\n');
                        for (int i = 0; i < indentationLevel; ++i)
                        {
                            builder.Append('\t');
                        }
                    }

                    SerializeString(e.ToString());
                    builder.Append(':');

                    indentationLevel++;
                    SerializeValue(obj[e], indentationLevel);
                    indentationLevel--;

                    first = false;
                }

                builder.Append('\n');
                for (int i = 0; i < indentationLevel - 1; ++i)
                {
                    builder.Append('\t');
                }
                builder.Append('}');
            }

            void SerializeArray(IList anArray, int indentationLevel)
            {
                builder.Append('[');

                bool first = true;

                for (int i = 0; i < anArray.Count; i++)
                {
                    object obj = anArray[i];
                    if (!first)
                    {
                        builder.Append(',');
                    }

                    SerializeValue(obj, indentationLevel);

                    first = false;
                }

                builder.Append(']');
            }

            void SerializeString(string str)
            {
                builder.Append('\"');

                char[] charArray = str.ToCharArray();
                for (int i = 0; i < charArray.Length; i++)
                {
                    char c = charArray[i];
                    switch (c)
                    {
                        case '"':
                            builder.Append("\\\"");
                            break;
                        case '\\':
                            builder.Append("\\\\");
                            break;
                        case '\b':
                            builder.Append("\\b");
                            break;
                        case '\f':
                            builder.Append("\\f");
                            break;
                        case '\n':
                            builder.Append("\\n");
                            break;
                        case '\r':
                            builder.Append("\\r");
                            break;
                        case '\t':
                            builder.Append("\\t");
                            break;
                        default:
                            int codepoint = Convert.ToInt32(c);
                            if ((codepoint >= 32) && (codepoint <= 126))
                            {
                                builder.Append(c);
                            }
                            else
                            {
                                builder.Append("\\u");
                                builder.Append(codepoint.ToString("x4"));
                            }
                            break;
                    }
                }

                builder.Append('\"');
            }

            void SerializeOther(object value)
            {
                if (value is float f)
                {
                    builder.Append(f.ToString("R", CultureInfo.InvariantCulture));
                }
                else if (value is int or uint or long or sbyte or byte or short or ushort or ulong)
                {
                    builder.Append(value);
                }
                else if (value is double or decimal)
                {
                    builder.Append(Convert.ToDouble(value).ToString("R", CultureInfo.InvariantCulture));
                }
                else if (value is Vector2 vector2)
                {
                    builder.Append(("\"v2(" + vector2.x.ToString("R", CultureInfo.InvariantCulture) + "," + vector2.y.ToString("R", CultureInfo.InvariantCulture) + ")\""));
                }
                else if (value is Vector3 vector3)
                {
                    builder.Append("\"v3(" + vector3.x.ToString("R", CultureInfo.InvariantCulture) + "," + vector3.y.ToString("R", CultureInfo.InvariantCulture) + "," + vector3.z.ToString("R", CultureInfo.InvariantCulture) + ")\"");
                }
                else if (value is Vector4 vectorValue)
                {
                    builder.Append(("\"v4(" + vectorValue.x.ToString("R", CultureInfo.InvariantCulture) + "," + vectorValue.y.ToString("R", CultureInfo.InvariantCulture) + "," + vectorValue.z.ToString("R", CultureInfo.InvariantCulture) + "," + vectorValue.w.ToString("R", CultureInfo.InvariantCulture) + ")\""));
                }
                else if (value is Quaternion quaternionValue)
                {
                    builder.Append(("\"q(" + quaternionValue.x.ToString("R", CultureInfo.InvariantCulture) + "," + quaternionValue.y.ToString("R", CultureInfo.InvariantCulture) + "," + quaternionValue.z.ToString("R", CultureInfo.InvariantCulture) + "," + quaternionValue.w.ToString("R", CultureInfo.InvariantCulture) + ")\""));
                }
                else if (value is Color colorValue)
                {
                    builder.Append("\"c(" + colorValue.r.ToString("R", CultureInfo.InvariantCulture) + "," + colorValue.g.ToString("R", CultureInfo.InvariantCulture) + "," + colorValue.b.ToString("R", CultureInfo.InvariantCulture) + "," + colorValue.a.ToString("R", CultureInfo.InvariantCulture) + ")\"");
                }
                else
                {
                    SerializeString(value.ToString());
                }
            }
        }
    }
}