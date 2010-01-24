/*
 * HFM.NET - Unit Info Class
 * Copyright (C) 2006 David Rawling
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
using System.Diagnostics.CodeAnalysis;

using ProtoBuf;

using HFM.Framework;

namespace HFM.Instances
{
   [Serializable]
   [ProtoContract]
   public class UnitInfo : IUnitInfo
   {
      #region Owner Data Properties
      private string _OwningInstanceName;
      /// <summary>
      /// Name of the Client Instance that owns this UnitInfo
      /// </summary>
      [ProtoMember(1)]
      public string OwningInstanceName
      {
         get { return _OwningInstanceName; }
         set { _OwningInstanceName = value; }
      }

      private string _OwningInstancePath;
      /// <summary>
      /// Path of the Client Instance that owns this UnitInfo
      /// </summary>
      [ProtoMember(2)]
      public string OwningInstancePath
      {
         get { return _OwningInstancePath; }
         set { _OwningInstancePath = value; }
      }
      #endregion

      #region Retrieval Time Property
      private DateTime _UnitRetrievalTime;
      /// <summary>
      /// Local time the logs used to generate this UnitInfo were retrieved
      /// </summary>
      [ProtoMember(3)]
      public DateTime UnitRetrievalTime
      {
         get { return _UnitRetrievalTime; }
         set { _UnitRetrievalTime = value; }
      } 
      #endregion

      #region Folding ID and Team Properties
      private string _FoldingID;
      /// <summary>
      /// The Folding ID (Username) attached to this work unit
      /// </summary>
      [ProtoMember(4)]
      public string FoldingID
      {
         get { return _FoldingID; }
         set { _FoldingID = value; }
      }

      private Int32 _Team;
      /// <summary>
      /// The Team number attached to this work unit
      /// </summary>
      [ProtoMember(5)]
      public Int32 Team
      {
         get { return _Team; }
         set { _Team = value; }
      }
      #endregion

      #region Unit Level Members
      private ClientType _TypeOfClient = ClientType.Unknown;
      /// <summary>
      /// Client Type for this work unit
      /// </summary>
      [ProtoMember(6)]
      public ClientType TypeOfClient
      {
         get { return _TypeOfClient; }
         set { _TypeOfClient = value; }
      }

      private DateTime _DownloadTime = DateTime.MinValue;
      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      [ProtoMember(7)]
      public DateTime DownloadTime
      {
         get { return _DownloadTime; }
         set { _DownloadTime = value; }
      }

      private DateTime _DueTime = DateTime.MinValue;
      /// <summary>
      /// Date/time the unit is due (preferred deadline)
      /// </summary>
      [ProtoMember(8)]
      public DateTime DueTime
      {
         get { return _DueTime; }
         set { _DueTime = value; }
      }

      private TimeSpan _UnitStartTime = TimeSpan.Zero;
      /// <summary>
      /// Unit Start Time Stamp (Time Stamp from First Parsable Line in LogLines)
      /// </summary>
      /// <remarks>Used to Determine Status when a LogLine Time Stamp is not available - See ClientInstance.HandleReturnedStatus</remarks>
      [ProtoMember(9)]
      public TimeSpan UnitStartTimeStamp
      {
         get { return _UnitStartTime; }
         set { _UnitStartTime = value; }
      }

      private DateTime _FinishedTime = DateTime.MinValue;
      /// <summary>
      /// Date/time the unit finished
      /// </summary>
      [ProtoMember(10)]
      public DateTime FinishedTime
      {
         get { return _FinishedTime; }
         set { _FinishedTime = value; }
      }

      private string _CoreVersion = String.Empty;
      /// <summary>
      /// Core Version Number
      /// </summary>
      [ProtoMember(11)]
      public string CoreVersion
      {
         get { return _CoreVersion; }
         set { _CoreVersion = value; }
      }

      private Int32 _ProjectID;
      /// <summary>
      /// Project ID Number
      /// </summary>
      [ProtoMember(12)]
      public Int32 ProjectID
      {
         get { return _ProjectID; }
         set { _ProjectID = value; } 
      }

      private Int32 _ProjectRun;
      /// <summary>
      /// Project ID (Run)
      /// </summary>
      [ProtoMember(13)]
      public Int32 ProjectRun
      {
         get { return _ProjectRun; }
         set { _ProjectRun = value; }
      }

      private Int32 _ProjectClone;
      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      [ProtoMember(14)]
      public Int32 ProjectClone
      {
         get { return _ProjectClone; }
         set { _ProjectClone = value; }
      }

      private Int32 _ProjectGen;
      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      [ProtoMember(15)]
      public Int32 ProjectGen
      {
         get { return _ProjectGen; }
         set { _ProjectGen = value; }
      }

      private String _ProteinName = String.Empty;
      /// <summary>
      /// Name of the unit
      /// </summary>
      [ProtoMember(16)]
      public String ProteinName
      {
         get { return _ProteinName; }
         set { _ProteinName = value; }
      }

      private string _ProteinTag = String.Empty;
      /// <summary>
      /// Tag string as read from the UnitInfo.txt file
      /// </summary>
      [ProtoMember(17)]
      public string ProteinTag
      {
         get { return _ProteinTag; }
         set { _ProteinTag = value; }
      }

      private WorkUnitResult _UnitResult = WorkUnitResult.Unknown;
      /// <summary>
      /// The Result of this Work Unit
      /// </summary>
      [ProtoMember(18)]
      public WorkUnitResult UnitResult
      {
         get { return _UnitResult; }
         set { _UnitResult = value; }
      }
      #endregion

      #region Frames/Percent Completed Unit Level Members
      private Int32 _RawFramesComplete;
      /// <summary>
      /// Raw number of steps complete
      /// </summary>
      [ProtoMember(19)]
      public Int32 RawFramesComplete
      {
         get { return _RawFramesComplete; }
         set
         {
            _RawFramesComplete = value;
         }
      }

      private Int32 _RawFramesTotal;
      /// <summary>
      /// Raw total number of steps
      /// </summary>
      [ProtoMember(20)]
      public Int32 RawFramesTotal
      {
         get { return _RawFramesTotal; }
         set
         {
            _RawFramesTotal = value;
         }
      }
      #endregion

      #region Frame (UnitFrame) Data Variables
      private Int32 _FramesObserved = 0;
      /// <summary>
      /// Number of Frames Observed on this Unit
      /// </summary>
      [ProtoMember(21)]
      public Int32 FramesObserved
      {
         get { return _FramesObserved; }
         set { _FramesObserved = value; }
      }

      private UnitFrame _CurrentFrame = null;
      /// <summary>
      /// Last Observed Frame on this Unit
      /// </summary>
      [ProtoMember(22)]
      public UnitFrame CurrentFrameConcrete
      {
         get { return _CurrentFrame; }
         set { _CurrentFrame = value; }
      }

      /// <summary>
      /// Last Observed Frame on this Unit
      /// </summary>
      public IUnitFrame CurrentFrame
      {
         get { return _CurrentFrame; }
      }

      private UnitFrame[] _UnitFrames = null;
      /// <summary>
      /// Frame Data for this Unit
      /// </summary>
      [ProtoMember(23)]
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public UnitFrame[] UnitFramesConcrete
      {
         get { return _UnitFrames; }
         set { _UnitFrames = value; }
      }
      
      /// <summary>
      /// Frame Data for this Unit
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public IUnitFrame[] UnitFrames
      {
         get { return _UnitFrames; }
      }
      #endregion
   }
}
