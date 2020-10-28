using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDMaker
{
    public static class RefData
    {
        #region 身份证地址码对照表
        static string PersonId { get; set; }
        #endregion

        #region 身份证
        public static List<string> PersonIdList { get; private set; }

        public static void Init()
        {
            // 身份证
            PersonId = Properties.Resources.ResourceManager.GetString("StringPersonIdProvince", Properties.Resources.Culture);
            PersonIdList = new List<string>();
            foreach (var row in PersonId.Trim().Split('\n'))
            {
                var item = row.Trim().Split();
                var code = item[0].Trim();
                var name = item[1].Trim();
                PersonIdList.Add(code + "-" + name);
            }

            // 公司机构地区GB/T 2260
            GB2260Area = Properties.Resources.ResourceManager.GetString("StringAdminAreaCode", Properties.Resources.Culture);
            InstAreaList = new List<string>();
            foreach (var row in GB2260Area.Trim().Split('\n'))
            {
                var item = row.Trim().Split();
                var code = item[0].Trim();
                var name = item[1].Trim();
                InstAreaList.Add(code + "-" + name);
            }

            // 卡种类
            BankCardClass = new List<string>();

            // 银行卡信息
            BankCardInfo = new Dictionary<string, Dictionary<string, string>>();
            var info = Properties.Resources.ResourceManager.GetString("StringBankCardInfo", Properties.Resources.Culture);
            var jsonBankCardInfo = (JObject)JsonConvert.DeserializeObject(info);
            foreach(var v in jsonBankCardInfo)
            {
                var dict = new Dictionary<string, string>();
                foreach(JProperty p in v.Value)
                {
                    dict[p.Name] = p.Value.ToString();
                }
                BankCardInfo[v.Key] = dict;
                // 卡类别
                var cardType = dict["cardType"];
                if (!BankCardClass.Contains(cardType))
                {
                    BankCardClass.Add(cardType);
                }
            }
        }
        #endregion

        #region 机构数据
        static string GB2260Area { get; set; }
        #endregion

        #region 公司机构
        public static List<string> InstAreaList { get; private set; }
        #endregion

        #region 银行卡信息
        public static Dictionary<string, Dictionary<string, string>> BankCardInfo { get; set; }
        public static List<string> BankCardClass { get; private set; }
        #endregion
    }
}
