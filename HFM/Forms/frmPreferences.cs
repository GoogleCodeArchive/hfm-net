/*
 * HFM.NET - User Preferences Form
 * Copyright (C) 2006-2007 David Rawling
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using HFM.Preferences;
using Debug=HFM.Instrumentation.Debug;

namespace HFM.Forms
{
   public partial class frmPreferences : Form
   {
      public PreferenceSet Prefs;

      public frmPreferences()
      {
         InitializeComponent();
      }

      private void checkBox2_CheckedChanged(object sender, EventArgs e)
      {
         if (chkScheduled.Checked)
         {
            txtCollectMinutes.ReadOnly = false;
         }
         else
         {
            txtCollectMinutes.ReadOnly = true;
         }
      }

      private void txtMinutes_KeyPress(object sender, KeyPressEventArgs e)
      {
         if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
         {
            e.Handled = true;
         }
      }

      private void txtMinutes_Validating(object sender, CancelEventArgs e)
      {
         int Minutes = 0;
         try
         {
            Minutes = Convert.ToInt16(txtCollectMinutes.Text);
         }
         catch
         {
            e.Cancel = true;
         }
         if ((Minutes > 180) || (Minutes < 1))
         {
            e.Cancel = true;
         }
      }

      private void frmPreferences_Shown(object sender, EventArgs e)
      {
         Prefs = PreferenceSet.Instance;

         DirectoryInfo di = new DirectoryInfo(Path.Combine(PreferenceSet.AppPath, "CSS"));
         StyleList.Items.Clear();
         foreach (FileInfo fi in di.GetFiles())
         {
            StyleList.Items.Add(fi.Name.ToLower().Replace(".css", ""));
         }

         // Visual Style tab
         StyleList.SelectedItem = Prefs.CSSFileName.ToLower().Replace(".css", "");

         // Scheduled Tasks tab
         txtWebGenMinutes.Text = Prefs.GenerateInterval.ToString();
         chkWebSiteGenerator.Checked = Prefs.GenerateWeb;
         if (Prefs.WebGenAfterRefresh)
         {
            radioFullRefresh.Checked = true;
         }
         else
         {
            radioSchedule.Checked = true;
         }
         chkSynchronous.Checked = Prefs.SyncOnLoad;
         chkScheduled.Checked = Prefs.SyncOnSchedule;
         chkOffline.Checked = Prefs.OfflineLast;

         cboPpdCalc.Items.Add(ePpdCalculation.LastFrame);
         cboPpdCalc.Items.Add(ePpdCalculation.LastThreeFrames);
         cboPpdCalc.Items.Add(ePpdCalculation.AllFrames);
         cboPpdCalc.Items.Add(ePpdCalculation.EffectiveRate);
         cboPpdCalc.Text = Prefs.PpdCalculation.ToString();

         txtCollectMinutes.Text = Prefs.SyncTimeMinutes.ToString();
         txtWebSiteBase.Text = Prefs.WebRoot;

         // Defaults
         chkDefaultConfig.Checked = Prefs.UseDefaultConfigFile;
         txtDefaultConfigFile.Text = Prefs.DefaultConfigFile;
         chkAutoSave.Checked = Prefs.AutoSaveConfig;
         txtLogFileViewer.Text = Prefs.LogFileViewer;
         txtFileExplorer.Text = Prefs.FileExplorer;
         cboMessageLevel.Items.Add(TraceLevel.Off.ToString());
         cboMessageLevel.Items.Add(TraceLevel.Error.ToString());
         cboMessageLevel.Items.Add(TraceLevel.Warning.ToString());
         cboMessageLevel.Items.Add(TraceLevel.Info.ToString());
         cboMessageLevel.Items.Add(TraceLevel.Verbose.ToString());
         if (Prefs.MessageLevel >= 0 && Prefs.MessageLevel <= 4)
         {
            cboMessageLevel.SelectedIndex = Prefs.MessageLevel;
         }
         else
         {
            cboMessageLevel.SelectedIndex = (int)TraceLevel.Info;
         }

         // Web
         txtEOCUserID.Text = Prefs.EOCUserID.ToString();
         txtStanfordUserID.Text = Prefs.StanfordID;
         txtStanfordTeamID.Text = Prefs.TeamID.ToString();
         txtProjectDownloadUrl.Text = Prefs.ProjectDownloadUrl;
         chkUseProxy.Checked = Prefs.UseProxy;
         chkUseProxyAuth.Checked = Prefs.UseProxyAuth;
         txtProxyServer.Text = Prefs.ProxyServer;
         txtProxyPort.Text = Prefs.ProxyPort.ToString();
         txtProxyUser.Text = Prefs.ProxyUser;
         txtProxyPass.Text = Prefs.ProxyPass;
      }

      private void StyleList_SelectedIndexChanged(object sender, EventArgs e)
      {
         String sStylesheet = Path.Combine(Path.Combine(PreferenceSet.AppPath, "CSS"), Path.ChangeExtension(StyleList.SelectedItem.ToString(), ".css"));
         StringBuilder sb = new StringBuilder();

         sb.Append("<HTML><HEAD><TITLE>Test CSS File</TITLE>");
         sb.Append("<LINK REL=\"Stylesheet\" TYPE=\"text/css\" href=\"file:///" + sStylesheet + "\" />");
         sb.Append("</HEAD><BODY>");

         sb.Append("<table class=\"Instance\">");
         sb.Append("<tr>");
         sb.Append("<td class=\"Heading\">Heading</td>");
         sb.Append("<td class=\"Blank\" width=\"100%\"></td>");
         sb.Append("</tr>");
         sb.Append("<tr>");
         sb.Append("<td class=\"LeftCol\">Left Col</td>");
         sb.Append("<td class=\"RightCol\">Right Column</td>");
         sb.Append("</tr>");
         sb.Append("<tr>");
         sb.Append("<td class=\"AltLeftCol\">Left Col</td>");
         sb.Append("<td class=\"AltRightCol\">Right Column</td>");
         sb.Append("</tr>");
         sb.Append("<tr>");
         sb.Append("<td class=\"LeftCol\">Left Col</td>");
         sb.Append("<td class=\"RightCol\">Right Column</td>");
         sb.Append("</tr>");
         sb.Append("<tr>");
         sb.Append("<td class=\"AltLeftCol\">Left Col</td>");
         sb.Append("<td class=\"AltRightCol\">Right Column</td>");
         sb.Append("</tr>");
         sb.Append("<tr>");
         sb.Append(String.Format("<td class=\"Plain\" colspan=\"2\" align=\"center\">Last updated {0} at {1}</td>", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString()));
         sb.Append("</tr>");
         sb.Append("</table>");
         sb.Append("</BODY></HTML>");

         wbCssSample.DocumentText = sb.ToString();
      }

      private void chkWebSiteGenerator_CheckedChanged(object sender, EventArgs e)
      {
         if (chkWebSiteGenerator.Checked)
         {
            radioSchedule.Enabled = true;
            radioSchedule_CheckedChanged(sender, e);
            lbl2MinutesToGen.Enabled = true;
            radioFullRefresh.Enabled = true;
            txtWebSiteBase.Enabled = true;
            txtWebSiteBase.ReadOnly = false;
            btnBrowseWebFolder.Enabled = true;
         }
         else
         {
            radioSchedule.Enabled = false;
            txtWebGenMinutes.Enabled = false;
            txtWebGenMinutes.ReadOnly = true;
            lbl2MinutesToGen.Enabled = false;
            radioFullRefresh.Enabled = false;
            txtWebSiteBase.Enabled = false;
            txtWebSiteBase.ReadOnly = true;
            btnBrowseWebFolder.Enabled = false;
         }
      }

      private void btnBrowseWebFolder_Click(object sender, EventArgs e)
      {
         if (txtWebSiteBase.Text != String.Empty)
         {
            locateWebFolder.SelectedPath = txtWebSiteBase.Text;
         }
         if (locateWebFolder.ShowDialog() == DialogResult.OK)
         {
            txtWebSiteBase.Text = locateWebFolder.SelectedPath;
         }
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         // Check for error condition on Name
         if (txtProjectDownloadUrl.BackColor == Color.Yellow) return;
      
         // Visual Styles tab
         Prefs.CSSFileName = StyleList.SelectedItem + ".css";

         // Scheduled Tasks tab
         Prefs.GenerateInterval = Int32.Parse(txtWebGenMinutes.Text);
         Prefs.GenerateWeb = chkWebSiteGenerator.Checked;
         if (radioFullRefresh.Checked)
         {
            Prefs.WebGenAfterRefresh = true;
         }
         else
         {
            Prefs.WebGenAfterRefresh = false;
         }
         Prefs.SyncOnLoad = chkSynchronous.Checked;
         Prefs.SyncOnSchedule = chkScheduled.Checked;
         Prefs.OfflineLast = chkOffline.Checked;
         Prefs.PpdCalculation = (ePpdCalculation)cboPpdCalc.SelectedItem;
         Prefs.SyncTimeMinutes = Int32.Parse(txtCollectMinutes.Text);
         Prefs.WebRoot = txtWebSiteBase.Text;

         // Defaults
         Prefs.UseDefaultConfigFile = chkDefaultConfig.Checked;
         Prefs.DefaultConfigFile = txtDefaultConfigFile.Text;
         Prefs.AutoSaveConfig = chkAutoSave.Checked;
         Prefs.LogFileViewer = txtLogFileViewer.Text;
         Prefs.FileExplorer = txtFileExplorer.Text;
         Prefs.MessageLevel = cboMessageLevel.SelectedIndex;

         // Web Settings tab
         Prefs.EOCUserID = Int32.Parse(txtEOCUserID.Text);
         Prefs.StanfordID = txtStanfordUserID.Text;
         Prefs.TeamID = Int32.Parse(txtStanfordTeamID.Text);
         Prefs.ProjectDownloadUrl = txtProjectDownloadUrl.Text;
         Prefs.UseProxy = chkUseProxy.Checked;
         Prefs.UseProxyAuth = chkUseProxyAuth.Checked;
         Prefs.ProxyServer = txtProxyServer.Text;
         Prefs.ProxyPort = Int32.Parse(txtProxyPort.Text);
         Prefs.ProxyUser = txtProxyUser.Text;
         Prefs.ProxyPass = txtProxyPass.Text;

         Prefs.Save();
         
         DialogResult = System.Windows.Forms.DialogResult.OK;
         Close();
      }

      private void btnCancel_Click(object sender, EventArgs e)
      {
         Prefs.Discard();
      }

      private void linkEOC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         try
         {
            Process.Start(String.Concat(PreferenceSet.EOCUserBaseURL, txtEOCUserID.Text));
         }
         catch (Exception ex)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error,
                                    String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
            MessageBox.Show("Failed to show EOC User Stats page.");
         }
      }

      private void linkStanford_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         try
         {
            Process.Start(String.Concat(PreferenceSet.StanfordBaseURL, txtStanfordUserID.Text));
         }
         catch (Exception ex)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error,
                                    String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
            MessageBox.Show("Failed to show Stanford User Stats page.");
         }
      }

      private void linkTeam_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         try
         {
            Process.Start(String.Concat(PreferenceSet.EOCTeamBaseURL, txtStanfordTeamID.Text));
         }
         catch (Exception ex)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error,
                                    String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
            MessageBox.Show("Failed to show EOC Team Stats page.");
         }
      }

      private void EnableProxy()
      {
         txtProxyServer.Enabled = true;
         txtProxyPort.Enabled = true;
         txtProxyServer.ReadOnly = false;
         txtProxyPort.ReadOnly = false;
         chkUseProxyAuth.Enabled = true;
      }

      private void DisableProxy()
      {
         txtProxyServer.Enabled = false;
         txtProxyPort.Enabled = false;
         txtProxyServer.ReadOnly = true;
         txtProxyPort.ReadOnly = true;
         chkUseProxyAuth.Enabled = false;
         DisableProxyAuth();
      }

      private void EnableProxyAuth()
      {
         txtProxyUser.Enabled = true;
         txtProxyUser.ReadOnly = false;
         txtProxyPass.Enabled = true;
         txtProxyPass.ReadOnly = false;
      }

      private void DisableProxyAuth()
      {
         txtProxyUser.Enabled = false;
         txtProxyUser.ReadOnly = true;
         txtProxyPass.Enabled = false;
         txtProxyPass.ReadOnly = true;
      }

      private void chkUseProxy_CheckedChanged(object sender, EventArgs e)
      {
         if (chkUseProxy.Checked)
         {
            EnableProxy();
            if (chkUseProxyAuth.Checked)
               EnableProxyAuth();
         }
         else
         {
            DisableProxy();
         }
      }

      private void chkUseProxyAuth_CheckedChanged(object sender, EventArgs e)
      {
         if (chkUseProxyAuth.Checked)
            EnableProxyAuth();
         else
            DisableProxyAuth();
      }

      private void chkDefaultConfig_CheckedChanged(object sender, EventArgs e)
      {
         if (chkDefaultConfig.Checked)
         {
            txtDefaultConfigFile.Enabled = true;
            txtDefaultConfigFile.ReadOnly = false;
            btnBrowseConfigFile.Enabled = true;
         }
         else
         {
            txtDefaultConfigFile.Enabled = false;
            txtDefaultConfigFile.ReadOnly = true;
            btnBrowseConfigFile.Enabled = false;
         }
      }

      private void btnBrowseConfigFile_Click(object sender, EventArgs e)
      {
         DoFolderBrowse(txtDefaultConfigFile, "hfm", "HFM Configuration Files|*.hfm");
      }

      private void btnBrowseLogViewer_Click(object sender, EventArgs e)
      {
         DoFolderBrowse(txtLogFileViewer, "exe", "Program Files|*.exe");
      }

      private void btnBrowseFileExplorer_Click(object sender, EventArgs e)
      {
         DoFolderBrowse(txtFileExplorer, "exe", "Program Files|*.exe");
      }
      
      private void DoFolderBrowse(Control txt, string extension, string filter)
      {
         if (String.IsNullOrEmpty(txt.Text) == false)
         {
            FileInfo fileInfo = new FileInfo(txt.Text);
            if (fileInfo.Exists)
            {
               openConfigDialog.InitialDirectory = fileInfo.DirectoryName;
               openConfigDialog.FileName = fileInfo.Name;
            }
            else
            {
               DirectoryInfo dirInfo = new DirectoryInfo(txt.Text);
               if (dirInfo.Exists)
               {
                  openConfigDialog.InitialDirectory = dirInfo.FullName;
                  openConfigDialog.FileName = String.Empty;
               }
            }
         }
         else
         {
            openConfigDialog.InitialDirectory = String.Empty;
            openConfigDialog.FileName = String.Empty;
         }

         openConfigDialog.DefaultExt = extension;
         openConfigDialog.Filter = filter;
         if (openConfigDialog.ShowDialog() == DialogResult.OK)
         {
            txt.Text = openConfigDialog.FileName;
         }
      }

      private void txtProjectDownloadUrl_Validating(object sender, CancelEventArgs e)
      {
      
         Regex rURL = new Regex(@"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Singleline);

         if (rURL.Match(txtProjectDownloadUrl.Text).Success == false)
         {
            txtProjectDownloadUrl.BackColor = Color.Yellow;
            txtProjectDownloadUrl.Focus();
            //ShowToolTip("URL must be a valid URL and be\r\nthe path to a valid Stanford Project Summary page.", txtProjectDownloadUrl, 5000);
         }
         else
         {
            txtProjectDownloadUrl.BackColor = SystemColors.Window;
            //toolTipCore.Hide(txtProjectDownloadUrl);
         }
      }

      private void radioSchedule_CheckedChanged(object sender, EventArgs e)
      {
         if (radioSchedule.Checked)
         {
            txtWebGenMinutes.Enabled = true;
            txtWebGenMinutes.ReadOnly = false;
         }
         else
         {
            txtWebGenMinutes.Enabled = false;
            txtWebGenMinutes.ReadOnly = true;
         }
      }
   }
}