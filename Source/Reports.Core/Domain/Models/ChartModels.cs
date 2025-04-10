using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Core.Domain.Models
{
    public class HighChartsModel
    {
        public HighChartsModel()
        {
            this.Categories = new List<String>();

            this.ChartType = "column";

            this.LegendLayout = "vertical";
            this.LegendAlign = "right";
            this.LegendVerticalAlign = "top";
            this.LegendX = -5;
            this.LegendY = 20;
            this.ChartHeight = 400;
            this.BorderColor = "#C1C1C1";
            this.XAxisColor = "#C1C1C1";
            this.YAxisColor = "#C1C1C1";

            this.PlotShadow = true;
            this.BackgroundColor = "#FFFFFF";
            this.BorderWidth = 0;
            this.PaddingLeft = 0;
            this.PaddingRight = 0;
            this.PaddingTop = 0;
            this.PaddingBottom = 0;
            this.PointWidth = 20;
            this.TitleColor = "#434343";
            this.SubTitleColor = "#4a4a4a";
            this.YAxis1LabelFormat = "{value}";
            this.YAxis2LabelFormat = "{value}";
        }

        public String ChartType { get; set; }
        public String BorderColor { get; set; }
        public String XAxisColor { get; set; }
        public String YAxisColor { get; set; }
        public Int32 BorderWidth { get; set; }
        public String BackgroundColor { get; set; }
        public Int32 PaddingLeft { get; set; }
        public Int32 PaddingRight { get; set; }
        public Int32 PaddingTop { get; set; }
        public Int32 PaddingBottom { get; set; }
        public Boolean PlotShadow { get; set; }
        public String ContainerId { get; set; }
        public String Title { get; set; }
        public String SubTitle { get; set; }
        public String DataSeriesName { get; set; }
        public Boolean ShowLegend { get; set; }
        public String LegendLayout { get; set; }
        public String LegendAlign { get; set; }
        public String LegendVerticalAlign { get; set; }
        public Int32 LegendX { get; set; }
        public Int32 LegendY { get; set; }
        public String YAxis1Title { get; set; }
        public String YAxis1LabelFormat { get; set; }
        public String YAxis2Title { get; set; }
        public String YAxis2LabelFormat { get; set; }
        public List<String> Categories { get; set; }
        public Boolean HideXAxis { get; set; }
        public Boolean HideYAxis { get; set; }
        public Boolean HideGridLines { get; set; }
        public Int32 ChartWidth { get; set; }
        public Int32 ChartHeight { get; set; }
        public Boolean ShowTitle { get; set; }
        public Boolean ShowSubTitle { get; set; }
        public Int32 PointWidth { get; set; }
        public String TitleColor { get; set; }
        public String SubTitleColor { get; set; }
        public Boolean ShowPercentage { get; set; }
        public Boolean ShowDollarSign { get; set; }
        public Boolean ShowDataLabels { get; set; }
        public String DataLabelColor { get; set; }
        public Boolean HasFilter { get; set; }
        public String FilterDataColumn { get; set; }
    }

    public class StackedColumnChartModel : HighChartsModel
    {
        public StackedColumnChartModel()
        {
            this.DataSeries = new List<StackedColumnChartDataSeriesModel>();
        }

        public List<StackedColumnChartDataSeriesModel> DataSeries { get; set; }
    }

    public class StackedColumnChartDataSeriesModel
    {
        public StackedColumnChartDataSeriesModel()
        {
            this.Data = new List<Int32>();
        }

        public String ColumnColor { get; set; }
        public List<Int32> Data { get; set; }
        public String Name { get; set; }
        public Boolean ShowInLegend { get; set; }
        public String Stack { get; set; }
    }

    public class MultiColumnChartModel : HighChartsModel
    {
        public MultiColumnChartModel()
        {
            this.DataSeries = new MultiColumnChartDataSeriesModel();
        }

        public MultiColumnChartDataSeriesModel DataSeries { get; set; }
    }

    public class MultiColumnChartDataSeriesModel
    {
        public MultiColumnChartDataSeriesModel()
        {
            this.Items = new List<MultiColumnChartDataSeries>();
        }

        public List<MultiColumnChartDataSeries> Items { get; set; }

        public void Add(String name, List<MultiColumnChartDataSerieValue> values)
        {
            this.Items.Add(new MultiColumnChartDataSeries(name, values));
        }

        public void Add(String name, Boolean hidden, List<MultiColumnChartDataSerieValue> values)
        {
            this.Items.Add(new MultiColumnChartDataSeries(name, hidden, values));
        }

        public void Add(String name, String serieType, List<MultiColumnChartDataSerieValue> values)
        {
            this.Items.Add(new MultiColumnChartDataSeries(name, serieType, values));
        }

        public void Add(String name, String serieType, String color, List<MultiColumnChartDataSerieValue> values)
        {
            this.Items.Add(new MultiColumnChartDataSeries(name, serieType, color, values));
        }

        public void Add(String name, String serieType, String color, Boolean hidden, List<MultiColumnChartDataSerieValue> values)
        {
            this.Items.Add(new MultiColumnChartDataSeries(name, serieType, color, hidden, values));
        }
    }

    public class MultiColumnChartDataSeries
    {
        public MultiColumnChartDataSeries()
        {
            this.Values = new List<MultiColumnChartDataSerieValue>();
            this.yAxis = 1;
        }

        public MultiColumnChartDataSeries(String name, List<MultiColumnChartDataSerieValue> values)
        {
            this.Name = name;
            this.Values = values;
            this.Hidden = false;
            this.yAxis = 1;
        }

        public MultiColumnChartDataSeries(String name, Boolean hidden, List<MultiColumnChartDataSerieValue> values)
        {
            this.Name = name;
            this.Values = values;
            this.Hidden = hidden;
            this.yAxis = 1;
        }

        public MultiColumnChartDataSeries(String name, String serieType, List<MultiColumnChartDataSerieValue> values)
        {
            this.Name = name;
            this.SerieType = serieType;
            this.Values = values;
            this.Hidden = false;
            this.yAxis = 1;
        }

        public MultiColumnChartDataSeries(String name, String serieType, String color, List<MultiColumnChartDataSerieValue> values)
        {
            this.Name = name;
            this.SerieType = serieType;
            this.Values = values;
            this.Color = color;
            this.Hidden = false;
            this.yAxis = 1;
        }

        public MultiColumnChartDataSeries(String name, String serieType, String color, Boolean hidden, List<MultiColumnChartDataSerieValue> values)
        {
            this.Name = name;
            this.SerieType = serieType;
            this.Values = values;
            this.Color = color;
            this.Hidden = hidden;
            this.yAxis = 1;
        }

        public String Name { get; set; }
        public List<MultiColumnChartDataSerieValue> Values { get; set; }
        public String SerieType { get; set; }
        public Int32 yAxis { get; set; }
        public String Color { get; set; }
        public Boolean Hidden { get; set; }
        public String DashStyle { get; set; }
    }

    public class MultiColumnChartDataSerieValue {
        public float Value { get; set; }
        public String Color { get; set; }
    }

    public class PieChartModel : HighChartsModel
    {
        public PieChartModel()
        {
            this.DataSeries = new List<PieChartDataSeries>();
            this.DrillDownDataSeries = new List<PieChartDrilldown>();
            this.InnerSizePercentage = 0;
        }

        public List<PieChartDataSeries> DataSeries { get; set; }
        public List<PieChartDrilldown> DrillDownDataSeries { get; set; }
        public Int32 PieSize { get; set; }
        public Byte InnerSizePercentage { get; set; }
    }

    public class ChartDataSerie
    {
        public ChartDataSerie()
        {

        }

        public ChartDataSerie(String name, float value)
        {
            this.Name = name;
            this.Value = value;
            this.Color = String.Empty;
        }

        public ChartDataSerie(String name, float value, String color)
        {
            this.Name = name;
            this.Value = value;
            this.Color = color;
        }

        public String Name { get; set; }
        public float Value { get; set; }
        public String Color { get; set; }
    }

    public class PieChartDataSeries : ChartDataSerie
    {
        public String DrillDownId { get; set; }

        public PieChartDataSeries()
        {
            this.DrillDownId = "null";
        }

        public PieChartDataSeries(String name, float value, String color)
        {
            this.Name = name;
            this.Value = value;
            this.Color = color;
            this.DrillDownId = "null";
        }

        public PieChartDataSeries(String name, float value, String color, String drilldownId)
        {
            this.Name = name;
            this.Value = value;
            this.Color = color;
            this.DrillDownId = drilldownId;
        }
    }
    public class PieChartDrilldown
    {
        public PieChartDrilldown()
        {
            this.DataSeries = new List<ChartDataSerie>();
        }

        public PieChartDrilldown(String name, String id)
        {
            this.Name = name;
            this.Id = id;

            this.DataSeries = new List<ChartDataSerie>();
        }
        public String Name { get; set; }
        public String Id { get; set; }

        public List<ChartDataSerie> DataSeries { get; set; }
    }

    public class WordCloudChartModel : HighChartsModel
    {
        public List<WordCloudChartData> DataSeries { get; set; }
    }

    public class WordCloudChartData
    {
        public String Name { get; set; }
        public Double Weight { get; set; }
        public String Color { get; set; }
    }

    public class DataSeriesModel
    {
        public DataSeriesModel()
        {
            this.Data = new Dictionary<String, Int32>();
        }

        public String ColumnColor { get; set; }
        public Dictionary<String, Int32> Data { get; set; }
        public String Name { get; set; }
        public Boolean ShowInLegend { get; set; }
    }

    public class SolidGaugeChartModel : ChartModel
    {
        public SolidGaugeChartModel()
        {
            this.IsPercentage = true;
            this.ShowRounderCorners = false;
            this.showTooltip = true;
        }

        public List<String> BgColors { get; set; }
        public Boolean CenterTooltop { get; set; }
        public Boolean TooltipAlwaysOn { get; set; }
        public Boolean IsPercentage { get; set; }
        public Boolean ShowRounderCorners { get; set; }
        public float? Value { get; set; }
        public Boolean showTooltip { get; set; }
    }


    public class ChartModel
    {
        public ChartModel()
        {
            this.DataSeries = new Dictionary<String, Int32>();
            this.Size = 200;
            this.LegendX = -5;
            this.LegendY = 20;
            this.ChartBackgroundColor = "#ffffff";
            this.TitleColor = "#434343";
            this.SubTitleColor = "#4a4a4a";
        }

        public String ChartBackgroundColor { get; set; }
        public String ContainerId { get; set; }
        public String Title { get; set; }
        public String SubTitle { get; set; }
        public String DataSeriesName { get; set; }
        public Dictionary<String, Int32> DataSeries { get; set; }
        public List<String> Colors { get; set; }
        public Boolean ShowLegend { get; set; }
        public Boolean ShowTitle { get; set; }
        public Boolean ShowSubTitle { get; set; }
        public String YAxisTitle { get; set; }
        public Int32 Size { get; set; }
        public Int32 LegendX { get; set; }
        public Int32 LegendY { get; set; }
        public Int32 Width { get; set; }
        public String TitleColor { get; set; }
        public String SubTitleColor { get; set; }
    }

    public class MultiPieChartModel
    {
        public MultiPieChartModel()
        {
            this.DataSeries = new List<DataSeriesModel>();

            this.TitleColor = "#434343";
            this.SubTitleColor = "#4a4a4a";
        }

        public String ContainerId { get; set; }
        public String Title { get; set; }
        public String SubTitle { get; set; }
        public String DataSeriesName { get; set; }
        public List<DataSeriesModel> DataSeries { get; set; }
        public Boolean ShowLegend { get; set; }
        public Boolean ShowTitle { get; set; }
        public Boolean ShowSubTitle { get; set; }
        public String YAxisTitle { get; set; }
        public String TitleColor { get; set; }
        public String SubTitleColor { get; set; }
    }

    public class ComparisonGraphModel
    {
        public ComparisonGraphModel()
        {
            this.DataSeries1 = new Dictionary<String, Int32>();
            this.DataSeries2 = new Dictionary<String, Int32>();
        }

        public Dictionary<String, Int32> DataSeries1 { get; set; }
        public Dictionary<String, Int32> DataSeries2 { get; set; }
        public String DataSeries1Name { get; set; }
        public String DataSeries2Name { get; set; }

    }
}
