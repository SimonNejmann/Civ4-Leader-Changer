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

        // A LeaderPanel is constructed from a WorldbuilderLeader - so store the leader and set it as the DataContext for the panel
        public LeaderPanel(WorldbuilderLeader leader)
        {
            this.leader = leader;
            this.DataContext = this.leader;
            InitializeComponent();
            AddCheckBoxes();
        }

        // Make sure the color rectangles are updated correctly: When stuff is first created, and when the ColorBox (ComboBox) selection is changed
        private void ColorBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { UpdateRectangles(); }
        private void BgRect_Initialized(object sender, EventArgs e) { UpdateRectangles(); }
        private void OutRect_Initialized(object sender, EventArgs e) { UpdateRectangles(); }
        private void ColorBox_Initialized(object sender, EventArgs e) { UpdateRectangles(); }

        // Update the color rectangles - but make sure to hold until everything has been initialized (!= null)
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

        // Create the starting tech checkboxes from the "Tech" enum and "leader.Techs"
        private void AddCheckBoxes()
        {
            // Instantiate a converter (flag enum entry <=> bool, see below)
            var converter = new FlagsEnumValueConverter();
            // Run through the enum entries
            foreach (Tech t in Enum.GetValues(typeof(Tech)))
            {
                // Except for Tech.NONE
                if (t != Tech.NONE)
                {

                    // Create a data binding
                    var binding = new Binding()
                    {
                        // Link the binding to "leader.Techs" ("leader" is the DataContext, so "Techs" looks there)
                        Path = new PropertyPath("Techs"),
                        // Add the converter instance
                        Converter = converter,
                        // Make sure the converter gets this enum entry as the parameter for this binding
                        ConverterParameter = t
                    };

                    // Create a CheckBox
                    var checkBox = new CheckBox()
                    {
                        // Wrap the text in a TextBlock - otherwise "THE_WHEEL" would become "THEWHEEL" with 'W' as the alt-shortcut
                        Content = new TextBlock() { Text = Enum.GetName(typeof(Tech), t) }
                    };

                    // Bind the CheckBox to the binding we made
                    checkBox.SetBinding(CheckBox.IsCheckedProperty, binding);
                    // Add the CheckBox to the StackPanel
                    TechPanel.Children.Add(checkBox);
                }
            }
        }

        // A converter that converts flag enum entries into boolean values
        public class FlagsEnumValueConverter : IValueConverter
        {
            // Save the combined enum value in "Convert" so that "ConvertBack" can work properly
            private int targetValue;

            public FlagsEnumValueConverter()
            {
            }

            // Convert: Enum entry => bool
            // value has the combined enum value
            // parameter has the exact enum entry what we want to see if it is set in "value"
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                // Cast parameter to int so we can work with it
                int mask = (int)parameter;
                // Cast value to int and store it for later
                this.targetValue = (int)value;

                // Now we can test if the specific entry is set in the combined value
                return ((mask & this.targetValue) != 0);
            }

            // Convert: Bool => enum entry
            // value now has a bool describing if the specific enum entry shoud be on or not. But as this only triggers
            //   when there is a change, we know that the entry will always wind up being toggled. Thus this is actually not needed
            // parameter again has the specific enum entry that needs to be toggled
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                // Toggle the entry in the stored value
                this.targetValue ^= (int)parameter;
                // Convert the result to an enum of the correct type and return it
                return Enum.Parse(targetType, this.targetValue.ToString());
            }
        }
    }
}
