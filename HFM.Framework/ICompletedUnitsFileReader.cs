﻿/*
 * HFM.NET - Completed Units File Reader Interface
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
 
using System.Collections.Generic;

using harlam357.Windows.Forms;

using HFM.Framework.DataTypes;

namespace HFM.Framework
{
   public interface ICompletedUnitsFileReader : IProgressProcessRunner
   {
      string CompletedUnitsFilePath { get; set; }
      CompletedUnitsReadResult Result { get; }
   
      void WriteCompletedUnitErrorLines(string filePath, IEnumerable<string> lines);
   }
}