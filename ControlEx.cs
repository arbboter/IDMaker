using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PubUtil
{
    #region 带标签的控件
    public abstract class LabelBox : FlowLayoutPanel
    {
        #region 属性
        public static int BoxWidth = 150;
        // Label控件
        public Label Label { get; set; }
        public string LabelText
        {
            get
            {
                return Label.Text;
            }
        }
        public string BoxText
        {
            get
            {
                return Box().Text;
            }
            set
            {
                Box().Text = value;
            }
        }
        public string BoxValue
        {
            get
            {
                var text = BoxText;
                var dp = text.IndexOf('-');
                if(dp > 0)
                {
                   return text.Substring(0, dp);
                }
                else
                {
                    return text;
                }
            }
            set
            {
                BoxText = value;
            }
        }
        #endregion

        public LabelBox(string labelText)
        {
            AutoSize = true;
            Label = new Label();
            Label.Text = labelText;
            Label.Size = new System.Drawing.Size(55, 25);
            Label.Anchor = AnchorStyles.Left;
            Label.TextAlign = ContentAlignment.MiddleLeft;
            Controls.Add(Label);
        }

        public virtual Control Box() { return null; }
    }

    public class LabelText : LabelBox
    {
        #region 属性
        public TextBox TextBox { get; set; }
        #endregion

        public LabelText(string labelText):base(labelText)
        {
            TextBox = new TextBox();
            TextBox.Size = new System.Drawing.Size(BoxWidth, 25);
            Controls.Add(TextBox);
        }
        public LabelText(string labelText, EventHandler eh) : this(labelText)
        {
            TextBox.TextChanged += eh;
        }

        public override Control Box() { return TextBox; }
    }

    public class LabelCombo : LabelBox
    {
        #region 属性
        public ComboBox ComboBox { get; set; }
        #endregion

        public LabelCombo(string labelText) : base(labelText)
        {
            ComboBox = new ComboBox();
            ComboBox.Size = new System.Drawing.Size(BoxWidth, 25);
            ComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            ComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
            Controls.Add(ComboBox);
        }

        public LabelCombo(string labelText, EventHandler selectChanged):this(labelText)
        {
            this.ComboBox.SelectedIndexChanged += selectChanged;
        }

        public LabelCombo(string labelText, EventHandler selectChanged, params string[] options) : this(labelText, selectChanged)
        {
            this.ComboBox.Items.AddRange(options);
        }

        public override Control Box() { return ComboBox; }
    }

    public class LabelDateTimePicker : LabelBox
    {
        #region 属性
        public DateTimePicker DateTimePicker { get; set; }
        #endregion

        public LabelDateTimePicker(string labelText) : base(labelText)
        {
            DateTimePicker = new DateTimePicker();
            DateTimePicker.Size = new System.Drawing.Size(BoxWidth, 25);
            Controls.Add(DateTimePicker);
        }

        public LabelDateTimePicker(string labelText, EventHandler valueChanged) : this(labelText)
        {
            this.DateTimePicker.ValueChanged += valueChanged;
        }

        public override Control Box() { return DateTimePicker; }
    }
    #endregion

    /// <summary>
    /// 自定义按钮
    /// </summary>
    public class ButtonEx : Button
    {
        public ButtonEx(string text)
        {
            this.Size = new System.Drawing.Size(80, 23);
            this.TabIndex = 8;
            this.Text = text;
            this.Anchor = AnchorStyles.Left;
            this.TextAlign = ContentAlignment.MiddleCenter;
            this.UseVisualStyleBackColor = true;
        }
    }
}
