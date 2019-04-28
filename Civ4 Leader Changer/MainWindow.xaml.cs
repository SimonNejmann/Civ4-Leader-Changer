using System.Windows;

namespace Civ4_Leader_Changer
{
    public partial class MainWindow : Window
    {
        WorldbuilderParser parser = new WorldbuilderParser();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = parser;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".CivBeyondSwordWBSave";
            dlg.Filter = "Worldbuilder Save (*.CivBeyondSwordWBSave)|*.CivBeyondSwordWBSave|All files|*.*";
            var res = dlg.ShowDialog();

            if (res.HasValue && res.Value)
            {
                var success = parser.ParseWorldbuilderSave(dlg.FileName);
                if (!success)
                {
                    MessageBox.Show($"Error: Could not read file {dlg.FileName}", "Error reading file", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                foreach (WorldbuilderLeader l in parser.leaders)
                {
                    if (l.Type == LeaderType.NONE)
                        continue;
                    LeaderPanel p = new LeaderPanel(l);
                    LeaderStack.Children.Add(p);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".CivBeyondSwordWBSave";
            dlg.Filter = "Worldbuilder Save (*.CivBeyondSwordWBSave)|*.CivBeyondSwordWBSave|All files|*.*";
            var res = dlg.ShowDialog();

            if (res.HasValue && res.Value)
            {
                parser.WriteWorldbuilderSave(dlg.FileName);
            }
        }

        private void DeityTechs_Click(object sender, RoutedEventArgs e)
        {
            parser.AddDeityTechs();
        }

        private void DefaultTechs_Click(object sender, RoutedEventArgs e)
        {
            parser.SetDefaultTechs();
        }
    }
}
