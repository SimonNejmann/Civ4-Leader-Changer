using System.Windows;

namespace Civ4_Leader_Changer
{
    public partial class MainWindow : Window
    {
        // Create an instance of WorldbuilderParser
        WorldbuilderParser parser = new WorldbuilderParser();

        public MainWindow()
        {
            InitializeComponent();
            // Set the parser as the DataContext for this window
            this.DataContext = parser;
        }

        // On-Click event handler for the "Load" button
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            // Prepare an Open File dialog box with the right filetype filters and show it
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".CivBeyondSwordWBSave";
            dlg.Filter = "Worldbuilder Save (*.CivBeyondSwordWBSave)|*.CivBeyondSwordWBSave|All files|*.*";
            var res = dlg.ShowDialog();

            if (res.HasValue && res.Value)
            {
                // If a file was chosen in the dialog box, then try to parse it
                var success = parser.ParseWorldbuilderSave(dlg.FileName);
                if (!success)
                {
                    // It the parser failed, then display an error message in a message box
                    MessageBox.Show($"Error: Could not read file {dlg.FileName}", "Error reading file", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // Run through the list of leaders that were just grabbed from the save file
                foreach (WorldbuilderLeader l in parser.leaders)
                {
                    // Skip the ones with type "NONE"
                    if (l.Type == LeaderType.NONE)
                        continue;
                    // Add a custom panel for each of the rest - each panel gets a reference to the leader it needs to display
                    LeaderPanel p = new LeaderPanel(l);
                    LeaderStack.Children.Add(p);
                }
            }
        }

        // On-Click event handler for the "Save" button
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Prepare a Save File dialog box with the right filetype filters and show it
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".CivBeyondSwordWBSave";
            dlg.Filter = "Worldbuilder Save (*.CivBeyondSwordWBSave)|*.CivBeyondSwordWBSave|All files|*.*";
            var res = dlg.ShowDialog();

            if (res.HasValue && res.Value)
            {
                // If a file were chosen, then save into it
                parser.WriteWorldbuilderSave(dlg.FileName);
            }
        }


        // On-Click event handlers for the two utility/shortcut buttons

        private void DeityTechs_Click(object sender, RoutedEventArgs e)
        {
            // Add the Deity difficulty bonus techs to all the leaders
            parser.AddDeityTechs();
        }
        private void DefaultTechs_Click(object sender, RoutedEventArgs e)
        {
            // Reset all the leaders to only their two default techs
            parser.SetDefaultTechs();
        }
    }
}
