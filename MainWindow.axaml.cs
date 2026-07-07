using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Controller;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using File = System.IO.File;
using Image = Avalonia.Controls.Image;

namespace ACNHPockets
{



    public class AppSettings
    {
        public string getipaddress { get; set; } = "192.168.1.100";
        public bool InvFreeze { get; set; } = false;
        public string Inventorydata { get; set; } = "";
    }

    public class SettingsService
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ACNHPockets",
            "acnhpockets-settings.json");

        public AppSettings Load()
        {
            if (!File.Exists(SettingsPath))
                return new AppSettings();

            var json = File.ReadAllText(SettingsPath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }

        public void Save(AppSettings settings)
        {
            var directory = Path.GetDirectoryName(SettingsPath)!;
            Directory.CreateDirectory(directory);

            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(SettingsPath, json);
        }
    }



    public partial class MainWindow : Window
    {


        Socket socket = null;
        //updated offset to work on latest verion of ACNH Game.  
        string chathex = "[main+5255A60]+40"; //"[main+4AA9CD8]+40";
        bool sendLock = false;

        bool online = false; 
        bool connecting = false;
        bool chatting = false;

        int GameCap = 24;
        controller controller = null;

        private int counter;
        byte[] header;

        private AppSettings appSettings;
        private SettingsService settingsService;

        public static DataTable itemSource;
        public static DataTable recipeSource;
        public static DataTable flowerSource;
        public static DataTable variationSource;
        public static Dictionary<string, string> OverrideDict;

        //protected override Type StyleKeyOverride => typeof(InventorySlot);

        InventorySlot[] invbuttons;


        public MainWindow()
        {
            InitializeComponent();
            settingsService = new SettingsService();
            appSettings = settingsService.Load();
            ipaddress.Text = appSettings.getipaddress;
            invFreezeToggle.IsChecked = appSettings.InvFreeze;
            invFreezeToggle.Content = appSettings.InvFreeze ? "ON" : "OFF";

            
            invbuttons = new InventorySlot[] { Slot1, Slot2, Slot3, Slot4, Slot5, Slot6, Slot7, Slot8, Slot9, Slot10, Slot11, Slot12, Slot13, Slot14, Slot15, Slot16, Slot17, Slot18, Slot19, Slot20, Slot21, Slot22, Slot23, Slot24, Slot25, Slot26, Slot27, Slot28, Slot29, Slot30, Slot31, Slot32, Slot33, Slot34, Slot35, Slot36, Slot37, Slot38, Slot39, Slot40 };


            Utilities.LogEvent("MainWindow", "Application started. Loaded settings: IP Address = " + appSettings.getipaddress + ", Inventory Freeze = " + appSettings.InvFreeze);


            if (File.Exists(Utilities.itemPath))
            {
                itemSource = LoadItemCSV(Utilities.itemPath);
            }

            if (File.Exists(Utilities.recipePath))
            {
                recipeSource = LoadItemCSV(Utilities.recipePath);
            }

            if (File.Exists(Utilities.flowerPath))
            {
                flowerSource = LoadItemCSV(Utilities.flowerPath);
            }

            if (File.Exists(Utilities.variationPath))
            {
                variationSource = LoadItemCSV(Utilities.variationPath);
            }

            if (File.Exists(Utilities.overridePath))
            {
                OverrideDict = CreateOverride(Utilities.overridePath);
            }

            PopulateComboBox();


            LoadImages(Slot1, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot2, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot3, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot4, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot5, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot6, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot7, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot8, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot9, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot10, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot11, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot12, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot13, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot14, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot15, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot16, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot17, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot18, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot19, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot20, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot21, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot22, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot23, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot24, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot25, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot26, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot27, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot28, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot29, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot30, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot31, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot32, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot33, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot34, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot35, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot36, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot37, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot38, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot39, Utilities.imagePath + "empty.png", "Empty", "0");
            LoadImages(Slot40, Utilities.imagePath + "empty.png", "Empty", "0");


            Utilities.BuildDictionary();

        }




        public List<string> items = new() {  };
        private void PopulateComboBox()
        {



            foreach (DataRow row in itemSource.Rows)
            {
                
                items.Add(row["eng"].ToString());

            }

            ACNHItems.ItemsSource = items;
            ACNHItems.MinimumPrefixLength = 0;
            ACNHItems.Text = string.Empty;
            ACNHItems.PopulateComplete();


        }

        private void SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var autoComplete = sender as AutoCompleteBox;
            if (autoComplete != null)
            {
                var selectedItem = autoComplete.SelectedItem;

                if (selectedItem != null)
                {
                    status.Text = "Selected Item: " + selectedItem.ToString();


                    string itemid = Utilities.GetIDFromName(selectedItem.ToString());
                    if (!string.IsNullOrEmpty(itemid))
                    {
                        LoadImages(Image, Utilities.GetImagePathFromID(itemid, itemSource), selectedItem.ToString(), "1");
                    }
                }
                else
                {
                    status.Text = "No item selected";
                }
            }

        }

        /*
         
                int decValue = Convert.ToInt32(count.Text) - 1;
                string hexValue;
                if (decValue < 0)
                    hexValue = "0";
                else
                    hexValue = decValue.ToString("X");
                count.Text = Utilities.PrecedingZeros(hexValue, 8);
                Utilities.LogEvent("MainWindow", "SetSlot1: Count set to " + count.Text);
        
         */

        public void ItemAdd(object sender, RoutedEventArgs args)
        {

            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            ///LoadImages(Image, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Item added to slot!";

        }





        public List<string> SlotItem = new() { };
        public List<string> SlotCount = new() { };
        public void SetSlot1(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
               count.Text = "1";
            }
            LoadImages(Slot1, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot1 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot2(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot2, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot2 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot3(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot3, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot3 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot4(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot4, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot4 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot5(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot5, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot5 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot6(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot6, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot6 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot7(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot7, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot7 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot8(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot8, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot8 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot9(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot9, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot9 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot10(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot10, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot10 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot11(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot11, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot11 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot12(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot12, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot12 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot13(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot13, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot13 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot14(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot14, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot14 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot15(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot15, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot15 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot16(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot16, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot16 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot17(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot17, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot17 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot18(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot18, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot18 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }
        public void SetSlot19(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot19, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot19 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot20(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot20, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot20 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot21(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot21, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot21 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }
        public void SetSlot22(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot22, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot22 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));

        }
        public void SetSlot23(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot23, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot23 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }
        public void SetSlot24(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot24, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot24 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }
        public void SetSlot25(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot25, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot25 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }
        public void SetSlot26(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot26, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot26 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }
        public void SetSlot27(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot27, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot27 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }
        public void SetSlot28(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot28, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot28 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }
        public void SetSlot29(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot29, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot29 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }
        public void SetSlot30(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot30, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot30 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }
        public void SetSlot31(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot31, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot31 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }
        public void SetSlot32(object sender, RoutedEventArgs args)
        {

            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot32, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot32 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }
        public void SetSlot33(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot33, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot33 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }
        public void SetSlot34(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot34, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot34 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }
        public void SetSlot35(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot35, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot35 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }
        public void SetSlot36(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot36, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot36 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }
        public void SetSlot37(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot37, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot37 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }
        public void SetSlot38(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot38, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot38 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }
        public void SetSlot39(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot39, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot39 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }
        public void SetSlot40(object sender, RoutedEventArgs args)
        {
            if (count.Text == string.Empty)
            {
                count.Text = "1";
            }
            LoadImages(Slot40, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
            status.Text = "Slot40 set!";
            SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
            SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
        }

        public void ConnectButton_Click(object sender, RoutedEventArgs args)
        {
            status.Text = "trying to connect to the Switch";
            Connect(ipaddress.Text);
            appSettings.getipaddress = ipaddress.Text;
            settingsService.Save(appSettings);
        }

        public void startChat_Click(object sender, RoutedEventArgs args)
        {
            if (online) 
            {
            Startchat();
            }
        }

        public void chatButton_Click(object sender, RoutedEventArgs args)
        {
            if (online) 
            {
            status.Text = "Chat Sent: " + chattext.Text;
            if (chattext.Text.Length <= GameCap)
            {
                sendchat(socket, chattext.Text);
            }
            else
            {
                status.Text = "Chat Text exceeds " + GameCap + " character limit.";
            }
            }
        }


        public void disconnectButton_Click(object sender, RoutedEventArgs args)
        {
            if (socket != null)
            {
                socket.Close();
                socket = null;
                status.Text = "Disconnected from Switch";
                online = false;
            }
        }

        private void InvFreezeButton_Click(object sender, RoutedEventArgs e)
        {
            if (online) 
            {
            var button = (ToggleButton)sender;
            if (button.IsChecked == true)
            {

                byte[] bank01To20 = Utilities.GetInventoryBank(socket, 1);
                byte[] bank21To40 = Utilities.GetInventoryBank(socket, 21);
                Utilities.SendString(socket, Utilities.Freeze(Utilities.ItemSlotBase, bank01To20));
                Utilities.SendString(socket, Utilities.Freeze(Utilities.ItemSlot21Base, bank21To40));
                button.Content = "ON";
                status.Text = "Inventory Freeze Enabled";
                appSettings.InvFreeze = true;
                settingsService.Save(appSettings);
            }
            else
            {
                Utilities.SendString(socket, Utilities.UnFreeze(Utilities.ItemSlotBase));
                Utilities.SendString(socket, Utilities.UnFreeze(Utilities.ItemSlot21Base));
                button.Content = "OFF";
                status.Text = "Inventory Freeze Disabled";
                appSettings.InvFreeze = false;
                settingsService.Save(appSettings);
            }
            }
        }


        void Connect(string getipaddress)
        {

            if (getipaddress == null)
            {
                status.Text = "You must enter your switch ip address";
                return;
            }

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(getipaddress), 6000);

            IAsyncResult result = socket.BeginConnect(ep, null, null);
            bool conSuceded = result.AsyncWaitHandle.WaitOne(3000, true);
            if (conSuceded == true)
            {
                online = true;
                status.Text = "Connected to " + getipaddress;
                controller = new controller(socket);

                //read inventory
                UpdateInventory();

            }
            else
            {
                online = false;
                status.Text = "Failed to connect to " + getipaddress;
                socket.Close();
                socket = null;
            }


        }


        private async void OpenNHIFile_Click(object sender, RoutedEventArgs args)
        {
            // Get top level from the current control. Alternatively, you can use Window reference instead.
            var topLevel = TopLevel.GetTopLevel(this);

            // Start async operation to open the dialog.
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open NHI File",
                AllowMultiple = false
            });

            if (files.Count >= 1)
            {
                // Open reading stream from the first file.
                /*
                await using var stream = await files[0].OpenReadAsync();
                using var streamReader = new StreamReader(stream);
                // Reads all the content of file as a text.
                var fileContent = await streamReader.ReadToEndAsync();
                */

                await using var stream = await files[0].OpenReadAsync();
                using var memoryStream = new MemoryStream();

                // Copy the stream to a memory stream and extract the bytes
                await stream.CopyToAsync(memoryStream);
                byte[] data = memoryStream.ToArray();

                //byte[] data = File.ReadAllBytes(fileContent);
                LoadInventory(data);



            }
        }

        private async void LoadInventory(byte[] data)
        {

            //InventorySlot[] invbuttons = new InventorySlot[] { Slot1, Slot2, Slot3, Slot4, Slot5, Slot6, Slot7, Slot8, Slot9, Slot10, Slot11, Slot12, Slot13, Slot14, Slot15, Slot16, Slot17, Slot18, Slot19, Slot20, Slot21, Slot22, Slot23, Slot24, Slot25, Slot26, Slot27, Slot28, Slot29, Slot30, Slot31, Slot32, Slot33, Slot34, Slot35, Slot36, Slot37, Slot38, Slot39, Slot40 };

            byte[][] item = ProcessNHI(data);

            
            string bank = "";

            byte[] b1 = new byte[160];
            byte[] b2 = new byte[160];
            //online
            if (online)
            {
                byte[] bank01To20 = Utilities.GetInventoryBank(socket, 1);
                byte[] bank21To40 = Utilities.GetInventoryBank(socket, 21);

                byte[] currentInventory = new byte[320];

                Array.Copy(bank01To20, 0, currentInventory, 0, 160);
                Array.Copy(bank21To40, 0, currentInventory, 160, 160);

                int emptyspace = NumOfEmpty(currentInventory);


                if (emptyspace < item.Length)
                {

                    var box = MessageBoxManager.GetMessageBoxStandard(
                    "Not enough inventory spaces!",
                    "Empty Spaces in your inventory : " + emptyspace + "\n" +
                                                                    "Number of items to Spawn : " + item.Length + "\n" +
                                                                    "\n" +
                                                                    "Press  [Yes]  to clear your inventory and spawn the items " + "\n" +
                                                                    "or  [No]  to cancel the spawn." + "\n" + "\n" +
                                                                    "[Warning] You will lose your items in your inventory!",
                    ButtonEnum.YesNo);

                    var result = await box.ShowAsync();

                    if (result == ButtonResult.Yes)
                    {
                        for (int i = 0; i < b1.Length; i++)
                        {
                            b1[i] = data[i];
                            b2[i] = data[i + 160];
                        }

                        Utilities.OverwriteAll(socket, b1, b2, ref counter);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    b1 = bank01To20;
                    b2 = bank21To40;
                    FillInventory(ref b1, ref b2, item);
                    Utilities.OverwriteAll(socket, b1, b2, ref counter);
                }
            }
            //offline
            else
            {
                foreach (InventorySlot btn in invbuttons)
                {
                    int slotId = Array.IndexOf(invbuttons, btn) + 1;
                    invbuttons[slotId - 1] = btn;
                }
                for (int i = 0; i < invbuttons.Length; i++)
                {
                    string first = Utilities.Flip(Utilities.PrecedingZeros(invbuttons[i].GetFlag0() + invbuttons[i].GetFlag1() + Utilities.PrecedingZeros(invbuttons[i].FillItemID(), 4), 8));
                    string second = Utilities.Flip(Utilities.PrecedingZeros(invbuttons[i].FillItemData(), 8));
                    bank = bank + first + second;
                }

                byte[] currentInventory = new byte[320];

                for (int i = 0; i < bank.Length / 2 - 1; i++)
                {
                    string tempStr = string.Concat(bank[(i * 2)].ToString(), bank[((i * 2) + 1)].ToString());
                    Utilities.LogEvent("MainWindow", "tempStr: " + i.ToString() + " " + tempStr);
                    currentInventory[i] = Convert.ToByte(tempStr, 16);
                }

                //save inventory data to settings
                appSettings.Inventorydata = Utilities.ByteToHexString(data);
                settingsService.Save(appSettings);

                int emptyspace = NumOfEmpty(currentInventory);

                if (emptyspace < item.Length)
                {
                    var box = MessageBoxManager.GetMessageBoxStandard(
                    "Not enough inventory spaces!",
                    "Empty Spaces in your inventory : " + emptyspace + "\n" +
                                                                    "Number of items to Spawn : " + item.Length + "\n" +
                                                                    "\n" +
                                                                    "Press  [Yes]  to clear your inventory and spawn the items " + "\n" +
                                                                    "or  [No]  to cancel the spawn." + "\n" + "\n" +
                                                                    "[Warning] You will lose your items in your inventory!",
                    ButtonEnum.YesNo);

                    var result = await box.ShowAsync();

                    if (result == ButtonResult.Yes)
                    {
                        for (int i = 0; i < b1.Length; i++)
                        {
                            b1[i] = data[i];
                            b2[i] = data[i + 160];
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    Array.Copy(currentInventory, 0, b1, 0, 160);
                    Array.Copy(currentInventory, 160, b2, 0, 160);

                    FillInventory(ref b1, ref b2, item);
                }

            }

           

            byte[] newInventory = new byte[320];
            Array.Copy(b1, 0, newInventory, 0, 160);
            Array.Copy(b2, 0, newInventory, 160, 160);

            foreach (InventorySlot btn in invbuttons)
            {


                int slotId = Array.IndexOf(invbuttons, btn) + 1; // +1 because slotId starts from 1

                InventorySlot slot = invbuttons[slotId - 1];

                    byte[] slotBytes = new byte[2];
                    byte[] flag0Bytes = new byte[1];
                    byte[] flag1Bytes = new byte[1];
                    byte[] dataBytes = new byte[4];
                    byte[] recipeBytes = new byte[2];
                    byte[] fenceBytes = new byte[2];

                    int slotOffset = ((slotId - 1) * 0x8);
                    int flag0Offset = 0x3 + ((slotId - 1) * 0x8);
                    int flag1Offset = 0x2 + ((slotId - 1) * 0x8);
                    int countOffset = 0x4 + ((slotId - 1) * 0x8);

                    Buffer.BlockCopy(newInventory, slotOffset, slotBytes, 0, 2);
                    Buffer.BlockCopy(newInventory, flag0Offset, flag0Bytes, 0, 1);
                    Buffer.BlockCopy(newInventory, flag1Offset, flag1Bytes, 0, 1);
                    Buffer.BlockCopy(newInventory, countOffset, dataBytes, 0, 4);
                    Buffer.BlockCopy(newInventory, countOffset, recipeBytes, 0, 2);
                    Buffer.BlockCopy(newInventory, countOffset + 0x2, fenceBytes, 0x0, 0x2);

                    string itemId = Utilities.Flip(Utilities.ByteToHexString(slotBytes));
                    string itemData = Utilities.Flip(Utilities.ByteToHexString(dataBytes));
                    string recipeData = Utilities.Flip(Utilities.ByteToHexString(recipeBytes));
                    string fenceData = Utilities.Flip(Utilities.ByteToHexString(fenceBytes));
                    string flag0 = Utilities.ByteToHexString(flag0Bytes);
                    string flag1 = Utilities.ByteToHexString(flag1Bytes);
                    UInt16 intId = Convert.ToUInt16(itemId, 16);
                    UInt32 itemCount = Convert.ToUInt32("0x" + itemData, 16) + 1;

                //Utilities.LogEvent("MainWindow", "Slot : " + slotId.ToString() + " ID : " + itemId + " Count : " + itemCount + " Data : " + itemData + " recipeData : " + recipeData + " Flag0 : " + flag0 + " Flag1 : " + flag1);

                if (itemId == "FFFE") //Nothing
                {
                    //Utilities.LogEvent("MainWindow", "Slot : " + slotId.ToString() + " is empty.");
                    LoadImages(slot, Utilities.imagePath + "empty.png", "Empty", "0");

                }
                else if (itemId == "16A2") //Recipe
                {
                    //Utilities.LogEvent("MainWindow", "Slot: " + slotId.ToString() + " is a recipe for " + Utilities.GetNameFromID(recipeData, recipeSource) + " ImagePath: " + Utilities.GetImagePathFromID(recipeData, recipeSource));
                    LoadImages(slot, Utilities.GetImagePathFromID(recipeData, recipeSource), Utilities.GetNameFromID(recipeData, recipeSource), "DIY");


                }
                else if (itemId == "1095") //Delivery
                {
                    //Utilities.LogEvent("MainWindow", "Slot: " + slotId.ToString() + " is a delivery for " + Utilities.GetNameFromID(recipeData, itemSource) + " ImagePath: " + Utilities.GetImagePathFromID(recipeData, itemSource));
                    LoadImages(slot, Utilities.GetImagePathFromID(recipeData, itemSource), Utilities.GetNameFromID(recipeData, itemSource), (Convert.ToUInt32("0x" + itemData, 16) + 1).ToString());
                }
                else if (itemId == "16A1") //Bottle Message
                {
                    //btn.Setup(Utilities.GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                    //Utilities.LogEvent("MainWindow", "Slot: " + slotId.ToString() + " is a bottle message for " + Utilities.GetNameFromID(recipeData, recipeSource) + " ImagePath: " + Utilities.GetImagePathFromID(recipeData, recipeSource));
                    LoadImages(slot, Utilities.GetImagePathFromID(recipeData, recipeSource), Utilities.GetNameFromID(recipeData, recipeSource), (Convert.ToUInt32("0x" + itemData, 16) + 1).ToString());

                }
                else if (itemId == "0A13") // Fossil
                {
                    //btn.Setup(Utilities.GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                    //Utilities.LogEvent("MainWindow", "Slot: " + slotId.ToString() + " is a fossil for " + Utilities.GetNameFromID(recipeData, itemSource) + " ImagePath: " + Utilities.GetImagePathFromID(recipeData, itemSource));
                    LoadImages(slot, Utilities.GetImagePathFromID(recipeData, itemSource), Utilities.GetNameFromID(recipeData, itemSource), "Fossil");
                }
                else if (itemId == "114A") // Money Tree
                {
                    //btn.Setup(Utilities.GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                    //Utilities.LogEvent("MainWindow", "Slot: " + slotId.ToString() + " is a money tree for " + Utilities.GetNameFromID(itemId, itemSource) + " ImagePath: " + Utilities.GetImagePathFromID(itemId, itemSource));
                    LoadImages(slot, Utilities.GetImagePathFromID(itemId, itemSource), Utilities.GetNameFromID(itemId, itemSource), (Convert.ToUInt32("0x" + itemData, 16) + 1).ToString());

                }
                else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                {
                    //btn.Setup(Utilities.GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                    //Utilities.LogEvent("MainWindow", "Slot: " + slotId.ToString() + " is a wall-mounted item for " + Utilities.GetNameFromID(itemId, itemSource) + " ImagePath: " + Utilities.GetImagePathFromID(itemId, itemSource));
                    LoadImages(slot, Utilities.GetImagePathFromID(itemId, itemSource), Utilities.GetNameFromID(itemId, itemSource), (Convert.ToUInt32("0x" + itemData, 16) + 1).ToString());
                }
                else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                {
                    //btn.Setup(Utilities.GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                    //Utilities.LogEvent("MainWindow", "Slot: " + slotId.ToString() + " is a fence variation for " + Utilities.GetNameFromID(itemId, itemSource) + " ImagePath: " + Utilities.GetImagePathFromID(itemId, itemSource));
                    LoadImages(slot, Utilities.GetImagePathFromID(itemId, itemSource), Utilities.GetNameFromID(itemId, itemSource), (Convert.ToUInt32("0x" + itemData, 16) + 1).ToString());
                }
                else
                {
                    ///btn.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                    //Utilities.LogEvent("MainWindow", "Slot: " + slotId.ToString() + " is an item for " + Utilities.GetNameFromID(itemId, itemSource) + " ImagePath: " + Utilities.GetImagePathFromID(itemId, itemSource));
                    LoadImages(slot, Utilities.GetImagePathFromID(itemId, itemSource), Utilities.GetNameFromID(itemId, itemSource), (Convert.ToUInt32("0x" + itemData, 16) + 1).ToString());

                }

            }

        }

        private static byte[][] ProcessNHI(byte[] data)
        {
            byte[] tempItem = new byte[8];
            bool[] isItem = new bool[40];
            int numOfitem = 0;

            for (int i = 0; i < 40; i++)
            {
                Buffer.BlockCopy(data, 0x8 * i, tempItem, 0, 8);
                if (!Utilities.ByteToHexString(tempItem).Equals("FEFF000000000000"))
                {
                    isItem[i] = true;
                    numOfitem++;
                }
            }

            byte[][] item = new byte[numOfitem][];
            int itemNum = 0;
            for (int j = 0; j < 40; j++)
            {
                if (isItem[j])
                {
                    item[itemNum] = new byte[8];
                    Buffer.BlockCopy(data, 0x8 * j, item[itemNum], 0, 8);
                    itemNum++;
                }
            }

            return item;
        }

        private static int NumOfEmpty(byte[] data)
        {
            byte[] tempItem = new byte[8];
            int num = 0;

            for (int i = 0; i < 40; i++)
            {
                Buffer.BlockCopy(data, 0x8 * i, tempItem, 0, 8);
                if (Utilities.ByteToHexString(tempItem).Equals("FEFF000000000000"))
                    num++;
            }
            return num;
        }

        private static void FillInventory(ref byte[] b1, ref byte[] b2, byte[][] item)
        {
            byte[] tempItem = new byte[8];
            int num = 0;

            for (int i = 0; i < 20; i++)
            {
                Buffer.BlockCopy(b1, 0x8 * i, tempItem, 0, 8);
                if (Utilities.ByteToHexString(tempItem).Equals("FEFF000000000000"))
                {
                    Buffer.BlockCopy(item[num], 0, b1, 0x8 * i, 8);
                    num++;
                }
                if (num >= item.Length)
                    return;
            }

            for (int j = 0; j < 20; j++)
            {
                Buffer.BlockCopy(b2, 0x8 * j, tempItem, 0, 8);
                if (Utilities.ByteToHexString(tempItem).Equals("FEFF000000000000"))
                {
                    Buffer.BlockCopy(item[num], 0, b2, 0x8 * j, 8);
                    num++;
                }
                if (num >= item.Length)
                    return;
            }
        }


        //public List<string> Item = new() { };
        //public List<string> Count = new() { };
        private async void SaveNHIFile_Click(object sender, RoutedEventArgs args)
        {



            if (SlotItem is not null)
            {

                string bank = "";
                string inventorydata = "";
                
                foreach (string Item in SlotItem)
                {

                    Utilities.LogEvent("MainWindow", "Item: " + Item);
                    Utilities.LogEvent("MainWindow", "Count: " + SlotCount[SlotItem.IndexOf(Item)]);
                    byte[] savedata = new byte[320];

                    //string inventorydata = Utilities.Flip(Utilities.PrecedingZeros(SlotCount[SlotItem.IndexOf(Item)], 8)) + Utilities.Flip(Utilities.PrecedingZeros(Item, 4)) + "00000000";

                    //string[] connectdata = [Item, SlotCount[SlotItem.IndexOf(Item)]];
                    //string inventorydata = string.Concat(connectdata);
                    inventorydata += Utilities.Flip(Utilities.PrecedingZeros(SlotCount[SlotItem.IndexOf(Item)], 8)) + Utilities.Flip(Utilities.PrecedingZeros(Item, 4)) + "00000000";

                    //inventorydata += Item + SlotCount[SlotItem.IndexOf(Item)];



                }
                

                //InventorySlot[] items = new InventorySlot[] { Slot1, Slot2, Slot3, Slot4, Slot5, Slot6, Slot7, Slot8, Slot9, Slot10, Slot11, Slot12, Slot13, Slot14, Slot15, Slot16, Slot17, Slot18, Slot19, Slot20, Slot21, Slot22, Slot23, Slot24, Slot25, Slot26, Slot27, Slot28, Slot29, Slot30, Slot31, Slot32, Slot33, Slot34, Slot35, Slot36, Slot37, Slot38, Slot39, Slot40 };

                //List<string> ItemsList = SlotItem.ToList();

                //InventorySlot[] items = new InventorySlot[] { (InventorySlot)ItemsList.ToArray() };



                /*
                 
string[] originalArray = { "apple", "banana" };

// 1. Convert to List
List<string> dynamicList = originalArray.ToList();

// 2. Use Add() to append the new string
dynamicList.Add("orange");

// 3. Convert back to an array
string[] newArray = dynamicList.ToArray();

                 
                 
                 */


                /*
                foreach (string Item in SlotItem)
                {
                 
                    Utilities.LogEvent("MainWindow", "Item: " + Item);
                    Utilities.LogEvent("MainWindow", "Count: " + SlotCount[SlotItem.IndexOf(Item)]);
                    InventorySlot slot = new InventorySlot();
                    //items;


                }
                */

                /*
                foreach (InventorySlot btn in items)
                {
                    int slotId = Array.IndexOf(items, btn) + 1;
                    items[slotId - 1] = btn;
                }
                for (int i = 0; i < items.Length; i++)
                {
                    string first = Utilities.Flip(Utilities.PrecedingZeros(items[i].GetFlag0() + items[i].GetFlag1() + Utilities.PrecedingZeros(items[i].FillItemID(), 4), 8));
                    string second = Utilities.Flip(Utilities.PrecedingZeros(items[i].FillItemData(), 8));
                    bank = bank + first + second;
                }

                byte[] currentInventory = new byte[320];

                for (int i = 0; i < bank.Length / 2 - 1; i++)
                {
                    string tempStr = string.Concat(bank[(i * 2)].ToString(), bank[((i * 2) + 1)].ToString());
                    Utilities.LogEvent("MainWindow", "tempStr: " + i.ToString() + " " + tempStr);
                    currentInventory[i] = Convert.ToByte(tempStr, 16);
                }
                */


                Utilities.LogEvent("MainWindow", "inventorydata: " + inventorydata);
                appSettings.Inventorydata = inventorydata;
                settingsService.Save(appSettings);
                SlotItem.Clear();
                SlotCount.Clear();
            }



            try
            {


                Utilities.LogEvent("MainWindow", "Preparing to save inventory data. Online status: " + online);


                //InventorySlot[] invbuttons = new InventorySlot[] { Slot1, Slot2, Slot3, Slot4, Slot5, Slot6, Slot7, Slot8, Slot9, Slot10, Slot11, Slot12, Slot13, Slot14, Slot15, Slot16, Slot17, Slot18, Slot19, Slot20, Slot21, Slot22, Slot23, Slot24, Slot25, Slot26, Slot27, Slot28, Slot29, Slot30, Slot31, Slot32, Slot33, Slot34, Slot35, Slot36, Slot37, Slot38, Slot39, Slot40 };


                // Get top level from the current control. Alternatively, you can use Window reference instead.
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel?.StorageProvider == null) return;

                // 2. Open the Save File Picker dialog
                var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Save a NHI File",
                    SuggestedFileName = "!!!test.nhi", // Default file name
                    DefaultExtension = "nhi"          // Default extension
                });

                // 3. If the user cancels, file will be null
                if (file == null) return;

                // 4. Open a write stream and save your bytes
                await using var stream = await file.OpenWriteAsync();
                //await stream.WriteAsync(data);

                string bank = "";

            //online
            if (online)
            {
                byte[] bank01To20 = Utilities.GetInventoryBank(socket, 1);
                byte[] bank21To40 = Utilities.GetInventoryBank(socket, 21);
                //bank = Utilities.ByteToHexString(bank01To20) + Utilities.ByteToHexString(bank21To40);


                byte[] save = new byte[320];

                Array.Copy(bank01To20, 0, save, 0, 160);
                Array.Copy(bank21To40, 0, save, 160, 160);
                await stream.WriteAsync(save, 0, save.Length);

                }
            //offline
            else
            {

                    /*
                    InventorySlot[] slotPointer = new InventorySlot[40];
                    //foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                    foreach (InventorySlot b in invbuttons)
                    {
                    //int slotId = int.Parse(b.Tag.ToString());
                    //slotPointer[slotId - 1] = b;

                    //int slotId = Array.IndexOf(invbuttons, b) + 1; // +1 because slotId starts from 1
                    //InventorySlot slot = invbuttons[slotId - 1];
                    //slotPointer[slotId - 1] = b;
                    }
                    */

                    //InventorySlot[] invbuttons = new InventorySlot[] { Slot1, Slot2, Slot3, Slot4, Slot5, Slot6, Slot7, Slot8, Slot9, Slot10, Slot11, Slot12, Slot13, Slot14, Slot15, Slot16, Slot17, Slot18, Slot19, Slot20, Slot21, Slot22, Slot23, Slot24, Slot25, Slot26, Slot27, Slot28, Slot29, Slot30, Slot31, Slot32, Slot33, Slot34, Slot35, Slot36, Slot37, Slot38, Slot39, Slot40 };


                    /*
                    InventorySlot[] slotPointer = new InventorySlot[40];
                    foreach (InventorySlot b in invbuttons)
                    {
                        Utilities.LogEvent("MainWindow", "Processing Slot: " + (Array.IndexOf(invbuttons, b) + 1).ToString());
                        //int slotId = int.Parse(b.Tag.ToString());
                        //slotPointer[slotId - 1] = b;
                        int slotId = Array.IndexOf(invbuttons, b) + 1; // +1 because slotId starts from 1
                        //InventorySlot slot = (InventorySlot)invbuttons[slotId - 1];
                        slotPointer[slotId - 1] = b;
                        //slotPointer[slotId - 1] = (InventorySlot)b;
                        //string first = Utilities.Flip(Utilities.PrecedingZeros(slot.GetFlag0() + slot.GetFlag1() + Utilities.PrecedingZeros(slot.FillItemID(), 4), 8));
                        //string second = Utilities.Flip(Utilities.PrecedingZeros(slot.FillItemData(), 8));
                        //bank = bank + first + second;

                        string first = Utilities.Flip(Utilities.PrecedingZeros(invbuttons[slotId - 1].GetFlag0() + slotPointer[slotId].GetFlag1() + Utilities.PrecedingZeros(slotPointer[slotId].FillItemID(), 4), 8));
                        string second = Utilities.Flip(Utilities.PrecedingZeros(invbuttons[slotId - 1].FillItemData(), 8));
                        Utilities.LogEvent("MainWindow", first + " " + second + " " + slotPointer[slotId - 1].GetFlag0() + " " + slotPointer[slotId].GetFlag1() + " " + slotPointer[slotId].FillItemID());
                        bank = bank + first + second;

                    }
                    */

                    /*
                    InventorySlot[] slotPointer = new InventorySlot[40];
                    foreach (InventorySlot b in invbuttons)
                    {
                        //int slotId = int.Parse(b.Tag.ToString());
                        //slotPointer[slotId - 1] = b;
                        int slotId = Array.IndexOf(invbuttons, b) + 1; // +1 because slotId starts from 1
                        InventorySlot slot = (InventorySlot)invbuttons[slotId - 1];
                        //slotPointer[slotId - 1] = (InventorySlot)b;
                        string first = Utilities.Flip(Utilities.PrecedingZeros(slot.GetFlag0() + slot.GetFlag1() + Utilities.PrecedingZeros(slot.FillItemID(), 4), 8));
                        string second = Utilities.Flip(Utilities.PrecedingZeros(slot.FillItemData(), 8));
                        bank = bank + first + second;

                    }
                    */

                    /*
                    for (int i = 0; i < slotPointer.Length; i++)
                    {
                    string first = Utilities.Flip(Utilities.PrecedingZeros(slotPointer[i].GetFlag0() + slotPointer[i].GetFlag1() + Utilities.PrecedingZeros(slotPointer[i].FillItemID(), 4), 8));
                    string second = Utilities.Flip(Utilities.PrecedingZeros(slotPointer[i].FillItemData(), 8));
                    Utilities.LogEvent("MainWindow", first + " " + second + " " + slotPointer[i].GetFlag0() + " " + slotPointer[i].GetFlag1() + " " + slotPointer[i].FillItemID ());
                    bank = bank + first + second;
                    }
                    */

                    /*

                                        byte[] save = new byte[320];

                                    for (int i = 0; i < bank.Length / 2 - 1; i++)
                                    {
                                        string data = string.Concat(bank[(i * 2)].ToString(), bank[((i * 2) + 1)].ToString());
                                        Utilities.LogEvent("MainWindow", i.ToString() + " " + data);
                                        save[i] = Convert.ToByte(data, 16);
                                    }
                                        //await using var stream = await file.OpenWriteAsync();
                                        //File.WriteAllBytes(file.Name, save);
                                        await stream.WriteAsync(save);
                                        //await stream.WriteAsync(save, 0, save.Length);

                    */


                    //InventorySlot[] slotPointer = new InventorySlot[40];
                    //foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                    //foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                    /*
                    foreach (InventorySlot btn in invbuttons)
                    {
                        //int slotId = int.Parse(btn.Tag.ToString());
                        int slotId = Array.IndexOf(invbuttons, btn) + 1; // +1 because slotId starts from 1
                        //slotPointer[slotId - 1] = btn;

                    }
                    for (int i = 0; i < invbuttons.Length; i++)
                    {
                        string first = Utilities.Flip(Utilities.PrecedingZeros(invbuttons[i].GetFlag0() + invbuttons[i].GetFlag1() + Utilities.PrecedingZeros(invbuttons[i].FillItemID(), 4), 8));
                        string second = Utilities.Flip(Utilities.PrecedingZeros(invbuttons[i].FillItemData(), 8));
                        Utilities.LogEvent("MainWindow", "slot: " + i.ToString() + " " + first + " " + second + " " + invbuttons[i].GetFlag0() + " " + invbuttons[i].GetFlag1() + " " + invbuttons[i].FillItemID());
                        bank = bank + first + second;


                        
                        Utilities.LogEvent("MainWindow", "Processing Slot: " + i.ToString());
                        Utilities.LogEvent("MainWindow", "Slot Pointer: " + invbuttons[i].ToString());
                        Utilities.LogEvent("MainWindow", "Slot Pointer Flag0: " + invbuttons[i].GetFlag0() + " Flag1: " + invbuttons[i].GetFlag1() + " ItemID: " + invbuttons[i].FillItemID() + " ItemData: " + invbuttons[i].FillItemData());
                        Utilities.LogEvent("MainWindow", "Slot Pointer FillItemID: " + invbuttons[i].FillItemID() + " FillItemData: " + invbuttons[i].FillItemData());
                        Utilities.LogEvent("MainWindow", "Slot Pointer FillItemID (Hex): " + Utilities.PrecedingZeros(invbuttons[i].FillItemID(), 4) + " FillItemData (Hex): " + Utilities.PrecedingZeros(invbuttons[i].FillItemData(), 8));
                        Utilities.LogEvent("MainWindow", "Slot Pointer FillItemID (Flipped): " + Utilities.Flip(Utilities.PrecedingZeros(invbuttons[i].FillItemID(), 4)) + " FillItemData (Flipped): " + Utilities.Flip(Utilities.PrecedingZeros(invbuttons[i].FillItemData(), 8)));
                        Utilities.LogEvent("MainWindow", "Slot Pointer FillItemID (Flipped + Preceding Zeros): " + Utilities.Flip(Utilities.PrecedingZeros(invbuttons[i].FillItemID(), 4)) + " FillItemData (Flipped + Preceding Zeros): " + Utilities.Flip(Utilities.PrecedingZeros(invbuttons[i].FillItemData(), 8)));
                        Utilities.LogEvent("MainWindow", "Slot Pointer FillItemID (Flipped + Preceding Zeros + Flag0 + Flag1): " + Utilities.Flip(Utilities.PrecedingZeros(invbuttons[i].GetFlag0() + invbuttons[i].GetFlag1() + Utilities.PrecedingZeros(invbuttons[i].FillItemID(), 4), 8)) + " FillItemData (Flipped + Preceding Zeros): " + Utilities.Flip(Utilities.PrecedingZeros(invbuttons[i].FillItemData(), 8)));
                        Utilities.LogEvent("MainWindow", "Item Name: " + Utilities.GetNameFromID(invbuttons[i].FillItemID()));
                        
                    }
                    */


                    byte[] save = new byte[320];
                    var s = settingsService.Load();
                    if (s.Inventorydata is not null)
                    {
                        save = Utilities.StringToByte(s.Inventorydata);
                        await stream.WriteAsync(save, 0, save.Length);
                    }

                }
        }
            catch (Exception ex)
            {
                Utilities.LogEvent("MainWindow", ex.Message + " This seems like a bad idea but it's fine for now.");
                return;
            }

        }




        /*

                private async void Spawnall_Click(object sender, RoutedEventArgs args)
                {


                    string bank = "";
                    string inventorydata = "";
                    foreach (InventorySlot b in invbuttons)
                    {

                        if (count.Text == string.Empty)
                        {
                            count.Text = "1";
                        }
                        LoadImages(b, Utilities.GetImagePathFromID(Utilities.GetIDFromName(ACNHItems.Text), itemSource), ACNHItems.Text, count.Text);
                        Utilities.LogEvent("MainWindow", "Item Spawned: " + ACNHItems.Text + " Count: " + count.Text);
                        Utilities.LogEvent("MainWindow", "Item ID: " + Utilities.GetIDFromName(ACNHItems.Text) + " Count (Hex): " + Utilities.ConvertItemCountToHex(count.Text));
                        SlotItem.Add(Utilities.GetIDFromName(ACNHItems.Text));
                        SlotCount.Add(Utilities.ConvertItemCountToHex(count.Text));
                        //Utilities.Flip(Utilities.PrecedingZeros(Utilities.GetIDFromName(ACNHItems.Text), 4));
                        //inventorydata += Utilities.Flip(Utilities.PrecedingZeros(Utilities.GetIDFromName(ACNHItems.Text), 8)) + Utilities.ConvertItemCountToHex(count.Text) + "000000";
                        //inventorydata += Utilities.Flip(Utilities.PrecedingZeros(SlotCount[SlotItem.IndexOf(ACNHItems.Text)], 8)) + Utilities.Flip(Utilities.PrecedingZeros(count.Text, 4)) + "00000000";

                    }






                    //Utilities.LogEvent("MainWindow", "inventorydata: " + inventorydata);
                    //appSettings.Inventorydata = inventorydata;
                    //settingsService.Save(appSettings);



                }

        */


        public void Spawnall_Click(object sender, RoutedEventArgs args)
        {
            if (ACNHItems.Text == "")
            {
                ///MessageBox.Show(@"Please enter an ID before sending item");
                return;
            }

            if (count.Text == "")
            {
                ///MessageBox.Show(@"Please enter an amount");
                return;
            }

            string itemID = Utilities.GetIDFromName(ACNHItems.Text);

            //if (HexModeButton.Tag.ToString() == "Normal")
            //{
                int decValue = Convert.ToInt32(count.Text) - 1;
                string itemAmount;
                if (decValue < 0)
                    itemAmount = "0";
                else
                    itemAmount = decValue.ToString("X");
                //Thread spawnAllThread = new(delegate () { SpawnAll(itemID, itemAmount); });
                //spawnAllThread.Start();
                SpawnAll(itemID, itemAmount);
            /*
            }
            else
            {
                string itemAmount = count.Text;
                Thread spawnAllThread = new(delegate () { SpawnAll(itemID, itemAmount); });
                spawnAllThread.Start();
            }
            */
            //this.ShowMessage(IDTextbox.Text);
        }

        private void SpawnAll(string itemId, string itemAmount)
        {

            if (online)
            {
                byte[] b = new byte[160];
                byte[] id = Utilities.StringToByte(Utilities.Flip(Utilities.PrecedingZeros(itemId, 8)));
                byte[] data = Utilities.StringToByte(Utilities.Flip(Utilities.PrecedingZeros(itemAmount, 8)));


                for (int i = 0; i < b.Length; i += 8)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        b[i + j] = id[j];
                        b[i + j + 4] = data[j];
                    }
                }

                try
                {
                    Utilities.OverwriteAll(socket, b, b, ref counter);
                }
                catch (Exception ex)
                {
                    Utilities.LogEvent("MainForm", "Multithreading badness. This will cause a crash later: " + ex.Message);
                }

                foreach (InventorySlot btn in invbuttons)
                {
                        if (Utilities.Turn2bytes(itemId) == "16A2") //Recipe
                        {
                            //btn.Setup(Utilities.GetNameFromID(Utilities.Turn2bytes(itemAmount), recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemAmount, 16), Utilities.GetImagePathFromID(Utilities.Turn2bytes(itemAmount), recipeSource), "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                            LoadImages(btn, Utilities.GetImagePathFromID(Utilities.Turn2bytes(itemAmount), recipeSource), Utilities.GetNameFromID(Utilities.Turn2bytes(itemAmount), recipeSource), "DIY");
                        }
                        else
                        {
                            //btn.Setup(Utilities.GetNameFromID(Utilities.Turn2bytes(itemId), itemSource), Convert.ToUInt16("0x" + Utilities.Turn2bytes(itemId), 16), Convert.ToUInt32("0x" + itemAmount, 16), Utilities.GetImagePathFromID(Utilities.Turn2bytes(itemId), itemSource, Convert.ToUInt32("0x" + itemAmount, 16)), "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                            LoadImages(btn, Utilities.GetImagePathFromID(Utilities.Turn2bytes(itemId), itemSource, Convert.ToUInt32("0x" + itemAmount, 16)), Utilities.GetNameFromID(Utilities.Turn2bytes(itemId), itemSource), (Convert.ToUInt32("0x" + itemAmount, 16) + 1).ToString());
                        }
                }

                Thread.Sleep(1000);
            }
            //offline
            else
            {
                foreach (InventorySlot btn in invbuttons)
                {

                        if (Utilities.Turn2bytes(itemId) == "16A2") //Recipe
                        {
                            //btn.Setup(Utilities.GetNameFromID(Utilities.Turn2bytes(itemAmount), recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemAmount, 16), Utilities.GetImagePathFromID(Utilities.Turn2bytes(itemAmount), recipeSource), "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                            LoadImages(btn, Utilities.GetImagePathFromID(Utilities.Turn2bytes(itemAmount), recipeSource), Utilities.GetNameFromID(Utilities.Turn2bytes(itemAmount), recipeSource), "DIY");
                    }
                        else
                        {
                            //btn.Setup(Utilities.GetNameFromID(Utilities.Turn2bytes(itemId), itemSource), Convert.ToUInt16("0x" + Utilities.Turn2bytes(itemId), 16), Convert.ToUInt32("0x" + itemAmount, 16), GetImagePathFromID(Utilities.Turn2bytes(itemId), itemSource, Convert.ToUInt32("0x" + itemAmount, 16)), "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                            LoadImages(btn, Utilities.GetImagePathFromID(Utilities.Turn2bytes(itemId), itemSource, Convert.ToUInt32("0x" + itemAmount, 16)), Utilities.GetNameFromID(Utilities.Turn2bytes(itemId), itemSource), (Convert.ToUInt32("0x" + itemAmount, 16) + 1).ToString());
                        }
                }
            }

        }




















        void UpdateInventory()
        {
            //AllowInventoryUpdate = false;
            status.Text = "Reading Inventory...";
            Utilities.LogEvent("MainWindow", "Reading Inventory...");


            try
            {

                Utilities.LogEvent("MainWindow", "Loading Inventory Banks...");

                byte[] bank01To20 = Utilities.GetInventoryBank(socket, 1);
                if (bank01To20 == null)
                {
                    return;
                }
                byte[] bank21To40 = Utilities.GetInventoryBank(socket, 21);
                if (bank21To40 == null)
                {
                    return;
                }

                string Bank1 = Utilities.ByteToHexString(bank01To20);
                string Bank2 = Utilities.ByteToHexString(bank21To40);

                Utilities.LogEvent("MainWindow", "Bank 1: " + Bank1);
                Utilities.LogEvent("MainWindow", "Bank 2: " + Bank2);


                foreach (InventorySlot b in invbuttons)
                {

                    
                    int slotId = Array.IndexOf(invbuttons, b) + 1; // +1 because slotId starts from 1


                    Utilities.LogEvent("MainWindow", "Button Count: " + invbuttons.Length);
                    Utilities.LogEvent("MainWindow", "Processing Slot: " + (slotId).ToString());

                    InventorySlot slot = invbuttons[slotId -1];

                    Utilities.LogEvent("MainWindow", "Button Slot: " + slot.ToString());


                    byte[] slotBytes = new byte[2];
                    byte[] flag0Bytes = new byte[1];
                    byte[] flag1Bytes = new byte[1];
                    byte[] dataBytes = new byte[4];
                    byte[] recipeBytes = new byte[2];
                    byte[] fenceBytes = new byte[2];

                    int slotOffset;
                    int countOffset;
                    int flag0Offset;
                    int flag1Offset;
                    if (slotId < 21)
                    {
                        slotOffset = ((slotId - 1) * 0x8);
                        flag0Offset = 0x3 + ((slotId - 1) * 0x8);
                        flag1Offset = 0x2 + ((slotId - 1) * 0x8);
                        countOffset = 0x4 + ((slotId - 1) * 0x8);
                    }
                    else
                    {
                        slotOffset = ((slotId - 21) * 0x8);
                        flag0Offset = 0x3 + ((slotId - 21) * 0x8);
                        flag1Offset = 0x2 + ((slotId - 21) * 0x8);
                        countOffset = 0x4 + ((slotId - 21) * 0x8);
                    }

                    if (slotId < 21)
                    {
                        Buffer.BlockCopy(bank01To20, slotOffset, slotBytes, 0x0, 0x2);
                        Buffer.BlockCopy(bank01To20, flag0Offset, flag0Bytes, 0x0, 0x1);
                        Buffer.BlockCopy(bank01To20, flag1Offset, flag1Bytes, 0x0, 0x1);
                        Buffer.BlockCopy(bank01To20, countOffset, dataBytes, 0x0, 0x4);
                        Buffer.BlockCopy(bank01To20, countOffset, recipeBytes, 0x0, 0x2);
                        Buffer.BlockCopy(bank01To20, countOffset + 0x2, fenceBytes, 0x0, 0x2);
                    }
                    else
                    {
                        Buffer.BlockCopy(bank21To40, slotOffset, slotBytes, 0x0, 0x2);
                        Buffer.BlockCopy(bank21To40, flag0Offset, flag0Bytes, 0x0, 0x1);
                        Buffer.BlockCopy(bank21To40, flag1Offset, flag1Bytes, 0x0, 0x1);
                        Buffer.BlockCopy(bank21To40, countOffset, dataBytes, 0x0, 0x4);
                        Buffer.BlockCopy(bank21To40, countOffset, recipeBytes, 0x0, 0x2);
                        Buffer.BlockCopy(bank21To40, countOffset + 0x2, fenceBytes, 0x0, 0x2);
                    }

                    string itemId = Utilities.Flip(Utilities.ByteToHexString(slotBytes));
                    string itemData = Utilities.Flip(Utilities.ByteToHexString(dataBytes));
                    string recipeData = Utilities.Flip(Utilities.ByteToHexString(recipeBytes));
                    string fenceData = Utilities.Flip(Utilities.ByteToHexString(fenceBytes));
                    string flag0 = Utilities.ByteToHexString(flag0Bytes);
                    string flag1 = Utilities.ByteToHexString(flag1Bytes);
                    UInt16 intId = Convert.ToUInt16(itemId, 16);
                    UInt32 itemCount = Convert.ToUInt32("0x" + itemData, 16) + 1;


                    Utilities.LogEvent("MainWindow", "Slot : " + slotId.ToString() + " ID : " + itemId + " Count : " + itemCount + " Data : " + itemData + " recipeData : " + recipeData + " Flag0 : " + flag0 + " Flag1 : " + flag1);


                    //LoadImages(Button slot, string imagePath, string text, string count)
                    if (itemId == "FFFE") //Nothing
                    {
                        Utilities.LogEvent("MainWindow", "Slot : " + slotId.ToString() + " is empty.");
                        LoadImages(slot, Utilities.imagePath + "empty.png", "Empty", "0");

                    }
                    else if (itemId == "16A2") //Recipe
                    {
                        Utilities.LogEvent("MainWindow", "Slot: " + slotId.ToString() + " is a recipe for " + Utilities.GetNameFromID(recipeData, recipeSource) + " ImagePath: " + Utilities.GetImagePathFromID(recipeData, recipeSource));
                        LoadImages(slot, Utilities.GetImagePathFromID(recipeData, recipeSource), Utilities.GetNameFromID(recipeData, recipeSource), "DIY");


                    }
                    else if (itemId == "1095") //Delivery
                    {
                        Utilities.LogEvent("MainWindow", "Slot: " + slotId.ToString() + " is a delivery for " + Utilities.GetNameFromID(recipeData, itemSource) + " ImagePath: " + Utilities.GetImagePathFromID(recipeData, itemSource));
                        LoadImages(slot, Utilities.GetImagePathFromID(recipeData, itemSource), Utilities.GetNameFromID(recipeData, itemSource), (Convert.ToUInt32("0x" + itemData, 16) + 1).ToString());
                    }
                    else if (itemId == "16A1") //Bottle Message
                    {
                        //btn.Setup(Utilities.GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                        Utilities.LogEvent("MainWindow", "Slot: " + slotId.ToString() + " is a bottle message for " + Utilities.GetNameFromID(recipeData, recipeSource) + " ImagePath: " + Utilities.GetImagePathFromID(recipeData, recipeSource));
                        LoadImages(slot, Utilities.GetImagePathFromID(recipeData, recipeSource), Utilities.GetNameFromID(recipeData, recipeSource), (Convert.ToUInt32("0x" + itemData, 16) + 1).ToString());

                    }
                    else if (itemId == "0A13") // Fossil
                    {
                        //btn.Setup(Utilities.GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                        Utilities.LogEvent("MainWindow", "Slot: " + slotId.ToString() + " is a fossil for " + Utilities.GetNameFromID(recipeData, itemSource) + " ImagePath: " + Utilities.GetImagePathFromID(recipeData, itemSource));
                        LoadImages(slot, Utilities.GetImagePathFromID(recipeData, itemSource), Utilities.GetNameFromID(recipeData, itemSource), "Fossil");
                    }
                    else if (itemId == "114A") // Money Tree
                    {
                        //btn.Setup(Utilities.GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                        Utilities.LogEvent("MainWindow", "Slot: " + slotId.ToString() + " is a money tree for " + Utilities.GetNameFromID(itemId, itemSource) + " ImagePath: " + Utilities.GetImagePathFromID(itemId, itemSource));
                        LoadImages(slot, Utilities.GetImagePathFromID(itemId, itemSource), Utilities.GetNameFromID(itemId, itemSource), (Convert.ToUInt32("0x" + itemData, 16) + 1).ToString());

                    }
                    else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                    {
                        //btn.Setup(Utilities.GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                        Utilities.LogEvent("MainWindow", "Slot: " + slotId.ToString() + " is a wall-mounted item for " + Utilities.GetNameFromID(itemId, itemSource) + " ImagePath: " + Utilities.GetImagePathFromID(itemId, itemSource));
                        LoadImages(slot, Utilities.GetImagePathFromID(itemId, itemSource), Utilities.GetNameFromID(itemId, itemSource), (Convert.ToUInt32("0x" + itemData, 16) + 1).ToString());
                    }
                    else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                    {
                        //btn.Setup(Utilities.GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                        Utilities.LogEvent("MainWindow", "Slot: " + slotId.ToString() + " is a fence variation for " + Utilities.GetNameFromID(itemId, itemSource) + " ImagePath: " + Utilities.GetImagePathFromID(itemId, itemSource));
                        LoadImages(slot, Utilities.GetImagePathFromID(itemId, itemSource), Utilities.GetNameFromID(itemId, itemSource), (Convert.ToUInt32("0x" + itemData, 16) + 1).ToString());
                    }
                    else
                    {
                        ///btn.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                        Utilities.LogEvent("MainWindow", "Slot: " + slotId.ToString() + " is an item for " + Utilities.GetNameFromID(itemId, itemSource) + " ImagePath: " + Utilities.GetImagePathFromID(itemId, itemSource));
                        LoadImages(slot, Utilities.GetImagePathFromID(itemId, itemSource), Utilities.GetNameFromID(itemId, itemSource), (Convert.ToUInt32("0x" + itemData, 16) + 1).ToString());

                    }

                }

            }


            catch (Exception ex)
            {
                //MyLog.LogEvent("MainForm", "UpdateInventory: " + ex.Message);
                //Invoke((MethodInvoker)delegate { InventoryAutoRefreshToggle.Checked = false; });
                //InventoryRefreshTimer.Stop();
                Utilities.LogEvent("MainWindow", ex.Message + " This seems like a bad idea but it's fine for now.");
                return;
            }

            //Utilities.LogEvent("MainWindow", "END!!!");
            //AllowInventoryUpdate = true;
        }

        private async void RefreshInventory_Click(object sender, RoutedEventArgs args)
        {
            Utilities.LogEvent("MainWindow", "Refreshing Inventory...");
            UpdateInventory();
        }
        private async void ClearInventory_Click(object sender, RoutedEventArgs args)
        {
            Utilities.LogEvent("MainWindow", "Clearing Inventory...");
            ClearInventory();
        }


        private void ClearInventory()
        {

            try
            {
                //online
                if (online)
                {
                    byte[] b = new byte[160];

                    //Debug.Print(Utilities.precedingZeros(itemID, 8));
                    //Debug.Print(Utilities.precedingZeros(itemAmount, 8));

                    for (int i = 0; i < b.Length; i += 8)
                    {
                        b[i] = 0xFE;
                        b[i + 1] = 0xFF;
                        for (int j = 0; j < 6; j++)
                        {
                            b[i + 2 + j] = 0x00;
                        }
                    }

                    Utilities.OverwriteAll(socket, b, b, ref counter);
                    //string result = Encoding.ASCII.GetString(Utilities.transform(b));
                    //Debug.Print(result);

                    foreach (InventorySlot btn in invbuttons)
                    {
                        //btn.Reset();
                        LoadImages(btn, Utilities.imagePath + "empty.png", "Empty", "0");
                    }

                    Thread.Sleep(1000);
                }
                //offline
                else
                {
                    foreach (InventorySlot btn in invbuttons)
                    {
                        //btn.Reset();
                        LoadImages(btn, Utilities.imagePath + "empty.png", "Empty", "0");

                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogEvent("MainWindow", ex.Message + " This is catastrophically bad, don't do this. Someone needs to fix this.");
            }

        }

        void LoadImages(Button slot, string imagePath, string text, string count) {

            //Utilities.LogEvent("MainWindow", "Loading Images for Slot: " + slot.Name + " ImagePath: " + imagePath + " Text: " + text + " Count: " + count);


            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Spacing = 1 // Adds a gap between the image and text
            };

            // 3. Create the Image control and load its source (only if imagePath is valid)
            if (!string.IsNullOrEmpty(imagePath))
            {
                var image = new Image
                {
                    Width = 90,
                    Height = 90,
                    Source = new Bitmap(imagePath)
                };
                stackPanel.Children.Add(image);
            }

            // 4. Create the TextBlock control
            var textBlock = new TextBlock
            {
                FontSize = 14,
                Text = text + "\n(" + count + ")",
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // 5. Assemble the visual tree
            stackPanel.Children.Add(textBlock);

            slot.Content = stackPanel;


        }

        void Startchat()
        {
            controller.detachController();
            controller.clickA();
            controller.clickZR();
            Thread.Sleep(1000);
            controller.clickB();
            controller.clickB();
            controller.clickB();
        }

        void Sendchat(Socket socket, string chattext)
        {

            Chat(socket);
            string cleanStr = chattext.Trim().Replace("\n", " ");

            if (sendLock)
                return;
            if (cleanStr.Equals(""))
                return;


            sendLock = true;

            Thread sendThread = new Thread(delegate () { sendChat(socket, cleanStr); });
            sendThread.Start();


        }

        static void SendString(Socket socket, byte[] buffer, int offset = 0, int size = 0, int timeout = 100)
        {
            int startTickCount = Environment.TickCount;
            int sent = 0;  // how many bytes is already sent
            if (size == 0)
                for (int i = offset; i < buffer.Length; i++)
                    if (buffer[i] == 0xA)
                    {
                        size = i + 1 - offset;
                        break;
                    }
            if (size == 0) size = buffer.Length - offset;
            do
            {
                if (Environment.TickCount > startTickCount + timeout)
                    throw new Exception("Timeout.");
                try
                {
                    sent += socket.Send(buffer, offset + sent, size - sent, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                        ex.SocketErrorCode == SocketError.IOPending ||
                        ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably full, wait and try again
                        //Thread.Sleep(10);
                    }
                    else
                        throw;  // any serious error occurr
                }
            } while (sent < size);
        }
        static int ReceiveString(Socket socket, byte[] buffer, int offset = 0, int size = 0, int timeout = 30000)
        {
            int startTickCount = Environment.TickCount;
            int received = 0;  // how many bytes is already received
            if (size == 0) size = buffer.Length - offset;
            do
            {
                if (Environment.TickCount > startTickCount + timeout)
                    throw new Exception("Timeout.");
                try
                {
                    received += socket.Receive(buffer, offset + received, size - received, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                        ex.SocketErrorCode == SocketError.IOPending ||
                        ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably empty, wait and try again
                        //Thread.Sleep(30);
                    }
                    else
                        throw;  // any serious error occurr
                }
            } while (received < size && buffer[received - 1] != 0xA);
            return received;
        }


        static void pokeAbsoluteAddress(Socket socket, string address, string value)
        {
            //lock (botLock)
            {
                string msg = string.Format("pokeAbsolute 0x{0:X8} 0x{1}\r\n", address, value);
                SendString(socket, Encoding.UTF8.GetBytes(msg));
            }
        }


        static string ByteToHexString(byte[] b)
        {
            string hexString = BitConverter.ToString(b);
            hexString = hexString.Replace("-", "");

            return hexString;
        }

        static byte[] ByteTrim(byte[] input)
        {
            int newLength = 1;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == 0x0)
                {
                    newLength = i;
                    break;
                }
            }

            byte[] newArray = new byte[newLength];
            Array.Copy(input, newArray, newArray.Length);

            return newArray;
        }

        static byte[] peekMainAddress(Socket socket, string address, int size)
        {
            //lock (botLock)
            {
                byte[] result = new byte[size];

                string msg = string.Format("peekMain 0x{0:X8} 0x{1}\r\n", address, size);
                //Debug.Print("PeekMain : " + msg);
                SendString(socket, Encoding.UTF8.GetBytes(msg));

                byte[] b = new byte[size * 2 + 64];
                int first_rec = ReceiveString(socket, b);
                string buffer = Encoding.ASCII.GetString(b, 0, size * 2);

                if (buffer == null)
                {
                    return null;
                }
                for (int i = 0; i < size; i++)
                {
                    result[i] = Convert.ToByte(buffer.Substring(i * 2, 2), 16);
                }

                return result;
            }
        }

        static byte[] peekAbsoluteAddress(Socket socket, string address, int size)
        {
            //lock (botLock)
            {
                byte[] result = new byte[size];

                string msg = string.Format("peekAbsolute 0x{0:X8} {1}\r\n", address, size);
                SendString(socket, Encoding.UTF8.GetBytes(msg));
                byte[] b = new byte[size * 2 + 64];
                int first_rec = ReceiveString(socket, b);
                string buffer = Encoding.ASCII.GetString(b, 0, size * 2);

                if (buffer == null)
                {
                    return null;
                }
                for (int i = 0; i < size; i++)
                {
                    result[i] = Convert.ToByte(buffer.Substring(i * 2, 2), 16);
                }

                return result;
            }
        }

        /*
        public static void pokeAbsoluteAddress(Socket socket, string address, string value)
        {
            //lock (botLock)
            {
                string msg = String.Format("pokeAbsolute 0x{0:X8} 0x{1}\r\n", address, value);
                SendString(socket, Encoding.UTF8.GetBytes(msg));
            }
        }
        */
        static ulong GetCoordinateAddress(string strInput, Socket s)
        {
            //lock (lockObject)
            {
                // Regex pattern to get operators and offsets from pointer expression.	
                string pattern = @"(\+|\-)([A-Fa-f0-9]+)";
                Regex regex = new Regex(pattern);
                Match match = regex.Match(strInput);

                // Get first offset from pointer expression and read address at that offset from main start.	
                var ofs = Convert.ToUInt64(match.Groups[2].Value, 16);
                var address = BitConverter.ToUInt64(peekMainAddress(s, ofs.ToString("X"), 0x8), 0);
                match = match.NextMatch();

                // Matches the rest of the operators and offsets in the pointer expression.	
                while (match.Success)
                {
                    // Get operator and offset from match.	
                    string opp = match.Groups[1].Value;
                    ofs = Convert.ToUInt64(match.Groups[2].Value, 16);

                    // Add or subtract the offset from the current stored address based on operator in front of offset.	
                    switch (opp)
                    {
                        case "+":
                            address += ofs;
                            break;
                        case "-":
                            address -= ofs;
                            break;
                    }

                    // Attempt another match and if successful read bytes at address and store the new address.	
                    match = match.NextMatch();
                    if (match.Success)
                    {
                        byte[] bytes = peekAbsoluteAddress(s, address.ToString("X"), 0x8);
                        address = BitConverter.ToUInt64(bytes, 0);
                    }
                }

                return address;
            }
        }



        void Chat(Socket Socket)
        {
            socket = Socket;

            ///InitializeComponent();            
            ///chattext.SelectAll();
            //chattext.CursorPosition = 0;
            //chattext.SelectionLength = chattext?.Length ?? 0;



        }


        void sendchat(Socket socket, string chattext)
        {
            string cleanStr = chattext.Trim().Replace("\n", " ");

            if (sendLock)
                return;
            if (cleanStr.Equals(""))
                return;


            sendLock = true;

            Thread sendThread = new Thread(delegate () { sendChat(socket, cleanStr); });
            sendThread.Start();
        }

        void sendChat(Socket socket, string message)
        {
            ulong ChatAddress = GetCoordinateAddress(chathex, socket);

            controller.clickR();
            Thread.Sleep(800);
            controller.clickY();

            byte[] StrBytes = Encoding.Unicode.GetBytes(message);
            byte[] sendBytes = new byte[StrBytes.Length * 2];
            Buffer.BlockCopy(StrBytes, 0, sendBytes, 0, StrBytes.Length);
            pokeAbsoluteAddress(socket, ChatAddress.ToString("X"), ByteToHexString(sendBytes));

            controller.clickPLUS();
            Thread.Sleep(400);

            controller.clickB();
            Thread.Sleep(200);
            controller.clickB();
            Thread.Sleep(200);
            controller.clickB();
            Thread.Sleep(200);

            sendLock = false;
        }


        string[] GetInventoryName()
        {
            string[] namelist = new string[8];
            //Debug.Print("Peek 8 Name:");
            byte[] tempHeader = null;
            bool headerFound = false;

            for (int i = 0; i < 8; i++)
            {
                byte[] b = Utilities.GetInventoryName(socket, i);
                if (b == null)
                {
                    namelist[i] = "NULL";
                }
                else
                    namelist[i] = Encoding.Unicode.GetString(b, 32, 20);
                namelist[i] = namelist[i].Replace("\0", string.Empty);
                if (namelist[i].Equals(string.Empty) && !headerFound)
                {
                    header = tempHeader;
                    headerFound = true;
                }
                tempHeader = b;
            }
            return namelist;
        }

        byte[] GetHeader()
        {
            return header;
        }


        static DataTable LoadItemCSV(string filePath)
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

        static DataTable LoadCSVwoKey(string filePath)
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

            return dt;
        }

        static byte[] LoadBinaryFile(string file)
        {
            return File.Exists(file) ? File.ReadAllBytes(file) : null;
        }
        static Dictionary<string, string> CreateOverride(string path)
        {
            Dictionary<string, string> dict = [];

            if (!File.Exists(path)) return dict;

            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                string[] parts = line.Split([" ; "], StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 3)
                {
                    dict.Add(parts[1], parts[2]);
                }
            }

            return dict;
        }






    }



}