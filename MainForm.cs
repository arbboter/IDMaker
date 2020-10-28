using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PubUtil;

namespace IDMaker
{
    public partial class MainForm : Form
    {
        #region 属性
        EnumIntDict<IdType> idType { get; set; }
        IdType curIdType { get; set; }
        #endregion

        #region 自定义控件
        LabelCombo combo;
        LabelText textPrefix;
        LabelText textMaxNum;
        ButtonEx button;
        #endregion

        public MainForm()
        {
            // 初始化控件
            InitializeComponent();
            InitMyComponent();
            RefData.Init();

            // 启动窗口位置
            this.StartPosition = FormStartPosition.CenterScreen;

            // 窗口大小变化
            this.SizeChanged += OnSizeChanged;
        }


        /// <summary>
        /// 格式化日志
        /// </summary>
        /// <param name="t"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Format(LogType t, String format, params object[] args)
        {
            var textBox = this.richTextBoxData;
            if (textBox.InvokeRequired)
            {
                textBox.Invoke(new Action(() =>
                {
                    Format(t, format, args);
                }));
            }
            else
            {
                // 颜色处理
                var text = string.Format(format, args);
                Color color = Color.Black;
                switch (t)
                {
                    case LogType.WARN:
                        color = Color.Orange;
                        break;
                    case LogType.INFO:
                        color = Color.Blue;
                        break;
                    case LogType.ERROR:
                        color = Color.Red;
                        break;
                    case LogType.FATAL:
                        color = Color.DarkRed;
                        break;
                }

                // 追加
                textBox.Focus();
                // 设置光标的位置到文本尾
                var lastLen = textBox.TextLength;
                string cur_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffffff");
                string log = String.Format("[{0}] #{1}# {2}\r\n", cur_time, EnumHelper.GetEnumDesc(t), text);
                textBox.AppendText(log);
                textBox.Select(lastLen, log.Length);
                textBox.SelectionColor = color;
                textBox.ScrollToCaret();
                textBox.Select(textBox.TextLength, 0);
            }
        }

        /// <summary>
        /// 大小变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnSizeChanged(object sender, EventArgs e)
        {
            // 更新高度
            var height = combo.Height + 10;
            var panel = this.flowLayoutPanelOper;
            var totalWidth = 0;
            foreach(Control c in panel.Controls)
            {
                totalWidth += c.Width;
            }
            int row = Convert.ToInt32(Math.Ceiling(1.0*totalWidth / panel.Width));
            var rs = new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, row * height);
            this.tableLayoutPanelMain.RowStyles[0] = rs;
        }

        /// <summary>
        /// 初始化自定义控件
        /// </summary>
        void InitMyComponent()
        {
            // 面板
            var panel = this.flowLayoutPanelOper;
            // ID类型
            idType = new EnumIntDict<IdType>();
            combo = new LabelCombo("号码类型");
            var box = combo.ComboBox;
            box.Width = 120;
            box.SelectedIndexChanged += OnIdTypeSelectChanged;
            box.Items.AddRange(idType.TextList.ToArray());
            panel.Controls.Add(combo);
            // 变动事件
            this.OnSizeChanged(this, null);
        }

        /// <summary>
        /// 添加操作页面公共控件
        /// </summary>
        void InitOperComponent()
        {
            // 面板
            var panel = this.flowLayoutPanelOper;
            textPrefix = new LabelText("号码前缀");
            textMaxNum = new LabelText("最大数目");
            textMaxNum.TextBox.Width /= 2;
            textMaxNum.BoxValue = "100";
            button = new ButtonEx("确定");
            button.MouseClick += OnButtonOk;
            panel.Controls.Add(textPrefix);
            panel.Controls.Add(textMaxNum);
            panel.Controls.Add(button);
            this.OnSizeChanged(this, null);
        }

        /// <summary>
        /// 按钮操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnButtonOk(object sender, MouseEventArgs e)
        {
            // 获取参数
            var number = textPrefix.BoxValue.ToUpper();
            var maxNum = Convert.ToInt32(textMaxNum.BoxValue);
            IDMaker idMaker;
            switch(curIdType)
            {
                case IdType.PersonId:
                    idMaker = new PersonId();
                    break;
                case IdType.CompanyUniformCode:
                    idMaker = new CompanyUniformCode();
                    break;
                case IdType.CompanyBusinessCard:
                    idMaker = new CompanyBusinessCard();
                    break;
                case IdType.CompanyOrgCode:
                    idMaker = new CompanyOrgCode();
                    break;
                case IdType.BankCard:
                    var bankCard = new BankCard();
                    bankCard.CardLen = GetSelectBankCardLen();
                    idMaker = bankCard;
                    break;
                default:
                    return;
            }
            // 校验
            if(number.Length >= idMaker.Length())
            {
                var pre = number.Substring(0, idMaker.Length() - 1);
                var right = pre + idMaker.CalcVerifyCode(pre);
                if(right != number)
                {
                    Format(LogType.ERROR, $"{number}非法，合法应该为:{right}");
                }
                else
                {
                    Format(LogType.INFO, $"{number}合法");
                }
                return;
            }
            // 生成
            var ids = new List<string>();
            idMaker.Make(number, ids, maxNum);
            if (ids.Count != 0)
            {
                Format(LogType.INFO, "本次生成{0}个账户:\r\n{1}", ids.Count, string.Join("\r\n", ids));
            }
            else
            {
                Format(LogType.ERROR, "输入账户信息{0}前缀非法，无法生成账户", ids.Count, number);
            }
        }

        /// <summary>
        /// 重置控件
        /// </summary>
        void ResetPanelControls()
        {
            // 面板
            var panel = this.flowLayoutPanelOper;
            while(panel.Controls.Count > 1)
            {
                panel.Controls.RemoveAt(1);
            }
        }

        /// <summary>
        /// 省份变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnBirthAddrSelectChanged(object sender, EventArgs e)
        {
            var box = (sender as ComboBox)?.Parent as LabelCombo;
            if(box == null)
            {
                return;
            }
            var nv = box.BoxValue;
            var value = textPrefix.BoxValue;
            Util.Set(ref value, nv.Substring(0, 6), 0);
            textPrefix.BoxValue = value;
        }

        /// <summary>
        /// 出生年月
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnBirthdaySelectChanged(object sender, EventArgs e)
        {
            var dtp = sender as DateTimePicker;
            if(dtp == null || textPrefix == null)
            {
                return;
            }
            var nv = dtp.Value.ToString("yyyyMMdd");
            var value = textPrefix.BoxValue;
            Util.Set(ref value, nv, 6);
            textPrefix.BoxValue = value;
        }

        /// <summary>
        /// 初始化个人身份证界面
        /// </summary>
        void InitPersonIdUi()
        {
            var panel = this.flowLayoutPanelOper;
            var controls = panel.Controls;
            controls.Add(new LabelCombo("籍贯地址", OnBirthAddrSelectChanged, RefData.PersonIdList.ToArray()));
            var birthDay = new LabelDateTimePicker("出生年月", OnBirthdaySelectChanged);
            birthDay.DateTimePicker.Value = new DateTime(1990, 1, 1);
            controls.Add(birthDay);
        }

        /// <summary>
        /// 登记管理部门代码变动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnManageSelectChanged(object sender, EventArgs e)
        {
            // 设置前缀
            var value = textPrefix.BoxValue;
            var box = (sender as ComboBox)?.Parent as LabelCombo;
            if (box == null)
            {
                return;
            }
            Util.Set(ref value, box.BoxValue, 0);
            textPrefix.BoxValue = value;

            // 设置机构类别代码
            var options = new List<string>();
            switch(box.BoxValue)
            {
                case "1":
                case "5":
                    options.AddRange(new string[] { "1-社会团体", "2-民办非企业单位", "3-基金会", "9-其他"});
                    break;
                case "9":
                    options.AddRange(new string[] { "1-企业", "2-个体工商户", "3-农民专业合作社", "9-其他" });
                    break;
                case "Y":
                    options.AddRange(new string[] { "1-其他" });
                    break;
                default:
                    break;
            }
            var ctrls = this.flowLayoutPanelOper.Controls;
            var instCls = ctrls[2] as LabelCombo;
            if(instCls != null)
            {
                instCls.BoxText = "";
                instCls.ComboBox.Items.Clear();
                instCls.ComboBox.Items.AddRange(options.ToArray());
            }
        }

        /// <summary>
        /// 机构类别代码变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnInstClassSelectChanged(object sender, EventArgs e)
        {
            // 设置前缀
            var value = textPrefix.BoxValue;
            var box = (sender as ComboBox)?.Parent as LabelCombo;
            if (box == null)
            {
                return;
            }
            Util.Set(ref value, box.BoxValue, 1);
            textPrefix.BoxValue = value;
        }

        void OnInstAreaSelectChanged(object sender, EventArgs e)
        {
            // 设置前缀
            var value = textPrefix.BoxValue;
            var box = (sender as ComboBox)?.Parent as LabelCombo;
            if (box == null)
            {
                return;
            }
            Util.Set(ref value, box.BoxValue, 2);
            textPrefix.BoxValue = value;
        }


        /// <summary>
        /// 统一代码机构信息变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnInstIdChanged(object sender, EventArgs e)
        {
            // 设置前缀
            var value = textPrefix.BoxValue;
            var box = sender as TextBox;
            if (box == null)
            {
                return;
            }
            var text = box.Text;
            text += "         ";
            Util.Set(ref value, text.Substring(0, 9), 8);
            textPrefix.BoxValue = value.Trim();
        }

        /// <summary>
        /// 初始化企业统一代码页面
        /// </summary>
        void InitCompanyUniformCodeUi()
        {
            var panel = this.flowLayoutPanelOper;
            var controls = panel.Controls;
            var options = new string[] { "1-机构编制", "5-民政", "9-工商", "Y-其他" };
            controls.Add(new LabelCombo("部门代码", OnManageSelectChanged, options));
            controls.Add(new LabelCombo("类别代码", OnInstClassSelectChanged));
            controls.Add(new LabelCombo("行政区码", OnInstAreaSelectChanged, RefData.InstAreaList.ToArray()));
            controls.Add(new LabelText("主体标识", OnInstIdChanged));
        }

        /// <summary>
        /// 营业执照(15位)机构顺序码变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnInstOrderIdChanged(object sender, EventArgs e)
        {
            // 设置前缀
            var value = textPrefix.BoxValue;
            var box = sender as TextBox;
            if (box == null)
            {
                return;
            }
            var text = box.Text;
            text += "        ";
            Util.Set(ref value, text.Substring(0, 8), 6);
            textPrefix.BoxValue = value.Trim();
        }

        /// <summary>
        /// 初始化营业执照(15位)页面
        /// </summary>
        void InitCompanyBusinessCardCodeUi()
        {
            var panel = this.flowLayoutPanelOper;
            var controls = panel.Controls;
            var areas = RefData.InstAreaList.ToArray();
            var options = new string[areas.Length + 1];
            Array.Copy(areas, 0, options, 1, areas.Length);
            options[0] = "100000-国家工商总局";
            controls.Add(new LabelCombo("登记代码", OnManageSelectChanged, options));
            controls.Add(new LabelText("顺序代码", OnInstOrderIdChanged));
        }

        /// <summary>
        /// 银行卡类别选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnBankCardClsChanged(object sender, EventArgs e)
        {
            // 卡类别
            var box = (sender as ComboBox)?.Parent as LabelCombo;
            if (box == null)
            {
                return;
            }
            // 银行卡名称
            var bankBox = this.flowLayoutPanelOper.Controls[2] as LabelCombo;
            if (bankBox == null)
            {
                return;
            }
            // 获取卡类别
            var cardType = box.BoxValue;
            var options = new List<string>();
            foreach(var kv in RefData.BankCardInfo)
            {
                var v = kv.Value;
                if(v["cardType"] == cardType)
                {
                    options.Add(string.Format("{0}-{1}{2}", kv.Key, v["bank"], v["cardName"]));
                }
            }
            bankBox.BoxValue = "";
            bankBox.ComboBox.Items.Clear();
            bankBox.ComboBox.Items.AddRange(options.ToArray());
        }

        /// <summary>
        /// 银行选择变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnBankChanged(object sender, EventArgs e)
        {
            Dictionary<string, string> bank;
            if (!GetSelectBankCard(out bank))
            {
                return;
            }
            var value = bank["BIN"];
            var len = Convert.ToInt32(bank["cardNoLength"]);
            var text = textPrefix.BoxValue;
            Util.Set(ref text, value, 0);
            // 多补几个0
            text += "00000000000000000000";
            text = text.Substring(0, len-1);
            textPrefix.BoxValue = text;
        }

        /// <summary>
        /// 获取银行卡信息
        /// </summary>
        /// <returns></returns>
        bool GetSelectBankCard(out Dictionary<string, string> bankInfo)
        {
            // 银行名称
            bankInfo = null;
            var bankBox = this.flowLayoutPanelOper.Controls[2] as LabelCombo;
            if (bankBox == null)
            {
                return false;
            }
            var bankCode = bankBox.BoxValue;
            if (!RefData.BankCardInfo.ContainsKey(bankCode))
            {
                return false;
            }
            bankInfo = RefData.BankCardInfo[bankCode];
            return true;
        }

        /// <summary>
        /// 获取银行卡长度
        /// </summary>
        /// <returns></returns>
        int GetSelectBankCardLen()
        {
            Dictionary<string, string> bank;
            if(!GetSelectBankCard(out bank))
            {
                return 0;
            }
            return Convert.ToInt32(bank["cardNoLength"]);
        }

        /// <summary>
        /// 银行卡变动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnBankCardIdChanged(object sender, EventArgs e)
        {
            Dictionary<string, string> bank;
            if(!GetSelectBankCard(out bank))
            {
                return;
            }
            // 设置前缀
            var value = textPrefix.BoxValue;
            var box = sender as TextBox;
            if (box == null)
            {
                return;
            }
            var bankCode = bank["BIN"];
            var len = Convert.ToInt32(bank["cardNoLength"]);
            var text = bankCode + box.Text + "                              ";
            textPrefix.BoxValue = text.Substring(0, len).Trim();
        }

        /// <summary>
        /// 初始化银行卡页面
        /// </summary>
        void InitBankCardCardCodeUi()
        {
            var panel = this.flowLayoutPanelOper;
            var controls = panel.Controls;
            controls.Add(new LabelCombo("卡号类别", OnBankCardClsChanged, RefData.BankCardClass.ToArray()));
            controls.Add(new LabelCombo("银行名称", OnBankChanged));
            controls.Add(new LabelText("卡号标识", OnBankCardIdChanged));
        }

        /// <summary>
        /// 选择的证件类型变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnIdTypeSelectChanged(object sender, EventArgs e)
        {
            var idType = (IdType)Convert.ToInt32(combo.BoxValue);
            ResetPanelControls();
            switch (idType)
            {
                case IdType.PersonId:
                    InitPersonIdUi();
                    break;
                case IdType.CompanyUniformCode:
                    InitCompanyUniformCodeUi();
                    break;
                case IdType.CompanyBusinessCard:
                    InitCompanyBusinessCardCodeUi();
                    break;
                case IdType.CompanyOrgCode:
                    break;
                case IdType.BankCard:
                    InitBankCardCardCodeUi();
                    break;
                default:
                    MessageBox.Show("非法ID类型");
                    return;
            }
            curIdType = idType;
            InitOperComponent();
        }
    }

    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        [Description("Debug")]
        DEBUG,
        [Description("Info")]
        INFO,
        [Description("Warn")]
        WARN,
        [Description("Error")]
        ERROR,
        [Description("Fatal")]
        FATAL
    }
    /// <summary>
    /// ID类型
    /// </summary>
    enum IdType
    {
        [Description("身份证")]
        PersonId,
        [Description("企业三证合一(18位)")]
        CompanyUniformCode,
        [Description("营业执照(15位)")]
        CompanyBusinessCard,
        [Description("组织机构代码")]
        CompanyOrgCode,
        [Description("银行卡号")]
        BankCard,
    }
}
