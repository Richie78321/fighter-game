using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FighterGame.Map;

namespace MapMaker
{
    public partial class CompileSelector : Form
    {
        private string mapFileDirectory;
        private string mapFileExtension;
        private bool onlyOneSelection;
        public CompileSelector(string mapFileDirectory, string mapFileExtension, bool onlyOneSelection = false)
        {
            InitializeComponent();

            this.mapFileDirectory = mapFileDirectory;
            this.mapFileExtension = mapFileExtension;
            this.onlyOneSelection = onlyOneSelection;
        }

        private void CompileSelector_Load(object sender, EventArgs e)
        {
            //Add close function
            this.FormClosing += CompileSelector_FormClosing;

            //Load map files into selection
            IEnumerable<string> mapFileNames = Directory.EnumerateFiles(Directory.GetCurrentDirectory() + "/" + mapFileDirectory);
            foreach (string fileName in mapFileNames)
            {
                //Ensure correct file type
                if (fileName.Contains(mapFileExtension))
                {
                    //Add name
                    mapCheckList.Items.Add(fileName.Substring(fileName.LastIndexOf('\\') + 1).TrimEnd(mapFileExtension.ToCharArray()));
                }
            }
        }

        private void CompileSelector_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Set dialogue result
            if (selectedMapFiles == null || selectedMapFiles.Length <= 0) DialogResult = DialogResult.No;
            else DialogResult = DialogResult.Yes;
        }

        public MapArchitecture[] selectedMapFiles = null;
        private void confirmationButton_Click(object sender, EventArgs e)
        {
            List<MapArchitecture> deserializedMapFiles = new List<MapArchitecture>();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            for (int i = 0; i < mapCheckList.Items.Count; i++)
            {
                //Add deserialized map file if checked
                if (mapCheckList.GetItemChecked(i))
                {
                    //Deserialize object
                    object deserializedObject = binaryFormatter.Deserialize(new MemoryStream(File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + mapFileDirectory + "/" + mapCheckList.Items[i] + mapFileExtension)));
                    
                    //Ensure that the object is of the correct type
                    if (deserializedObject is MapArchitecture) deserializedMapFiles.Add((MapArchitecture)deserializedObject);
                }
            }

            //Set array
            selectedMapFiles = deserializedMapFiles.ToArray();
            Close();
        }

        private void itemCheck(object sender, ItemCheckEventArgs e)
        {
            if (onlyOneSelection && e.NewValue == CheckState.Checked) for (int i = 0; i < mapCheckList.Items.Count; i++) if (i != e.Index) mapCheckList.SetItemCheckState(i, CheckState.Unchecked);
        }
    }
}
