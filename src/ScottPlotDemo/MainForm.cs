using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using ScottPlot;
using ScottPlot.Plottable;

using ScottPlotDemo.Utils;

namespace ScottPlotDemo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            FormsPlotScatterInitialize();
            ScatterPlot();
            FormsPlotLiveSignalInitialize();
            LiveSignalPlot();
            FormsPlotHistogramInitialize();
            HistogramPlot();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DataUpdateTimer?.Stop();
            RenderTimer?.Stop();
        }

        #region ScatterPlot

        private ScatterPlot _scatterPlot;

        private MarkerPlot _hightlightMarker;

        private int _lastHighlightedIndex = -1;

        private HLine _hline;

        private Annotation _annotationTopLeft;

        /// <summary>
        /// 初始化 FormsPlotScatter。
        /// </summary>
        private void FormsPlotScatterInitialize()
        {
            FormsPlotScatter.Plot.Title("ScatterPlot", fontName: Font.Name);
            FormsPlotScatter.Plot.XLabel("Time");
            FormsPlotScatter.Plot.YLabel("Value");
            FormsPlotScatter.Plot.Style(Style.Default);
            FormsPlotScatter.Plot.Palette = Palette.Microcharts;
        }

        public void ScatterPlot()
        {
            #region Data

            double[] xs = DataGen.Consecutive(51);
            double[] sin = DataGen.Sin(51);
            _scatterPlot = FormsPlotScatter.Plot.AddScatter(xs, sin, label: "sin");

            #endregion

            #region Hover Marker

            // 添加一個紅色圓圈，滑鼠移動時，作為突出顯示的點指示器。
            _hightlightMarker = FormsPlotScatter.Plot.AddMarker(0, 0, MarkerShape.openCircle, 15, Color.Red);
            _hightlightMarker.IsVisible = false;

            FormsPlotScatter.MouseMove += new MouseEventHandler(FormsPlotScatter_MouseMove);
            FormsPlotScatter.MouseLeave += new EventHandler(FormsPlotScatter_MouseLeave);

            #endregion

            #region Horizontal line

            _hline = FormsPlotScatter.Plot.AddHorizontalLine(0.00D);
            _hline.LineWidth = 1;
            _hline.PositionLabel = true;
            _hline.PositionLabelBackground = _hline.Color;
            _hline.DragEnabled = true;
            _hline.PositionFormatter = y => $"Y={y:F2}";

            #endregion

            #region Legend, Annotation

            // 設定圖例可見性和位置，預設為右下角。
            FormsPlotScatter.Plot.Legend();
            // 設定註釋可見性和位置。
            _annotationTopLeft = FormsPlotScatter.Plot.AddAnnotation("(---, ---)", 10, 10);
            _annotationTopLeft.Font.Size = Font.Size;

            #endregion

            #region Text

            // 文字在指定位置。
            Text text = FormsPlotScatter.Plot.AddText("sample text", 25.0, 0.0, size: 16, color: Color.Blue);
            // 點在文字的左下角。
            text.Alignment = Alignment.LowerLeft;

            #endregion

            FormsPlotScatter.Refresh();
        }

        /// <summary>
        /// 移動滑鼠時，顯示 Hightlight Marker 的數值。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormsPlotScatter_MouseMove(object sender, MouseEventArgs e)
        {
            // 確定離光標最近的點。
            (double mouseCoordX, double mouseCoordY) = FormsPlotScatter.GetMouseCoordinates();
            double xyRatio = FormsPlotScatter.Plot.XAxis.Dims.PxPerUnit / FormsPlotScatter.Plot.YAxis.Dims.PxPerUnit;
            // 返回距離給定坐標最近的數據點的位置和索引。
            (double pointX, double pointY, int pointIndex) = _scatterPlot.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);

            // 如果突出顯示的點發生變化，則渲染並更新當前點的位置。
            if (_lastHighlightedIndex != pointIndex)
            {
                _lastHighlightedIndex = pointIndex;

                // 移動紅色圓圈到指定位置上並顯示。
                _hightlightMarker.X = pointX;
                _hightlightMarker.Y = pointY;
                _hightlightMarker.IsVisible = true;

                label1.Text = $"Closest point to ({e.X:N0}, {e.Y:N0}) is index {pointIndex} ({pointX:N2}, {pointY:N2})";
                _annotationTopLeft.Label = $"({pointX:N2}, {pointY:N2})";

                FormsPlotScatter.Refresh();
            }
        }

        /// <summary>
        /// 滑鼠離開時，隱藏 Hightlight Marker 的數值。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormsPlotScatter_MouseLeave(object sender, EventArgs e)
        {
            // 清除 Hightlight Marker 的數值
            label1.Text = "Closest point to (---, ---) is index --- (---, ---)";
            _annotationTopLeft.Label = "(---, ---)";
            // 隱藏 Hightlight Marker。
            _hightlightMarker.IsVisible = false;

            FormsPlotScatter.Refresh();
        }

        /// <summary>
        /// 重設 Scatter 圖。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButton_Click(object sender, EventArgs e)
        {
            // 清除 Hightlight Marker 的數值
            label1.Text = "Closest point to (---, ---) is index --- (---, ---)";
            _annotationTopLeft.Label = "(---, ---)";
            // 隱藏 Hightlight Marker。
            _hightlightMarker.IsVisible = false;

            // Hline 回到原點。
            _hline.DragTo(0.0, 0.0, true);

            FormsPlotScatter.Plot.AxisAuto();
            FormsPlotScatter.Refresh();
        }

        #endregion

        #region SingalPlot

        private SignalPlotConst<double> _signalPlot;

        private readonly double[] _liveData = new double[61];

        private Annotation _annotationTopRight;

        private Crosshair _crosshair;

        //private readonly double[] _xPositions = { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60 };

        //private string[] _xLabels;

        ///// <summary>
        ///// 更新 FormsPlot x 軸的時間刻度。
        ///// </summary>
        //private void XAxisDataTimeUpdate()
        //{
        //    // 更新時間刻度為過去 60 秒。
        //    DateTime now = DateTime.Now;
        //    _xLabels = new string[]
        //    {
        //        now.AddSeconds(-60).ToString("HH:mm:ss"),
        //        now.AddSeconds(-55).ToString("HH:mm:ss"),
        //        now.AddSeconds(-50).ToString("HH:mm:ss"),
        //        now.AddSeconds(-45).ToString("HH:mm:ss"),
        //        now.AddSeconds(-40).ToString("HH:mm:ss"),
        //        now.AddSeconds(-35).ToString("HH:mm:ss"),
        //        now.AddSeconds(-30).ToString("HH:mm:ss"),
        //        now.AddSeconds(-25).ToString("HH:mm:ss"),
        //        now.AddSeconds(-20).ToString("HH:mm:ss"),
        //        now.AddSeconds(-15).ToString("HH:mm:ss"),
        //        now.AddSeconds(-10).ToString("HH:mm:ss"),
        //        now.AddSeconds(-5).ToString("HH:mm:ss"),
        //        now.AddSeconds(0).ToString("HH:mm:ss"),
        //    };
        //}

        private DateTime _xAxisTickNow;

        /// <summary>
        /// 格式化 x 軸的刻度。
        /// </summary>
        /// <param name="postion"></param>
        /// <returns></returns>
        private string XAxisTickFormatter(double postion)
        {
            switch (postion)
            {
                case 0:
                    return _xAxisTickNow.AddSeconds(0).ToString("HH:mm:ss");
                case 5:
                    return _xAxisTickNow.AddSeconds(-5).ToString("HH:mm:ss");
                case 10:
                    return _xAxisTickNow.AddSeconds(-10).ToString("HH:mm:ss");
                case 15:
                    return _xAxisTickNow.AddSeconds(-15).ToString("HH:mm:ss");
                case 20:
                    return _xAxisTickNow.AddSeconds(-20).ToString("HH:mm:ss");
                case 25:
                    return _xAxisTickNow.AddSeconds(-25).ToString("HH:mm:ss");
                case 30:
                    return _xAxisTickNow.AddSeconds(-30).ToString("HH:mm:ss");
                case 35:
                    return _xAxisTickNow.AddSeconds(-35).ToString("HH:mm:ss");
                case 40:
                    return _xAxisTickNow.AddSeconds(-40).ToString("HH:mm:ss");
                case 45:
                    return _xAxisTickNow.AddSeconds(-45).ToString("HH:mm:ss");
                case 50:
                    return _xAxisTickNow.AddSeconds(-50).ToString("HH:mm:ss");
                case 55:
                    return _xAxisTickNow.AddSeconds(-55).ToString("HH:mm:ss");
                case 60:
                    return _xAxisTickNow.AddSeconds(-60).ToString("HH:mm:ss");
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 初始化 FormsPlot。
        /// </summary>
        /// 
        private void FormsPlotLiveSignalInitialize()
        {
            FormsPlotLiveSignal.Plot.Title("Memory Usage");
            FormsPlotLiveSignal.Plot.XLabel("Time (seconds)");
            FormsPlotLiveSignal.Plot.YLabel("Memory (MB)");

            #region Style

            FormsPlotLiveSignal.Plot.Style(Style.Black);
            FormsPlotLiveSignal.Plot.Palette = Palette.Amber;

            #endregion

            #region FormsPlot Configuration

            // 取消綁定右鍵選單事件。
            FormsPlotLiveSignal.RightClicked -= FormsPlotLiveSignal.DefaultRightClickEvent;
            // 關閉滾輪縮放。
            FormsPlotLiveSignal.Configuration.ScrollWheelZoom = false;
            // 關閉雙擊滾輪中鍵啟動效能檢視。
            FormsPlotLiveSignal.Configuration.DoubleClickBenchmark = false;
            // 關閉單擊滾輪中鍵縮放置適合大小。
            FormsPlotLiveSignal.Configuration.MiddleClickAutoAxis = false;
            // 關閉單擊滾輪中鍵選取範圍縮放。
            FormsPlotLiveSignal.Configuration.MiddleClickDragZoom = false;
            // 關閉左鍵移動。
            FormsPlotLiveSignal.Configuration.LeftClickDragPan = false;
            // 關閉右鍵縮放。
            FormsPlotLiveSignal.Configuration.RightClickDragZoom = false;

            #endregion

            #region Axis

            // 限制 x、y 軸刻度的上、下限。
            FormsPlotLiveSignal.Plot.SetAxisLimits(xMin: 0, xMax: 60, yMin: 0, yMax: AppHepler.RamUasge() * 1.8);
            // 自動設置 x、y 軸的限制，以適應數據。
            FormsPlotLiveSignal.Plot.AxisAuto(0, 0);

            // 設定 x 軸主要刻度(和網格)位置和標籤。
            _xAxisTickNow = DateTime.Now;
            FormsPlotLiveSignal.Plot.XAxis.TickLabelFormat(XAxisTickFormatter);
            // 設定 x 軸刻度標籤的包裝密度
            FormsPlotLiveSignal.Plot.XAxis.TickDensity(2.5);

            //XAxisDataTimeUpdate();
            // 設定 x 軸主要刻度（和網格）位置和標籤。
            //FormsPlotLiveSignal.Plot.XAxis.ManualTickPositions(_xPositions, _xLabels);

            // 設定 x 軸刻度標籤的樣式。
            FormsPlotLiveSignal.Plot.XAxis.Color(Color.White);
            FormsPlotLiveSignal.Plot.XAxis.TickLabelStyle(color: Color.White, fontSize: 14, rotation: 30);
            FormsPlotLiveSignal.Plot.XAxis.MajorGrid(enable: true, color: Color.White, lineStyle: LineStyle.Solid);
            FormsPlotLiveSignal.Plot.XAxis.MinorGrid(enable: true, color: Color.DimGray, lineStyle: LineStyle.Dot);

            // 設定 y 軸刻度標籤的樣式。
            FormsPlotLiveSignal.Plot.YAxis.Color(Color.White);
            FormsPlotLiveSignal.Plot.YAxis.TickLabelStyle(color: Color.White, fontSize: 14);
            FormsPlotLiveSignal.Plot.YAxis.MajorGrid(enable: true, color: Color.White, lineStyle: LineStyle.Solid);
            FormsPlotLiveSignal.Plot.YAxis.MinorGrid(enable: true, color: Color.DimGray, lineStyle: LineStyle.Dot);

            #endregion
        }

        private async void LiveSignalPlot()
        {
            #region Signal or SignalConst

            // Signal 圖具有均勻分佈的 x 點並且渲染速度非常快。
            //_signalPlot = FormsPlotLiveSignal.Plot.AddSignal(_liveData, label: "Memory");

            // SignalConts 圖具有均勻分佈的 x 點，渲染速度比 Signal 圖快。
            // 預處理需要一點時間，並且需要 4 倍於 Signal 的內存。
            _signalPlot = FormsPlotLiveSignal.Plot.AddSignalConst(_liveData, label: "Memory");
            // 關閉顯示的資料圓點。
            _signalPlot.MarkerShape = MarkerShape.none;
            // 在曲線下方顯示純色填充。
            _signalPlot.FillBelow(color: Color.Red, alpha: 0.65);

            #endregion

            #region Crosshair

            _crosshair = FormsPlotLiveSignal.Plot.AddCrosshair(0, 0);
            _crosshair.VerticalLine.IsVisible = false;
            _crosshair.HorizontalLine.Color = Color.Yellow;
            _crosshair.HorizontalLine.PositionLabelBackground = Color.Yellow;
            _crosshair.HorizontalLine.PositionLabelFont.Color = Color.Black;
            FormsPlotLiveSignal_MouseLeave(null, null);

            FormsPlotLiveSignal.MouseEnter += new EventHandler(FormsPlotLiveSignal_MouseEnter);
            FormsPlotLiveSignal.MouseLeave += new EventHandler(FormsPlotLiveSignal_MouseLeave);
            FormsPlotLiveSignal.MouseMove += new MouseEventHandler(FormsPlotLiveSignal_MouseMove);

            #endregion

            #region Legend, Annotation

            // 設定圖例可見性和位置，預設為右下角。
            FormsPlotLiveSignal.Plot.Legend(location: Alignment.UpperLeft);
            // 設定註釋可見性和位置。
            _annotationTopRight = FormsPlotLiveSignal.Plot.AddAnnotation("----- MB", -10, 10);
            _annotationTopRight.Font.Size = Font.Size;
            _annotationTopRight.IsVisible = true;

            #endregion

            // 阻塞調用執行緒，同步執行。
            FormsPlotLiveSignal.Refresh();
            // 不會阻塞調用執行緒
            // FormsPlotLiveSignal.RefreshRequest();

            DataUpdateTimer.Enabled = true;
            await Task.Delay(1000);
            RenderTimer.Enabled = true;
        }

        private void DataUpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _xAxisTickNow = DateTime.Now;
            // 將整個圖表滾動到左側。
            Array.Copy(_liveData, 1, _liveData, 0, _liveData.Length - 1);
            _liveData[_liveData.Length - 1] = AppHepler.RamUasge(); //GetRandomNumber(24.0, 26.0);            
        }

        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            // 設定 FormsPlot 的 y 軸上、下限。
            FormsPlotLiveSignal.Plot.SetAxisLimitsY(yMin: 0, yMax: _liveData.Max() * 1.8);
            // 更新 Annotation 的顯示文字。
            _annotationTopRight.Label = $"{_liveData[_liveData.Length - 1]:N2} MB";

            // 阻塞調用執行緒，同步執行。
            FormsPlotLiveSignal.Refresh();
        }

        #region Crosshair Event

        private void FormsPlotLiveSignal_MouseEnter(object sender, EventArgs e) => _crosshair.IsVisible = true;

        private void FormsPlotLiveSignal_MouseLeave(object sender, EventArgs e)
        {
            _crosshair.IsVisible = false;
            FormsPlotLiveSignal.Refresh();
        }

        private void FormsPlotLiveSignal_MouseMove(object sender, MouseEventArgs e)
        {
            (double coordinateX, double coordinateY) = FormsPlotLiveSignal.GetMouseCoordinates();

            _crosshair.X = coordinateX;
            _crosshair.Y = coordinateY;

            FormsPlotLiveSignal.Refresh();
            //FormsPlotLiveSignal.Refresh(lowQuality: true, skipIfCurrentlyRendering: true);
        }

        #endregion

        #endregion

        #region Histogram

        private LollipopPlot _barPlot;

        private void FormsPlotHistogramInitialize()
        {
            // customize the plot style
            FormsPlotHistogram.Plot.Title("貼合偏移");
            FormsPlotHistogram.Plot.YAxis.Label("數量");
            FormsPlotHistogram.Plot.XAxis.Label("偏移量 (mm)");

            // 限制 x、y 軸刻度的上、下限。
            FormsPlotHistogram.Plot.SetAxisLimits(yMin: 0);
            // 自動設置 x、y 軸的限制，以適應數據。
            FormsPlotHistogram.Plot.AxisAuto(0, 0);
        }

        private void HistogramPlot()
        {
            var rand = new Random(65535);
            double[] values = DataGen.RandomNormal(rand, pointCount: 1000, mean: 0.02, stdDev: 0.01);

            #region HistogramPlot

            (double[] counts, double[] binEdges) =
                ScottPlot.Statistics.Common.Histogram(
                    values: values, 
                    min: values.Min(), 
                    max: values.Max() + 0.001, 
                    binSize: 0.001);
            double[] leftEdges = binEdges.Take(binEdges.Length - 1).ToArray();

            _barPlot = FormsPlotHistogram.Plot.AddLollipop(values: counts, positions: leftEdges);
            _barPlot.BarWidth = 0.001;
            _barPlot.ShowValuesAboveBars = true;
            _barPlot.BorderColor = ColorTranslator.FromHtml("#82add9");

            #endregion

            #region Vline

            VLine vlineMean = FormsPlotHistogram.Plot.AddVerticalLine(x: values.Average(), color: Color.Red, width: 0.5F, style: LineStyle.Solid, label: "平均值");
            vlineMean.PositionLabelBackground = Color.Red;
            vlineMean.PositionLabel = true;

            FormsPlotHistogram.Plot.AddVerticalLine(x: values.Min(), color: Color.Gray, width: 0.01F, style: LineStyle.Dash, label: "最小/最大值");
            FormsPlotHistogram.Plot.AddVerticalLine(x: values.Max(), color: Color.Gray, width: 0.01F, style: LineStyle.Dash);

            #endregion

            FormsPlotHistogram.Plot.Legend(location: Alignment.UpperRight);

            FormsPlotHistogram.Plot.AxisAuto();
            FormsPlotHistogram.Refresh();
        }

        #endregion
    }
}
