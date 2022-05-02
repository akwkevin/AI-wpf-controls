using System;
using System.Collections.Generic;
using System.Linq;

namespace AIStudio.Wpf.Controls.Core
{
    /// <summary>
    /// 标识类型的基类
    /// </summary>
    /// <typeparam name="TValue">标识值的类型</typeparam>
    /// <typeparam name="TFlag">标识的类型</typeparam>
    public abstract class FlagBase<TValue, TFlag> : IEquatable<TFlag>, IComparable<TFlag>, IComparable
        where TFlag : FlagBase<TValue, TFlag>
    {
        #region 静态成员

        /// <summary>
        /// 默认值,有可能为空，取决于具体标识的设置
        /// </summary>
        public static TFlag DefaultFlag
        {
            get; private set;
        }

        /// <summary>
        /// 标识映射关系
        /// </summary>
        private static Dictionary<TValue, TFlag> FlagMap;

        static FlagBase()
        {
            //调用子类的类构造器完成初始化
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(TFlag).TypeHandle);
        }

        /// <summary>
        /// 初始化方法，应该在子类的类型构造器里调用且只调用一次。
        /// </summary>
        /// <param name="comparer">比较器，用来比较值和标志的对应关系，没有个性化需求的话这个参数填null即可</param>
        protected static void Initialize(IEqualityComparer<TValue> comparer = null)
        {
            FlagMap = new Dictionary<TValue, TFlag>(comparer);

            Type flagtype = typeof(TFlag);
            Type attrtype = typeof(DefaultFlagAttribute);

            var fields = flagtype
                .GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .Where(x => x.FieldType == flagtype);

            foreach (var item in fields)
            {
                TFlag flag = (TFlag)item.GetValue(null);
                if (flag == null)
                    continue;

                FlagMap.Add(flag.Value, flag);

                if (DefaultFlag == null && Attribute.IsDefined(item, attrtype))
                {
                    DefaultFlag = flag;
                }
            }
        }

        #endregion

        #region 公开方法
        /// <summary>
        /// 获取此类型所有的标记。
        /// </summary>
        /// <returns>所有标记</returns>
        public static List<TFlag> GetFlags()
        {
            return FlagMap.Values.ToList();
        }

        /// <summary>
        /// 获取此类型所有的标记，按照序号升序排列。
        /// </summary>
        /// <returns>按照序号排序的所有标记</returns>
        public static List<TFlag> GetSortedFlags()
        {
            return FlagMap.Values.OrderBy(x => x.SerialNumber).ToList();
        }

        /// <summary>
        /// 获取指定值的标识，如果不合法则为null。
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>标识，如果不合法则为null</returns>
        public static TFlag GetFlag(TValue value)
        {
            return (TFlag)value;
        }

        /// <summary>
        /// 获取与指定的值相关联的标识。
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="flag">标识</param>
        /// <returns>是否存在指定的值相关联的标识</returns>
        public static bool TryGetFlag(TValue value, out TFlag flag)
        {
            if (value == null)
            {
                flag = null;
                return false;
            }

            return FlagMap.TryGetValue(value, out flag);
        }

        /// <summary>
        /// 获取标识的实际值，如果标识为空会返回默认标识的值，如果默认标识也为空就返回标识值类型的默认值
        /// </summary>
        /// <param name="flag">标识</param>
        /// <returns>值</returns>
        public static TValue GetValue(TFlag flag)
        {
            return (TValue)(FlagBase<TValue, TFlag>)flag;
        }

        /// <summary>
        /// 检查是否存在指定值的标识。
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>是否存在指定值的标识</returns>
        public static bool IsDefined(TValue value)
        {
            if (value == null)
                return false;

            return FlagMap.ContainsKey(value);
        }

        #endregion

        #region 显式类型转换

        public static explicit operator TValue(FlagBase<TValue, TFlag> flag)
        {
            return flag == null ? (DefaultFlag == null ? default(TValue) : DefaultFlag.Value) : flag.Value;
        }

        public static explicit operator FlagBase<TValue, TFlag>(TValue value)
        {
            if (value == null)
                return null;

            TFlag result;
            if (FlagMap.TryGetValue(value, out result))
            {
                return result;
            }

            return null;
        }

        #endregion

        #region 实例成员

        /// <summary>
        /// 私有的实际值
        /// </summary>
        private readonly TValue value;

        /// <summary>
        /// 标识的实际值
        /// </summary>
        public TValue Value
        {
            get
            {
                return value;
            }
        }

        /// <summary>
        /// 私有的序号
        /// </summary>
        private readonly int serialNumber;

        /// <summary>
        /// 用来排序的序号
        /// </summary>
        public int SerialNumber
        {
            get
            {
                return serialNumber;
            }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="serialNumber">顺序序号</param>
        protected FlagBase(TValue value, int serialNumber)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            this.value = value;
            this.serialNumber = serialNumber;
        }

        #endregion

        #region 重写方法，暂时把这些方法标成了sealed，不允许子类重写，因为这些行为对于任何子类来说应该是严格统一的。

        public sealed override bool Equals(object obj)
        {
            /*
             * 因为同一个标识在内存中只存在一个对象，所以直接比较引用就可以，又快又简单
             * 但是注意，假如obj对象是TValue类型，这个方法会返回false，这是符合CLR对Equals方法的设计逻辑的。
             */
            return object.ReferenceEquals(this, obj);
        }

        public sealed override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        public sealed override string ToString()
        {
            return value.ToString();
        }

        #endregion

        #region IEquatable<TFlag> 成员
        public bool Equals(TFlag other)
        {
            return object.ReferenceEquals(this, other);
        }
        #endregion

        #region IComparable<TFlag> 接口成员

        public int CompareTo(TFlag other)
        {
            if (other == null)
                return 1;

            if (this.serialNumber > other.serialNumber)
                return 1;
            else if (this.serialNumber == other.serialNumber)
                return 0;
            else
                return -1;
        }

        #endregion

        #region IComparable 

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            TFlag other = obj as TFlag;

            if (other != null)
            {
                return this.CompareTo(other);
            }
            else
            {
                return 0;
            }

        }
        #endregion

    }

    /// <summary>
    /// 标明默认标识的Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class DefaultFlagAttribute : Attribute
    {
        public DefaultFlagAttribute()
        {
        }
    }
}
