using System;

using HFM.Proteins;

namespace HFM.Instances
{
   public class DisplayInstance
   {
      #region Members & Read Only Properties
      /// <summary>
      /// 
      /// </summary>
      private ClientStatus _Status;
      /// <summary>
      /// 
      /// </summary>
      public ClientStatus Status
      {
         get { return _Status; }
      }

      /// <summary>
      /// Private member holding the percentage progress of the unit
      /// </summary>
      private string _Progress;
      /// <summary>
      /// Current progress (percentage) of the unit
      /// </summary>
      public string Progress
      {
         get { return _Progress; }
      }

      /// <summary>
      /// 
      /// </summary>
      private String _InstanceName;
      /// <summary>
      /// 
      /// </summary>
      public String Name
      {
         get { return _InstanceName; }
      }

      /// <summary>
      /// 
      /// </summary>
      private ClientType _ClientType;
      /// <summary>
      /// 
      /// </summary>
      public ClientType ClientType
      {
         get { return _ClientType; }
      }

      /// <summary>
      /// Private member holding the time per frame of the unit
      /// </summary>
      private TimeSpan _TimePerFrame;
      /// <summary>
      /// 
      /// </summary>
      public TimeSpan TPF
      {
         get { return _TimePerFrame; }
         set { _TimePerFrame = value; }
      }

      /// <summary>
      /// Private variable holding the PPD rating for this instance
      /// </summary>
      private double _PPD;
      /// <summary>
      /// PPD rating for this instance
      /// </summary>
      public double PPD
      {
         get { return _PPD; }
      }

      /// <summary>
      /// Private variable holding the PPD rating for this instance
      /// </summary>
      private double _PPD_MHz;
      /// <summary>
      /// PPD rating for this instance
      /// </summary>
      public double PPD_MHz
      {
         get { return _PPD_MHz; }
      }

      /// <summary>
      /// Private variable holding the ETA
      /// </summary>
      private TimeSpan _ETA;
      /// <summary>
      /// ETA for this instance
      /// </summary>
      public TimeSpan ETA
      {
         get { return _ETA; }
      }

      /// <summary>
      /// 
      /// </summary>
      private String _Core;
      /// <summary>
      /// 
      /// </summary>
      public String Core
      {
         get { return _Core; }
      }

      /// <summary>
      /// 
      /// </summary>
      private string _CoreVersion;
      /// <summary>
      /// 
      /// </summary>
      public string CoreVersion
      {
         get { return _CoreVersion; }
      }

      /// <summary>
      /// 
      /// </summary>
      private string _ProjectRunCloneGen;

      /// <summary>
      /// 
      /// </summary>
      public string ProjectRunCloneGen
      {
         get { return _ProjectRunCloneGen; }
      }

      /// <summary>
      /// 
      /// </summary>
      private int _Credit;
      /// <summary>
      /// 
      /// </summary>
      public int Credit
      {
         get { return _Credit; }
      }

      /// <summary>
      /// 
      /// </summary>
      private int _Complete;
      /// <summary>
      /// 
      /// </summary>
      public int Complete
      {
         get { return _Complete; }
      }

      /// <summary>
      /// 
      /// </summary>
      private int _Failed;
      /// <summary>
      /// 
      /// </summary>
      public int Failed
      {
         get { return _Failed; }
      }
      
      /// <summary>
      /// 
      /// </summary>
      private string _Username;
      /// <summary>
      /// 
      /// </summary>
      public string Username
      {
         get { return _Username; }
      }

      /// <summary>
      /// 
      /// </summary>
      private int _Team;
      /// <summary>
      /// 
      /// </summary>
      public int Team
      {
         get { return _Team; }
      }
      
      /// <summary>
      /// Private member holding the download time of the unit
      /// </summary>
      private DateTime _DownloadTime;
      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      public DateTime DownloadTime
      {
         get { return _DownloadTime; }
      }

      /// <summary>
      /// 
      /// </summary>
      private DateTime _Deadline;
	   /// <summary>
	   /// 
	   /// </summary>
      public DateTime Deadline
      {
         get { return _Deadline; }
      }

      public string Dummy
      {
         get { return String.Empty; }
      }
      #endregion
      
      public void Load(ClientInstance baseInstance)
      {
         _Status = baseInstance.Status;
         _Progress = String.Format("{0:00}%", baseInstance.CurrentUnitInfo.PercentComplete);
         _InstanceName = baseInstance.InstanceName;
         _ClientType = baseInstance.CurrentUnitInfo.TypeOfClient;
         _TimePerFrame = baseInstance.CurrentUnitInfo.TimePerFrame;
         _PPD = Math.Round(baseInstance.CurrentUnitInfo.PPD, 1);
         _PPD_MHz = Math.Round(baseInstance.CurrentUnitInfo.PPD / baseInstance.ClientProcessorMegahertz, 3);
         _ETA = baseInstance.CurrentUnitInfo.ETA;
         _Core = baseInstance.CurrentUnitInfo.CurrentProtein.Core;
         _CoreVersion = baseInstance.CurrentUnitInfo.CoreVersion;
         _ProjectRunCloneGen = baseInstance.CurrentUnitInfo.ProjectRunCloneGen;
         _Credit = baseInstance.CurrentUnitInfo.CurrentProtein.Credit;
         _Complete = baseInstance.NumberOfCompletedUnitsSinceLastStart;
         _Failed = baseInstance.NumberOfFailedUnitsSinceLastStart;
         _Username = baseInstance.CurrentUnitInfo.FoldingID;
         _Team = baseInstance.CurrentUnitInfo.Team;
         _DownloadTime = baseInstance.CurrentUnitInfo.DownloadTime;
         _Deadline = baseInstance.CurrentUnitInfo.DownloadTime.AddDays(baseInstance.CurrentUnitInfo.CurrentProtein.PreferredDays);
      }
      
      public void UpdateName(string Key)
      {
         _InstanceName = Key;
      }
   }
}