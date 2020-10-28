using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDMaker
{
    /// <summary>
    /// ID生成器
    /// </summary>
    public abstract class IDMaker
    {
        #region 虚函数
        public abstract string CalcVerifyCode(string number);
        public virtual void Make(string number, List<string> result, int maxNum=128)
        {
            if (result.Count >= maxNum)
            {
                return;
            }
            if (number.Length + 1 >= this.Length())
            {
                var vc = this.CalcVerifyCode(number);
                if(vc==null ||vc.Length==0)
                {
                    return;
                }
                result.Add(number+vc);
                return;
            }
            for (int i = 0; i < 10 && result.Count < maxNum; i++)
            {
                this.Make(number + char.Parse(i.ToString()), result, maxNum);
            }
        }
        public abstract int Length();
        #endregion
    }

    /// <summary>
    /// 身份证生成器
    /// </summary>
    public class PersonId : IDMaker
    {
        #region 实现抽象方法
        public override string CalcVerifyCode(string number)
        {
            int VerifyResult = 0;
            int[] VerifyValue = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
            for (int i = 0; i < 17; i++)
                VerifyResult = (VerifyValue[i] * (number[i] - 48)) + VerifyResult;
            var vc = ("10X98765432")[VerifyResult % 11];
            return vc.ToString();
        }

        public override int Length()
        {
            return 18;
        }
        #endregion
    }

    /// <summary>
    /// 企业统一代码
    /// 第1位：登记管理部门代码（共一位字符）
    /// 第2位：机构类别代码（共一位字符）
    /// 第3位~第8位：登记管理机关行政区划码（共六位阿拉伯数字）
    /// 第9位~第17位：主体标识码（组织机构代码）（共九位字符）
    /// 第18位：校验码?（共一位字符）
    /// </summary>
    public class CompanyUniformCode : IDMaker
    {
        #region 实现抽象方法
        public override string CalcVerifyCode(string number)
        {
            var str = "0123456789ABCDEFGHJKLMNPQRTUWXY";
            int[] ws = new int[] { 1, 3, 9, 27, 19, 26, 16, 17, 20, 29, 25, 13, 8, 24, 10, 30, 28 };
            var sum = 0;
            for (var i = 0; i < Length()-1; i++)
            {
                sum += str.IndexOf(number.ElementAt(i)) * ws[i];
            }
            var c18 = 31 - (sum % 31);
            switch(c18)
            {
                case 10: return "A";
                case 11: return "B";
                case 12: return "C";
                case 13: return "D";
                case 14: return "E";
                case 15: return "F";
                case 16: return "G";
                case 17: return "H";
                case 18: return "J";
                case 19: return "K";
                case 20: return "L";
                case 21: return "M";
                case 22: return "N";
                case 23: return "P";
                case 24: return "Q";
                case 25: return "R";
                case 26: return "T";
                case 27: return "U";
                case 28: return "W";
                case 29: return "X";
                case 30: return "Y";
                default: return "";
            }
        }
        public override int Length()
        {
            return 18;
        }
        #endregion
    }

    /// <summary>
    /// 营业执照(15位)
    /// </summary>
    public class CompanyBusinessCard : IDMaker
    {
        #region 实现抽象方法
        public override string CalcVerifyCode(string number)
        {
            var str = "0123456789ABCDEFGHJKLMNPQRTUWXY";
            int[] ws = new int[] { 1, 3, 9, 27, 19, 26, 16, 17, 20, 29, 25, 13, 8, 24, 10, 30, 28 };
            var sum = 0;
            for (var i = 0; i < number.Length-1; i++)
            {
                sum += str.IndexOf(number.ElementAt(i)) * ws[i];
            }
            var c18 = 31 - (sum % 31);
            switch (c18)
            {
                case 10: return "A";
                case 11: return "B";
                case 12: return "C";
                case 13: return "D";
                case 14: return "E";
                case 15: return "F";
                case 16: return "G";
                case 17: return "H";
                case 18: return "J";
                case 19: return "K";
                case 20: return "L";
                case 21: return "M";
                case 22: return "N";
                case 23: return "P";
                case 24: return "Q";
                case 25: return "R";
                case 26: return "T";
                case 27: return "U";
                case 28: return "W";
                case 29: return "X";
                case 30: return "Y";
                default: return "";
            }
        }
        public override int Length()
        {
            return 15;
        }
        #endregion
    }

    /// <summary>
    /// 机构组织代码
    /// </summary>
    public class CompanyOrgCode : IDMaker
    {
        #region 实现抽象方法
        public override string CalcVerifyCode(string number)
        {
            var ws = new int[] { 3, 7, 9, 10, 5, 8, 4, 2 };
            var sum = 0;
            for(int i=0; i<Length()-1; i++)
            {
                var v = number.ElementAt(i);
                var nv = 0;
                if(char.IsDigit(v))
                {
                    nv = Convert.ToInt32(v) - 48;
                }
                else
                {
                    nv = Convert.ToInt32(v) - 55;
                }
                sum += ws[i] * nv;
            }
            var vc = 11 - sum % 11;
            switch(vc)
            {
                case 10:
                    return "-X";
                case 11:
                    return "-0";
                default:
                    return "-" + vc.ToString();
            }
        }
        public override int Length()
        {
            return 9;
        }
        #endregion
    }

    /// <summary>
    /// 银行卡号
    /// </summary>
    public class BankCard : IDMaker
    {
        public int CardLen = 16;

        #region 实现抽象方法
        /// <summary>
        /// 校验码为银行卡号最后一位，采用LUHN算法，亦称模10算法。计算方法如下：
        /// 第一步：从右边第1个数字开始每隔一位乘以2；
        /// 第二步： 把在第一步中获得的乘积的各位数字相加，然后再与原号码中未乘2的各位数字相加；
        /// 第三步：对于第二步求和值中个位数求10的补数，如果个位数为0则该校验码为0。
        /// 举例：
        /// 6259 6508 7177 209（不含校验码的银行卡号）
        /// 第一步：6*2=12，5*2=10，6*2=12，0*2=0，7*2=14，7*2=14，2*2=4，9*2=18
        /// 第二步：1+2 + 1+0 + 1+2 + 0 + 1+4 + 1+4 + 4 + 1+8 = 30
        /// 30 + 2+9+5+8+1+7+0 = 62
        /// 第三步：10-2=8
        /// 所以，校验码是8，完整的卡号应该是6259650871772098。
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public override string CalcVerifyCode(string number)
        {
            // 转数字
            var sum = 0;
            for(int i=0; i<number.Length; i++)
            {
                var v = Convert.ToInt32(number.ElementAt(i)) - 48;
                if((i & 1) == 0)
                {
                    var pn = v * 2;
                    sum += (pn % 10 + pn / 10);
                }
                else
                {
                    sum += v;
                }
            }
            var vc = 10 - sum % 10;
            if(vc == 10)
            {
                vc = 0;
            }
            return vc.ToString();
            
        }
        public override int Length()
        {
            return CardLen;
        }
        public override void Make(string number, List<string> result, int maxNum = 128)
        {
            if (result.Count >= maxNum)
            {
                return;
            }
            if (number.Length + 1 == this.Length())
            {
                var vc = this.CalcVerifyCode(number);
                if (vc == null || vc.Length == 0)
                {
                    return;
                }
                result.Add(number + vc);
                return;
            }
            for (int i = 0; i < 10 && result.Count < maxNum; i++)
            {
                this.Make(number + char.Parse(i.ToString()), result, maxNum);
            }
        }
        #endregion
    }
}
