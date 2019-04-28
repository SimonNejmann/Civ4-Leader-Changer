using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Civ4_Leader_Changer
{
    public partial class LeaderPanel : UserControl
    {
        WorldbuilderLeader leader;

        public LeaderPanel(WorldbuilderLeader leader)
        {
            this.leader = leader;
            this.DataContext = this.leader;
            InitializeComponent();
            AddCheckBoxes();
        }

        private void ColorBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { UpdateRectangles(); }
        private void BgRect_Initialized(object sender, EventArgs e) { UpdateRectangles(); }
        private void OutRect_Initialized(object sender, EventArgs e) { UpdateRectangles(); }
        private void ColorBox_Initialized(object sender, EventArgs e) { UpdateRectangles(); }

        private void UpdateRectangles()
        {
            if (ColorBox != null)
            {
                if (bgRect != null)
                    bgRect.Fill = PlayerColors.getBackground((PlayerColor)ColorBox.SelectedItem);

                if (outRect != null)
                    outRect.Fill = PlayerColors.getOutline((PlayerColor)ColorBox.SelectedItem);
            }
        }

        private void AddCheckBoxes()
        {
            var converter = new FlagsEnumValueConverter();
            foreach (Tech t in Enum.GetValues(typeof(Tech)))
            {
                if (t != Tech.NONE)
                {
                    var binding = new Binding()
                    {
                        Path = new PropertyPath("Techs"),
                        Converter = converter,
                        ConverterParameter = t
                    };

                    var checkBox = new CheckBox()
                    {
                        Content = new TextBlock() { Text = Enum.GetName(typeof(Tech), t) }
                    };

                    checkBox.SetBinding(CheckBox.IsCheckedProperty, binding);
                    TechPanel.Children.Add(checkBox);
                }
            }
        }

        public class FlagsEnumValueConverter : IValueConverter
        {
            private int targetValue;

            public FlagsEnumValueConverter()
            {
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                int mask = (int)parameter;
                this.targetValue = (int)value;

                return ((mask & this.targetValue) != 0);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                this.targetValue ^= (int)parameter;
                return Enum.Parse(targetType, this.targetValue.ToString());
            }
        }
    }
}
