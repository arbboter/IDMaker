namespace IDMaker
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.richTextBoxData = new System.Windows.Forms.RichTextBox();
            this.flowLayoutPanelOper = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 1;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Controls.Add(this.richTextBoxData, 0, 1);
            this.tableLayoutPanelMain.Controls.Add(this.flowLayoutPanelOper, 0, 0);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 2;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1363, 772);
            this.tableLayoutPanelMain.TabIndex = 0;
            // 
            // richTextBoxData
            // 
            this.richTextBoxData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxData.Location = new System.Drawing.Point(3, 63);
            this.richTextBoxData.Name = "richTextBoxData";
            this.richTextBoxData.Size = new System.Drawing.Size(1357, 706);
            this.richTextBoxData.TabIndex = 0;
            this.richTextBoxData.Text = "";
            // 
            // flowLayoutPanelOper
            // 
            this.flowLayoutPanelOper.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelOper.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanelOper.Name = "flowLayoutPanelOper";
            this.flowLayoutPanelOper.Size = new System.Drawing.Size(1357, 54);
            this.flowLayoutPanelOper.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1363, 772);
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "账号生成器";
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.RichTextBox richTextBoxData;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelOper;
    }
}

