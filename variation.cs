using System;
using System.Data;
using System.IO;
using System.Linq;

namespace ACNHPockets
{
    public partial class Variation ///: Form
    {
        private static DataTable itemSource;
        private InventorySlot[,] selection;
        private int lengthX;
        private int lengthY;
        
        /*
        public Variation(int height = 265)
        {
            InitializeComponent();
            Size = new Size(Width, height);
        }
        */

        private static DataTable LoadItemCSV(string filePath)
        {
            var dt = new DataTable();

            File.ReadLines(filePath).Take(1)
                .SelectMany(x => x.Split([" ; "], StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(x => dt.Columns.Add(x.Trim()));

            File.ReadLines(filePath).Skip(1)
                .Select(x => x.Split([" ; "], StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(line => dt.Rows.Add(line));

            if (dt.Columns.Contains("id"))
                dt.PrimaryKey = [dt.Columns["id"]];

            return dt;
        }

        public static string GetImagePathFromID(string itemID, DataTable source)
        {
            DataRow row = source.Rows.Find(itemID);

            if (row == null)
            {
                return ""; //row not found
            }
            else
            {
                string path = Utilities.imagePath + row["iName"] + "_Remake_0_0" + ".png";
                if (File.Exists(path))
                {
                    return path;
                }

                //row found set the index and find the file
                path = Utilities.imagePath + row["iName"] + ".png";
                if (File.Exists(path))
                {
                    return path;
                }

                return "";
            }

        }

        private void Variation_Load(object sender, EventArgs e)
        {
            if (File.Exists(Utilities.variationPath))
            {
                itemSource = LoadItemCSV(Utilities.variationPath);

                /*
                furnitureGridView.DataSource = loadItemCSV(variationPath);
                //furnitureGridView.Columns["ID"].Visible = false;
                //furnitureGridView.Columns["iName"].Visible = false;
                furnitureGridView.Columns[0].Width = 150;
                furnitureGridView.Columns[1].Width = 110;

                furnitureGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                furnitureGridView.DefaultCellStyle.BackColor = Color.FromArgb(255, 47, 49, 54);
                furnitureGridView.DefaultCellStyle.ForeColor = Color.FromArgb(255, 114, 105, 110);
                furnitureGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);

                furnitureGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 57, 60, 67);
                furnitureGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                furnitureGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);

                furnitureGridView.EnableHeadersVisualStyles = false;

                furnitureGridView.Font = new Font("Arial", 10);

                DataGridViewImageColumn furnitureImageColumn = new DataGridViewImageColumn
                {
                    Name = "Image",
                    HeaderText = "Image",
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                };
                furnitureGridView.Columns.Insert(3, furnitureImageColumn);
                furnitureImageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                foreach (DataGridViewColumn c in furnitureGridView.Columns)
                {
                    c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    c.HeaderCell.Style.Font = new Font("Arial", 9, FontStyle.Bold);
                }
                */
            }
        }


        private void RemoveVariation()
        {
            for (int j = 0; j < lengthX; j++)
            {
                for (int k = 0; k < lengthY; k++)
                {
                    //Controls.Remove(selection[j, k]);
                }
            }
        }

        /*
            this.selectedItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.selectedItem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.selectedItem.FlatAppearance.BorderSize = 0;
            this.selectedItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectedItem.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.selectedItem.ForeColor = System.Drawing.Color.White;
            this.selectedItem.Location = new System.Drawing.Point(9, 9);
            this.selectedItem.Margin = new System.Windows.Forms.Padding(0);
            this.selectedItem.Name = "selectedItem";
            this.selectedItem.Size = new System.Drawing.Size(128, 128);
            this.selectedItem.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.selectedItem.UseVisualStyleBackColor = false;
         */

        public static int FindMaxVariation(string name)
        {
            for (int i = 9; i >= 0; i--)
            {
                string path = Utilities.imagePath + name + "_Remake_" + i + "_0" + ".png";
                if (File.Exists(path))
                {
                    return i;
                }
            }
            return -1;
        }
        public static int FindMaxSubVariation(string name)
        {
            for (int i = 9; i >= 0; i--)
            {
                string path = Utilities.imagePath + name + "_Remake_0_" + i + ".png";
                if (File.Exists(path))
                {
                    return i;
                }
            }
            return -1;
        }

        /*
        public void ReceiveID(string id, string language, string value = "00000000")
        {
            RemoveVariation();
            itemIDLabel.Text = id;
            DataRow row = GetRowFromID(id);
            if (row != null)
            {
                infoLabel.Text = "";
                string name = row[language].ToString();
                //string idString = row["id"].ToString();
                UInt16 itemID = Convert.ToUInt16("0x" + row["id"], 16);
                //UInt16 data = 0x0;
                //string category = row[1].ToString();
                string iName = row["iName"].ToString();
                //string path = GetImagePathFromID(idString, itemSource);

                //updateSelectedItemInfo(selectedItem.displayItemName(), selectedItem.displayItemID(), selectedItem.displayItemData());
                //Debug.Print(row[0].ToString() + " " + row[1].ToString() + " " + row[2].ToString() + " " + row[3].ToString() + " ");
                int MaxVariation = FindMaxVariation(iName);
                int MaxSubxVariation = FindMaxSubVariation(iName);

                if (MaxVariation >= 0 && MaxSubxVariation >= 0)
                    ShowVariation(name, itemID, FindMaxVariation(iName), FindMaxSubVariation(iName), iName, value);
                else
                    infoLabel.Text = @"Did you forget the image pack?";
            }
            else
            {
                infoLabel.Text = @"No variation found.";
            }
        }
        */

        public static DataRow GetRowFromID(string id)
        {
            if (itemSource == null)
            {
                if (File.Exists(Utilities.variationPath))
                    itemSource = LoadItemCSV(Utilities.variationPath);
                if (itemSource == null)
                    return null;
            }
            DataRow row = itemSource.Rows.Find(id);

            return row;
        }

        public static InventorySlot[,] GetVariationList(string id, string flag0 = "00", string flag1 = "00", string value = "00000000", string language = "eng")
        {
            DataRow row = GetRowFromID(id);
            if (row != null)
            {
                UInt16 itemID = Convert.ToUInt16("0x" + row["id"], 16);
                string iName = row["iName"].ToString();

                string name = row[language].ToString();
                int main = FindMaxVariation(iName);
                int sub = FindMaxSubVariation(iName);

                if (main >= 0 && sub >= 0)
                {
                    InventorySlot[,] variationList = new InventorySlot[main + 1, sub + 1];

                    for (int j = 0; j <= main; j++)
                    {
                        for (int k = 0; k <= sub; k++)
                        {
                            variationList[j, k] = new InventorySlot();

                            string path = Utilities.imagePath + iName + "_Remake_" + j + "_" + k + ".png";

                            if (ItemAttr.HasFenceWithVariation(itemID)) // Fence with Variation
                            {
                                string front = Utilities.PrecedingZeros((j + (0x20 * k)).ToString("X"), 4);
                                string back = Utilities.Turn2bytes(value);
                                uint newValue = Convert.ToUInt32(front + back, 16);
                                variationList[j, k].Setup(name, itemID, newValue, path, true, "", flag0, flag1);
                            }
                            else
                            {
                                variationList[j, k].Setup(name, itemID, (uint)(j + (0x20 * k)), path, true, "", flag0, flag1);
                            }
                        }
                    }
                    return variationList;
                }
                else
                    return null;
            }
            else
            {
                return null;
            }
        }
    }
}
