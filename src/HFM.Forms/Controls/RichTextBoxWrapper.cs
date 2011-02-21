/*
 * HFM.NET - RichTextBox Wrapper Class
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Forms.Controls
{
   public interface ILogFileViewer
   {
      bool Visible { get; set; }
   
      string LogOwnedByInstanceName { get; }
      
      void SetLogLines(IEnumerable<LogLine> lines, string logOwnedByInstance);

      string[] Lines { get; }
      
      void SetNoLogLines();
      
      void ScrollToBottom();
      
      void HighlightLines();
      
      void RemoveHighlight();
   }

   public partial class RichTextBoxWrapper : RichTextBox, ILogFileViewer
   {
      private IList<LogLine> _logLines;
      
      private string _logOwnedByInstanceName = String.Empty;
      
      public string LogOwnedByInstanceName
      {
         get { return _logOwnedByInstanceName; }
      }
   
      public RichTextBoxWrapper()
      {
         InitializeComponent();
      }

      public void SetLogLines(IEnumerable<LogLine> lines, string logOwnedByInstance)
      {
         _logOwnedByInstanceName = logOwnedByInstance;

#if !LOGTOOL
         // limit the maximum number of log lines
         int lineOffset = lines.Count() - Constants.MaxDisplayableLogLines;
         if (lineOffset > 0)
         {
            lines = lines.Where((x, i) => i > lineOffset);
         }
#endif

         _logLines = lines.ToList();
         Lines = (from ILogLine line in lines select line.LineRaw).ToArray();
      }
      
      public void SetNoLogLines()
      {
         _logLines = null;
      
         Text = "No Log Available";
         RemoveHighlight();
      }

      public void HighlightLines()
      {
         if (_logLines == null) return;
         
         SuspendLayout();
         
         RemoveHighlight();
         
         ForeColor = Color.SlateGray;

         for (int i = 0; i < _logLines.Count; i++)
         {
            ILogLine line = _logLines[i];
            if (line.LineType.Equals(LogLineType.WorkUnitFrame))
            {
               DoLineHighlight(i, Color.Green);
            }
            else if (line.LineType.Equals(LogLineType.ClientShutdown) ||
                     line.LineType.Equals(LogLineType.ClientCoreCommunicationsError) ||
                     line.LineType.Equals(LogLineType.ClientCoreCommunicationsErrorShutdown) ||
                     line.LineType.Equals(LogLineType.ClientEuePauseState) ||
                     line.LineType.Equals(LogLineType.WorkUnitCoreShutdown))
            {
               DoLineHighlight(i, Color.DarkRed);
            }
            else if (line.LineType.Equals(LogLineType.Error))
            {
               DoLineHighlight(i, Color.DarkOrange);
            }
            else if (line.LineType.Equals(LogLineType.Unknown) == false)
            {
               DoLineHighlight(i, Color.Blue);
            }
         }
         
         ResumeLayout(false);
      }
      
      public void RemoveHighlight()
      {
         if (ForeColor.Equals(Color.Black) == false)
         {
            ForeColor = Color.Black;

            SelectAll();
            SelectionColor = ForeColor;
         }
      }

      private void DoLineHighlight(int lineIndex, Color color)
      {
         int firstChar = GetFirstCharIndexFromLine(lineIndex);
         int length = Lines[lineIndex].Length;
         Select(firstChar, length);

         SelectionColor = color;
         //SelectionBackColor = color;
      }

      #region Native Scroll Messages (don't call under Mono)

      public void ScrollToBottom()
      {
         SelectionStart = TextLength;
      
         if (PlatformOps.IsRunningOnMono())
         {
            ScrollToCaret();
         }
         else
         {
            NativeMethods.SendMessage(Handle, NativeMethods.WM_VSCROLL, new IntPtr(NativeMethods.SB_BOTTOM), new IntPtr(0));
         }
      }

      public void ScrollToTop()
      {
         if (PlatformOps.IsRunningOnMono())
         {
            throw new NotImplementedException("This function is not implemented when running under the Mono Runtime.");
         }

         NativeMethods.SendMessage(Handle, NativeMethods.WM_VSCROLL, new IntPtr(NativeMethods.SB_TOP), new IntPtr(0));
      }

      public void ScrollLineDown()
      {
         if (PlatformOps.IsRunningOnMono())
         {
            throw new NotImplementedException("This function is not implemented when running under the Mono Runtime.");
         }

         NativeMethods.SendMessage(Handle, NativeMethods.WM_VSCROLL, new IntPtr(NativeMethods.SB_LINEDOWN), new IntPtr(0));
      }

      public void ScrollLineUp()
      {
         if (PlatformOps.IsRunningOnMono())
         {
            throw new NotImplementedException("This function is not implemented when running under the Mono Runtime.");
         }

         NativeMethods.SendMessage(Handle, NativeMethods.WM_VSCROLL, new IntPtr(NativeMethods.SB_LINEUP), new IntPtr(0));
      }

      public void ScrollToLine(int lineNumber)
      {
         if (PlatformOps.IsRunningOnMono())
         {
            throw new NotImplementedException("This function is not implemented when running under the Mono Runtime.");
         }

         NativeMethods.SendMessage(Handle, NativeMethods.EM_LINESCROLL, new IntPtr(0), new IntPtr(lineNumber));
      }
      
      #endregion
   }
}
