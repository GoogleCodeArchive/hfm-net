/*
 * HFM.NET - Application Entry Point
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Core.Resource;

using harlam357.Windows.Forms;

using HFM.Forms;
using HFM.Classes;
using HFM.Instances;
using HFM.Framework;

namespace HFM
{
   static class Program
   {
      public static string[] Args;

      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main(string[] args)
      {
         Args = args;

         Application.ApplicationExit += ApplicationExit;
         AppDomain.CurrentDomain.AssemblyResolve += CustomResolve;

         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);

         #region Primary Initialization

         // Issue 180 - Restore the already running instance to the screen.
         try
         {
            if (!SingleInstanceHelper.Start())
            {
               SingleInstanceHelper.SignalFirstInstance(args);
               return;
            }
         }
         catch (Exception ex)
         {
            ExceptionDialog.ShowErrorDialog(ex, PlatformOps.ApplicationNameAndVersionWithRevision, Environment.OSVersion.VersionString,
               "Single Instance Helper Failed to Start.", 
               Constants.GoogleGroupUrl, true);
            return;
         }

         try
         {
            var container = new WindsorContainer(new XmlInterpreter(new ConfigResource("castle")));
            InstanceProvider.SetContainer(container);
         }
         catch (Exception ex)
         {
            ExceptionDialog.ShowErrorDialog(ex, PlatformOps.ApplicationNameAndVersionWithRevision, Environment.OSVersion.VersionString,
               "Windsor Container Failed to Initialize.  Either components are missing or the HFM.exe.config file is corrupt.",
               Constants.GoogleGroupUrl, true);
            return;
         }

         IPreferenceSet prefs;
         try
         {
            prefs = InstanceProvider.GetInstance<IPreferenceSet>();
            if (prefs.Initialize() == false) return;
         }
         catch (Exception ex)
         {
            ExceptionDialog.ShowErrorDialog(ex, PlatformOps.ApplicationNameAndVersionWithRevision, Environment.OSVersion.VersionString,
               "Preferences Failed to Initialize.  The user.config file is likely corrupt.",
               Constants.GoogleGroupUrl, true);
            return;
         }

         try
         {
            SetupTraceListeners(prefs, InstanceProvider.GetInstance<IMessagesView>());
         }
         catch (Exception ex)
         {
            ExceptionDialog.ShowErrorDialog(ex, PlatformOps.ApplicationNameAndVersionWithRevision, Environment.OSVersion.VersionString,
               "Logging Failed to Initialize.",
               Constants.GoogleGroupUrl, true);
            return;
         }

         if (PlatformOps.IsRunningOnMono())
         {
            HfmTrace.WriteToHfmConsole("Running on Mono...");
         }

         try
         {
            var database = InstanceProvider.GetInstance<IUnitInfoDatabase>();
            database.DatabaseFilePath = Path.Combine(prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), Constants.SqLiteFilename);
         }
         catch (Exception ex)
         {
            ExceptionDialog.ShowErrorDialog(ex, PlatformOps.ApplicationNameAndVersionWithRevision, Environment.OSVersion.VersionString,
               "UnitInfo Database Failed to Initialize.",
               Constants.GoogleGroupUrl, true);
            return;
         }
         
         try
         {
            ClearCacheFolder(prefs);
         }
         catch (Exception ex)
         {
            ExceptionDialog.ShowErrorDialog(ex, PlatformOps.ApplicationNameAndVersionWithRevision, Environment.OSVersion.VersionString,
               "Failed to create or clear the data cache folder.",
               Constants.GoogleGroupUrl, true);
            return;
         }

         // Read Containers
         var statsData = InstanceProvider.GetInstance<IXmlStatsDataContainer>();
         statsData.Read();
         var proteinCollection = InstanceProvider.GetInstance<IProteinCollection>();
         proteinCollection.Read();
         var benchmarkContainer = InstanceProvider.GetInstance<IProteinBenchmarkContainer>();
         benchmarkContainer.Read();
         var unitInfoContainer = InstanceProvider.GetInstance<IUnitInfoContainer>();
         unitInfoContainer.Read();

         // Initialize the Instance Collection
         var instanceCollection = InstanceProvider.GetInstance<IInstanceCollection>();
         instanceCollection.Initialize();

         frmMain frm;
         try
         {
            frm = InstanceProvider.GetInstance<frmMain>();
            frm.Initialize();
         }
         catch (Exception ex)
         {
            ExceptionDialog.ShowErrorDialog(ex, PlatformOps.ApplicationNameAndVersionWithRevision, Environment.OSVersion.VersionString,
               "Primary UI Failed to Initialize.",
               Constants.GoogleGroupUrl, true);
            return;
         }

         try
         {
            SingleInstanceHelper.RegisterIpcChannel(frm.SecondInstanceStarted);
         }
         catch (Exception ex)
         {
            ExceptionDialog.ShowErrorDialog(ex, PlatformOps.ApplicationNameAndVersionWithRevision, Environment.OSVersion.VersionString,
               "Single Instance IPC Channel Failed to Register.",
               Constants.GoogleGroupUrl, true);
            return;
         }

         #endregion

         // Register the Unhandled Exception Dialog
         ExceptionDialog.RegisterForUnhandledExceptions(PlatformOps.ApplicationNameAndVersionWithRevision, 
            Environment.OSVersion.VersionString, HfmTrace.WriteToHfmConsole);
         
         try
         {
            Application.Run(frm);
         }
         finally
         {
            SingleInstanceHelper.Stop();
         }
      }

      private static void ApplicationExit(object sender, EventArgs e)
      {
         // Save preferences
         var prefs = InstanceProvider.GetInstance<IPreferenceSet>();
         prefs.Save();
         // Save the benchmark collection
         var benchmarkContainer = InstanceProvider.GetInstance<IProteinBenchmarkContainer>();
         benchmarkContainer.Write();

         HfmTrace.WriteToHfmConsole("----------");
         HfmTrace.WriteToHfmConsole("Exiting...");
         HfmTrace.WriteToHfmConsole(String.Empty);
      }

      private static System.Reflection.Assembly CustomResolve(object sender, ResolveEventArgs args)
      {
         const string sqliteDll = "System.Data.SQLite";
         if (args.Name.StartsWith(sqliteDll))
         {
            string platform = PlatformOps.IsRunningOnMono() ? "Mono" : Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            if (platform != null)
            {
               string filePath = Path.GetFullPath(Path.Combine(Application.StartupPath, Path.Combine(Path.Combine("SQLite", platform), String.Concat(sqliteDll, ".dll"))));
               HfmTrace.WriteToHfmConsole(String.Format(CultureInfo.CurrentCulture, "SQLite DLL Path: {0}", filePath));
               if (File.Exists(filePath))
               {
                  return System.Reflection.Assembly.LoadFile(filePath);
               }
            }
         }
         return null;
      }

      /// <summary>
      /// Clears the log cache folder specified by the CacheFolder setting
      /// </summary>
      private static void ClearCacheFolder(IPreferenceSet prefs)
      {
         DateTime start = HfmTrace.ExecStart;

         string cacheFolder = Path.Combine(prefs.GetPreference<string>(Preference.ApplicationDataFolderPath),
                                           prefs.GetPreference<string>(Preference.CacheFolder));

         var di = new DirectoryInfo(cacheFolder);
         if (di.Exists == false)
         {
            di.Create();
         }
         else
         {
            foreach (var fi in di.GetFiles())
            {
               try
               {
                  fi.Delete();
               }
               catch (Exception ex)
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("Failed to clear cache file '{0}'.", fi.Name), ex);
               }
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Info, start);
      }

      /// <summary>
      /// Creates Trace Listener for Log File writing and Message Window output
      /// </summary>
      private static void SetupTraceListeners(IPreferenceSet prefs, IMessagesView messagesView)
      {
         // Ensure the HFM User Application Data Folder Exists
         var applicationDataFolderPath = prefs.GetPreference<string>(Preference.ApplicationDataFolderPath);
         if (Directory.Exists(applicationDataFolderPath) == false)
         {
            Directory.CreateDirectory(applicationDataFolderPath);
         }

         string logFilePath = Path.Combine(applicationDataFolderPath, Constants.HfmLogFileName);
         string prevLogFilePath = Path.Combine(applicationDataFolderPath, Constants.HfmPrevLogFileName);

         var fi = new FileInfo(logFilePath);
         if (fi.Exists && fi.Length > 512000)
         {
            var fi2 = new FileInfo(prevLogFilePath);
            if (fi2.Exists)
            {
               fi2.Delete();
            }
            fi.MoveTo(prevLogFilePath);
         }

         var listener = new TextWriterTraceListener(logFilePath);
         Trace.Listeners.Add(listener);
         Trace.AutoFlush = true;

         // Set Level to Warning to catch any errors that come from loading the preferences
         TraceLevelSwitch.Instance.Level = TraceLevel.Warning;

         HfmTrace.Instance.TextMessage += ((sender, e) => messagesView.AddMessage(e.Message));
         HfmTrace.WriteToHfmConsole(String.Empty);
         HfmTrace.WriteToHfmConsole(String.Format("Starting - HFM.NET v{0}", PlatformOps.ApplicationVersionWithRevision));
         HfmTrace.WriteToHfmConsole(String.Empty);

         // Get the actual TraceLevel from the preferences
         TraceLevelSwitch.Instance.Level = (TraceLevel)prefs.GetPreference<int>(Preference.MessageLevel);
      }
   }
}
