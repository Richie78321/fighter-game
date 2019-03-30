using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FighterGame.Map;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MapMaker
{
    public partial class Form1 : Form
    {
        private const string MAP_FILE_DIRECTORY = "MapFiles";
        private const string MAP_FILE_EXTENSION = ".bin";

        //Object
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Ensure existence of directory
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/" + MAP_FILE_DIRECTORY);

            //Set max size
            platformSizeNumeric.Maximum = MapStandards.MAP_TILE_DIMENSIONS - 1;
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty
            | BindingFlags.Instance | BindingFlags.NonPublic, null,
            mapPanel, new object[] { true });
        }

        private void platformButton_CheckedChanged(object sender, EventArgs e)
        {
            platformOptionPanel.Visible = platformButton.Checked;
        }

        private void mapPanelPaint(object sender, PaintEventArgs e)
        {
            //Draw map architecture
            Platform[] mapPlatforms = mapArchitecture.MapPlatforms;
            for (int i = 0; i < mapPlatforms.Length; i++)
            {
                for (int j = 0; j < mapPlatforms[i].TileLength; j++)
                {
                    RectangleF relativePlatformRectangle = mapPlatforms[i].RelativeRectangle((float)mapPanel.Width / MapStandards.MAP_SIZE);
                    e.Graphics.DrawRectangle(Pens.Black, relativePlatformRectangle.X, relativePlatformRectangle.Y, relativePlatformRectangle.Width, relativePlatformRectangle.Height);
                }
            }
            
            //Check if mouse should be drawn
            if (platformButton.Checked && mapPanel.ClientRectangle.IntersectsWith(new Rectangle(mapPanel.PointToClient(MousePosition), new Size((int)Math.Ceiling(MapStandards.TILE_SIZE * mapPanel.Width) * (int)platformSizeNumeric.Value, (int)Math.Ceiling(MapStandards.TILE_SIZE * mapPanel.Width)))))
            {
                Point clientPoint = mapPanel.PointToClient(MousePosition);

                //Draw potential platform 
                for (int i = 0; i < (int)platformSizeNumeric.Value; i++)
                {
                    e.Graphics.DrawRectangle(Pens.Blue, clientPoint.X + (MapStandards.TILE_SIZE * (mapPanel.Width / MapStandards.MAP_SIZE) * i), clientPoint.Y, MapStandards.TILE_SIZE * (mapPanel.Width / MapStandards.MAP_SIZE), MapStandards.TILE_SIZE * (mapPanel.Width / MapStandards.MAP_SIZE));
                }
            }
        }

        private void drawTimer_Tick(object sender, EventArgs e)
        {
            mapPanel.Invalidate();
        }

        private MapArchitecture mapArchitecture = new MapArchitecture();
        private void mapPanelMouseClick(object sender, MouseEventArgs e)
        {
            //Handle
            if (platformButton.Checked)
            {
                //Attempt to add new platform
                mapArchitecture.AddPlatform(new Platform(new Microsoft.Xna.Framework.Vector2((float)mapPanel.PointToClient(MousePosition).X / (mapPanel.Width / MapStandards.MAP_SIZE), (float)mapPanel.PointToClient(MousePosition).Y / (mapPanel.Width / MapStandards.MAP_SIZE)), (int)platformSizeNumeric.Value));
            }
        }

        private void compileMapFileButton_Click(object sender, EventArgs e)
        {
            //Select map files
            CompileSelector compileSelector = new CompileSelector(MAP_FILE_DIRECTORY, MAP_FILE_EXTENSION);
            if (compileSelector.ShowDialog() == DialogResult.Yes)
            {
                //Serialize array
                MemoryStream serializationStream = new MemoryStream();
                new BinaryFormatter().Serialize(serializationStream, compileSelector.selectedMapFiles);
                byte[] buffer = serializationStream.GetBuffer();
                Array.Resize(ref buffer, (int)serializationStream.Length);

                //Write serialized array to file
                File.WriteAllBytes(Directory.GetCurrentDirectory() + "/" + MapStandards.MAP_COLLECTION_FILE_NAME + MAP_FILE_EXTENSION, buffer);

                MessageBox.Show("Map collection file compiled with " + compileSelector.selectedMapFiles.Length + " maps.");
            }
        }

        private const string INVALID_MAP_NAME_MSG = "Failed to Save: Invalid Map File Name";
        private void saveMapButton_Click(object sender, EventArgs e)
        {
            //Verify map name
            if (!string.IsNullOrWhiteSpace(mapNameTextBox.Text))
            {
                //Determine if overwrite
                if (File.Exists(Directory.GetCurrentDirectory() + "/" + MAP_FILE_DIRECTORY + "/" + mapNameTextBox.Text + MAP_FILE_EXTENSION) && MessageBox.Show("A map file already exists with this file name. Would you like to overwrite it?", "Overwrite Map File", MessageBoxButtons.YesNo) == DialogResult.No) return;
                else
                {
                    //Save map file
                    MemoryStream serializationStream = new MemoryStream();
                    new BinaryFormatter().Serialize(serializationStream, mapArchitecture);
                    byte[] buffer = serializationStream.GetBuffer();
                    Array.Resize(ref buffer, (int)serializationStream.Length);
                    File.WriteAllBytes(Directory.GetCurrentDirectory() + "/" + MAP_FILE_DIRECTORY + "/" + mapNameTextBox.Text + MAP_FILE_EXTENSION, buffer);

                    MessageBox.Show("Map saved.");
                }
            }
            else MessageBox.Show(INVALID_MAP_NAME_MSG);
        }

        private void loadMapFileButton_Click(object sender, EventArgs e)
        {
            //Run compile selector
            CompileSelector compileSelector = new CompileSelector(MAP_FILE_DIRECTORY, MAP_FILE_EXTENSION, onlyOneSelection: true);
            if (compileSelector.ShowDialog() == DialogResult.Yes)
            {
                //Selected successfully
                mapArchitecture = compileSelector.selectedMapFiles[0];
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            mapArchitecture = new MapArchitecture();
        }
    }
}
