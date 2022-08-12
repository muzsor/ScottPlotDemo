using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using ScottPlot;
using ScottPlot.Plottable;
using ScottPlot.Renderable;
using ScottPlot.Statistics;

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

        private LollipopPlot _lollipopPlotX;

        private LollipopPlot _lollopopPlotY;

        private void FormsPlotHistogramInitialize()
        {
            FormsPlotHistogram.Plot.Title("貼合偏移");
            FormsPlotHistogram.Plot.YAxis.Label("數量");
            FormsPlotHistogram.Plot.XAxis.Label("偏移量 (mm)");
        }

        private void HistogramPlot()
        {
            double[] xOffset = { 0.038, -0.025, 0.039, 0.036, 0.037, 0.034, 0.042, 0.024, 0.026, 0.1, 0.07, 0.12, -0.1, -0.11, -0.058, 0.071 };
            double[] yOffset = { 0.055, 0.028, 0.039, 0.022, 0.05, 0.045, 0.02, 0.043, 0.013, 0.014, 0.001, 0.011, 0.005, 0.007, 0.005, 0.021 };

            double xMin = xOffset.Min();
            double xMax = xOffset.Max();

            double yMin = yOffset.Min();
            double yMax = yOffset.Max();

            #region HistogramPlot for x offset

            //(double[] xCounts, double[] xBinEdges) = Common.Histogram(values: xOffset, min: Math.Round(xMin, 3), max: Math.Round(xMax, 3), binSize: 0.01);
            //double[] xLeftEdges = xBinEdges.Take(xBinEdges.Length - 1).ToArray();

            var xHistogram = new SortedDictionary<double, int>();
            foreach (double x in xOffset)
            {
                if (xHistogram.ContainsKey(x))
                {
                    xHistogram[x]++;
                }
                else
                {
                    xHistogram[x] = 1;
                }
            }
            var xCountsList = new List<double>();
            var xBinEdgesList = new List<double>();
            foreach (KeyValuePair<double, int> pair in xHistogram)
            {
                xCountsList.Add(pair.Value);
                xBinEdgesList.Add(pair.Key);
            }
            xBinEdgesList.Add(0);
            double[] xCounts = xCountsList.ToArray();
            double[] xBinEdges = xBinEdgesList.ToArray();
            double[] xLeftEdges = xBinEdges.Take(xBinEdges.Length - 1).ToArray();

            _lollipopPlotX = FormsPlotHistogram.Plot.AddLollipop(values: xCounts, positions: xLeftEdges, color: Color.Green);
            _lollipopPlotX.Label = "x 偏移值";
            _lollipopPlotX.BarWidth = 0.001;
            //_lollipopPlotX.ShowValuesAboveBars = true;

            #endregion

            #region HistogramPlot for y offset

            //(double[] yCounts, double[] yBinEdges) = Common.Histogram(values: yOffset, min: yMin, max: yMax, binSize: 0.01);
            //double[] yLeftEdges = yBinEdges.Take(yBinEdges.Length - 1).ToArray();

            var yHistogram = new SortedDictionary<double, int>();
            foreach (double y in yOffset)
            {
                if (yHistogram.ContainsKey(y))
                {
                    yHistogram[y]++;
                }
                else
                {
                    yHistogram[y] = 1;
                }
            }
            var yCountsList = new List<double>();
            var yBinEdgesList = new List<double>();
            foreach (KeyValuePair<double, int> pair in yHistogram)
            {
                yCountsList.Add(pair.Value);
                yBinEdgesList.Add(pair.Key);
            }
            yBinEdgesList.Add(0);
            double[] yCounts = yCountsList.ToArray();
            double[] yBinEdges = yBinEdgesList.ToArray();
            double[] yLeftEdges = yBinEdges.Take(yBinEdges.Length - 1).ToArray();

            _lollopopPlotY = FormsPlotHistogram.Plot.AddLollipop(values: yCounts, positions: yLeftEdges, color: Color.Blue);
            _lollopopPlotY.Label = "y 偏移值";
            _lollopopPlotY.BarWidth = 0.001;
            _lollopopPlotY.ShowValuesAboveBars = true;

            #endregion

            #region Vline for x offset

            double xAvg = xOffset.Average();
            VLine xVlineMean = FormsPlotHistogram.Plot.AddVerticalLine(
                x: xAvg,
                color: Color.Green,
                width: 0.01F,
                style: LineStyle.Dot,
                label: $"x 偏移平均值( {xAvg:0.###} mm)");
            xVlineMean.PositionLabelBackground = Color.Green;
            xVlineMean.PositionFormatter = x => $"{x:0.###}";

            //xVlineMean.PositionLabel = true;
            //VLine xVlineMin = FormsPlotHistogram.Plot.AddVerticalLine(x: xMin, color: Color.Green, width: 0.01F, style: LineStyle.Dash, label: "x 偏移最小/最大值");
            //xVlineMin.PositionLabelBackground = Color.Gray;
            //xVlineMin.PositionFormatter = x => $"{x:0.###}";
            //xVlineMin.PositionLabel = true;
            //VLine xVlineMax = FormsPlotHistogram.Plot.AddVerticalLine(x: xMax, color: Color.Green, width: 0.01F, style: LineStyle.Dash);
            //xVlineMax.PositionLabelBackground = Color.Gray;
            //xVlineMax.PositionLabel = true;
            //xVlineMax.PositionFormatter = x => $"{x:0.###}";

            #endregion

            #region Vline for y offset

            double yAvg = yOffset.Average();
            VLine yVlineMean = FormsPlotHistogram.Plot.AddVerticalLine(
                x: yAvg,
                color: Color.Blue,
                width: 0.01F,
                style: LineStyle.Dot,
                label: $"y 偏移平均值( {yAvg:0.###} mm)");
            yVlineMean.PositionLabelBackground = Color.Blue;
            yVlineMean.PositionFormatter = x => $"{x:0.###}";

            //yVlineMean.PositionLabel = true;
            //VLine yVlineMin = FormsPlotHistogram.Plot.AddVerticalLine(x: yMin, color: Color.Gray, width: 0.01F, style: LineStyle.Dash, label: "最小/最大值");
            //yVlineMin.PositionLabelBackground = Color.Gray;
            //yVlineMin.PositionFormatter = x => $"{x:0.###}";
            //yVlineMin.PositionLabel = true;
            //VLine yVlineMax = FormsPlotHistogram.Plot.AddVerticalLine(x: yMax, color: Color.Gray, width: 0.01F, style: LineStyle.Dash);
            //yVlineMax.PositionLabelBackground = Color.Gray;
            //yVlineMax.PositionLabel = true;
            //yVlineMax.PositionFormatter = x => $"{x:0.###}";

            #endregion

            #region Text for x offet

            // 文字在指定位置。
            Text xMinText = FormsPlotHistogram.Plot.AddText($"{xMin:0.###}", xMin, xCounts[0], size: 14, color: Color.Green);
            // 點在文字的左下角。
            xMinText.Alignment = Alignment.UpperLeft;
            xMinText.Font.Bold = true;

            // 文字在指定位置。
            Text xMaxText = FormsPlotHistogram.Plot.AddText($"{xMax:0.###}", xMax, xCounts[xCounts.Length - 1], size: 14, color: Color.Green);
            // 點在文字的左下角。
            xMaxText.Alignment = Alignment.UpperLeft;
            xMaxText.Font.Bold = true;

            #endregion

            #region Text for y offet

            // 文字在指定位置。
            Text yMinText = FormsPlotHistogram.Plot.AddText($"{yMin:0.###}", yMin, yCounts[0], size: 14, color: Color.Blue);
            // 點在文字的左下角。
            yMinText.Alignment = Alignment.UpperLeft;
            yMinText.Font.Bold = true;

            // 文字在指定位置。
            Text yMaxText = FormsPlotHistogram.Plot.AddText($"{yMax:0.###}", yMax, yCounts[yCounts.Length - 1], size: 14, color: Color.Blue);
            // 點在文字的左下角。
            yMaxText.Alignment = Alignment.UpperLeft;
            yMaxText.Font.Bold = true;

            #endregion

            Legend legend = FormsPlotHistogram.Plot.Legend(location: Alignment.UpperLeft);
            legend.FontSize = 16;

            // 限制 x、y 軸刻度的上、下限。
            double xCountsMax = xCounts.Max();
            double yCountsMax = yCounts.Max();
            FormsPlotHistogram.Plot.SetAxisLimits(yMin: 0, yMax: (xCountsMax > yCountsMax ? xCountsMax : yCountsMax) * 1.3);
            FormsPlotHistogram.Refresh();
        }

        #endregion
    }
}
