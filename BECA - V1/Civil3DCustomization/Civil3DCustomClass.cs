﻿using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using BecaUserInterface;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;

[assembly: ExtensionApplication(typeof(BecaApplication.Civil3DCustomClass))]

namespace BecaApplication
{
    public class Civil3DCustomClass : IExtensionApplication
    {
        static string _buttonName = string.Empty;
        static string _tabName = string.Empty;

        public void Initialize()
        {
            if (Autodesk.AutoCAD.ApplicationServices.Application.Version.Major >= 23)//Only from 2019
            {
                if (Autodesk.Windows.ComponentManager.Ribbon == null)
                {
                    Autodesk.Windows.ComponentManager.ItemInitialized += ComponentManager_ItemInitialized;
                    Autodesk.Windows.ComponentManager.ItemExecuted += ComponentManager_ItemExecuted;
                }
                else
                {
                    //CreateCivil3DRibbon();
                    Autodesk.Windows.ComponentManager.ItemExecuted += ComponentManager_ItemExecuted;
                }
            }
            else
            {
                Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                Database db = doc.Database;
                Editor ed = doc.Editor;
                ed.WriteMessage("\nCADSymInsert.dll is incompatible with this version of AutoCAD.");
                throw new NotImplementedException();
            }
        }

        private void ComponentManager_ItemExecuted(object sender, Autodesk.Internal.Windows.RibbonItemExecutedEventArgs e)
        {
            // ((e.Item as RibbonButton).CommandHandler).
            // var macro = ((RibbonCommandButton)((Autodesk.Windows.RibbonCommandItem)e.Item).CommandHandler).Macro;
            if (e.Item != null)
                _buttonName = e.Item.AutomationName;
            else
                _buttonName = string.Empty;
            _tabName = Autodesk.Windows.ComponentManager.Ribbon != null ? Autodesk.Windows.ComponentManager.Ribbon.Name : string.Empty;

            string temp = _tabName.ToLower().Replace(".cuix", "").Replace("b - civil", "").Replace("b-civil", "").Replace("_", "").Trim();
            _tabName = string.IsNullOrEmpty(temp) ? "New Zealand" : ((temp == "au") ? "Australia" : "Singapore");




            //22-Oct-2018
            // _tabName = Autodesk.Windows.ComponentManager.Ribbon != null ? Autodesk.Windows.ComponentManager.Ribbon.ActiveTab.Title : string.Empty;
            // MessageBox.Show("You have clicked " + e.Item.AutomationName + ".");
        }

        private void ComponentManager_ItemInitialized(object sender, Autodesk.Windows.RibbonItemEventArgs e)
        {
            if (Autodesk.Windows.ComponentManager.Ribbon != null)
            {
                //CreateCivil3DRibbon();
            }
        }

        public void Terminate()
        {
            throw new NotImplementedException();
        }

        [LispFunction("SymInsert")]//  [LispFunction("MenuLoadSwitch")]//24-10-2018
        public void MenuLoadSwitch(ResultBuffer rbArgs)
        {
            if (rbArgs != null && rbArgs.AsArray().Length == 2)
            {
                List<string> args = new List<string>();
                foreach (TypedValue rb in rbArgs)
                {
                    args.Add(rb.Value.ToString());
                }
                string strVal1 = args[0]; //Menu Name
                string strVal2 = args[1]; //File Name
                if (strVal1 != "" && strVal2 != "")
                {
                    string svOptionFromLisp = strVal1;
                    string folderPathFromLisp = strVal1;
                    if (Autodesk.AutoCAD.ApplicationServices.Application.Version.Major >= 23)
                        loadSigns(svOptionFromLisp, folderPathFromLisp);
                    else
                        MessageBox.Show("Tool does not support this version, kindly contact support.", "Beca", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("Invalid arguments found", "Beca", MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show("Arguments count doesn't match", "Beca", MessageBoxButton.OK);
            }
        }

        // [CommandMethod("BECAGUI", CommandFlags.Session)]
        // [CommandMethod("SymInsert", CommandFlags.Session)]//24-10-2018
        public void AusSigns()
        {
            var symbolLibrary = new SymbolLibrary(_tabName, _buttonName, "");
            if (!symbolLibrary.abortScreenOpen)
                symbolLibrary.ShowDialog();
        }

        public void loadSigns(string svOptionFromLisp = "", string folderPathFromLisp = "")
        {
            var symbolLibrary = new SymbolLibrary(_tabName, _buttonName, "", svOptionFromLisp, folderPathFromLisp);
            if (!symbolLibrary.abortScreenOpen)
                symbolLibrary.ShowDialog();
        }

        //[CommandMethod("AusBECAGUI", CommandFlags.Session)]
        //public void AusSigns()
        //{
        //    var symbolLibrary = new SymbolLibrary("Australia", "", "B-Civil_AU.xml");
        //    symbolLibrary.ShowDialog();
        //}

        //[CommandMethod("NZBECAGUI", CommandFlags.Session)]
        //public void NZSigns()
        //{
        //    var symbolLibrary = new SymbolLibrary("New Zealand", "", "B-Civil.xml");
        //    symbolLibrary.ShowDialog();
        //}

        //[CommandMethod("SGBECAGUI", CommandFlags.Session)]
        //public void SGSigns()
        //{
        //    var symbolLibrary = new SymbolLibrary("Singapore", "", "B-Civil_SG.xml");
        //    symbolLibrary.ShowDialog();
        //}

        //[CommandMethod("AusSigns",CommandFlags.Session)]
        //public void AusSigns()
        //{
        //    var symbolLibrary = new SymbolLibrary("Australia","Signs","B-Civil_AU.xml");
        //    symbolLibrary.ShowDialog();
        //}

        //[CommandMethod("AusRoadMarkings", CommandFlags.Session)]
        //public void AusRoadMarkings()
        //{
        //    var symbolLibrary = new SymbolLibrary("Australia", "RoadMarkings", "B-Civil_AU.xml");
        //    symbolLibrary.ShowDialog();
        //}

        //[CommandMethod("NZSigns", CommandFlags.Session)]
        //public void NZSigns()
        //{
        //    var symbolLibrary = new SymbolLibrary("New Zealand", "Signs", "B-Civil.xml");
        //    symbolLibrary.ShowDialog();
        //}

        //[CommandMethod("NZRoadMarkings", CommandFlags.Session)]
        //public void NZRoadMarkings()
        //{
        //    var symbolLibrary = new SymbolLibrary("New Zealand", "RoadMarkings", "B-Civil.xml");
        //    symbolLibrary.ShowDialog();
        //}

        //[CommandMethod("SGSigns", CommandFlags.Session)]
        //public void SGSigns()
        //{
        //    var symbolLibrary = new SymbolLibrary("Singapore", "Signs", "B-Civil_SG.xml");
        //    symbolLibrary.ShowDialog();
        //}

        //[CommandMethod("SGRoadMarkings", CommandFlags.Session)]
        //public void SGRoadMarkings()
        //{
        //    var symbolLibrary = new SymbolLibrary("Singapore", "RoadMarkings", "B-Civil_SG.xml");
        //    symbolLibrary.ShowDialog();
        //}

        [CommandMethod("CreateCivil3DRibbon", CommandFlags.Session)]
        public void CreateCivil3DRibbon()
        {
            Autodesk.Windows.RibbonTab rbnTab = new Autodesk.Windows.RibbonTab();
            rbnTab.Title = "Beca";
            rbnTab.Id = "Civil3d";

            Autodesk.Windows.RibbonPanelSource panlSourceAus = new Autodesk.Windows.RibbonPanelSource();
            panlSourceAus.Title = "Australia";

            Autodesk.Windows.RibbonButton rbnSignAus = new Autodesk.Windows.RibbonButton();
            //rbnSignAus.Text = "Signs";
            rbnSignAus.ShowText = true;
            rbnSignAus.ShowImage = true;
            rbnSignAus.Image = Images.getBitmap(Properties.Resources.Signs);
            rbnSignAus.LargeImage = Images.getBitmap(Properties.Resources.Signs);
            rbnSignAus.Size = RibbonItemSize.Large;
            rbnSignAus.CommandHandler = new AusSignCommandHandler();
            rbnSignAus.CommandParameter = "AusSign";
            rbnSignAus.Orientation = System.Windows.Controls.Orientation.Horizontal;
            rbnSignAus.Width = 500;
            panlSourceAus.Items.Add(rbnSignAus);

            Autodesk.Windows.RibbonRowBreak rowBreak = new RibbonRowBreak();
            panlSourceAus.Items.Add(rowBreak);

            Autodesk.Windows.RibbonButton rbnRoadSignAus = new Autodesk.Windows.RibbonButton();
            //rbnRoadSignAus.Text = "Road Markings";
            rbnRoadSignAus.ShowText = true;
            rbnRoadSignAus.ShowImage = true;
            rbnRoadSignAus.Image = Images.getBitmap(Properties.Resources.Road_Markings);
            rbnRoadSignAus.LargeImage = Images.getBitmap(Properties.Resources.Road_Markings);
            rbnRoadSignAus.Size = RibbonItemSize.Large;
            rbnRoadSignAus.CommandParameter = "AusRoadSign";
            rbnRoadSignAus.CommandHandler = new AusSignCommandHandler();
            rbnRoadSignAus.Orientation = System.Windows.Controls.Orientation.Horizontal;
            rbnRoadSignAus.Width = 500;
            panlSourceAus.Items.Add(rbnRoadSignAus);

            Autodesk.Windows.RibbonPanel rbnPanelAus = new RibbonPanel();
            rbnPanelAus.Source = panlSourceAus;
            rbnTab.Panels.Add(rbnPanelAus);

            Autodesk.Windows.RibbonPanelSource panlSourceNZ = new Autodesk.Windows.RibbonPanelSource();
            panlSourceNZ.Title = "Newzeland";

            Autodesk.Windows.RibbonButton rbnSignNZ = new Autodesk.Windows.RibbonButton();
            //rbnSignNZ.Text = "Signs";
            rbnSignNZ.ShowText = true;
            rbnSignNZ.ShowImage = true;
            rbnSignNZ.Image = Images.getBitmap(Properties.Resources.Signs);
            rbnSignNZ.LargeImage = Images.getBitmap(Properties.Resources.Signs);
            rbnSignNZ.Size = RibbonItemSize.Large;
            rbnSignNZ.CommandParameter = "NZSign";
            rbnSignNZ.CommandHandler = new AusSignCommandHandler();
            rbnSignNZ.Orientation = System.Windows.Controls.Orientation.Horizontal;
            rbnSignNZ.Width = 500;
            panlSourceNZ.Items.Add(rbnSignNZ);

            Autodesk.Windows.RibbonRowBreak rowBreakNZ = new RibbonRowBreak();
            panlSourceNZ.Items.Add(rowBreakNZ);

            Autodesk.Windows.RibbonButton rbnRoadSignNZ = new Autodesk.Windows.RibbonButton();
            //rbnRoadSignNZ.Text = "Road Markings";
            rbnRoadSignNZ.ShowText = true;
            rbnRoadSignNZ.ShowImage = true;
            rbnRoadSignNZ.Image = Images.getBitmap(Properties.Resources.Road_Markings);
            rbnRoadSignNZ.LargeImage = Images.getBitmap(Properties.Resources.Road_Markings);
            rbnRoadSignNZ.Size = RibbonItemSize.Large;
            rbnRoadSignNZ.CommandParameter = "NZRoadSign";
            rbnRoadSignNZ.CommandHandler = new AusSignCommandHandler();
            rbnRoadSignNZ.Orientation = System.Windows.Controls.Orientation.Horizontal;
            rbnRoadSignNZ.Width = 500;
            panlSourceNZ.Items.Add(rbnRoadSignNZ);

            Autodesk.Windows.RibbonPanel rbnPanelNZ = new RibbonPanel();
            rbnPanelNZ.Source = panlSourceNZ;

            rbnTab.Panels.Add(rbnPanelNZ);

            rbnTab.IsActive = true;

            Autodesk.Windows.ComponentManager.Ribbon.Tabs.Add(rbnTab);
        }
    }

    public class AusSignCommandHandler : System.Windows.Input.ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if ((parameter as RibbonButton).CommandParameter.Equals("AusSign"))
                MessageBox.Show("AusSign");
            else if ((parameter as RibbonButton).CommandParameter.Equals("AusRoadSign"))
                MessageBox.Show("AusRoadSign");
            else if ((parameter as RibbonButton).CommandParameter.Equals("NZSign"))
                MessageBox.Show("NZSign");
            else if ((parameter as RibbonButton).CommandParameter.Equals("NZRoadSign"))
                MessageBox.Show("NZRoadSign");
            else if ((parameter as RibbonButton).CommandParameter.Equals("SGSign"))
                MessageBox.Show("SGSign");
            else if ((parameter as RibbonButton).CommandParameter.Equals("SGRoadSign"))
                MessageBox.Show("SGRoadSign");
        }
    }

    public class Images
    {
        public static BitmapImage getBitmap(Bitmap image)
        {
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = stream;
            bmp.EndInit();

            return bmp;
        }
    }
}
