namespace ScottPlotDemo
{
    partial class MainForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.FormsPlotLiveSignal = new ScottPlot.FormsPlot();
            this.label1 = new System.Windows.Forms.Label();
            this.DataUpdateTimer = new System.Timers.Timer();
            this.RenderTimer = new System.Windows.Forms.Timer(this.components);
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.FormsPlotScatter = new ScottPlot.FormsPlot();
            this.ResetButton = new System.Windows.Forms.Button();
            this.FormsPlotHistogram = new ScottPlot.FormsPlot();
            ((System.ComponentModel.ISupportInitialize)(this.DataUpdateTimer)).BeginInit();
            this.TableLayoutPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // FormsPlotLiveSignal
            // 
            this.FormsPlotLiveSignal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FormsPlotLiveSignal.Location = new System.Drawing.Point(1, 1);
            this.FormsPlotLiveSignal.Margin = new System.Windows.Forms.Padding(0);
            this.FormsPlotLiveSignal.Name = "FormsPlotLiveSignal";
            this.FormsPlotLiveSignal.Size = new System.Drawing.Size(490, 379);
            this.FormsPlotLiveSignal.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(94, 346);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(340, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Closest point to (---, ---) is index --- (---, ---)";
            // 
            // DataUpdateTimer
            // 
            this.DataUpdateTimer.Interval = 1000D;
            this.DataUpdateTimer.SynchronizingObject = this;
            this.DataUpdateTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.DataUpdateTimer_Elapsed);
            // 
            // RenderTimer
            // 
            this.RenderTimer.Interval = 250;
            this.RenderTimer.Tick += new System.EventHandler(this.RenderTimer_Tick);
            // 
            // TableLayoutPanel
            // 
            this.TableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.TableLayoutPanel.ColumnCount = 2;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel.Controls.Add(this.panel1, 1, 0);
            this.TableLayoutPanel.Controls.Add(this.FormsPlotLiveSignal, 0, 0);
            this.TableLayoutPanel.Controls.Add(this.FormsPlotHistogram, 0, 1);
            this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.TableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 2;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel.Size = new System.Drawing.Size(984, 761);
            this.TableLayoutPanel.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.FormsPlotScatter);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.ResetButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(495, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(485, 373);
            this.panel1.TabIndex = 4;
            // 
            // FormsPlotScatter
            // 
            this.FormsPlotScatter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FormsPlotScatter.Location = new System.Drawing.Point(0, 0);
            this.FormsPlotScatter.Margin = new System.Windows.Forms.Padding(0);
            this.FormsPlotScatter.Name = "FormsPlotScatter";
            this.FormsPlotScatter.Size = new System.Drawing.Size(485, 339);
            this.FormsPlotScatter.TabIndex = 1;
            // 
            // ResetButton
            // 
            this.ResetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ResetButton.Location = new System.Drawing.Point(3, 341);
            this.ResetButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(84, 30);
            this.ResetButton.TabIndex = 5;
            this.ResetButton.Text = "Reset";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // FormsPlotHistogram
            // 
            this.FormsPlotHistogram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FormsPlotHistogram.Location = new System.Drawing.Point(1, 381);
            this.FormsPlotHistogram.Margin = new System.Windows.Forms.Padding(0);
            this.FormsPlotHistogram.Name = "FormsPlotHistogram";
            this.FormsPlotHistogram.Size = new System.Drawing.Size(490, 379);
            this.FormsPlotHistogram.TabIndex = 5;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 761);
            this.Controls.Add(this.TableLayoutPanel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MinimumSize = new System.Drawing.Size(1000, 800);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ScottPlotDemo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.DataUpdateTimer)).EndInit();
            this.TableLayoutPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ScottPlot.FormsPlot FormsPlotLiveSignal;
        private System.Windows.Forms.Label label1;
        private System.Timers.Timer DataUpdateTimer;
        private System.Windows.Forms.Timer RenderTimer;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        private System.Windows.Forms.Panel panel1;
        private ScottPlot.FormsPlot FormsPlotScatter;
        private System.Windows.Forms.Button ResetButton;
        private ScottPlot.FormsPlot FormsPlotHistogram;
    }
}

