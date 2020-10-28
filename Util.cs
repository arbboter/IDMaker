using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PubUtil
{
    public static class Util
    {
        /// <summary>
        /// 设置字符串固定位置值
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        public static string Set(ref string text, string value, int startIndex)
        {
            for(int i = text.Length; i < value.Length+startIndex; i++)
            {
                text += " ";
            }
            text = text.Substring(0, startIndex) + value + text.Substring(startIndex+value.Length);
            return text;
        }
    }


    #region 枚举字典
    public class EnumIntDict<T> where T : Enum
    {
        // int列表值
        public List<int> IntList { get; private set; }
        // 描述列表值
        public List<string> DescList { get; private set; }
        // 枚举值列表
        public List<Enum> EnumList { get; private set; }
        // 文本列表
        public List<string> TextList { get; private set; }

        // 字典值
        public Dictionary<int, string> DescByInt { get; private set; }
        public Dictionary<Enum, string> DescByEnum { get; private set; }
        public Dictionary<string, int> IntByDesc { get; private set; }
        public Dictionary<Enum, int> IntByEnum { get; private set; }
        public Dictionary<int, T> EnumByInt { get; private set; }
        public Dictionary<string, T> EnumByDesc { get; private set; }
        public Dictionary<Enum, string> TextByEnum { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public EnumIntDict()
        {
            IntList = new List<int>();
            DescList = new List<string>();
            EnumList = new List<Enum>();
            TextList = new List<string>();
            DescByInt = new Dictionary<int, string>();
            DescByEnum = new Dictionary<Enum, string>();
            IntByDesc = new Dictionary<string, int>();
            IntByEnum = new Dictionary<Enum, int>();
            EnumByInt = new Dictionary<int, T>();
            EnumByDesc = new Dictionary<string, T>();
            TextByEnum = new Dictionary<Enum, string>();

            var list = Enum.GetValues(typeof(T)).OfType<T>().ToList();
            foreach (var v in list)
            {
                // 值
                var iv = Convert.ToInt32(v);
                var dv = EnumHelper.GetEnumDesc(v);
                var text = string.Format("{0}-{1}", iv, dv);

                // 列表
                IntList.Add(Convert.ToInt32(iv));
                DescList.Add(dv);
                EnumList.Add(v);
                TextList.Add(text);

                // 字典
                DescByInt.Add(iv, dv);
                DescByEnum.Add(v, dv);
                IntByEnum.Add(v, iv);
                IntByDesc.Add(dv, iv);
                EnumByDesc.Add(dv, v);
                EnumByInt.Add(iv, v);
                TextByEnum.Add(v, text);
            }
        }

        /// <summary>
        /// 解析文本为int值和文本
        /// </summary>
        /// <param name="text"></param>
        /// <param name="iv"></param>
        /// <param name="dv"></param>
        /// <returns></returns>
        public bool ParseText(string text, ref int iv, ref string dv)
        {
            var dp = text.IndexOf('-');
            if (dp == -1)
            {
                return false;
            }
            iv = Convert.ToInt32(text.Substring(0, dp));
            dv = text.Substring(dp + 1);
            return true;
        }

        /// <summary>
        /// 解析文本为enum值和文本
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ev"></param>
        /// <param name="dv"></param>
        /// <returns></returns>
        public bool ParseText(string text, ref T ev, ref string dv)
        {
            int iv = 0;
            if (!ParseText(text, ref iv, ref dv))
            {
                return false;
            }
            ev = EnumByInt[iv];
            return true;
        }
    }
    #endregion

    #region 枚举函数
    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// 返回枚举值的描述信息。
        /// </summary>
        /// <param name="value">要获取描述信息的枚举值。</param>
        /// <returns>枚举值的描述信息。</returns>
        public static string GetEnumDesc<T>(T e)
        {
            Type enumType = typeof(T);
            DescriptionAttribute attr = null;

            // 获取枚举常数名称。
            string name = Enum.GetName(enumType, e);
            if (name != null)
            {
                // 获取枚举字段。
                FieldInfo fieldInfo = enumType.GetField(name);
                if (fieldInfo != null)
                {
                    // 获取描述的属性。
                    attr = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false) as DescriptionAttribute;
                }
            }

            // 返回结果
            if (attr != null && !string.IsNullOrEmpty(attr.Description))
                return attr.Description;
            else
                return string.Empty;
        }

        /// <summary>
        /// 通过枚举类型获取枚举列表;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<T> GetEnumList<T>() where T : Enum
        {
            List<T> list = Enum.GetValues(typeof(T)).OfType<T>().ToList();
            return list;
        }

        /// <summary>
        /// 通过枚举类型获取枚举列表:值-描述;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<string> GetEnumVD<T>() where T : Enum
        {
            var listKd = new List<string>();
            List<T> list = Enum.GetValues(typeof(T)).OfType<T>().ToList();
            foreach (var v in list)
            {
                var k = Convert.ToInt32(v);
                var d = GetEnumDesc(v);
                listKd.Add(string.Format("{0}-{1}", k, d));
            }
            return listKd;
        }

        /// <summary>
        /// 通过枚举类型获取枚举字典:值-描述;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<Enum, string> GetEnumKV<T>() where T : Enum
        {
            var dict = new Dictionary<Enum, string>();
            List<T> list = Enum.GetValues(typeof(T)).OfType<T>().ToList();
            foreach (var v in list)
            {
                dict.Add(v, GetEnumDesc(v));
            }
            return dict;
        }

        /// <summary>
        /// 枚举描述转枚举值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static T GetEnumValue<T>(string desc)
        {
            System.Reflection.FieldInfo[] fields = typeof(T).GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                // 获取描述属性
                object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (objs.Length > 0 && (objs[0] as DescriptionAttribute).Description == desc)
                {
                    return (T)field.GetValue(null);
                }
            }
            return default(T);
        }

        /// <summary>
        /// 返回枚举项的描述信息。
        /// </summary>
        /// <param name="e">要获取描述信息的枚举项。</param>
        /// <returns>枚举项的描述信息。</returns>
        public static string GetEnumDesc(Enum e)
        {
            if (e == null)
            {
                return string.Empty;
            }
            Type enumType = e.GetType();
            DescriptionAttribute attr = null;

            // 获取枚举字段。
            FieldInfo fieldInfo = enumType.GetField(e.ToString());
            if (fieldInfo != null)
            {
                // 获取描述的属性。
                attr = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false) as DescriptionAttribute;
            }

            // 返回结果
            if (attr != null && !string.IsNullOrEmpty(attr.Description))
                return attr.Description;
            else
                return string.Empty;
        }
    }

    /// <summary>
    /// 枚举键值对
    /// </summary>
    public class EnumKeyValue
    {
        public int Key { get; set; }
        public Enum Name { get; set; }
        public string Desc { get; set; }
    }
    #endregion
}
