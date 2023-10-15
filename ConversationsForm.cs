using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HiredGunsData;
using System.IO;

namespace Hired_Guns_3_Map_Editor
{
    public partial class ConversationsForm : Form
    {
        private MainForm _mainForm;
        public MainForm Mainform
        {
            get
            {
                return _mainForm;
            }
            set
            {
                _mainForm = value;
            }
        }
        private List<ConversationCombo> convCombos = new List<ConversationCombo>();
        public ConversationsForm(MainForm mainForm)
        {
            Mainform = mainForm;
            //Mainform.Enabled = false;
            InitializeComponent();
            UpdateConversations(Mainform.GetMapData());
        }
        private void ConversationsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Mainform == null)
            {
                //Close();
                //return;
            }
            //Mainform.Enabled = true;
        }
        public void UpdateConversations(HGMapData mapData)
        {
            if(mapData == null)
            {
                return;
            }
            int convAmount = mapData.CountConversations();
            if(convCombos.Count < convAmount)
            {
                convCombos.AddRange(new ConversationCombo[convAmount - convCombos.Count]);
            }
            else if (convCombos.Count > convAmount)
            {
                for(int i = convAmount; i < convCombos.Count - convAmount; i++)
                {
                    Controls.Remove(convCombos[i].indexLabel);
                    Controls.Remove(convCombos[i].btn);
                    Controls.Remove(convCombos[i].textBox);
                }
                convCombos.RemoveRange(mapData.conversations.Length, convCombos.Count - convAmount);
            }
            Mainform.SetBlockHandlerKeep();
            for(int i = 0; i < convAmount; i++)
            {
                HGConversation conv = mapData.conversations[i];
                ConversationCombo convCombo = convCombos[i];
                if(convCombo == null || convCombo.btn == null)
                {
                    convCombo = new ConversationCombo(i,ConversationsPanel);
                    convCombos[i] = convCombo;
                    convCombo.btn.Click += new EventHandler(ConvComboButton_Click);
                    EditorToolTip.SetToolTip(convCombo.btn, "Click to set the selected cell position as this conversation's position.");
                    convCombo.textBox.TextChanged += new EventHandler(ConversationTextBox_TextChanged);
                }
                convCombo.btn.Text = $"{conv.coords}";
                convCombo.textBox.Text = $"{conv.text}";
            }
            Mainform.SetBlockHandlerKeep(false);
            UpdateAddDeleteConvButtons();
            Invalidate();
        }
        private void ConversationTextBox_TextChanged(object sender, EventArgs e)
        {
            HGMapData mapData = Mainform.GetMapData();
            if (mapData == null)
            {
                return;
            }
            ConversationTextBox c = (ConversationTextBox)sender;
            if (c == null)
            {
                return;
            }
            //MessageBox.Show($"[ConversationTextBox_TextChanged] index: {c.index}, length: {mapData.CountConversations()}");
            if(_mainForm.blockHandlerKeep == false)
            {
                mapData.conversations[c.index].text = c.Text;
            }
        }
        private void ConvComboButton_Click(object sender, EventArgs e)
        {
            if (Mainform == null)
            {
                return;
            }
            HGMapData mapData = Mainform.GetMapData();
            if (mapData == null)
            {
                return;
            }
            
            ConversationPosButton c = (ConversationPosButton)sender;
            if (c == null)
            {
                MessageBox.Show($"c = null");
                return;
            }
            Coordinate3 coords = (Coordinate3)Mainform.GetCoordsSelected().Clone();
            mapData.conversations[c.index].coords = coords;
            c.Text = coords.ToString();
            if(coords.z == _mainForm.ZLevel)
            {
                _mainForm.UpdateMapView(coords);
            }
        }
        private void AddConversationButton_Click(object sender, EventArgs e)
        {
            if (Mainform == null)
            {
                return;
            }
            HGMapData mapData = Mainform.GetMapData();
            if(mapData == null)
            {
                return;
            }
            ConversationCombo convCombo = new ConversationCombo(convCombos.Count, ConversationsPanel);
            convCombos.Add(convCombo);
            convCombo.btn.Click += new EventHandler(ConvComboButton_Click);
            EditorToolTip.SetToolTip(convCombo.btn, "Click to set the selected cell position as this conversation's position.");
            convCombo.textBox.TextChanged += new EventHandler(ConversationTextBox_TextChanged);

            int convAmount = mapData.CountConversations();
            /*
            HGConversation[] convArray = new HGConversation[convAmount + 1];
            for(int i = 0; i < convArray.Length; i++)
            {
                if(i < mapData.conversations.Length && mapData.conversations[i] == null)
                {
                    break;
                }
                convArray[i] = i < mapData.conversations.Length ? (HGConversation)mapData.conversations[i].Clone() : new HGConversation() { text = "\"New message\"" };
            }
            mapData.conversations = convArray;
            */
            List<HGConversation> convList = mapData.conversations.ToList();
            convList.Add(new HGConversation() { text = "\"New message\"" });
            mapData.conversations = convList.ToArray();
            convAmount = mapData.CountConversations();
            //HGConversation conv = mapData.conversations[convAmount - 1];
            //convCombo.btn.Text = $"({conv.coords})";
            //convCombo.textBox.Text = conv.text;
            UpdateAddDeleteConvButtons();
        }
        private void DeleteConversationButton_Click(object sender, EventArgs e)
        {
            if(Mainform == null)
            {
                return;
            }
            HGMapData mapData = Mainform.GetMapData();
            if(mapData == null)
            {
                return;
            }
            List<HGConversation> convList = mapData.conversations.ToList();
            Controls.Remove(convCombos[convCombos.Count - 1].indexLabel);
            Controls.Remove(convCombos[convCombos.Count - 1].btn);
            Controls.Remove(convCombos[convCombos.Count - 1].textBox);
            convCombos[convCombos.Count - 1].indexLabel.Dispose();
            convCombos[convCombos.Count - 1].btn.Dispose();
            convCombos[convCombos.Count - 1].textBox.Dispose();
            convList.RemoveAt(convList.Count - 1);
            convCombos.RemoveAt(convCombos.Count - 1);
            mapData.conversations = convList.ToArray();
            
            UpdateConversations(Mainform.GetMapData());
        }
        private void UpdateAddDeleteConvButtons()
        {
            DeleteConversationButton.Location = new Point(3, convCombos.Count > 0 ? convCombos[convCombos.Count - 1].indexLabel.Location.Y + 23 : 3);
            AddConversationButton.Location = new Point(38, convCombos.Count > 0 ? convCombos[convCombos.Count - 1].indexLabel.Location.Y + 23 : 3);
        }
        private class ConversationCombo
        {
            public int index;
            public Label indexLabel;
            public ConversationPosButton btn;
            public ConversationTextBox textBox;

            public ConversationCombo(int indexP, Control parent)
            {
                index = indexP;
                indexLabel = new Label();
                indexLabel.Parent = parent;
                parent.Controls.Add(indexLabel);
                indexLabel.Location = new Point(3,3 + 23 * index);
                indexLabel.Width = 29;
                indexLabel.Height = 23;
                //indexLabel.Text = $"{index.ToString().PadLeft(2, '0')}";
                indexLabel.Text = index.ToString();
                indexLabel.TextAlign = ContentAlignment.MiddleRight;

                btn = new ConversationPosButton();
                btn.index = index;
                btn.Parent = parent;
                parent.Controls.Add(btn);
                btn.Location = new Point(39, 3 + 23 * index);
                btn.Width = 69;
                btn.Height = 23;
                btn.Text = "Set";
                btn.TextAlign = ContentAlignment.MiddleCenter;

                textBox = new ConversationTextBox();
                textBox.index = index;
                textBox.Parent = parent;
                parent.Controls.Add(textBox);
                textBox.Location = new Point(112, 3 + 23 * index + 1);
                textBox.Width = 640;
                textBox.Height = 23;
            }
        }
        private void ImportConversations(string path)
        {
            if(File.Exists(path) == false)
            {
                return;
            }
            HGMapData mapData = Mainform.GetMapData();
            if (mapData == null)
            {
                return;
            }
            List<HGConversation> convList = new List<HGConversation>();
            convCombos.Clear();
            string[] lines = File.ReadAllLines(path);
            int convCount = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string s = lines[i];
                if(string.IsNullOrEmpty(s))
                {
                    continue;
                }
                if(s.Length < 9)
                {
                    continue;
                }
                int coordsX = 0;
                int coordsY = 0;
                int coordsZ = 0;
                if (int.TryParse(s.Substring(0,2),out coordsX) == false || int.TryParse(s.Substring(3, 2), out coordsY) == false || int.TryParse(s.Substring(6, 2), out coordsZ) == false)
                {
                    continue;
                }
                convList.Add(new HGConversation() { text = s.Substring(9), coords = new Coordinate3(coordsX, coordsY, coordsZ) });
                ConversationCombo convCombo = new ConversationCombo(convCount, ConversationsPanel);
                convCombos.Add(convCombo);
                convCombo.btn.Click += new EventHandler(ConvComboButton_Click);
                EditorToolTip.SetToolTip(convCombo.btn, "Click to set the selected cell position as this conversation's position.");
                convCombo.textBox.TextChanged += new EventHandler(ConversationTextBox_TextChanged);
                convCount++;
            }
            mapData.conversations = convList.ToArray();
            AddConversationButton.Location = new Point(12, convCombos[convCombos.Count - 1].indexLabel.Location.Y + 23);
            UpdateConversations(Mainform.GetMapData());
        }
        private void HorizonsImportButton_Click(object sender, EventArgs e)
        {
            if(Mainform == null)
            {
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Mainform.editorDir;
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream inStream;
                if ((inStream = openFileDialog.OpenFile()) != null)
                {
                    ImportConversations(openFileDialog.FileName);
                }
                inStream.Close();
            }
        }
        private void CopyEscButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText($"{Convert.ToChar(027)}");
        }

        
    }
    public class ConversationPosButton : Button
    {
        public int index;
    }
    public class ConversationTextBox : TextBox
    {
        public int index;
    }
}
